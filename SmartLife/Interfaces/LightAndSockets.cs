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
	public interface ISwitch : IDevice
	{
		void Switch(bool state);
	}
	public interface IDim : ISwitch
	{
		void Dim(int percent);
	}
	public interface IColorLight : IDim
	{
		void SetColor(byte warmWhite, byte coldWhite, byte red, byte green, byte blue);
	}
}