namespace SmartLife.Interfaces
{
	public interface IReport { }
	public interface IReport<T> : IReport
	{
		T Value { get; }
	}
	public interface IMeasurementReport : IReport
	{
		ReportType Type { get; }
		string Unit { get; }
	}

	public abstract class MeasurementReport<T> : IMeasurementReport
	{
		protected MeasurementReport(ReportType type, T value, string unit)
		{
			Value = value;
			Unit  = unit;
		}

		public ReportType Type { get; }
		public T Value { get; }
		public string Unit { get; }

		public override string ToString() { return $"{Type.ToString()} {Value} {Unit}"; }
	}
	public enum ReportType { Lux, Power, Temperature, Humidity, Vibration, Motion, UV }

}