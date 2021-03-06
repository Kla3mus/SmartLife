﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SmartLife.Interfaces
{
	public interface IDevice
	{
		string DeviceId { get; }
		List<string> Types { get; }
	}

	public abstract class Device : IDevice
	{
		public abstract string DeviceId { get; }

		public List<string> Types
		{
			get
			{
				var illegalInterfaces = new List<Type> {
					                                       typeof(IDevice),
														   typeof(IMeasure),
					                                       typeof(ISensor),
													   };

				return ((TypeInfo)GetType()).ImplementedInterfaces
				                               .Where(x => !illegalInterfaces.Contains(x))
				                               .Select(x => x.Name)
				                               .ToList();
			}
		}
	}

	public interface IMeasure : IDevice { }

	public interface ISensor : IDevice { }

	public interface IVibrationSensor : ISensor
	{
		event EventHandler<SensorReport> VibrationSensorTriggered;
	}

	public class VibrationSensorReport : SensorReport
	{
		public VibrationSensorReport(bool value) : base(ReportType.Vibration, value) { }
	}

	public interface IMotionSensor : IDevice
	{
		event EventHandler<SensorReport> MotionSensorTriggered;
	}

	public class MotionSensorReport : SensorReport
	{
		public MotionSensorReport(bool value) : base(ReportType.Motion, value) { }
	}
}