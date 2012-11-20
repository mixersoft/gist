using NUnit.Framework;
using Snaphappi;
using System;

namespace SnaphappiTest
{
	[ TestFixture ]
	public class ParameterProcessorTest
	{
		[ Test ]
		public void TestSplitParameter1()
		{
			var info = ParameterProcessor.ParseUrl(@"snaphappi://SGVsbG8gV29ybGQh_aHR0cDovL3d3dy5zbmFwaGFwcGkuY29t_ur");

			var taskType  = ParameterProcessor.TaskType.UploadResampled;
			var authToken = "Hello World!";
			var sessionID = "http://www.snaphappi.com";

			Assert.AreEqual(authToken, info.AuthToken, "Is task auth token correct?");
			Assert.AreEqual(sessionID, info.SessionID, "Is session ID correct?");
			Assert.AreEqual(taskType,  info.Type,      "Is task type correct?");
		}

		[ Test ]
		public void TestSplitParameter2()
		{
			var info = ParameterProcessor.ParseUrl(@"snaphappi://MA==_MQ==_uo");

			var taskType = ParameterProcessor.TaskType.UploadOriginals;

			Assert.AreEqual("0",      info.AuthToken, "Is authToken correct?");
			Assert.AreEqual("1",      info.SessionID, "Is session ID correct?");
			Assert.AreEqual(taskType, info.Type,      "Is task type correct?");
		}
	}
}
