using NUnit.Framework;
using Snaphappi;
using System.Globalization;
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

		[ Test ]
		public void TestParseExifTime()
		{
			// arbitrary valid time
			Assert.AreEqual
				( "01/31/2011 14:42:54"
				, DateTimeEx.ParseExifTime("2011:01:31 14:42:54")
					.ToString(CultureInfo.InvariantCulture)
				);
			// earliest time representable by DateTime
			Assert.AreEqual
				( "01/01/0001 00:00:00"
				, DateTimeEx.ParseExifTime("0001:01:01 00:00:00")
					.ToString(CultureInfo.InvariantCulture)
				);
			// latest time representable by DateTime
			Assert.AreEqual
				( "12/31/9999 23:59:59"
				, DateTimeEx.ParseExifTime("9999:12:31 23:59:59")
					.ToString(CultureInfo.InvariantCulture)
				);
			// invalid format: dashes instead of colons in date
			Assert.Throws<FormatException>(() => DateTimeEx.ParseExifTime("2011-01-31 14:42:54"));
		}

		[ Test ]
		public void TestTryParseExifTime()
		{
			DateTime time;
			// arbitrary valid time
			Assert.IsTrue(DateTimeEx.TryParseExifTime("2011:01:31 14:42:54", out time));
			Assert.AreEqual("01/31/2011 14:42:54", time.ToString(CultureInfo.InvariantCulture));
			// invalid format: dashes instead of colons in date
			time = DateTime.MinValue;
			Assert.IsFalse(DateTimeEx.TryParseExifTime("2011-01-31 14:42:54", out time));
			Assert.AreEqual(DateTime.MinValue, time);
		}
	}
}
