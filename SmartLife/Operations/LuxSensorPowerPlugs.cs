using System;
using System.Collections.Generic;
using SmartLife.Interfaces;

namespace SmartLife.core.Demo
{
	public class LuxSensorPowerPlugs : IOperation
	{
		private readonly IList<IDim> _powerPlugs;

		public DateTime From { get; set; }
		public DateTime To { get; set; }

		public LuxSensorPowerPlugs(ILuxMeasure luxMeasure, IList<IDim> powerPlugs)
		{
			_powerPlugs = powerPlugs;

			Devices.Add(luxMeasure);
			foreach (var powerPlug in _powerPlugs)
			{
				powerPlug.Switch(false);
				Devices.Add(powerPlug);
			}

			luxMeasure.LuxMeasurementTaken += (sender, report) =>
			                                  {
				                                  var value = report.Value < 10;
												  if (IsActive) { 
					                                  foreach (IDim powerPlug in _powerPlugs)
						                                  powerPlug.Switch(value);
												  }
											  };
		}

		private bool currentSocketState = false;
		public bool IsActive { get; private set; }

		public void Attach()
		{
			if (IsActive)
				return;

			IsActive = true;
			if (currentSocketState != IsActive) { 
				foreach (IDim powerPlug in _powerPlugs)
					powerPlug.Switch(true);
			}
		}
		public void Detach()
		{
			if (!IsActive)
				return;

			IsActive = false;
		}

		public IList<IDevice> Devices { get; } = new List<IDevice>();
		public OperationInformation OperationInformation => new OperationInformation
		                                                    {
																Name = "Keep it lit!",
																Description = "Will adjust the dimmable lights so that the room keeps at minimum a defined lux.",
																TypeFullName = nameof(LuxSensorPowerPlugs)
		                                                    };
	}
}