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
			AddHashedFile(@"b",   1);
			AddHashedFile(@"a\c", 0);
			AddHashedFile(@"a\d", 1);
			AddHashedFile(@"a\e", 0);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;

			fileFinder.Find(@"a\b", 1);

			Assert.NotNull(match);
			Assert.AreEqual(match.OldLocation, @"a\b");
			Assert.AreEqual(match.NewLocation, @"a\d");
		}

		private void AddHashedFile(string path, int hash)
		{
			fileSystem.filePaths.Add(path);
			photoLoader.hashes.Add(path, hash);
		}
	}
}
