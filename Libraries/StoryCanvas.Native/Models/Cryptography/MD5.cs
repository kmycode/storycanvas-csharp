using StoryCanvas.Shared.Models.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libraries.StoryCanvas.Native.Models.Cryptography
{
	class MD5 : IMD5
	{
		public string FromByteArray(byte[] bytes)
		{
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] bs = md5.ComputeHash(bytes);
			md5.Dispose();

			var result = new StringBuilder();
			foreach (byte b in bs)
			{
				result.Append(b.ToString("x2"));
			}

			return result.ToString();
		}
	}
}
