using SmartLife.Interfaces;

namespace SmartLife.Interfaces
{
	public interface ISwitch : IDevice
	{
		void Switch(bool state);
	}
	public interface IDim : ISwitch
	{
		void Dim(int percent);
	}
	public interface IColorLight : IDim
	{
		void SetColor(byte warmWhite, byte coldWhite, byte red, byte green, byte blue);
	}
}