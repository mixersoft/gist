﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IApp
	{
		event Action LoadUploadResampled;
		event Action LoadUploadOriginals;

		void Quit();
	}
}
