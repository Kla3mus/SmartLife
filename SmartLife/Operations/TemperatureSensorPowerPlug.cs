using System.Collections.Generic;
using SmartLife.Interfaces;

namespace SmartLife.core.Demo
{
	public class TemperatureSensorPowerPlug : IOperation
	{
		private readonly ISwitch _powerPlug;
		private readonly float _temperatureSwitch;
		private bool _currentSocketState;

		/// <summary>
		///  If tempreature is over set value, switches will be activated
		/// </summary>
		/// <param name="temperatureMeasure"></param>
		/// <param name="powerPlug"></param>
		/// <param name="temperatureSwitch"></param>
		public TemperatureSensorPowerPlug(ITemperatureMeasure temperatureMeasure, ISwitch powerPlug, float temperatureSwitch)
		{
			_powerPlug = powerPlug;
			_temperatureSwitch = temperatureSwitch;
			((IStateChange)powerPlug).StateChanged += (sender, report) => { _currentSocketState = report.Value; };
			temperatureMeasure.TemperatureMeasurementTaken += TemperatureMeasurementTaken;

			Devices.Add(temperatureMeasure);
			Devices.Add(powerPlug);
		}

		private void TemperatureMeasurementTaken(object sender, MeasurementReport<float> e)
		{
			if (!IsActive)
				return;

			var newState = e.Value > _temperatureSwitch;
			if (_currentSocketState != newState)
				_powerPlug.Switch(newState);
		}

		public bool IsActive { get; private set; }

		public void Attach()
		{
			if (IsActive)
				return;

			IsActive = true;
		}

		public void Detach()
		{
			if (!IsActive)
				return;

			IsActive = false;
			_powerPlug.Switch(false);
		}

		public IList<IDevice> Devices { get; } = new List<IDevice>();
		public OperationInformation OperationInformation => new OperationInformation
		{
			Name = "I'm warm.. fan me!",
			Description = "If temperature falls below specified value, turn on fan",
			TypeFullName = nameof(TemperatureSensorPowerPlug)
		};
	}
}