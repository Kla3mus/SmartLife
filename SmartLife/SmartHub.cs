using System;
using System.Collections.Generic;
using System.Linq;
using RJCP.IO.Ports;
using SmartLife.Devices;
using SmartLife.Interfaces;
using ZWave;
using ZWave.Channel;

namespace SmartLife
{
	public interface ILogger
	{
		void Log(string s);
	}

	public class SmartHub : IDisposable
	{
		private readonly ILogger _logger;

		readonly IStorage<DeviceWrapper> _deviceWrapperStorage;
		public SmartHub(ILogger logger, IStorage<DeviceWrapper> deviceWrapperStorage = null, string portName = null)
		{
			logger.Log("Initializing Smartlife");
			Operations = new List<IOperation>();
			_logger = logger;
			_deviceWrapperStorage = deviceWrapperStorage;

			//We're gong to change this in the future, when we add support for more frameworks
			if (string.IsNullOrEmpty(portName))
				portName = SerialPortStream.GetPortNames().First(element => element != "COM1");

			logger.Log($"Starting Smartlife using {portName}");

			var controller = new ZWaveController(new SerialPort(portName));
			controller.ChannelClosed += (sender, args) => { logger.Log($"ChannelClosed {args}"); };
			controller.Error += (sender, args) => { logger.Log($"Error {args.Error.Message}"); };
			controller.Open();

			var nodes = controller.GetNodes();
			nodes.Wait();
			var deviceFactory = new DeviceFactory(nodes.Result.Where(x => x.NodeID != 001).ToList());
			Devices = deviceFactory.Devices;

			foreach (var device in Devices.Where(x => x is UnknownDevice))
				logger.Log($"{device.DeviceId} is unknown");

			foreach (var device in Devices.Where(x => x is UnResponsiveDevice))
				logger.Log($"{device.DeviceId} is unresponsive");

			EventLogging();
			logger.Log("Smartlife Initialized");
		}

		private void EventLogging()
		{
			foreach (var deviceWrapper in DeviceWrappers)
			{
				var device = deviceWrapper.Device;

				if (device is IPowerPlug)
					((IPowerPlug)device).StateChanged += (sender, report) => { _logger.Log($"{device.DeviceId} IPowerPlug {report.Value}"); };

				if (device is ITemperatureMeasure)
					((ITemperatureMeasure)device).TemperatureMeasurementTaken += (sender, report) => { _logger.Log($"{device.DeviceId} ITemperatureMeasure {report.Value} {report.Unit}"); };

				if (device is IMotionSensor)
					((IMotionSensor)device).MotionSensorTriggered += (sender, report) => { _logger.Log($"{device.DeviceId} IMotionSensor {report.Value}"); };

				if (device is IPowerMeasure)
					((IPowerMeasure)device).PowerMeasurementTaken += (sender, report) => { _logger.Log($"{device.DeviceId} IPowerMeasure {report.Value} {report.Unit}"); };

				if (device is ILuxMeasure)
					((ILuxMeasure)device).LuxMeasurementTaken += (sender, report) => { _logger.Log($"{device.DeviceId} ILuxMeasure {report.Value} {report.Unit}"); };
			}
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

		public void Dispose()
		{
			_logger.Log("Closing SmartLife");
		}
	}
}
