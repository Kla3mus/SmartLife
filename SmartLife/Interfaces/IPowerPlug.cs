using System;

namespace SmartLife.Interfaces
{
	public interface IStateChange : IDevice
	{
		event EventHandler<SensorReport> StateChanged;
	}

	public class StateChangeReport : SensorReport
	{
		public StateChangeReport(bool value) : base(ReportType.State, value) { }
	}
}