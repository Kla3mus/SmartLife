using ZWave;

namespace SmartLife.Devices.Z_Wave.AeoTec
{
	public class Bulb : OOMI.Bulb
	{
		public Bulb(Node node) : base(node) { }

		public override string DeviceId => $"Z-Wave_{Node.NodeID}_AoeTec_Led_Bulb";

	}
}
