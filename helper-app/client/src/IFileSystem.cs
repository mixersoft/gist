using System;
using System.Collections.Generic;

namespace Snaphappi
{
	public interface IFileSystem
	{
		bool FileExists(string path);

		bool FolderExists(string folderPath);

		IEnumerable<string> ListFiles(string folderPath);

		IEnumerable<string> ListFolders(string folderPath);
	}
}
