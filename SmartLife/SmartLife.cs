using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RJCP.IO.Ports;
using SmartLife.Interfaces;
using ZWave;

namespace SmartLife
{
	public class SmartLife
	{
		readonly IStorage<DeviceWrapper> _deviceWrapperStorage;
		public SmartLife(IStorage<DeviceWrapper> deviceWrapperStorage = null, string portName = null)
		{
			Operations = new List<IOperation>();
			_deviceWrapperStorage = deviceWrapperStorage;
			//We're gong to change this in the future, when we add support for more frameworks
			if (string.IsNullOrEmpty(portName))
				portName = SerialPortStream.GetPortNames().First(element => element != "COM1");

			var controller = new ZWaveController(portName);
			controller.ChannelClosed += (sender, args) => { Console.WriteLine($"ChannelClosed {args}"); };
			controller.Error += (sender, args) => { Console.WriteLine($"Error {args.Error.Message}"); };
			controller.Open();

			var nodes = controller.GetNodes();
			nodes.Wait();

			var deviceFactory = new DeviceFactory(nodes.Result.Where(x => x.NodeID != 001).ToList());
			Devices = deviceFactory.Devices;
		}

		public IEnumerable<IDevice> Devices { get; }
		private IEnumerable<DeviceWrapper> deviceWrappers;

		public void SaveDeviceWrappers()
		{
			_deviceWrapperStorage?.Save(DeviceWrappers);
		}

		public IDictionary<string, List<DeviceWrapper>> Zones =>
			deviceWrappers.GroupBy(x => x.Zone)
			              .ToDictionary(x => x.Key, y => y.ToList());

		public IEnumerable<DeviceWrapper> DeviceWrappers
		{
			get
			{
				if (_deviceWrapperStorage == null)
					return Devices.Select(x => new DeviceWrapper(x));

				if (deviceWrappers != null && !deviceWrappers.Any())
					return deviceWrappers;

				if ((deviceWrappers == null || !deviceWrappers.Any()) && Devices.Any())
					return Devices.Select(x => new DeviceWrapper(x));

				var deviceWrappersFromStorage = _deviceWrapperStorage.Get();
				var result = new List<DeviceWrapper>();

				foreach (var device in Devices)
				{
					var wrapper = deviceWrappersFromStorage.FirstOrDefault(x => x.Device.DeviceId == device.DeviceId);
					if (deviceWrappersFromStorage.FirstOrDefault(x => x.Device.DeviceId == device.DeviceId) != null)
					{
						result.Add(new DeviceWrapper(device)
						           {
									   Description = wrapper.Description,
									   Name = wrapper.Name
						           });

					}
					else
						result.Add(new DeviceWrapper(device));
				}

				deviceWrappers = result;
				return result;
			}
		}


		public IEnumerable<IOperation> Operations { get; }

		public void AddOperation(IOperation operation)
		{
			operation.Attach();
			Operations.ToList().Add(operation);
		}
	}

	public class DeviceWrapper
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Zone { get; set; }
		public string DeviceId { get; set; }

		public DeviceWrapper(IDevice device)
		{
			Device = device;
			DeviceId = device.DeviceId;
		}

		[JsonIgnore]
		public IDevice Device { get; }
	}

	public class Zone
	{
		public IEnumerable<IDevice> Devices { get; set; }
	}
}
