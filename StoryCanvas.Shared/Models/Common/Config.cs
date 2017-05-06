using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoryCanvas.Shared.Models.Common
{
	[DataContract]
	public static class Config
	{
		/// <summary>
		/// フォルダ選択のパス
		/// </summary>
		[DataMember]
		private static string _folderChoosePath = null;
		public static string FolderChoosePath
		{
			get
			{
				return _folderChoosePath;
			}
			set
			{
				_folderChoosePath = value;
			}
		}
	}
}
