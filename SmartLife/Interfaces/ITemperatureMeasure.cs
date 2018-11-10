using System;

namespace SmartLife.Interfaces
{
	public interface ITemperatureMeasure : IMeasure
	{
		event EventHandler<MeasurementReport<float>> TemperatureMeasurementTaken;
	}
	public class TemperatureMeasuredReport : MeasurementReport<float>
	{
		public TemperatureMeasuredReport(float value, string unit) : base(ReportType.Temperature, value, unit)
		{
		}
	}
}