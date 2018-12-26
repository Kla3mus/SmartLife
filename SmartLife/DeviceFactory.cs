﻿using System.Collections.Generic;
using SmartLife.Devices;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife
{
	public class ManufacturerWrapper
	{
		private readonly ManufacturerSpecificReport _msr;
		private readonly Node _node;

		public ManufacturerWrapper(ManufacturerSpecificReport ms)
		{
			_msr = ms;
			_node = _msr.Node;
		}

		public ManufacturerWrapper(Node node)
		{
			_node = node;
		}

		public IDevice Device
		{
			get
			{
				if (_msr == null)
					return new UnResponsiveDevice(_node);

				if (_msr.ProductID == 4099 && _msr.ManufacturerID == 271 && _msr.ProductType == 1538)
					return new WallPlug(_msr.Node);

				if (_msr.ProductID == 100 && _msr.ManufacturerID == 134 && _msr.ProductType == 2)
					return new Devices.Z_Wave.AeoTec.MultiSensor6(_msr.Node);

				if (_msr.ProductID == 98 && _msr.ManufacturerID == 362 && _msr.ProductType == 3)
					return new Devices.Z_Wave.OOMI.Bulb(_msr.Node);

				if (_msr.ProductID == 98 && _msr.ManufacturerID == 134 && _msr.ProductType == 3)
					return new Devices.Z_Wave.AeoTec.Bulb(_msr.Node);

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
				ManufacturerWrapper wrapper;
				try
				{
					var result = node.GetCommandClass<ManufacturerSpecific>().Get();
					result.Wait();
					wrapper = new ManufacturerWrapper(result.Result);
				}
				catch
				{
					wrapper = new ManufacturerWrapper(node);
				}
				Devices.Add(wrapper.Device);
			}
		}
	}
}
