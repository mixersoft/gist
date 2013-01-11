using NUnit.Framework;
using Snaphappi;
using System.IO;
using System;

namespace SnaphappiTest
{
	[ TestFixture ]
	public class PhotoLoaderTest
	{
		private PhotoLoader photoLoader;

		[ SetUp ]
		public void Setup()
		{
			photoLoader = new PhotoLoader();
		}

		[ Test ]
		public void TestDateTime()
		{
			Assert.AreEqual
				( "2011:01:31 14:42:54"
				, photoLoader.GetImageDateTime(@"img\bp~0CCF308F-6ED8-459A-BF5A-85F6AC85F12B.jpg")
				);
			Assert.AreEqual
				( ""
				, photoLoader.GetImageDateTime(@"img\bm~0CCF308F-6ED8-459A-BF5A-85F6AC85F12B.jpg")
				);
			Assert.AreEqual
				( ""
				, photoLoader.GetImageDateTime(@"img\non-existent")
				);
		}

		[ Test ]
		public void TestImageHash()
		{
			var bmHash = photoLoader.GetImageHash(@"img\bm~0CCF308F-6ED8-459A-BF5A-85F6AC85F12B.jpg");
			var bpHash = photoLoader.GetImageHash(@"img\bp~0CCF308F-6ED8-459A-BF5A-85F6AC85F12B.jpg");
			var bsHash = photoLoader.GetImageHash(@"img\bs~0CCF308F-6ED8-459A-BF5A-85F6AC85F12B.jpg");
			Assert.AreEqual(bmHash, bpHash, "Are BM and BP hashes equal?");
			Assert.AreEqual(bmHash, bsHash, "Are BM and BS hashes equal?");

			var otherHash = photoLoader.GetImageHash(@"397BFCBA-4031-4D3F-8505-F9E4C9DEEE6A.jpg");
			Assert.AreNotEqual(bmHash, otherHash, @"Do different images have different hashes?");
		}
	}
}
