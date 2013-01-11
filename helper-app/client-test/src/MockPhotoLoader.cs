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
		public Dictionary<string, string> times    = new Dictionary<string,string>();

		#endregion

		#region IPhotoLoader Members

		public byte[] GetPreview(string path)
		{
			return previews[path];
		}

		public int GetImageHash(string path)
		{
			return hashes[path];
		}

		public string GetImageDateTime(string path)
		{
			return times[path];
		}

		#endregion
	}
}
