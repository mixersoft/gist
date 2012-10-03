﻿using System;

namespace Snaphappi
{
	public interface IURTaskControlService
	{
		string[] GetFolders();
		
		void ReportFolderNotFound(string folder);

		void ReportUploadFailed(string folder, string path);

		void ReportFolderUploadComplete(string folder);

		void ReportFolderFileCount(string folder, int count);
	}
}