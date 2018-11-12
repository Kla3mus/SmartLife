using SmartLife.Interfaces;

namespace SmartLife_Console
{
	public class MotionSensorPowerPlug : IOperation
	{
		private readonly IMotionSensor _motionSensor;
		private readonly IPowerPlug _powerPlug;

		public MotionSensorPowerPlug(IMotionSensor motionSensor, IPowerPlug powerPlug)
		{
			_motionSensor = motionSensor;
			_powerPlug    = powerPlug;
		}

		private bool isAttached = false;
		public void Attach()
		{
			if (isAttached)
				return;

			_motionSensor.MotionSensorTriggered += MotionSensorOnMotionSensorTriggered;
			isAttached                          =  true;
		}
		public void Detach()
		{
			if (!isAttached)
				return;

			_motionSensor.MotionSensorTriggered -= MotionSensorOnMotionSensorTriggered;
			isAttached = false;
		}

		private void MotionSensorOnMotionSensorTriggered(object sender, SensorReport e)
		{
			_powerPlug.Switch(e.Value);
		}
	}
}