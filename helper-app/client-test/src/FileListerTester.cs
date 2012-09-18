using NUnit.Framework;
using Snaphappi;
using System;
using System.Collections.Generic;

namespace SnaphappiTest
{
	[ TestFixture ]
	class FileListerTester
	{
		private MockFileSystem fileSystem;

		private FileLister fileLister;

		[ SetUp ]
		public void Setup()
		{
			fileSystem = new MockFileSystem();
			fileLister = new FileLister(fileSystem);
		}

		[ Test ]
		public void TestListing()
		{
			var foundFiles = new List<string>();
			fileLister.FileFound += foundFiles.Add;

			var notFoundFolders = new List<string>();
			fileLister.FolderNotFound += notFoundFolders.Add;

			bool isFinished = false;
			fileLister.Finished += () => isFinished = true;

			var files = new string[]
				{ @"file0"
				, @"dir0\file0", @"dir0\file1"
				, @"dir1\file0", @"dir1\file1"
				, @"dir2\"
				};
			foreach (var file in files)
				fileSystem.filePaths.Add(file);

			fileLister.UpdateFolders(new string[] { "dir0", "dir2", "dir3" });

			fileLister.Start();
			fileLister.Wait();

			CollectionAssert.AreEquivalent
				( new string[] { @"dir0\file0", @"dir0\file1" }
				, foundFiles
				, "Where the files found?"
				);
			CollectionAssert.AreEquivalent
				( new string[] { "dir3" }
				, notFoundFolders
				, "Where the non-existent folders reported?"
				);
			Assert.IsTrue(isFinished, "Did the search complete?");
		}
	}
}
