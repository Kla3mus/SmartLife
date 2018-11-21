using System.Collections.Generic;
using Newtonsoft.Json;
using SmartLife.Interfaces;

namespace SmartLife
{
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

			Device   = device;
			DeviceId = device.DeviceId;
		}

		[JsonIgnore]
		public IDevice Device { get; }
	}
}