using System.Collections.Generic;
using SmartLife.Devices;
using SmartLife.Devices.Z_Wave.AeoTec;
using SmartLife.Devices.Z_Wave.OOMI;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife
{
	public class ManufacturerWrapper
	{
		private readonly ManufacturerSpecificReport _msr;

		public ManufacturerWrapper(ManufacturerSpecificReport ms)
		{
			_msr = ms;
		}

		public IDevice Device
		{
			get
			{
				if (_msr.ProductID == 4099 && _msr.ManufacturerID == 271 && _msr.ProductType == 1538)
					return new WallPlug(_msr.Node);

				if (_msr.ProductID == 100 && _msr.ManufacturerID == 134 && _msr.ProductType == 2)
					return new MultiSensor6(_msr.Node);

				if (_msr.ProductID == 98 && _msr.ManufacturerID == 362 && _msr.ProductType == 3)
					return new Bulb(_msr.Node);

				return new UnknownDevice(_msr.Node);
			}
		}
	}

	public class DeviceFactory
	{
		public List<IDevice> Devices { get; } = new List<IDevice>();

		public DeviceFactory(IEnumerable<Node> nodes)
		{
			foreach (var node in nodes)
			{
				var result = node.GetCommandClass<ManufacturerSpecific>().Get();
				result.Wait();

				var wrapper = new ManufacturerWrapper(result.Result);
				Devices.Add(wrapper.Device);
			}
		}

	}
}
