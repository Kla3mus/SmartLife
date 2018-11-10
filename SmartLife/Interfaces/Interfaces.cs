using System;

namespace SmartLife.Interfaces
{
	public interface IDivice
	{
		string DeviceId { get; }

	}

	public interface IMeasure : IDivice { }

	public interface ISensor : IDivice { }

	public interface IVibrationSensor : ISensor
	{
		event EventHandler<SensorReport> VibrationSensorTriggered;
	}

	public class VibrationSensorReport : SensorReport
	{
		public VibrationSensorReport(bool value) : base(ReportType.Vibration, value) { }
	}

	public interface IMotionSensor : IDivice
	{
		event EventHandler<SensorReport> MotionSensorTriggered;
	}

	public class MotionSensorReport : SensorReport
	{
		public MotionSensorReport(bool value) : base(ReportType.Vibration, value) { }
	}
}