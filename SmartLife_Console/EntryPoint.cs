namespace SmartLife_Console
{
	public class EntryPoint
	{
		static void Main(string[] args)
		{
			var p = new Program();
			p.GetDevices().Wait();
			p.Run().Wait();
		}
	}
}