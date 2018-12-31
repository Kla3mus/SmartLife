using System;
using System.Threading.Tasks;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife.Devices.Z_Wave.AeoTec
{
	public enum MotionSensorSensitivity
	{
		Minimum_0 = 0
		, Low_1 = 1
		, MediumLow_2 = 2
		, MediumHigh_3 = 3
		, High_4 = 4
		, Maximum_5 = 5
	}
	public class MultiSensor6Settings
	{
		private readonly Node _node;
		private TimeSpan? _originalMotionSensorUpdateTime;
		private MotionSensorSensitivity _originalType;

		private bool HasRunUpdateSettings;
		public MultiSensor6Settings(Node node) { _node = node; }

		public TimeSpan? MotionSensorUpdateTime { get; set; }

		public MotionSensorSensitivity Type { get; set; }

		public async void GetUpdatedSettings()
		{
			await GetMotionSensorUpdateTime();
			HasRunUpdateSettings = true;
		}

		public async void ApplyChanges()
		{
			if (!HasRunUpdateSettings)
				return;

			if (_originalMotionSensorUpdateTime != MotionSensorUpdateTime)
				await SetMotionSensorUpdateTime();
		}

		private async Task SetMotionSensorUpdateTime()
		{
			if (!MotionSensorUpdateTime.HasValue)
				return;

			int? value;

			if (MotionSensorUpdateTime.Value.TotalSeconds >= 10)
				value = Convert.ToSByte(MotionSensorUpdateTime.Value.TotalSeconds);
			else
				value = 0xF0; //Will add support for this later..

			if (!value.HasValue)
				return;

			await _node.GetCommandClass<Configuration>().Set(0x3, value.Value);
		}

		private async Task GetMotionSensorUpdateTime()
		{
			var command = _node.GetCommandClass<Configuration>();
			var value   = Convert.ToInt16((await command.Get(3)).Value);

			if (value >= 10 && value <= 255) { _originalMotionSensorUpdateTime = new TimeSpan(0, 0, value); }

			else if (value >= 256 && value <= 3600)
			{
				if (Math.DivRem(value, 60, out var result) == 0)
					_originalMotionSensorUpdateTime = new TimeSpan(0, value, 0);
				else
					_originalMotionSensorUpdateTime = new TimeSpan(0, value + 1, 0);
			}
			else { _originalMotionSensorUpdateTime = null; }

			MotionSensorUpdateTime = _originalMotionSensorUpdateTime;
		}
	}
}