using System;
using System.IO;
using System.Threading.Tasks;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife.Devices.Z_Wave.AeoTec
{
	/// <summary>
	///     Protocol: Z-Wave
	///     Company: AeoTec by Aeon LABS
	///     Device: MultiSensor 6 (Z-Wave Multisensor)
	///     Model: ZW100-C
	/// </summary>
	public class MultiSensor6 : ITemperatureMeasure, IHumidityMeasure, IUvMeasure, ILuxMeasure, IVibrationSensor
	                            , IMotionSensor
	{
		private readonly Node _node;

		public MultiSensor6(Node node)
		{
			_node    = node;
			Settings = new MultiSensor6Settings(_node);
			node.GetCommandClass<SensorMultiLevel>().Changed += (sender, args) =>
			                                                    {
				                                                    if (args.Report.Type == SensorType.Temperature)
					                                                    TemperatureMeasurementTaken
						                                                    ?.Invoke(this, new TemperatureMeasuredReport(args.Report.Value
							                                                                                       , args
							                                                                                         .Report
							                                                                                         .Unit));

				                                                    if (args.Report.Type == SensorType.Luminance)
					                                                    LuxMeasurementTaken
						                                                    ?.Invoke(this
						                                                             , new
							                                                             LuxMeasureReport(args.Report.Value
							                                                                              , args
							                                                                                .Report
							                                                                                .Unit));

				                                                    if (args.Report.Type == SensorType.RelativeHumidity)
					                                                    HumidityMeasurementTaken
						                                                    ?.Invoke(this
						                                                             , new
							                                                             HumidityMeasureReport(args.Report.Value
							                                                                                   , args
							                                                                                     .Report
							                                                                                     .Unit));

				                                                    if (args.Report.Type == SensorType.Ultraviolet)
					                                                    UVMeasurementTaken
						                                                    ?.Invoke(this
						                                                             , new
							                                                             UVMeasureReport(args.Report.Value
							                                                                             , args
							                                                                               .Report
							                                                                               .Unit));
			                                                    };
			node.GetCommandClass<Alarm>().Changed += (sender, args) =>
			                                         {
				                                         VibrationSensorTriggered
					                                         ?.Invoke(this, new VibrationSensorReport(true));
			                                         };
			node.GetCommandClass<Basic>().Changed += (sender, args) =>
			                                         {
				                                         bool? isDetected = null;

				                                         if (0x00 == args.Report.Value)
					                                         isDetected = true;
				                                         if (0xFF == args.Report.Value)
					                                         isDetected = false;

				                                         if (isDetected == null)
					                                         throw new
						                                         InvalidDataException($"Value from report was {args.Report.Value}, only expect 0x00 or 0xFF");

				                                         MotionSensorTriggered?.Invoke(this
				                                                                       , new
					                                                                       MotionSensorReport(isDetected
						                                                                                          .Value));
			                                         };
		}

		public MultiSensor6Settings Settings { get; }
		public event EventHandler<MeasurementReport<float>> HumidityMeasurementTaken;
		public event EventHandler<MeasurementReport<float>> LuxMeasurementTaken;
		public event EventHandler<SensorReport> MotionSensorTriggered;
		public event EventHandler<MeasurementReport<float>> TemperatureMeasurementTaken;

		public string DeviceId => $"Z-Wave #{_node.NodeID}";
		public event EventHandler<MeasurementReport<float>> UVMeasurementTaken;

		public event EventHandler<SensorReport> VibrationSensorTriggered;
	}

	public class MultiSensor6Settings
	{
		private readonly Node _node;

		private TimeSpan? _originalMotionSensorUpdateTime;

		private MotionSensorSensitivity _originalType;

		private bool HasRunUpdateSettings;
		public MultiSensor6Settings(Node node) { _node = node; }

		public TimeSpan? MotionSensorUpdateTime { get; set; }

		public MotionSensorSensitivity Type { get; set; }

		public async void GetUpdatedSettings()
		{
			GetMotionSensorUpdateTime().Wait();
			HasRunUpdateSettings = true;
		}

		public async void ApplyChanges()
		{
			if (!HasRunUpdateSettings)
				return;

			if (_originalMotionSensorUpdateTime != MotionSensorUpdateTime)
				await SetMotionSensorUpdateTime();
		}

		private async Task SetMotionSensorUpdateTime()
		{
			if (!MotionSensorUpdateTime.HasValue)
				return;

			int? value;

			if (MotionSensorUpdateTime.Value.TotalSeconds >= 10)
				value = Convert.ToSByte(MotionSensorUpdateTime.Value.TotalSeconds);
			else
				value = 0xF0; //Will add support for this later..

			if (!value.HasValue)
				return;

			await _node.GetCommandClass<Configuration>().Set(0x3, value.Value);
		}

		private async Task GetMotionSensorUpdateTime()
		{
			var command = _node.GetCommandClass<Configuration>();
			var value   = Convert.ToInt16((await command.Get(3)).Value);

			if (value >= 10 && value <= 255) { _originalMotionSensorUpdateTime = new TimeSpan(0, 0, value); }

			else if (value >= 256 && value <= 3600)
			{
				if (Math.DivRem(value, 60, out var result) == 0)
					_originalMotionSensorUpdateTime = new TimeSpan(0, value, 0);
				else
					_originalMotionSensorUpdateTime = new TimeSpan(0, value + 1, 0);
			}
			else { _originalMotionSensorUpdateTime = null; }

			MotionSensorUpdateTime = _originalMotionSensorUpdateTime;
		}
	}

	public enum MotionSensorSensitivity
	{
		Minimum_0 = 0
		, Low_1 = 1
		, MediumLow_2 = 2
		, MediumHigh_3 = 3
		, High_4 = 4
		, Maximum_5 = 5
	}
}