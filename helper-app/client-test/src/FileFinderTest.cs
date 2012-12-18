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
			fileSystem.filePaths.Add("b");
			fileSystem.filePaths.Add(@"a\c");
			fileSystem.filePaths.Add(@"a\d");
			photoLoader.hashes.Add(@"a\c", 0);
			photoLoader.hashes.Add(@"a\d", 1);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;

			fileFinder.Find(@"a\b", 1);

			Assert.NotNull(match);
			Assert.AreEqual(match.OldLocation, @"a\b");
			Assert.AreEqual(match.NewLocation, @"a\d");
		}
	}
}
