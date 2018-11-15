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

		public void SaveDeviceWrappers()
		{
			_deviceWrapperStorage?.Save(DeviceWrappers);
		}

		public IDictionary<string, List<DeviceWrapper>> Zones 
		{
			get
			{
				var result = new Dictionary<string, List<DeviceWrapper>>();
				foreach (var zone in DeviceWrappers.SelectMany(x => x.Zones).Distinct())
					result.Add(zone, DeviceWrappers.Where(x => x.Zones.Any(y => x.Zones.Contains(y))).ToList());

				return result;
			}
		}

		private IEnumerable<DeviceWrapper> _deviceWrappers;
		public IEnumerable<DeviceWrapper> DeviceWrappers
		{
			get
			{
				if (_deviceWrapperStorage == null)
					return Devices.Select(x => new DeviceWrapper(x));

				if (_deviceWrappers != null && !_deviceWrappers.Any())
					return _deviceWrappers;

				var deviceWrappersFromStorage = _deviceWrapperStorage.Get();

				if (deviceWrappersFromStorage == null && Devices.Any())
					return Devices.Select(x => new DeviceWrapper(x));
				
				var result = new List<DeviceWrapper>();
				foreach (var device in Devices)
				{
					var wrapper = deviceWrappersFromStorage.FirstOrDefault(x => x.DeviceId == device.DeviceId);
					if (wrapper != null)
					{
						result.Add(new DeviceWrapper(device)
						           {
									   Description = wrapper.Description,
									   Name = wrapper.Name,
									   Zones = wrapper.Zones,
									   DeviceId = wrapper.DeviceId
						           });

					}
					else
						result.Add(new DeviceWrapper(device));
				}

				_deviceWrappers = result;
				return result;
			}
		}


		public IList<IOperation> Operations { get; }

		public void AddOperation(IOperation operation)
		{
			Operations.Add(operation);
		}
	}

	public class DeviceWrapper
	{
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public List<string> Zones { get; set; } = new List<string>();
		public string DeviceId { get; set; }

		public DeviceWrapper(IDevice device)
		{
			if (device == null)
				return;

			Device = device;
			DeviceId = device.DeviceId;
		}

		[JsonIgnore]
		public IDevice Device { get; }
	}
}
