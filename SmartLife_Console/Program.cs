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

			var powerPlug = (IPowerPlug)_smartLife.Devices.First(x => x is IPowerPlug);
			var motionSensor = (IMotionSensor)_smartLife.Devices.First(x => x is IMotionSensor);

			_smartLife.AddOperation(new MotionSensorPowerPlug(motionSensor, powerPlug));
		}

		public string DoAction(string s)
		{
			switch (s)
			{
				case "0": //Off
					foreach (IPowerPlug powerPlug in _smartLife.Devices.Where(x => x is IPowerPlug))
						powerPlug.Switch(false);

					foreach (var operation in _smartLife.Operations)
						operation.Detach();

					break;
				case "1": //On
					foreach (IPowerPlug powerPlug in _smartLife.Devices.Where(x => x is IPowerPlug))
						powerPlug.Switch(true);

					foreach (var operation in _smartLife.Operations)
						operation.Detach();
					break;
				case "2": //Automatic
					foreach (IPowerPlug powerPlug in _smartLife.Devices.Where(x => x is IPowerPlug))
						powerPlug.Switch(false);

					foreach (var operation in _smartLife.Operations)
						operation.Attach();
					break;
				
				default:

					return "No operation";
			}

			return "Executed";
		}
	}
}
