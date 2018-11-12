using System;

namespace SmartLife.Interfaces
{
	public interface IDevice
	{
		string DeviceId { get; }
	}

	public interface IMeasure : IDevice { }

	public interface ISensor : IDevice { }

	public interface IVibrationSensor : ISensor
	{
		event EventHandler<SensorReport> VibrationSensorTriggered;
	}

	public class VibrationSensorReport : SensorReport
	{
		public VibrationSensorReport(bool value) : base(ReportType.Vibration, value) { }
	}

	public interface IMotionSensor : IDevice
	{
		event EventHandler<SensorReport> MotionSensorTriggered;
	}

	public class MotionSensorReport : SensorReport
	{
		public MotionSensorReport(bool value) : base(ReportType.Motion, value) { }
	}
	public interface IOperation
	{
		void Attach();
		void Detach();
	}
}