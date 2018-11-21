using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SmartLife;
using SmartLife.Devices.Z_Wave.OOMI;
using SmartLife.Interfaces;

namespace SmartLife_Console
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
		private readonly SmartLife.SmartLife _smartLife;
		public Program()
		{
			_smartLife = new SmartLife.SmartLife(new ConsoleLogger(), new FileStorage<DeviceWrapper>("DeviceWrappers.txt"));

			_smartLife.SaveDeviceWrappers();

			var zone = _smartLife.Zones.Where(x => x.Key == "#1").SelectMany(x => x.Value).Select(x => x.Device);

			var powerPlug = (IPowerPlug)zone.First(x => x is IPowerPlug);
			var motionSensor = (IMotionSensor)zone.First(x => x is IMotionSensor);

			_smartLife.AddOperation(new MotionSensorPowerPlug(motionSensor, new List<IPowerPlug> { powerPlug }));

			var plugs = _smartLife.DeviceWrappers.Where(x => !x.Zones.Any()).Where(x => x is ILedRing).Select(x => (IPowerPlug)x.Device).ToList();
			_smartLife.AddOperation(new LuxSensorPowerPlugs((ILuxMeasure)motionSensor, plugs));

			bulb = (Bulb)_smartLife.DeviceWrappers.First(x => x.DeviceId.ToLower().Contains("bulb")).Device;


			
		}

		private Bulb bulb;

		public string DoAction(string s)
		{
			var activeDevices = _smartLife.Operations.Where(x => x.IsActive).SelectMany(x => x.Devices);
			switch (s)
			{
				case "0": //Off
					foreach (IPowerPlug powerPlug in _smartLife.Devices.Where(x => x is IPowerPlug && activeDevices.All(y => x.DeviceId != y.DeviceId)))
						powerPlug.Switch(false);

					foreach (IDim powerPlug in _smartLife.Devices.Where(x => x is IDim && activeDevices.All(y => x.DeviceId != y.DeviceId)))
						powerPlug.Dim(0);

					return "turned off non active things";
				case "1": //On
					foreach (IPowerPlug powerPlug in _smartLife.Devices.Where(x => x is IPowerPlug && activeDevices.All(y => x.DeviceId != y.DeviceId)))
						powerPlug.Switch(true);

					foreach (IDim powerPlug in _smartLife.Devices.Where(x => x is IDim && activeDevices.All(y => x.DeviceId != y.DeviceId)))
						powerPlug.Dim(99);

					return "turned on non active things";
				case "2":
					foreach (var operation in _smartLife.Operations)
						operation.Attach();
					return "attached operations";
				case "3":
					foreach (var operation in _smartLife.Operations)
						operation.Detach();
					return "detached operations";
				case "4":

					var milliseconds = 1000;
					bulb.Dim(0);
					Thread.Sleep(milliseconds);
					bulb.Dim(1);
					Thread.Sleep(milliseconds);
					bulb.Dim(25);
					Thread.Sleep(milliseconds);
					bulb.Dim(50);
					Thread.Sleep(milliseconds);
					bulb.Dim(75);
					Thread.Sleep(milliseconds);
					bulb.Dim(99);
					return "ok.. check the lamp :)";
				default:
					return "No operation";
			}
		}
	}
}
