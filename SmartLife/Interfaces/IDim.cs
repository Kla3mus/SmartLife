using SmartLife.Interfaces;

namespace SmartLife.Interfaces
{
	public interface ILightSwitch : IDevice
	{
		void Switch(bool state);
	}
	public interface IDim : ILightSwitch
	{
		void Dim(int percent);
	}
	public interface IColorLight : IDim
	{
		void SetColor(byte warmWhite, byte coldWhite, byte red, byte green, byte blue);
	}
}