using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Models.IO
{
    public interface IAuthBrowserProvider
    {
		IAuthBrowser OpenBrowser();
    }
}
