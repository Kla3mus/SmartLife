using System.Collections.Generic;
using System.Linq;
using SmartLife.Interfaces;

namespace SmartLife.core.Demo
{
	public class LuxSensorPowerPlugs : IOperation
	{
		private readonly ILuxMeasure _luxMeasure;
		private readonly List<IPowerPlug> _powerPlugs;

		public LuxSensorPowerPlugs(ILuxMeasure luxMeasure, IList<IPowerPlug> powerPlugs)
		{
			_luxMeasure = luxMeasure;
			_powerPlugs = powerPlugs.ToList();

			Devices.Add(luxMeasure);
			foreach (var powerPlug in powerPlugs)
			{
				Devices.Add(powerPlug);
				powerPlug.StateChanged += (sender, report) => { currentSocketState = report.Value; };
			}

			luxMeasure.LuxMeasurementTaken += (sender, report) =>
			                                  {
				                                  var value = report.Value < 10;
												  if (IsActive)
					                                  foreach (IPowerPlug powerPlug in _powerPlugs)
						                                  powerPlug.Switch(value);
			                                  };
		}

		private bool currentSocketState = false;
		public bool IsActive { get; private set; }

		public void Attach()
		{
			if (IsActive)
				return;

			IsActive = true;
			if (currentSocketState != IsActive)
				foreach (IPowerPlug powerPlug in _powerPlugs)
					powerPlug.Switch(true);
		}
		public void Detach()
		{
			if (!IsActive)
				return;

			IsActive = false;
		}

		public IList<IDevice> Devices { get; } = new List<IDevice>();
	}
}