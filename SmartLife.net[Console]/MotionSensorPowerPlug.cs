using System.Collections.Generic;
using SmartLife.Interfaces;

namespace SmartLife.net.Demo
{
	public class MotionSensorPowerPlug : IOperation
	{
		private readonly IList<IPowerPlug> _powerPlugs;

		private bool _currentSocketState;

		public MotionSensorPowerPlug(IMotionSensor motionSensor, IList<IPowerPlug> powerPlugs)
		{
			_powerPlugs   = powerPlugs;

			Devices.Add(motionSensor);
			foreach (var powerPlug in powerPlugs)
			{
				Devices.Add(powerPlug);
				powerPlug.StateChanged += (sender, report) => { _currentSocketState = report.Value; };
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