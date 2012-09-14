using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public class OriginalFileInfo
	{
		public readonly string directory;
		public readonly string relativePath;
		public readonly int    hash;

		public OriginalFileInfo
			( string directory
			, string relativePath
			, int    hash
			)
		{
			this.directory    = directory;
			this.relativePath = relativePath;
			this.hash         = hash;
		}
	}
}
