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
			var info = ParameterProcessor.SplitUrl(@"snaphappi://5_abc_ur");

			var taskType = ParameterProcessor.TaskType.UploadResampled;

			Assert.AreEqual(5,        info.TaskID,    "Is task ID correct?");
			Assert.AreEqual("abc",    info.SessionID, "Is session ID correct?");
			Assert.AreEqual(taskType, info.Type,      "Is task type correct?");
		}

		[ Test ]
		public void TestSplitParameter2()
		{
			var info = ParameterProcessor.SplitUrl(@"snaphappi://0_1_uo");

			var taskType = ParameterProcessor.TaskType.UploadOriginals;

			Assert.AreEqual(0,        info.TaskID,    "Is task ID correct?");
			Assert.AreEqual("1",      info.SessionID, "Is session ID correct?");
			Assert.AreEqual(taskType, info.Type,      "Is task type correct?");
		}
	}
}
