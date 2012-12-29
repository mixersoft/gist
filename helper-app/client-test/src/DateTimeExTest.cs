using NUnit.Framework;
using Snaphappi;
using System;

namespace SnaphappiTest
{
	[ TestFixture ]
	public class DateTimeExTest
	{
		[ Test ]
		public void TestFromUnixTime()
		{
			Assert.AreEqual
				( new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				, DateTimeEx.FromUnixTime(0x0)
				, "Is the date correct?"
				);

			// from http://blogs.msdn.com/b/oldnewthing/archive/2003/09/05/54806.aspx
			Assert.AreEqual
				( new DateTime(2002, 11, 27, 3, 25, 0, DateTimeKind.Utc)
				, DateTimeEx.FromUnixTime(0x3DE43B0C)
				, "Is the date correct?"
				);
		}

		[ Test ]
		public void TestToUnixTime()
		{
			Assert.AreEqual
				( 0x0
				, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTime()
				, "Is the timestamp correct?"
				);

			// from http://blogs.msdn.com/b/oldnewthing/archive/2003/09/05/54806.aspx
			Assert.AreEqual
				( 0x3DE43B0C
				, new DateTime(2002, 11, 27, 3, 25, 0, DateTimeKind.Utc).ToUnixTime()
				, "Is the timestamp correct?"
				);
		}
	}
}
