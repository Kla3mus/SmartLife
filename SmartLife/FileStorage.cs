using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SmartLife
{
	public class FileStorage<T> : IStorage<T>
	{
		private readonly string _fileName;
		public FileStorage(string fileName)
		{
			_fileName = fileName;
		}
		public void Save(IEnumerable<T> data)
		{
			if (File.Exists(_fileName))
				File.Delete(_fileName);

			File.WriteAllText(_fileName, JsonConvert.SerializeObject(data, new JsonSerializerSettings
			                                                               {
																			   Formatting = Formatting.Indented
			                                                               }));
		}

		public IEnumerable<T> Get()
		{
			if (!File.Exists(_fileName))
				return null;
			
			var text = File.ReadAllText(_fileName);
			return JsonConvert.DeserializeObject<List<T>>(text);
		}
	}
}