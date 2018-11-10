using System;

namespace SmartLife.Interfaces
{
	public interface IHumidityMeasure : IMeasure
	{
		event EventHandler<MeasurementReport<float>> HumidityMeasurementTaken;
	}
	public class HumidityMeasureReport : MeasurementReport<float>
	{
		public HumidityMeasureReport(float value, string unit) : base(ReportType.Humidity, value, unit)
		{
		}
	}
}