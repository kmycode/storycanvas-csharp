using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StoryCanvas.Shared.Common
{
	public enum ApplicationVersionModifier : int
	{
		Stable = 2,
		Beta = 1,
		Alpha = 0,
	}

    public class ApplicationVersionResolver
    {
		private string _version;
		public string Version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;

				// 2.2.0 beta
				var versionModifier = value.Split(' ');
				this.Modifier = ApplicationVersionModifier.Stable;
				if (versionModifier.Count() == 2)
				{
					var mod = versionModifier.ElementAt(1);
					switch (mod)
					{
						case "beta":
							this.Modifier = ApplicationVersionModifier.Beta;
							break;
						case "alpha":
							this.Modifier = ApplicationVersionModifier.Alpha;
							break;
					}
				}

				// 2.2.0
				var versionStr = versionModifier.ElementAt(0);
				this.Versions = versionStr.Split('.');
			}
		}
		private string[] Versions;

		public ApplicationVersionModifier Modifier { get; private set; }

		public ApplicationVersionResolver(string version)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version is null!");
			}
			this.Version = version;
		}

		public int Major
		{
			get
			{
				return int.Parse(this.Versions[0]);
			}
		}

		public int Minor
		{
			get
			{
				if (this.Versions.Length > 1)
				{
					return int.Parse(this.Versions[1]);
				}
				else
				{
					return 0;
				}
			}
		}

		public int MinorMinor
		{
			get
			{
				if (this.Versions.Length > 2)
				{
					return int.Parse(this.Versions[2]);
				}
				else
				{
					return 0;
				}
			}
		}

		public static bool operator ==(ApplicationVersionResolver a, ApplicationVersionResolver b)
		{
			return a.Version == b.Version;
		}

		public static bool operator !=(ApplicationVersionResolver a, ApplicationVersionResolver b)
		{
			return a.Version != b.Version;
		}

		public static bool operator <(ApplicationVersionResolver a, ApplicationVersionResolver b)
		{
			if (a.Major < b.Major)
			{
				return true;
			}
			else if (a.Major == b.Major && a.Minor < b.Minor)
			{
				return true;
			}
			else if (a.Major == b.Major && a.Minor == b.Minor && a.MinorMinor < b.MinorMinor)
			{
				return true;
			}
			else if (a.Major == b.Major && a.Minor == b.Minor && a.MinorMinor == b.MinorMinor && (int)a.Modifier < (int)b.Modifier)
			{
				return true;
			}
			return false;
		}

		public static bool operator >(ApplicationVersionResolver a, ApplicationVersionResolver b)
		{
			return b < a;
		}

		public static bool operator <=(ApplicationVersionResolver a, ApplicationVersionResolver b)
		{
			return (a == b) || (a < b);
		}

		public static bool operator >=(ApplicationVersionResolver a, ApplicationVersionResolver b)
		{
			return b <= a;
		}
	}
}
