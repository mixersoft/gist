using NUnit.Framework;
using Snaphappi;
using System;

namespace SnaphappiTest.src
{
	[ TestFixture ]
	public class DeviceIDTest
	{
		private MockRegistry registry;
		private DeviceID       userID;

		[ SetUp ]
		public void Setup()
		{
			registry = new MockRegistry();
			userID   = new DeviceID(registry, @"HKCU\Software\Snaphappi");
		}

		[ Test ]
		public void GetID()
		{
			var id1 = userID.GetID();
			var id2 = userID.GetID();

			Assert.NotNull(id1, "Are IDs non-null?");
			Assert.AreNotEqual(string.Empty, id1, "Are IDs non-empty?");
			Assert.AreEqual(id1, id2, "Are the subsequently retrieved IDs identical?");
		}
	}
}
