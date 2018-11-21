using SmartLife.Interfaces;
using ZWave;

namespace SmartLife.Devices
{
	public class UnknownDevice : IDevice
	{
		private Node node;

		public UnknownDevice(Node node)
		{
			this.node = node;
		}

		public string DeviceId => $"Z-Wave #{node.NodeID} Unknown";
	}
}
