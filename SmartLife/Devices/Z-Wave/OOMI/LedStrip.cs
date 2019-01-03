using ZWave;

namespace SmartLife.Devices.Z_Wave.OOMI
{
	/// <summary>
	/// Oomi LED Strip RGBW Z-Wave+ (FT121-C)
	/// </summary>
	public class LedStrip : OOMI.Bulb
	{
		public LedStrip(Node node) : base(node) { }
		public override string DeviceId => $"Z-Wave_{Node.NodeID}_OOMI_LED_STRIP_RGBW";

	}
}
