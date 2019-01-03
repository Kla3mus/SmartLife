using SmartLife.Interfaces;
using ZWave;

namespace SmartLife.Devices
{
	public class UnknownDevice : Device, IDevice
	{
		protected readonly Node Node;
		public UnknownDevice(Node node) { Node = node; }
		public override string DeviceId => $"Z-Wave_{Node.NodeID}_Unknown";
	}

	public class UnResponsiveDevice : Device, IDevice
	{
		protected readonly Node Node;
		public UnResponsiveDevice(Node node) { Node = node; }
		public override string DeviceId => $"Z-Wave_{Node.NodeID}_UnResponsive";

	}
}
