using System;

namespace SmartLife.Interfaces
{
	public interface IDivice
	{
		string DeviceId { get; }
	}

	public interface IMeasure : IDivice { }

	public interface IVibrationSensor : IDivice
	{
		//event EventHandler<VibrationSensorReport> CurrentPowerLoad;
	}

	public interface IMotionSensor : IDivice
	{
		//event EventHandler<MotionSensorReport> CurrentPowerLoad;
	}
}