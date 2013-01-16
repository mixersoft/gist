using System.Globalization;
using System;

namespace Snaphappi
{
	public static class DateTimeEx
	{
		private static readonly DateTime epoch
			= new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime ParseExifTime(string time)
		{
			return DateTime.ParseExact
				( time
				, "yyyy:MM:dd HH:mm:ss"
				, CultureInfo.InvariantCulture
				);
		}

		public static bool TryParseExifTime(string time, out DateTime result)
		{
			return DateTime.TryParseExact
				( time
				, "yyyy:MM:dd HH:mm:ss"
				, CultureInfo.InvariantCulture
				, DateTimeStyles.None
				, out result
				);
		}

		public static DateTime FromUnixTime(int time)
		{
			return epoch.AddSeconds(time);
		}

		public static int ToUnixTime(this DateTime time)
		{
			return (int)(time.ToUniversalTime() - epoch).TotalSeconds;
		}
	}
}
