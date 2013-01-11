using Snaphappi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SnaphappiTest
{
	class MockFileSystem : IFileSystem
	{
		#region data

		public HashSet<string> filePaths = new HashSet<string>();

		#endregion // data

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
			return filePaths
				.Select(Path.GetDirectoryName)
				.Distinct()
				.Where(path => path != "" && Path.GetDirectoryName(path) == folderPath);
		}

		public bool FileExists(string path)
		{
			return filePaths.Contains(path);
		}

		public bool FolderExists(string path)
		{
			return filePaths.Any(p => Path.GetDirectoryName(p) == path);
		}

		#endregion // IFileSystem Members
	}
}
