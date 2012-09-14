using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IFileSystem
	{
		bool FileExists(string path);
	}
}
