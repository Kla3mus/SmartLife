using System;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife.Devices.Z_Wave.AeoTec
{
	/// <summary>
	/// Protocol: Z-Wave
	/// Company: AeoTec by Aeon LABS
	/// Device: MultiSensor 6 (Z-Wave Multisensor)
	/// Model: ZW100-C
	/// </summary>
	public class MultiSensor6 : ITemperatureMeasure, IHumidityMeasure, IUvMeasure, ILuxMeasure, IVibrationSensor, IMotionSensor
	{
		public event EventHandler<IMeasurementReport> TemperatureMeasurementTaken;
		public event EventHandler<IMeasurementReport> HumidityMeasurementTaken;
		public event EventHandler<IMeasurementReport> UVMeasurementTaken;
		public event EventHandler<IMeasurementReport> LuxMeasurementTaken;
		private readonly Node _node;

		public MultiSensor6(Node node)
		{
			_node = node;
			node.GetCommandClass<SensorMultiLevel>().Changed += (sender, args) =>
			                                                    {
				                                                    if (args.Report.Type == SensorType.Temperature)
					                                                    TemperatureMeasurementTaken?.Invoke(this , new TemperatureMeasuredReport(args.Report.Value , args .Report .Unit));

				                                                    if (args.Report.Type == SensorType.Luminance)
					                                                    LuxMeasurementTaken?.Invoke(this, new LuxMeasureReport(args.Report.Value, args.Report.Unit));
																	
				                                                    if (args.Report.Type == SensorType.RelativeHumidity)
																		HumidityMeasurementTaken?.Invoke(this, new HumidityMeasureReport(args.Report.Value, args.Report.Unit));
				                                                    
				                                                    if (args.Report.Type == SensorType.Ultraviolet)
																		UVMeasurementTaken?.Invoke(this, new UVMeasureReport(args.Report.Value, args.Report.Unit));
																};
			//node.GetCommandClass<Alarm>().Changed            += Alarm_Changed;
			//node.GetCommandClass<Basic>().Changed += (sender, args) => { };
		}

		public string DeviceId => $"Z-Wave #{_node.NodeID}";

	}
}
