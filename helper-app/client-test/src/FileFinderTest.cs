using NUnit.Framework;
using Snaphappi;
using System.Collections.Generic;
using System.IO;
using System;

namespace SnaphappiTest
{
	[ TestFixture ]
	public class FileFinderTest
	{
		private MockFileSystem fileSystem;

		private FileFinder fileFinder;

		[ SetUp ]
		public void Setup()
		{
			fileSystem = new MockFileSystem();
			fileFinder = new FileFinder(fileSystem);
		}

		[ Test ]
		public void TestFiles()
		{
			var found = new List<string>();
			fileFinder.FileFound += file => found.Add(MakePath(file));

			var notFound = new List<string>();
			fileFinder.FileNotFound += file => notFound.Add(MakePath(file));

			fileSystem.filePaths.Add(@"dir\file");

			fileFinder.FileInfo = new OriginalFileInfo[]
				{ new OriginalFileInfo("",    "file", 0)
				, new OriginalFileInfo("dir", "",     0)
				, new OriginalFileInfo("dir", "file", 0)
				, new OriginalFileInfo("",    "",     0)
				};

			bool isDone = false;
			fileFinder.Done += () => { isDone = true; };

			fileFinder.Start();
			fileFinder.Wait();

			CollectionAssert.AreEquivalent
				( new string[] { @"dir\file" }
				, found
				, "Were all the expected files found?"
				);
			CollectionAssert.AreEquivalent
				( new string[] { "file", "dir", "" }
				, notFound
				, "Were all the expected files not found?"
				);
			Assert.IsTrue(isDone, "Did the search complete?");
		}

		private string MakePath(OriginalFileInfo file)
		{
			return Path.Combine(file.directory, file.relativePath);
		}
	}
}
