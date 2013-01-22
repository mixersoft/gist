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
		public  void FindByName_Match()
		{
			AddFile(@"b",   "2013:01:10 00:00:02", 0);
			AddFile(@"a\a", "2013:01:10 00:00:02", 0);
			AddFile(@"a\b", "2013:01:10 00:00:02", 0);
			AddFile(@"a\c", "2013:01:10 00:00:02", 0);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;

			bool fileNotFound = false;
			fileFinder.FileNotFound += (a, b) => fileNotFound = true;

			var target = new UploadTarget(@"a\b", MakeExifDateTime("2013:01:10 00:00:02"), new ImageID("0"));
			fileFinder.FindByName(target);

			Assert.NotNull(match);
			Assert.AreSame(target, match.Target);
			Assert.AreEqual(target.FilePath, match.NewPath);

			Assert.IsFalse(fileNotFound);
		}

		[ Test ]
		public  void FindByName_PathMismatch()
		{
			AddFile(@"b", "2013:01:10 00:00:02", 0);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;

			UploadTarget notFoundTarget = null;
			SearchType   notFoundType   = SearchType.Hash;
			fileFinder.FileNotFound += (tar, type) => { notFoundTarget = tar; notFoundType = type; };

			var target = new UploadTarget(@"a\b", MakeExifDateTime("2013:01:10 00:00:02"), new ImageID("0"));
			fileFinder.FindByName(target);

			Assert.IsNull(match);

			Assert.AreSame(target, notFoundTarget);
			Assert.AreEqual(SearchType.Name, notFoundType);
		}

		[ Test ]
		public  void FindByName_TimeMismatch()
		{
			AddFile(@"a\b", "2013-01-10 00:00:01", 0);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;

			UploadTarget notFoundTarget = null;
			SearchType   notFoundType   = SearchType.Hash;
			fileFinder.FileNotFound += (tar, type) => { notFoundTarget = tar; notFoundType = type; };

			var target = new UploadTarget(@"a\b", MakeExifDateTime("2013:01:10 00:00:02"), new ImageID("0"));
			fileFinder.FindByName(target);

			Assert.IsNull(match);

			Assert.AreSame(target, notFoundTarget);
			Assert.AreEqual(SearchType.Name, notFoundType);
		}

		[ Test ]
		public  void FindByHash_Match()
		{
			AddFile("a", "2013:01:10 00:00:00", 0); // wrong everything
			AddFile("b", "2013:01:10 00:00:02", 0); // wrong hash
			AddFile("c", "2013:01:10 00:00:02", 1); // correct
			AddFile("d", "2013:01:10 00:00:00", 1); // wrong date

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;

			bool fileNotFound = false;
			fileFinder.FileNotFound += (a, b) => fileNotFound = true;

			var target = new UploadTarget("b", MakeExifDateTime("2013:01:10 00:00:02"), new ImageID("0"));
			fileFinder.FindByHash(target, 1);

			Assert.NotNull(match);
			Assert.AreSame(target, match.Target);
			Assert.AreEqual("c", match.NewPath);

			Assert.IsFalse(fileNotFound);
		}

		[ Test ]
		public  void FindByHash_HashMismatch()
		{
			AddFile("b", "2013:01:10 00:00:02", 0);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;
			
			UploadTarget notFoundTarget = null;
			SearchType   notFoundType   = SearchType.Name;
			fileFinder.FileNotFound += (tar, type) => { notFoundTarget = tar; notFoundType = type; };

			var target = new UploadTarget("b", MakeExifDateTime("2013:01:10 00:00:02"), new ImageID("0"));
			fileFinder.FindByHash(target, 1);

			Assert.IsNull(match);

			Assert.AreSame(target, notFoundTarget);
			Assert.AreEqual(SearchType.Hash, notFoundType);
		}

		[ Test ]
		public  void FindByHash_TimeMismatch()
		{
			AddFile("b", "2013:01:10 00:00:01", 1);

			FileMatch match = null;
			fileFinder.FileFound += m => match = m;
			
			UploadTarget notFoundTarget = null;
			SearchType   notFoundType   = SearchType.Name;
			fileFinder.FileNotFound += (tar, type) => { notFoundTarget = tar; notFoundType = type; };

			var target = new UploadTarget("b", MakeExifDateTime("2013:01:10 00:00:02"), new ImageID("0"));
			fileFinder.FindByHash(target, 1);

			Assert.IsNull(match);

			Assert.AreSame(target, notFoundTarget);
			Assert.AreEqual(SearchType.Hash, notFoundType);
		}

		private void AddFile(string path, string dateTime, int hash)
		{
			fileSystem.filePaths.Add(path);
			photoLoader.times.Add(path, dateTime);
			photoLoader.hashes.Add(path, hash);
		}

		private int MakeExifDateTime(string dateTime)
		{
			return DateTimeEx.ParseExifTime(dateTime).ToUnixTime();
		}
	}
}