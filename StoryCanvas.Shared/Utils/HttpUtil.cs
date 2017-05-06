using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Utils
{
    public static class HttpUtil
    {
		public static IDictionary<string, string> GetUrlParameters(string url)
		{
			var parameters = new Dictionary<string, string>();

			var queries = url.Split('?');
			if (queries.Length >= 2)
			{
				var query = queries[1];
				var parameterPairs = query.Split('&');
				foreach (var pair in parameterPairs)
				{
					var q = pair.Split('=');
					if (q.Length >= 2)
					{
						parameters.Add(q[0], q[1]);
					}
				}
			}

			return parameters;
		}
    }
}
