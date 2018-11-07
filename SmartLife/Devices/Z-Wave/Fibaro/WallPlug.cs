using System;
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
		private readonly Node _node;

		public WallPlug(Node node)
		{
			_node = node;
			node.GetCommandClass<SwitchBinary>().Changed += OnChanged;
		}

		public event EventHandler<bool> SwitchStateChanged;
		private void OnChanged(object sender, ReportEventArgs<SwitchBinaryReport> e)
		{
			SwitchStateChanged?.Invoke(this, e.Report.Value);
		}

		public async void Switch(bool state)
		{
			await _node.GetCommandClass<SwitchBinary>().Set(state);
		}

		public event EventHandler<decimal> CurrentPowerLoad;
		public string DeviceId => $"Z-Wave #{_node.NodeID}";
	}
}