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

		public bool FolderExists(string path)
		{
			return Directory.Exists(path);
		}

		public IEnumerable<string> ListFiles(string folderPath)
		{
			return Directory.GetFiles(folderPath);
		}

		public IEnumerable<string> ListFolders(string folderPath)
		{
			return Directory.GetDirectories(folderPath);
		}

		#endregion
	}
}
