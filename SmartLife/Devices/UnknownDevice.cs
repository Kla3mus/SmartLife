using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife.Devices
{
	public class UnknownDevice : Device
	{
		protected readonly Node Node;
		protected readonly ManufacturerSpecificReport msr;
		public UnknownDevice(Node msrNode, ManufacturerSpecificReport msr)
		{
			Node = msrNode;
			this.msr = msr;
		}
		public override string DeviceId => $"Z-Wave_{Node.NodeID}_Unknown[productId|{msr.ProductID}]_[productType|{msr.ProductType}]_[manufacturerID|{msr.ManufacturerID}]";
	}

	public class UnResponsiveDevice : Device
	{
		protected readonly Node Node;
		public UnResponsiveDevice(Node node) { Node = node; }
		public override string DeviceId => $"Z-Wave_{Node.NodeID}_UnResponsive";

	}
}
