using System;

namespace SmartLife
{
	public class Log
	{
		public string Message { get; set; }
		public LevelEnum Level { get; set; }
		public Exception Exception { get; set; }
		public enum LevelEnum
		{
			Debug, Info, Error, Fatal
		}
	}
}