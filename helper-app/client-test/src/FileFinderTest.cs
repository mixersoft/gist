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
			AddFile(@"b",   "2013-01-10 00:00:02", 1);
			AddFile(@"a\c", "2013-01-10 00:00:00", 0);
			AddFile(@"a\d", "2013-01-10 00:00:00", 1);
			AddFile(@"a\e", "2013-01-10 00:00:02", 1);
			AddFile(@"a\f", "2013-01-10 00:00:02", 0);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;

			fileFinder.Find(@"a\b", DateTime.Parse("2013-01-10 00:00:02").ToUnixTime(), 1);

			Assert.NotNull(match);
			Assert.AreEqual(match.OldLocation, @"a\b");
			Assert.AreEqual(match.NewLocation, @"a\e");
		}

		private void AddFile(string path, string dateTime, int hash)
		{
			fileSystem.filePaths.Add(path);
			photoLoader.times.Add(path, dateTime);
			photoLoader.hashes.Add(path, hash);
		}
	}
}
