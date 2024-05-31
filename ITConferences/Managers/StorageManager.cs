using Newtonsoft.Json;
using System.Reflection;

namespace ITConferences.Managers
{
	public class StorageManager<T>
	{
		private readonly string FilePath;

		public StorageManager(string fileName)
		{
			string assemblyLocation = Assembly.GetExecutingAssembly().Location;

			string? rootPath = Path.GetDirectoryName(assemblyLocation);

			//if (!string.IsNullOrEmpty(rootPath))
			//{
			//	while (!Directory.GetFiles(rootPath, "*.csproj").Any())
			//	{
			//		rootPath = Directory.GetParent(rootPath)?.FullName;
			//	}
			//}

			var parentPath = Path.Combine(rootPath, "Database");

            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }

            this.FilePath = Path.Combine(parentPath, fileName);
        }

		public List<T> GetAll()
		{
			if (!File.Exists(FilePath))
				return new List<T>();

			string json = File.ReadAllText(FilePath);
			var res = JsonConvert.DeserializeObject<List<T>>(json);

			if (res == null) return new List<T>();

			return res;
		}

		public void Add(T item)
		{
			List<T> items = GetAll();
			items.Add(item);
			SaveAll(items);
		}

		public void MultiAdd(List<T> items)
		{
			List<T> currents = GetAll();
			currents.AddRange(items);
			SaveAll(currents);
		}

		public void Update(Func<T, bool> predicate, Action<T> updateAction)
		{
			List<T> items = GetAll();
			var itemToUpdate = items.FirstOrDefault(predicate);
			if (itemToUpdate != null)
			{
				updateAction(itemToUpdate);
				SaveAll(items);
			}
		}

		public void Delete(Func<T, bool> predicate)
		{
			List<T> items = GetAll();
			var itemToDelete = items.FirstOrDefault(predicate);
			if (itemToDelete != null)
			{
				items.Remove(itemToDelete);
				SaveAll(items);
			}
		}

		public void SaveAll(List<T> items)
		{
			using (StreamWriter sw = new StreamWriter(FilePath, false))
			{
				string json = JsonConvert.SerializeObject(items, Formatting.Indented);

				sw.Write(json);
			}
		}
	}
}
