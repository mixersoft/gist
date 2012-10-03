using NUnit.Framework;
using Snaphappi;
using System;
using System.Collections.Generic;

namespace SnaphappiTest
{
	[ TestFixture ]
	class FileListerTester
	{
		private struct FoundFile : IEquatable<FoundFile>
		{
			public readonly string FolderPath;
			public readonly string FilePath;

			public FoundFile(string folderPath, string filePath)
			{
				FolderPath = folderPath;
				FilePath   = filePath;
			}

			public bool Equals(FoundFile other)
			{
				return other.FolderPath == FolderPath
					&& other.FilePath == FilePath;
			}
		}

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
			var foundFiles = new List<FoundFile>();
			fileLister.FileFound += (folderPath, filePath) => foundFiles.Add(new FoundFile(folderPath, filePath));

			var notFoundFolders = new List<string>();
			fileLister.FolderNotFound += notFoundFolders.Add;

			var files = new string[]
				{ @"file0"
				, @"dir0\file0", @"dir0\file1"
				, @"dir1\file0", @"dir1\file1"
				, @"dir2\"
				};
			foreach (var file in files)
				fileSystem.filePaths.Add(file);

			foreach (var folder in new string[] { "dir0", "dir2", "dir3" })
				fileLister.SearchFolder(folder);

			CollectionAssert.AreEquivalent
				( new FoundFile[] { new FoundFile("dir0", @"dir0\file0"), new FoundFile("dir0", @"dir0\file1") }
				, foundFiles
				, "Where the files found?"
				);
			CollectionAssert.AreEquivalent
				( new string[] { "dir3" }
				, notFoundFolders
				, "Where the non-existent folders reported?"
				);
		}
	}
}
