using System;

namespace SmartLife.Interfaces
{
	public interface IPowerPlug : IStateChange
	{
		void Switch(bool state);
	}

	public interface IStateChange : IDivice
	{
		event EventHandler<IReport> StateChanged;
	}

	public class StateChangeReport: IReport<bool>
	{
		public StateChangeReport(bool value) { Value = value; }
		public bool Value { get; }
	}
}