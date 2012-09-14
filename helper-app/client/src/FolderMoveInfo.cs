using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public class FolderMoveInfo
	{
		public readonly string srcPath;
		public readonly string dstPath;

		public FolderMoveInfo(string srcPath, string dstPath)
		{
			this.srcPath = srcPath;
			this.dstPath = dstPath;
		}
	}
}
