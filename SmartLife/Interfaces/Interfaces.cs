using System;

namespace SmartLife.Interfaces
{

	public interface IDivice
	{
		string DeviceId { get; }
	}

	public interface IPowerPlug : IDivice
	{
		void Switch(bool state);
		event EventHandler<bool> SwitchStateChanged;
	}

	public interface IPowerMeasure : IDivice
	{
		event EventHandler<float> CurrentPowerLoad;
	}

	public interface ITemperatureMeasure : IDivice
	{
		
	}

	public enum MeasurementType
	{
		Watt,
		Celsius,
		Lux,
	}
}