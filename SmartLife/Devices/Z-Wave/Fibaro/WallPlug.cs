using System;
using System.Threading.Tasks;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife
{
	/// <summary>
	///     Protocol: Z-Wave
	///     Company: Fibaro
	///     Device: Wall Plub
	///     Model: FGWPE/F-102 ZW5
	/// </summary>
	public class WallPlug : IPowerPlug, IPowerMeasure, ILedRing
	{
		public event EventHandler<SensorReport> StateChanged;
		public event EventHandler<MeasurementReport<float>> PowerMeasurementTaken;

		private readonly Node _node;

		public WallPlug(Node node)
		{
			_node                                        =  node;
			node.GetCommandClass<SwitchBinary>().Changed += (sender, args) => { StateChanged?.Invoke(this, new StateChangeReport(args.Report.Value)); };
			node.GetCommandClass<Meter>().Changed += (sender, args) => { PowerMeasurementTaken?.Invoke(this, new PowerMeasureMeasurementReport(args.Report.Value, args.Report.Unit)); };
		}

		public async void SetEnabledColor(EnabledLedRingColor color)
		{
			await _node.GetCommandClass<Configuration>().Set(41, (sbyte)color);
		}

		public async void SetDisabledColor(DisabledLedRingColor color)
		{
			await _node.GetCommandClass<Configuration>().Set(42, (sbyte)color);
		}

		public async Task<DisabledLedRingColor> GetDisabledColor()
		{
			var value = Convert.ToByte((await _node.GetCommandClass<Configuration>().Get(42)).Value);
			return (DisabledLedRingColor)value;
		}

		public async Task<EnabledLedRingColor> GetEnabledColor()
		{
			var value = Convert.ToByte((await _node.GetCommandClass<Configuration>().Get(41)).Value);
			return (EnabledLedRingColor)value;
		}

		

		public async void Switch(bool state) { await _node.GetCommandClass<SwitchBinary>().Set(state); }

		public string DeviceId => $"Z-Wave #{_node.NodeID}";
	}
}