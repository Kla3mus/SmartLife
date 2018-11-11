using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RJCP.IO.Ports;
using SmartLife.Interfaces;
using ZWave;

namespace SmartLife
{
	public class SmartLife
	{
		private readonly ZWaveController _controller;
		public SmartLife(string portName = null)
		{
			//We're gong to change this in the future, when we add support for more frameworks
			if (string.IsNullOrEmpty(portName))
				portName = SerialPortStream.GetPortNames().First(element => element != "COM1");

			_controller = new ZWaveController(portName);
			_controller.ChannelClosed += (sender, args) => { Console.WriteLine($"ChannelClosed {args}"); };
			_controller.Error += (sender, args) => { Console.WriteLine($"Error {args.Error.Message}"); };
			_controller.Open();

			var nodes = _controller.GetNodes();
			nodes.Wait();

			var deviceFactory = new DeviceFactory(nodes.Result.ToList());
			Devices = deviceFactory.Devices;
		}

		public IEnumerable<IDevice> Devices { get; }

	}
}
