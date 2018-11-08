using System;

namespace SmartLife.Interfaces
{
	public interface IUvMeasure : IMeasure
	{
		event EventHandler<IMeasurementReport> UVMeasurementTaken;
	}
	public class UVMeasureReport : MeasurementReport<float>
	{
		public UVMeasureReport(float value, string unit) : base(ReportType.UV, value, unit)
		{
		}
	}
}