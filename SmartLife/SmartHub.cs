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
		private ZWaveController _controller;
		private string ZWavePortName = null;

		readonly IStorage<DeviceWrapper> _deviceWrapperStorage;

		public SmartHub(ILogger logger, IStorage<DeviceWrapper> deviceWrapperStorage = null, string portName = null)
		{
			ZWavePortName = portName;
			_logger = logger;
			_logger.Log("Initializing Smartlife");
			Operations = new List<IOperation>();
			_deviceWrapperStorage = deviceWrapperStorage;

			Initialize();
		}

		private void Initialize()
		{
			//We're gong to change this in the future, when we add support for more frameworks
			if (string.IsNullOrEmpty(ZWavePortName))
				ZWavePortName = SerialPortStream.GetPortNames().First(element => element != "COM1");

			_logger.Log($"Starting Smartlife using {ZWavePortName}");

			_controller =  new ZWaveController(new SerialPort(ZWavePortName));
			_controller.ChannelClosed += (sender, args) => { _logger.Log($"ChannelClosed {args}"); };
			_controller.Error         += (sender, args) => { _logger.Log($"Error {args.Error.Message}"); };
			_controller.Open();

			var nodes = _controller.GetNodes();
			nodes.Wait();
			var deviceFactory = new DeviceFactory(nodes.Result.Where(x => x.NodeID != 001).ToList());
			Devices = deviceFactory.Devices;

			foreach (var device in Devices.Where(x => x is UnknownDevice))
				_logger.Log($"{device.DeviceId} is unknown");

			foreach (var device in Devices.Where(x => x is UnResponsiveDevice))
				_logger.Log($"{device.DeviceId} is unresponsive");

			EventLogging();
			_logger.Log("Smartlife Initialized");
		}

		private void EventLogging()
		{
			foreach (var deviceWrapper in DeviceWrappers)
			{
				var device = deviceWrapper.Device;

				if (device is IPowerPlug)
					((IPowerPlug)device).StateChanged += (sender, report) => { _logger.Log($"{device.DeviceId} IPowerPlug {report.Value}"); };

				if (device is IPowerMeasure)
					((IPowerMeasure)device).PowerMeasurementTaken += (sender, report) => { _logger.Log($"{device.DeviceId} IPowerMeasure {report.Value} {report.Unit}"); };

				if (device is ITemperatureMeasure)
					((ITemperatureMeasure)device).TemperatureMeasurementTaken += (sender, report) => { _logger.Log($"{device.DeviceId} ITemperatureMeasure {report.Value} {report.Unit}"); };

				if (device is IMotionSensor)
					((IMotionSensor)device).MotionSensorTriggered += (sender, report) => { _logger.Log($"{device.DeviceId} IMotionSensor {report.Value}"); };

				if (device is ILuxMeasure)
					((ILuxMeasure)device).LuxMeasurementTaken += (sender, report) => { _logger.Log($"{device.DeviceId} ILuxMeasure {report.Value} {report.Unit}"); };

				if (device is IUvMeasure)
					((IUvMeasure)device).UVMeasurementTaken += (sender, report) => { _logger.Log($"{device.DeviceId} IUvMeasure {report.Value} {report.Unit}"); };

				if (device is IHumidityMeasure)
					((IHumidityMeasure)device).HumidityMeasurementTaken += (sender, report) => { _logger.Log($"{device.DeviceId} IHumidityMeasure {report.Value} {report.Unit}"); };

				if (device is IVibrationSensor)
					((IVibrationSensor)device).VibrationSensorTriggered += (sender, report) => { _logger.Log($"{device.DeviceId} IVibrationSensor {report.Value}"); };
			}
		}

		public IEnumerable<IDevice> Devices { get; private set; }

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
