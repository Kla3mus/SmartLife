using System.Collections.Generic;
using System.Linq;
using SmartLife;
using SmartLife.Interfaces;

namespace SmartLife_Console
{
	public class Program
	{
		private readonly SmartLife.SmartLife _smartLife;
		public Program()
		{
			_smartLife = new SmartLife.SmartLife(new FileStorage<DeviceWrapper>("DeviceWrappers.txt"));

			_smartLife.SaveDeviceWrappers();

			var zone = _smartLife.Zones.Where(x => x.Key == "#1").SelectMany(x => x.Value).Select(x => x.Device);

			var powerPlug = (IPowerPlug)zone.First(x => x is IPowerPlug);
			var motionSensor = (IMotionSensor)zone.First(x => x is IMotionSensor);

			_smartLife.AddOperation(new MotionSensorPowerPlug(motionSensor, new List<IPowerPlug> { powerPlug }));

			var plugs = _smartLife.DeviceWrappers.Where(x => !x.Zones.Any()).Select(x => (IPowerPlug)x.Device).ToList();
			_smartLife.AddOperation(new LuxSensorPowerPlugs((ILuxMeasure)motionSensor, plugs));
		}

		public string DoAction(string s)
		{
			var activeDevices = _smartLife.Operations.Where(x => x.IsActive).SelectMany(x => x.Devices);
			switch (s)
			{
				case "0": //Off
					foreach (IPowerPlug powerPlug in _smartLife.Devices.Where(x => x is IPowerPlug && activeDevices.All(y => x.DeviceId != y.DeviceId)))
						powerPlug.Switch(false);
					return "turned off non active things";
				case "1": //On
					foreach (IPowerPlug powerPlug in _smartLife.Devices.Where(x => x is IPowerPlug && activeDevices.All(y => x.DeviceId != y.DeviceId)))
						powerPlug.Switch(true);
					return "turned on non active things";
				case "2":
					foreach (var operation in _smartLife.Operations)
						operation.Attach();
					return "attached operations";
				case "3":
					foreach (var operation in _smartLife.Operations)
						operation.Detach();
					return "detached operations";
				default:
					return "No operation";
			}
		}
	}
}
