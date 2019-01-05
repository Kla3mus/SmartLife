using System;
using System.Threading.Tasks;
using ZWave;
using ZWave.CommandClasses;

namespace SmartLife.net.Devices.AeoTec
{
	public class MotionSensorSettings : ISettings
	{
		private readonly Node _node;
		private TimeSpan? _originalUpdateTime;
		private MotionSensorSensitivity _originalType;

		public MotionSensorSettings(Node node)
		{
			_node = node;
			GetUpdateTime().Wait();
		}

		/// <summary>
		/// Needs to be between 10 seconds -> 4min 15sec
		/// or
		/// between x min -> 60 minutes
		/// </summary>
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

			int? value;

			if (UpdateTime.Value.TotalSeconds >= 10)
				value = Convert.ToSByte(UpdateTime.Value.TotalSeconds);
			else
				value = 0xF0; //Will add support for this later..

			await _node.GetCommandClass<Configuration>().Set(3, value.Value);
		}

		private async Task GetUpdateTime()
		{
			var command = _node.GetCommandClass<Configuration>();
			var value   = Convert.ToInt16((await command.Get(3)).Value);

			if (value >= 10 && value <= 255) { _originalUpdateTime = new TimeSpan(0, 0, value); }

			else if (value >= 256 && value <= 3600)
			{
				if (Math.DivRem(value, 60, out var result) == 0)
					_originalUpdateTime = new TimeSpan(0, value, 0);
				else
					_originalUpdateTime = new TimeSpan(0, value + 1, 0);
			}
			else { _originalUpdateTime = null; }

			UpdateTime = _originalUpdateTime;
		}
	}
}