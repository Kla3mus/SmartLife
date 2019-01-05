using System;
using System.Threading.Tasks;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife.net.Devices.AeoTec
{
	public class MultiSensor6Settings : ISettings
	{
		private readonly Node _node;

		public MotionSensorSettings MotionSensorSettings => new MotionSensorSettings(_node);
		//public VibrationSensorSettings VibrationSensorSettings => new VibrationSensorSettings(_node);
		//public TemperatureSensorSettings TemperatureSensorSettings => new TemperatureSensorSettings(_node);
		//public LuxSensorSettings LuxSensorSettings => new LuxSensorSettings(_node);
		//public UvSensorSettings UvSensorSettings => new UvSensorSettings(_node);
		//public HumiditySensorSettings HumiditySensorSettings => new HumiditySensorSettings(_node);

		public MultiSensor6Settings(Node node)
		{
			_node = node;
			GetUpdateTime().Wait();
		}

		private TimeSpan? _originalUpdateTime;

		public TimeSpan? UpdateTime { get; set; }

		public async void ApplyChanges()
		{
			if (_originalUpdateTime != UpdateTime)
				await SetUpdateTime();
		}

		private async Task SetUpdateTime()
		{
			if (!UpdateTime.HasValue)
				return;

			int? value = Convert.ToSByte(UpdateTime.Value.TotalSeconds);

			await _node.GetCommandClass<Configuration>().Set(0x6f, value.Value);
			await _node.GetCommandClass<Configuration>().Set(0x70, value.Value);
			await _node.GetCommandClass<Configuration>().Set(0x71, value.Value);
		}

		private async Task GetUpdateTime()
		{
			var command = _node.GetCommandClass<Configuration>();
			var reportGroup1   = Convert.ToInt16((await command.Get(0x6f)).Value);

			_originalUpdateTime = UpdateTime = TimeSpan.FromSeconds(reportGroup1);
		}
	}
}