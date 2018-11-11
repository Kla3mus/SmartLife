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

			File.WriteAllText(_fileName, JsonConvert.SerializeObject(data));
		}

		public IEnumerable<T> Get()
		{
			var text = File.ReadAllText(_fileName);
			Newtonsoft.Json.Linq.JArray jsonResponse = JsonConvert.DeserializeObject(text) as Newtonsoft.Json.Linq.JArray;
			return jsonResponse.ToObject<List<T>>();
		}
	}
}