using System;

namespace Snaphappi
{
	public class App : IApp
	{
		#region interface

		public const int ExitSuccess = 0;
		public const int ExitFailure = 1;

		public void Load()
		{
			if (Loaded != null)
				Loaded();
		}

		#endregion

		#region IApp Members

		public event Action Loaded;
		public event Action Terminated;

		public void Quit()
		{
			Terminated();
			Environment.Exit(0);
		}

		#endregion
	}
}
