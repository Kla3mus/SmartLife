using System;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;
using ZWave.Devices.Zipato;

namespace SmartLife.Devices.Z_Wave.OOMI
{
	public class Bulb : IColorLight
	{
		private readonly Node _node;
		private readonly RgbwLightBulb _nbu;

		public Bulb(Node node)
		{
			_node = node;
			_nbu = new RgbwLightBulb(node);
		}

		public string DeviceId => $"Z-Wave #{_node.NodeID} Bulb";

		public void SetColor(byte warmWhite, byte coldWhite, byte red, byte green, byte blue)
		{
			_nbu.SetColor(warmWhite, coldWhite, red, green, blue);
		}

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
			var task = config.Set(Convert.ToByte(percent));
			task.Wait();
		}
	}
}
