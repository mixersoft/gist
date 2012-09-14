using Snaphappi;
using System.Collections.Generic;
using System;

namespace SnaphappiTest
{
	class MockFileSystem : IFileSystem
	{
		public HashSet<string> filePaths = new HashSet<string>();
		
		#region IFileSystem Members

		public bool FileExists(string path)
		{
			return filePaths.Contains(path);
		}

		#endregion
	}
}
