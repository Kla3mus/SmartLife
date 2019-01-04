using System;
using System.Collections.Generic;
using SmartLife.Interfaces;

namespace SmartLife
{
	public interface ISmartHouseFramework
	{
		event EventHandler<Log> Logged;
		void Start();
		IEnumerable<IDevice> Devices { get; }
	}
}