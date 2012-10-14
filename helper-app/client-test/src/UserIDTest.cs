using NUnit.Framework;
using Snaphappi;
using System;

namespace SnaphappiTest.src
{
	[ TestFixture ]
	public class UserIDTest
	{
		private MockRegistry registry;
		private UserID       userID;

		[ SetUp ]
		public void Setup()
		{
			registry = new MockRegistry();
			userID   = new UserID(registry);
		}

		[ Test ]
		public void TestGetID()
		{
			var id1 = userID.GetID();
			var id2 = userID.GetID();

			Assert.NotNull(id1, "Are IDs non-null?");
			Assert.AreNotEqual(string.Empty, id1, "Are IDs non-empty?");
			Assert.AreEqual(id1, id2, "Are the subsequently retrieved IDs identical?");
		}
	}
}
