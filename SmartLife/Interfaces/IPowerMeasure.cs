using System;

namespace SmartLife.Interfaces
{
	public interface IPowerMeasure : IMeasure
	{
		event EventHandler<MeasurementReport<float>> PowerMeasurementTaken;
	}

	public class PowerMeasureMeasurementReport : MeasurementReport<float>
	{
		public PowerMeasureMeasurementReport(float value, string unit) 
			: base(ReportType.Power, value, unit) { }
	}
}