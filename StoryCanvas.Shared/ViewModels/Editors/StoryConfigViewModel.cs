using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.Shared.Messages.Picker;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;
using static StoryCanvas.Shared.Models.Story.StoryConfig;

namespace StoryCanvas.Shared.ViewModels.Editors
{
    public class StoryConfigViewModel : ViewModelBase
    {
		private StoryConfig Model = StoryModel.Current.StoryConfig;

		public StoryConfigViewModel()
		{
			this.StoreModel(this.Model);

			StoryModel.Current.LoadStreamCompleted += () =>
			{
				this.SpendModel(this.Model);
				this.Model = StoryModel.Current.StoryConfig;
				this.StoreModel(this.Model);
				this.OnPropertyChanged("");
			};
		}

		/// <summary>
		/// ストーリーのタイトル
		/// </summary>
		public string Title
		{
			get
			{
				return this.Model.Title;
			}
			set
			{
				this.Model.Title = value;
			}
		}

		/// <summary>
		/// 著者名
		/// </summary>
		public string AuthorName
		{
			get
			{
				return this.Model.AuthorName;
			}
			set
			{
				this.Model.AuthorName = value;
			}
		}

		/// <summary>
		/// 一行コメント（概要）
		/// </summary>
		public string Comment
		{
			get
			{
				return this.Model.Comment;
			}
			set
			{
				this.Model.Comment = value;
			}
		}

		/// <summary>
		/// 概要
		/// </summary>
		public string Overview
		{
			get
			{
				return this.Model.Overview;
			}
			set
			{
				this.Model.Overview = value;
			}
		}

		/// <summary>
		/// カスタムカラーの一覧
		/// </summary>
		public ReadOnlyCollection<ColorResourceWrapper> ColorCustom
		{
			get
			{
				return StoryModel.Current.StoryConfig.ColorCustom;
			}
		}

		/// <summary>
		/// カスタムカラーの色選択画面の表示
		/// </summary>
		private RelayCommand _customColorPickerCommand;
		public RelayCommand CustomColorPickerCommand
		{
			get
			{
				return this._customColorPickerCommand = this._customColorPickerCommand ?? new RelayCommand((obj) =>
				{
					if (obj is ColorResourceWrapper)
					{
						Messenger.Default.Send(this, new ColorPickerMessage(((ColorResourceWrapper)obj).Color, (color) =>
						{
							((ColorResourceWrapper)obj).Color = color;
						}));
					}
				});
			}
		}

		/// <summary>
		/// オートセーブの状況
		/// </summary>
		public bool IsAutoSave
		{
			get
			{
				return this.Model.IsAutoSave;
			}
			set
			{
				throw new NotImplementedException("IsAutoSaveは、2.0.1で撤廃。かわりにIsAutoSaveWPF、XFを使用のこと");
			}
		}

		/// <summary>
		/// WPFでオートセーブが有効か
		/// </summary>
		public bool IsAutoSaveWPF
		{
			get
			{
				return this.Model.IsAutoSaveWPF;
			}
			set
			{
				this.Model.IsAutoSaveWPF = value;
			}
		}

		/// <summary>
		/// XFでオートセーブが有効か
		/// </summary>
		public bool IsAutoSaveXF
		{
			get
			{
				return this.Model.IsAutoSaveXF;
			}
			set
			{
				this.Model.IsAutoSaveXF = value;
			}
		}

		private RelayCommand _autoSaveEnqueteCommand;
		public RelayCommand AutoSaveEnqueteCommand
		{
			get
			{
				return this._autoSaveEnqueteCommand = this._autoSaveEnqueteCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new OpenUrlMessage("https://docs.google.com/forms/d/e/1FAIpQLSfF-G5IqcrEq582sI_3BtYhg8GEzEu6v_WFpdgW47rrQ9LkVQ/viewform"));
				});
			}
		}

	}
}
