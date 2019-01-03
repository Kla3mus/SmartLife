using System;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;
using ZWave.Devices.Zipato;

namespace SmartLife.Devices.Z_Wave.OOMI
{
	public class Bulb : Device, IColorLight
	{
		protected readonly Node Node;
		private readonly RgbwLightBulb _nbu;

		public Bulb(Node node)
		{
			Node = node;
			_nbu = new RgbwLightBulb(node);
		}

		public override string DeviceId => $"Z-Wave_{Node.NodeID}_OOMI_Bulb";

		public void SetColor(byte warmWhite, byte coldWhite, byte red, byte green, byte blue)
		{
			_nbu.SetColor(warmWhite, coldWhite, red, green, blue);
		}

		public event EventHandler<SensorReport> StateChanged;

		public async void Switch(bool state)
		{
			if (state)
				await Node.GetCommandClass<Basic>().Set(0xFF);
			else 
				await Node.GetCommandClass<Basic>().Set(0x00);
		}

		public void Dim(int percent)
		{
			var config = Node.GetCommandClass<SwitchMultiLevel>();
			var task = config.Set(Convert.ToByte(percent));
			task.Wait();
		}
	}
}
