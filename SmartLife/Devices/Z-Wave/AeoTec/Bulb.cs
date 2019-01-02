using ZWave;

namespace SmartLife.Devices.Z_Wave.AeoTec
{
	public class Bulb : OOMI.Bulb
	{
		public Bulb(Node node) : base(node) { }

		public override string DeviceId => $"Z-Wave #{Node.NodeID} AoeTec Led Bulb";

	}
}
