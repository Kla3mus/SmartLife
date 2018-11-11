using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RJCP.IO.Ports;
using SmartLife;
using SmartLife.Devices.Z_Wave.AeoTec;
using SmartLife.Interfaces;
using ZWave;

namespace SmartLife_Console
{
	class Program_old
	{
		private readonly ZWaveController _controller;
		public Program_old()
		{
			var portName = SerialPortStream.GetPortNames().First(element => element != "COM1");

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

				if (protocolInfo.GenericType == GenericType.SensorMultiLevel)
				{
					var multi = new MultiSensor6(node);

					var multiSettings = multi.Settings;
					multiSettings.GetUpdatedSettings();

					multiSettings.MotionSensorUpdateTime = new TimeSpan(0,0,30);

					multiSettings.ApplyChanges();

					_devices.Add(multi);
				}
			}

			return true;
		}

		private readonly List<IDevice> _devices = new List<IDevice>();

		public async Task<bool> Run()
		{
			foreach (IDevice device in _devices)
			{
				if ((device is IPowerPlug))
					((IPowerPlug)device).StateChanged += (sender, b) => { Console.WriteLine($"{((IDevice)sender).DeviceId} -> {b}"); };

				if ((device is IPowerMeasure))
					((IPowerMeasure)device).PowerMeasurementTaken += (sender, b) => { Console.WriteLine($"{((IDevice)sender).DeviceId} -> {b}"); };

				if ((device is ILuxMeasure))
					((ILuxMeasure)device).LuxMeasurementTaken += (sender, b) => { Console.WriteLine($"{((IDevice)sender).DeviceId} -> {b}"); };

				if ((device is ITemperatureMeasure))
					((ITemperatureMeasure)device).TemperatureMeasurementTaken += (sender, b) => { Console.WriteLine($"{((IDevice)sender).DeviceId} -> {b}"); };

				if ((device is IUvMeasure))
					((IUvMeasure)device).UVMeasurementTaken += (sender, b) => { Console.WriteLine($"{((IDevice)sender).DeviceId} -> {b}"); };

				if ((device is IHumidityMeasure))
					((IHumidityMeasure)device).HumidityMeasurementTaken += (sender, b) => { Console.WriteLine($"{((IDevice)sender).DeviceId} -> {b}"); };

				if ((device is IVibrationSensor))
					((IVibrationSensor)device).VibrationSensorTriggered += (sender, b) => { Console.WriteLine($"{((IDevice)sender).DeviceId} -> {b}"); };

				if ((device is IMotionSensor))
					((IMotionSensor)device).MotionSensorTriggered += (sender, b) =>
					{
						Console.WriteLine($"{((IDevice)sender).DeviceId} -> {b}");
						if (CurrentState == State.Auto)
							SetStates(b.Value);
					};
			}

			ChangeProgramState();
			return true;
		}
		public enum State { On = 1, Off = 0, Auto = 2}

		private State CurrentState { get; set; } = State.Auto;

		private void UpdateAccordingToState()
		{
			switch (CurrentState)
			{
				case State.On:
					SetStates(true);
					break;
				case State.Off:
				case State.Auto:
					SetStates(false);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ChangeProgramState()
		{
			while (true)
			{
				var command = Console.ReadLine();
				if (command == "1")
				{
					CurrentState = State.On;
					UpdateAccordingToState();
				}
				if (command == "0")
				{
					CurrentState = State.Off;
					UpdateAccordingToState();
				}
				if (command == "2")
				{
					CurrentState = State.Auto;
					UpdateAccordingToState();
				}
				if (command == "9") { SetAllColors(); }
			}
		}

		private void SetAllColors()
		{
			foreach (var device in _devices)
			{
				if (!(device is ILedRing))
					continue;

				var l = (ILedRing)device;
				if (isOn)
				{
					foreach (EnabledLedRingColor value in Enum.GetValues(typeof(EnabledLedRingColor)))
					{
						l.SetEnabledColor(value);
						Thread.Sleep(1000);
					}
				}
				else
				{
					foreach (DisabledLedRingColor value in Enum.GetValues(typeof(DisabledLedRingColor)))
					{
						l.SetDisabledColor(value);
						Thread.Sleep(1000);
					}
				}
			}
		}

		private bool isOn = true;
		private void SetStates(bool state)
		{
			foreach (var device in _devices)
			{
				if (!(device is IPowerPlug))
					continue;

				var d = (IPowerPlug)device;
				d.Switch(state);
				isOn = state;
			}
		}
	}
}