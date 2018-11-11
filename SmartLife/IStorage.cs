using System.Collections.Generic;

namespace SmartLife
{
	public interface IStorage<T>
	{
		void Save(IEnumerable<T> data);
		IEnumerable<T> Get();
	}
}