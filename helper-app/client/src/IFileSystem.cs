using System;
using System.Collections.Generic;

namespace Snaphappi
{
	public interface IFileSystem
	{
		bool FileExists(string path);

		IEnumerable<string> ListFiles(string folderPath);
	}
}
