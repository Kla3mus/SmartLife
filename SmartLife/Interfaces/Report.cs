namespace SmartLife.Interfaces
{
	public interface IReport<T> 
	{
		T Value { get; }
	}

	public abstract class SensorReport : IReport<bool>
	{
		protected SensorReport(ReportType reportType, bool value)
		{
			Value = value;
			Type = reportType;
		}
		public bool Value { get; }
		public ReportType Type { get; }
		public override string ToString() { return $"{Type.ToString()} {Value}"; }
	}

	public abstract class MeasurementReport<T> : IReport<T>
	{
		protected MeasurementReport(ReportType type, T value, string unit)
		{
			Value = value;
			Unit  = unit;
			Type = type;
		}

		public ReportType Type { get; }
		public T Value { get; }
		public string Unit { get; }

		public override string ToString() { return $"{Type.ToString()} {Value} {Unit}"; }
	}
	public enum ReportType { Lux, Power, Temperature, Humidity, Vibration, Motion, UV, State }

}