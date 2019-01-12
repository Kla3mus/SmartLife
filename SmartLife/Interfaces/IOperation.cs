using System.Collections.Generic;

namespace SmartLife.Interfaces
{
	public interface IOperation
	{
		bool IsActive { get; }
		void Attach();
		void Detach();
		IList<IDevice> Devices { get; }
		OperationInformation OperationInformation { get; }
	}
}