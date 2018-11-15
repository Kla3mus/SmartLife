using System.Collections.Generic;
using SmartLife.Interfaces;

namespace SmartLife_Console
{
	public class MotionSensorPowerPlug : IOperation
	{
		private readonly IMotionSensor _motionSensor;
		private readonly IList<IPowerPlug> _powerPlugs;

		private bool currentSocketState;

		public MotionSensorPowerPlug(IMotionSensor motionSensor, IList<IPowerPlug> powerPlugs)
		{
			_motionSensor = motionSensor;
			_powerPlugs   = powerPlugs;

			Devices.Add(motionSensor);
			foreach (var powerPlug in powerPlugs)
			{
				Devices.Add(powerPlug);
				powerPlug.StateChanged += (sender, report) => { currentSocketState = report.Value; };
			}

			_motionSensor.MotionSensorTriggered += MotionSensorOnMotionSensorTriggered;
		}

		public bool IsActive { get; private set; }

		public void Attach()
		{
			if (IsActive)
				return;

			IsActive = true;
			if (currentSocketState != IsActive)
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