using System;
using System.IO;
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
	public class MultiSensor6 : ITemperatureMeasure, IHumidityMeasure, IUvMeasure, ILuxMeasure, IVibrationSensor, IMotionSensor
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
				                                         if (args.Report.Type == AlarmType.General &&
				                                             args.Report.Detail == AlarmDetailType
					                                             .TamperingProductCoveringRemoved)
															 VibrationSensorTriggered?.Invoke(this, new VibrationSensorReport(true));
			                                         };
			node.GetCommandClass<Basic>().Changed += (sender, args) =>
			                                         {
				                                         bool? isDetected = null;

				                                         if (0x00 == args.Report.Value)
					                                         isDetected = false;
				                                         if (0xFF == args.Report.Value)
					                                         isDetected = true;

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

		public string DeviceId => $"Z-Wave #{_node.NodeID} MultiSensor6";
		public event EventHandler<MeasurementReport<float>> UVMeasurementTaken;
		public event EventHandler<MeasurementReport<float>> HumidityMeasurementTaken;
		public event EventHandler<MeasurementReport<float>> LuxMeasurementTaken;
		public event EventHandler<MeasurementReport<float>> TemperatureMeasurementTaken;
		public event EventHandler<SensorReport> MotionSensorTriggered;
		public event EventHandler<SensorReport> VibrationSensorTriggered;
	}
}