﻿using System;

namespace SmartLife.core.Demo
{
	public class EntryPoint
	{
		static void Main(string[] args)
		{
			var logger = new ConsoleLogger();
			var p = new Program(logger);
			while (true)
				logger.Log(p.DoAction(Console.ReadLine()));

		}
	}
}