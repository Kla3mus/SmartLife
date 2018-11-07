using System.Threading.Tasks;

namespace SmartLife.Interfaces
{
	public interface ILedRing : IDivice
	{
		void SetEnabledColor(EnabledLedRingColor color);
		void SetDisabledColor(DisabledLedRingColor color);
		Task<DisabledLedRingColor> GetDisabledColor();
		Task<EnabledLedRingColor> GetEnabledColor();
	}
	public enum EnabledLedRingColor : byte
	{
		Off = 0,
		PowerLoadContinous = 1,
		PowerLoadStep = 2,
		White = 3,
		Red = 4,
		Green = 5,
		Blue = 6,
		Yellow = 7,
		Cyan = 8,
		Magenta = 9,
	}

	public enum DisabledLedRingColor : byte
	{
		Off = 0,
		LastMeasuredPower = 1,
		White = 3,
		Red = 4,
		Green = 5,
		Blue = 6,
		Yellow = 7,
		Cyan = 8,
		Magenta = 9,
	}
}