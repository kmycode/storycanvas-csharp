using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Models.Cryptography;

namespace StoryCanvas.Shared.Utils
{
    public static class AutofacUtil
    {
		private static ContainerBuilder _builder;
		public static ContainerBuilder Builder
		{
			get
			{
				if (_builder == null)
				{
					_builder = new ContainerBuilder();
				}
				return _builder;
			}
		}

		private static IContainer _container;
		public static IContainer Container
		{
			get
			{
				if (_container == null)
				{
					_container = _builder.Build();
				}
				return _container;
			}
		}

		public static IAuthBrowserProvider AuthBrowserProvider { get; set; }

		public static StorageModelBase OneDriveStorage { internal get; set; }

		public static IMD5 MD5 { get; set; }
    }
}
