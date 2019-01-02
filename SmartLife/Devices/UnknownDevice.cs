using SmartLife.Interfaces;
using ZWave;

namespace SmartLife.Devices
{
	public class UnknownDevice : Device, IDevice
	{
		protected readonly Node Node;
		public UnknownDevice(Node node) { Node = node; }
		public override string DeviceId => $"Z-Wave #{Node.NodeID} Unknown";
	}

	public class UnResponsiveDevice : UnknownDevice
	{
		public override string DeviceId => $"Z-Wave #{Node.NodeID} UnResponsive";
		public UnResponsiveDevice(Node node) : base(node) { }
	}
}
