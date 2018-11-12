using System;

namespace SmartLife_Console
{
	public class EntryPoint
	{
		static void Main(string[] args)
		{
			var p = new Program();
			while (true)
				Console.WriteLine(p.DoAction(Console.ReadLine()));
		}
	}
}