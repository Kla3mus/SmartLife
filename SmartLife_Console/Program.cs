using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RJCP.IO.Ports;
using SmartLife;
using SmartLife.Interfaces;
using ZWave;

namespace SmartLife_Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var p = new Program();
			p.GetDevices().Wait();
			p.Run().Wait();
		}

		private readonly ZWaveController _controller;
		public Program()
		{
			var portName = SerialPortStream.GetPortNames().Where(element => element != "COM1").First();

			_controller = new ZWaveController(portName);
			_controller.ChannelClosed += (sender, args) => { Console.WriteLine($"ChannelClosed {args}"); };
			_controller.Error += (sender, args) => { Console.WriteLine($"Error {args.Error.Message}"); };
			_controller.Open();
		}

		public async Task<bool> GetDevices()
		{
			var homeid = await _controller.GetHomeID();
			Console.WriteLine($"homeid #{homeid}");
			var version = await _controller.GetVersion();
			Console.WriteLine($"version {version}");
			var nodeid = await _controller.GetNodeID();
			Console.WriteLine($"nodeid #{nodeid}");

			var nodes = await _controller.GetNodes();
			foreach (var node in nodes)
			{
				var protocolInfo = await node.GetProtocolInfo();
				if (protocolInfo.GenericType == GenericType.SwitchBinary)
					_devices.Add(new WallPlug(node));
			}

			return true;
		}

		private readonly List<IDivice> _devices = new List<IDivice>();

		public async Task<bool> Run()
		{
			foreach (IDivice device in _devices)
			{
				if (!(device is IPowerPlug))
					continue;

				var d = (IPowerPlug)device;
				d.SwitchStateChanged += (sender, b) => { Console.WriteLine($"{((IDivice)sender).DeviceId} -> State changed! {b}"); };

				var p = (IPowerMeasure)device;
				p.CurrentPowerLoad += (sender, b) => { Console.WriteLine($"{((IDivice)sender).DeviceId} -> Current Load! {b}"); };
			}

			ChangeState();
			return true;
		}

		private void ChangeState()
		{
			while (true)
				SetStates(Console.ReadLine() == "1");
		}

		private void SetStates(bool state)
		{
			foreach (var device in _devices)
			{
				if (!(device is IPowerPlug))
					continue;

				var d = (IPowerPlug)device;
				d.Switch(state);

				var l = (ILedRing)device;
				l.SetDisabledColor(DisabledLedRingColor.Red);
				l.SetEnabledColor(EnabledLedRingColor.Green);
			}
		}
	}
}
