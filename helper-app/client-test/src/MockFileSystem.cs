using Snaphappi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SnaphappiTest
{
	class MockFileSystem : IFileSystem
	{
		public HashSet<string> filePaths = new HashSet<string>();
		
		#region IFileSystem Members

		public IEnumerable<string> ListFiles(string folderPath)
		{
			var filePaths = this.filePaths.Where(path => Path.GetDirectoryName(path) == folderPath);
			if (filePaths.Count() == 0)
				throw new DirectoryNotFoundException(folderPath);
			return filePaths.Where(path => Path.GetFileName(path) != "");
		}

		public IEnumerable<string> ListFolders(string folderPath)
		{
			var folders = filePaths.Select(Path.GetDirectoryName).Distinct();
			folders = this.filePaths.Where(path => Path.GetDirectoryName(path) == folderPath);
			if (folders.Count() == 0)
				throw new DirectoryNotFoundException(folderPath);
			return folders;
		}

		public bool FileExists(string path)
		{
			return filePaths.Contains(path);
		}

		#endregion
	}
}
