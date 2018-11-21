using System;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife.Devices.Z_Wave.OOMI
{
	public class Bulb : IDim
	{
		private readonly Node _node;

		public Bulb(Node node)
		{
			_node = node;
		}

		public string DeviceId => $"Z-Wave #{_node.NodeID} Bulb";
		public event EventHandler<SensorReport> StateChanged;

		public async void Switch(bool state)
		{
			if (state)
				await _node.GetCommandClass<Basic>().Set(0xFF);
			else 
				await _node.GetCommandClass<Basic>().Set(0x00);
		}

		public void Dim(int percent)
		{
			var config = _node.GetCommandClass<SwitchMultiLevel>();

			var blah = Convert.ToByte(percent);

			var task1 = config.Set(blah);
			task1.Wait();
		}
	}

	public interface IDim : IDevice
	{
		void Dim(int percent);
	}
}
