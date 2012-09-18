using System;
using System.Collections.Generic;
using System.IO;

namespace Snaphappi
{
	class FileSystem : IFileSystem
	{
		#region IFileSystem Members

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public IEnumerable<string> ListFiles(string folder)
		{
			return Directory.GetFiles(folder);
		}

		#endregion
	}
}
