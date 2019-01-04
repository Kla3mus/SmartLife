using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SmartLife.Devices;
using SmartLife.Interfaces;

namespace SmartLife.core.Demo
{

	public class ConsoleLogger : ILogger
	{
		public void Log(string s)
		{
			Console.WriteLine($"{DateTime.Now} {s}");
		}
	}

	public class Program
	{
		private readonly SmartHub _smartHub;
		public Program(ConsoleLogger logger)
		{
			ZWaveFramework zwave = new ZWaveFramework();
			_smartHub = new SmartHub(logger, new List<ISmartHouseFramework> { zwave }, new FileStorage<DeviceWrapper>("DeviceWrappers.txt"));

			_smartHub.SaveDeviceWrappers();

			foreach (var smartLifeDeviceWrapper in _smartHub.DeviceWrappers.Where(x => x.Device is UnknownDevice))
				logger.Log($"Found a unknown device! {smartLifeDeviceWrapper.DeviceId} {smartLifeDeviceWrapper.Device}");

			var zone = _smartHub.Zones.Where(x => x.Key == "#1").SelectMany(x => x.Value).Select(x => x.Device);

			var powerPlug = zone.FirstOrDefault(x => x is IPowerPlug);
			var motionSensor = zone.FirstOrDefault(x => x is IMotionSensor);

			if (powerPlug != null && motionSensor != null)
			{
				_smartHub.AddOperation(new MotionSensorPowerPlug((IMotionSensor)motionSensor, new List<IPowerPlug> { (IPowerPlug)powerPlug }));

				var plugs = _smartHub.DeviceWrappers.Where(x => !x.Zones.Any()).Where(x => x is ILedRing).Select(x => (IPowerPlug)x.Device).ToList();
				_smartHub.AddOperation(new LuxSensorPowerPlugs((ILuxMeasure)motionSensor, plugs));
			}

			var temp = _smartHub.DeviceWrappers.Select(x => x.Device).Where(x => x is IColorLight);
			foreach (var device in temp)
				_bulbs.Add((IColorLight)device);
		}

		private readonly List<IColorLight> _bulbs = new List<IColorLight>();

		public string DoAction(string s)
		{
			var activeDevices = _smartHub.Operations.Where(x => x.IsActive).SelectMany(x => x.Devices);

			var array = s.Split(' ');

			if (array.FirstOrDefault() == "4")
			{
				try
				{
					var warmWhite = byte.Parse(array[1]);
					var coldWhite = byte.Parse(array[2]);
					var red = byte.Parse(array[3]);
					var green = byte.Parse(array[4]);
					var blue = byte.Parse(array[5]);

					foreach (var colorLight in _bulbs)
						colorLight.SetColor(warmWhite, coldWhite, red, green, blue);
				}
				catch { return "failed"; }
				return "Color changed";
			}
			else
			{
				switch (s)
				{
					case "0": //Off
						foreach (IPowerPlug powerPlug in _smartHub.Devices.Where(x => x is IPowerPlug && activeDevices.All(y => x.DeviceId != y.DeviceId)))
							powerPlug.Switch(false);

						foreach (IDim powerPlug in _smartHub.Devices.Where(x => x is IDim && activeDevices.All(y => x.DeviceId != y.DeviceId)))
							powerPlug.Dim(0);

						return "turned off non active things";
					case "1": //On
						foreach (IPowerPlug powerPlug in _smartHub.Devices.Where(x => x is IPowerPlug && activeDevices.All(y => x.DeviceId != y.DeviceId)))
							powerPlug.Switch(true);

						foreach (IDim powerPlug in _smartHub.Devices.Where(x => x is IDim && activeDevices.All(y => x.DeviceId != y.DeviceId)))
							powerPlug.Dim(99);

						return "turned on non active things";
					case "2":
						foreach (var operation in _smartHub.Operations)
							operation.Attach();
						return "attached operations";
					case "3":
						foreach (var operation in _smartHub.Operations)
							operation.Detach();
						return "detached operations";
					case "5":
						ScrollColors();
						return "finished";
					case "6":
						BlinkColors();
						return "finished blinking";
					default:
						return "No operation";
				}
			}
		}

		private void BlinkColors()
		{
			byte warmWhite = 0;
			byte coldWhite = 0;

			for (int i = 0; i < 20; i++)
			{
				if (red == 0)
				{
					red = 255;
					green = 0;
					foreach (var colorLight in _bulbs)
						colorLight.SetColor(warmWhite, coldWhite, red, green, blue);
					Thread.Sleep(1000);

					continue;
				}

				if (red == 255)
				{
					red = 0;
					green = 255;
					foreach (var colorLight in _bulbs)
						colorLight.SetColor(warmWhite, coldWhite, red, green, blue);

					Thread.Sleep(1000);
					continue;
				}
			}

			red = 0;
			green = 0;
			blue = 255;
			foreach (var colorLight in _bulbs)
				colorLight.SetColor(warmWhite, coldWhite, red, green, blue);
		}

		byte red = 255;
		byte green = 0;
		byte blue = 0;

		private void ScrollColors()
		{
			ChangeValue(ref green, true);
			ChangeValue(ref red, false);
			ChangeValue(ref blue, true);

			ChangeValue(ref green, false);
			ChangeValue(ref red, true);
			ChangeValue(ref blue, false);
		}

		private void ChangeValue(ref byte color, bool increase)
		{
			byte warmWhite = 0;
			byte coldWhite = 0;

			for (int i = 0; i < 51; i++)
			{
				if (increase)
				{
					color++;
					color++;
					color++;
					color++;
					color++;
				}
				else
				{
					color--;
					color--;
					color--;
					color--;
					color--;
				}

				foreach (var colorLight in _bulbs)
					colorLight.SetColor(warmWhite, coldWhite, red, green, blue);

				Thread.Sleep(10);
			}
		}
	}
}
