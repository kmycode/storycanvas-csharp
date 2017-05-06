using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Messages.Picker
{
	public class OpenFilePickerMessage : FilePickerMessageBase { }

	public class OpenImageFilePickerMessage : OpenFilePickerMessage { }

	public class SaveFilePickerMessage : FilePickerMessageBase { }

    public abstract class FilePickerMessageBase
    {
		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName;

		/// <summary>
		/// ディレクトリ
		/// </summary>
		public string Directory
		{
			get
			{
				return System.IO.Path.GetDirectoryName(this.FileName);
			}
		}

		/// <summary>
		/// ファイルが選択されたか
		/// </summary>
		public bool IsSelected
		{
			get
			{
				return this.FileName != null &&  this.FileName != "";
			}
		}
    }
}
