using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Models.Cryptography
{
    public interface IMD5
    {
		string FromByteArray(byte[] bytes);
    }
}
