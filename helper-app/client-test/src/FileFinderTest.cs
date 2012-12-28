using NUnit.Framework;
using Snaphappi;
using System;

namespace SnaphappiTest
{
	[ TestFixture ]
	public class FileFinderTest
	{
		private MockFileSystem  fileSystem;
		private MockPhotoLoader photoLoader;
		private FileFinder      fileFinder;

		[ SetUp ]
		public void Setup()
		{
			fileSystem  = new MockFileSystem();
			photoLoader = new MockPhotoLoader();
			fileFinder  = new FileFinder(fileSystem, photoLoader);
		}

		[ Test ]
		public void TestFind()
		{
			AddFile(@"b",   2, 1);
			AddFile(@"a\c", 0, 0);
			AddFile(@"a\d", 0, 1);
			AddFile(@"a\e", 2, 1);
			AddFile(@"a\f", 2, 0);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;

			fileFinder.Find(@"a\b", 2, 1);

			Assert.NotNull(match);
			Assert.AreEqual(match.OldLocation, @"a\b");
			Assert.AreEqual(match.NewLocation, @"a\e");
		}

		private void AddFile(string path, int timestamp, int hash)
		{
			fileSystem.filePaths.Add(path);
			fileSystem.timestamps.Add(path, timestamp);
			photoLoader.hashes.Add(path, hash);
		}
	}
}
