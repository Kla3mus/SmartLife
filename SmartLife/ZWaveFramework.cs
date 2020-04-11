using System;
using System.Collections.Generic;
using System.Linq;
using RJCP.IO.Ports;
using SmartLife.Devices;
using SmartLife.Interfaces;
using ZWave;
using ZWave.Channel;

namespace SmartLife
{
	public class ZWaveFramework : ISmartHouseFramework
	{
		private readonly string ZWavePortName;
		private ZWaveController _controller;
		public ZWaveFramework(string port = "")
		{
			ZWavePortName = port;
			if (string.IsNullOrEmpty(ZWavePortName))
				ZWavePortName = SerialPortStream.GetPortNames().Last(element => element != "COM1");
		}

		public event EventHandler<Log> Logged;

		public void Start()
		{
			_controller = new ZWaveController(new SerialPort(ZWavePortName));
			_controller.ChannelClosed += LogChannelClosed;
			_controller.Error += LogError;
			_controller.Open();
		}

		public void Dispose()
		{
			_controller.ChannelClosed -= LogChannelClosed;
			_controller.Error -= LogError;
			_controller.Close();
		}

		private void LogError(object sender, ErrorEventArgs args)
		{
			Logged?.Invoke(sender, new Log
			{
				Level = SmartLife.Log.LevelEnum.Error,
				Exception = args.Error,
				Message = $"ZWaveController Error {args.Error.Message}"
			});
		}

		private void LogChannelClosed(object sender, EventArgs args)
		{
			Logged?.Invoke(sender, new Log
			{
				Level = SmartLife.Log.LevelEnum.Info,
				Message = $"ZWaveController ChannelClosed {args}"
			});
		}

		private void Log(object obj, Log log)
		{
			Logged?.Invoke(obj, log);
		}
		public IEnumerable<IDevice> Devices
		{
			get
			{
				var nodes = _controller.GetNodes();
				nodes.Wait();
				var deviceFactory = new DeviceFactory(nodes.Result.Where(x => x.NodeID != 001).ToList());
				var devices       = deviceFactory.Devices;

				var nonWorkingDevices = new List<IDevice>();

				foreach (var device in devices.Where(x => x is UnknownDevice))
				{
					Log(this, new Log { Level = SmartLife.Log.LevelEnum.Info, Message = $"{device.DeviceId} is unknown" });
					nonWorkingDevices.Add(device);
				}

				foreach (var device in devices.Where(x => x is UnResponsiveDevice))
				{
					Log(this, new Log { Level = SmartLife.Log.LevelEnum.Info, Message = $"{device.DeviceId} is unresponsive" });
					nonWorkingDevices.Add(device);
				}

				return devices.Where(x => !nonWorkingDevices.Contains(x));
			}
		}
	}
}