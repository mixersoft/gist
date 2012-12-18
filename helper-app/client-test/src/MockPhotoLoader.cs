using Snaphappi;
using System;
using System.Collections.Generic;

namespace SnaphappiTest
{
	public class MockPhotoLoader : IPhotoLoader
	{
		#region data

		public Dictionary<string, byte[]> previews = new Dictionary<string,byte[]>();
		public Dictionary<string, int>    hashes   = new Dictionary<string,int>();

		#endregion

		#region IPhotoLoader Members

		public byte[] GetPreview(string path)
		{
			return previews[path];
		}

		public int GetHash(string path)
		{
			return hashes[path];
		}

		#endregion
	}
}
