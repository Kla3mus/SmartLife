using System.Collections.Generic;
using System.Linq;
using SmartLife.Interfaces;

namespace SmartLife.core.Demo
{

	public class MotionSensorPowerPlug : IOperation
	{
		private readonly IList<ISwitch> _powerPlugs;
		private bool _currentSocketState;

		/// <summary>
		///  If motion is noticed, switches will be activated
		/// </summary>
		/// <param name="motionSensor"></param>
		/// <param name="powerPlugs">must be of type switch, with state change reporting</param>
		public MotionSensorPowerPlug(IMotionSensor motionSensor, IList<IDevice> powerPlugs)
		{
			_powerPlugs   = powerPlugs.Where(x => x is IStateChange && x is ISwitch).Select(x => (ISwitch)x).ToList();

			Devices.Add(motionSensor);
			foreach (var powerPlug in powerPlugs)
			{
				Devices.Add(powerPlug);
				((IStateChange)powerPlug).StateChanged += (sender, report) => { _currentSocketState = report.Value; };
			}

			motionSensor.MotionSensorTriggered += MotionSensorOnMotionSensorTriggered;
		}

		public bool IsActive { get; private set; }

		public void Attach()
		{
			if (IsActive)
				return;

			IsActive = true;
			if (_currentSocketState != IsActive)
				foreach (var powerPlug in _powerPlugs)
					powerPlug.Switch(true);
		}

		public void Detach()
		{
			if (!IsActive)
				return;

			IsActive = false;
		}

		public IList<IDevice> Devices { get; } = new List<IDevice>();

		private void MotionSensorOnMotionSensorTriggered(object sender, SensorReport e)
		{
			if (IsActive)
				foreach (var powerPlug in _powerPlugs)
					powerPlug.Switch(e.Value);
		}
	}
}