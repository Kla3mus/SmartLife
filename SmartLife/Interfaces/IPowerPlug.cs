using System;

namespace SmartLife.Interfaces
{
	public interface IPowerPlug : IStateChange
	{
		void Switch(bool state);
	}

	public interface IStateChange : IDivice
	{
		event EventHandler<SensorReport> StateChanged;
	}

	public class StateChangeReport : SensorReport
	{
		public StateChangeReport(bool value) : base(ReportType.State, value) { }
	}
}