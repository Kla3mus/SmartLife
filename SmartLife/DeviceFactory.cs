using System.Collections.Generic;
using SmartLife.Interfaces;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife
{
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
