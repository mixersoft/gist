using NUnit.Framework;
using Snaphappi;
using System;
using System.IO;

namespace SnaphappiTest
{
	[ TestFixture ]
	public class TempFileTest
	{
		[ Test ]
		public void TestTempFile()
		{
			string path = null;
			using (var file = new TempFile())
			{
				path = file.Path;
				Assert.True(File.Exists(path), "Does the file exist?");
			}
			Assert.False(File.Exists(path), "Was the file cleaned up?");
		}
	}
}
