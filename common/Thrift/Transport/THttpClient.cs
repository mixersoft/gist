/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *	 http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Thrift.Transport
{
	public class THttpClient : TTransport
	{
		private readonly Uri uri;
		private Stream inputStream;
		private MemoryStream outputStream = new MemoryStream();
		private IDictionary<String, String> customHeaders = new Dictionary<string, string>();

		public THttpClient(Uri u)
		{
			uri = u;

			ConnectTimeout = 300000;
			ReadTimeout    = 300000;
			MaxRetryCount  = 1;
		}

		public int ConnectTimeout { get; set; }
		public int MaxRetryCount  { get; set; }
		public int ReadTimeout    { get; set; }

		public IDictionary<String, String> CustomHeaders
		{
			get
			{
				return customHeaders;
			}
		}

		public override bool IsOpen
		{
			get
			{
				return true;
			}
		}

		public override void Open()
		{
		}

		public override void Close()
		{
			if (inputStream != null)
			{
				inputStream.Close();
				inputStream = null;
			}
			if (outputStream != null)
			{
				outputStream.Close();
				outputStream = null;
			}
		}

		public override int Read(byte[] buf, int off, int len)
		{
			if (inputStream == null)
			{
				throw new TTransportException(TTransportException.ExceptionType.NotOpen, "No request has been sent");
			}

			try
			{
				int ret = inputStream.Read(buf, off, len);

				if (ret == -1)
				{
					throw new TTransportException(TTransportException.ExceptionType.EndOfFile, "No more data available");
				}

				return ret;
			}
			catch (IOException iox)
			{ 
				throw new TTransportException(TTransportException.ExceptionType.Unknown, iox.ToString());
			}
		}

		public override void Write(byte[] buf, int off, int len)
		{
			outputStream.Write(buf, off, len);
		}

		public override void Flush()
		{
			try 
			{
				SendRequest();
			}
			finally
			{
				outputStream = new MemoryStream();
			}
		}

		private void SendRequest()
		{
			int retryCount = 0;
			for (;;)
			{
				try
				{
					HttpWebRequest connection = CreateRequest();

					byte[] data = outputStream.ToArray();
					connection.ContentLength = data.Length;

					using (Stream requestStream = connection.GetRequestStream())
						requestStream.Write(data, 0, data.Length);

					if (inputStream != null)
						inputStream.Close();
					inputStream = connection.GetResponse().GetResponseStream();

					break;
				}
				catch (IOException iox)
				{
					throw new TTransportException(TTransportException.ExceptionType.Unknown, iox.ToString());
				}
				catch (WebException wx)
				{
					if (retryCount <= MaxRetryCount)
					{
						System.Diagnostics.Debug.WriteLine("Retrying after exception: " + wx.ToString());
						++retryCount;
					}
					else
					{
						throw new TTransportException(TTransportException.ExceptionType.Unknown, "Couldn't connect to server: " + wx);
					}
				}
			}
		}

		private HttpWebRequest CreateRequest()
		{
			HttpWebRequest connection = (HttpWebRequest)WebRequest.Create(uri);

			if (ConnectTimeout > 0)
			{
				connection.Timeout = ConnectTimeout;
			}
			if (ReadTimeout > 0)
			{
				connection.ReadWriteTimeout = ReadTimeout;
			}

			// Make the request
			connection.ContentType = "application/x-thrift";
			connection.Accept = "application/x-thrift";
			connection.UserAgent = "C#/THttpClient";
			connection.Method = "POST";
			connection.ProtocolVersion = HttpVersion.Version10;

			//add custom headers here
			foreach (KeyValuePair<string, string> item in customHeaders)
			{
				connection.Headers.Add(item.Key, item.Value);
			}

			return connection;
		}
	}
}
