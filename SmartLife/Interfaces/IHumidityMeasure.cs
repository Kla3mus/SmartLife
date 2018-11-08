using System;

namespace SmartLife.Interfaces
{
	public interface IHumidityMeasure : IMeasure
	{
		event EventHandler<IMeasurementReport> HumidityMeasurementTaken;
	}
	public class HumidityMeasureReport : MeasurementReport<float>
	{
		public HumidityMeasureReport(float value, string unit) : base(ReportType.Humidity, value, unit)
		{
		}
	}
}