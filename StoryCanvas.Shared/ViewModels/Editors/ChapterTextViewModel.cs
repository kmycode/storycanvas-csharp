using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Messages.IO;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.ViewModels.Editors
{
	public enum ChapterTextViewMode
	{
		Scenes,					// シーンを並べた画面
		ScenesText,				// シーンの脚本をひとつづきの文章として表示する画面
		VerticalScenesText,		// ScenesTextを縦書きで表示
	}

    public class ChapterTextViewModel : ViewModelBase
    {
		private static ChapterTextViewModel _default;
		public static ChapterTextViewModel Default
		{
			get
			{
				if (_default == null)
				{
					_default = new ChapterTextViewModel();
				}
				return _default;
			}
		}

		private StoryModel StoryModel = StoryModel.Current;
		private StoryEditorModel EditorModel = new StoryEditorModel();

		private PrintModel _printModel;
		private PrintModel PrintModel
		{
			get
			{
				if (this._printModel == null)
				{
					this._printModel = new PrintModel
					{
						PageInfo = this.StoryModel.StoryConfig.PrintChapterTextInfo,
					};
				}
				return this._printModel;
			}
		}

		public ChapterTextViewModel()
		{
			this.StoreModel(this.StoryModel);
			this.StoreModel(this.EditorModel);
			this.EditorModel.ChapterSelection.SelectionChanged += (s, e) =>
			{
				this.OnPropertyChanged("SceneItems");
				this.OnPropertyChanged("IsSceneSelected");
				this.OnPropertyChanged("AllScenesText");
				this.OnPropertyChanged("AllVerticalScenesTextHTML");
			};

			// 章と脚本の画面を開いた時、内容を更新
			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.ChapterTextPage)
				{
#if XAMARIN_FORMS
					// XFのObjectPickerでは、NameをBindingできない
					this.Chapters.OnCollectionChanged();
#endif
					this.OnPropertyChanged("SceneItems");
					this.OnPropertyChanged("IsSceneSelected");
					this.OnPropertyChanged("SelectedSceneItem");
					this.OnPropertyChanged("AllScenesText");
					this.OnPropertyChanged("AllVerticalScenesTextHTML");
				}
			};

#if XAMARIN_FORMS
			StoryModel.Current.LoadStreamCompleted += () =>
			{
				this.SelectedSceneItem = null;
			};
#endif
		}

		public EntityListModel<ChapterEntity> Chapters
		{
			get
			{
				return this.StoryModel.Chapters;
			}
		}

		public ChapterEntity SelectedChapter
		{
			get
			{
				return this.EditorModel.ChapterSelection.Selected;
			}
			set
			{
				this.EditorModel.ChapterSelection.Selected = value;
				this.OnPropertyChanged("IsChapterSelected");
			}
		}

		public bool IsChapterSelected
		{
			get
			{
				return this.SelectedChapter != null;
			}
		}

		public bool IsSceneSelected
		{
			get
			{
				return this.SelectedSceneItem != null;
			}
		}

		public IEnumerable<SceneRelatedItem> SceneItems
		{
			get
			{
				if (this.SelectedChapter != null)
				{
					var relates = this.StoryModel.SceneChapterRelation.FindRelated(this.SelectedChapter);
					return from item in relates
						   orderby item.Entity1.Order
						   select new SceneRelatedItem(item.Entity1);
				}
				return null;
			}
		}

		// Xamarin.Forms対応
		private SceneRelatedItem _selectedSceneItem;
		public SceneRelatedItem SelectedSceneItem
		{
			get
			{
				return this._selectedSceneItem;
			}
			set
			{
				this._selectedSceneItem = value;
				this.OnPropertyChanged();
				this.OnPropertyChanged("IsSceneSelected");
			}
		}

		/// <summary>
		/// 画面モード
		/// </summary>
		private ChapterTextViewMode _viewMode;
		public ChapterTextViewMode ViewMode
		{
			get
			{
				return this._viewMode;
			}
			private set
			{
				this._viewMode = value;
				this.OnPropertyChanged();
			}
		}
		
		/// <summary>
		/// 章に属する全てのシーンの脚本をつなげて返す
		/// </summary>
		public string AllScenesText
		{
			get
			{
				return this.SelectedChapter?.GetScenesText(true);
			}
		}

		/// <summary>
		/// 章に属するすべてのシーンの脚本を縦書きで表示するHTMLを返す
		/// </summary>
		public string AllVerticalScenesTextHTML
		{
			get
			{
				int fontSize = 16;
#if XAMARIN_FORMS
				if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.iOS)
				{
					fontSize = 10;
				}
#endif
				return @"<html>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
<body style=""height:95%;margin:0"">
<div style=""writing-mode:tb-rl;-webkit-writing-mode:vertical-rl;height:90%;line-height:140%;font-size:" + fontSize + @"px"">
<div style=""padding:25px"">" + this.AllScenesText?.Replace("\n","<br>") + @"
</div>
</div>
<script type=""text/javascript"">
window.scrollTo(document.body.scrollWidth,0);
</script>
</body>
</html>";
			}
		}

		/// <summary>
		/// 次のビューモードへ切り替え
		/// </summary>
		private RelayCommand _nextViewModeCommand;
		public RelayCommand NextViewModeCommand
		{
			get
			{
				return this._nextViewModeCommand = this._nextViewModeCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedChapter != null)
					{
#if XAMARIN_FORMS
						if (this.SelectedSceneItem == null)
						{
							Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectSceneMessage")));
							return;
						}
#endif
						switch (this.ViewMode)
						{
							case ChapterTextViewMode.Scenes:
								this.ViewMode = ChapterTextViewMode.ScenesText;
								this.OnPropertyChanged("AllScenesText");
								break;
							case ChapterTextViewMode.ScenesText:
								this.ViewMode = ChapterTextViewMode.VerticalScenesText;
								this.OnPropertyChanged("AllVerticalScenesTextHTML");
								this.ConfigPrintSelectedChapterTextCommand.OnCanExecuteChanged();
								break;
							case ChapterTextViewMode.VerticalScenesText:
								this.ViewMode = ChapterTextViewMode.Scenes;
								this.ConfigPrintSelectedChapterTextCommand.OnCanExecuteChanged();
								break;
						}
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectChapterMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 現在選択中のシーンを編集（Xamarin.Forms用。WPF用のはSceneRelatedItem内のEditSceneCommandを使う）
		/// </summary>
		private RelayCommand _editSelectedSceneCommand;
		public RelayCommand EditSelectedSceneCommand
		{
			get
			{
				return this._editSelectedSceneCommand = this._editSelectedSceneCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedSceneItem != null)
					{
						var message = new StartEditSceneMessage(() =>
						{
							Messenger.Default.Send(this, new EditSceneEntityNewMessage(this.SelectedSceneItem.Scene));
						});
						message.EditorClosed += (sender, e) =>
						{
							// 章と脚本画面を更新
							this.OnPropertyChanged("SelectedSceneItem");
						};
						Messenger.Default.Send(this, message);
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectSceneMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 章に含まれるシーンの脚本をまとめて印刷
		/// </summary>
		private RelayCommand _printSelectedChapterTextCommand;
		public RelayCommand PrintSelectedChapterTextCommand
		{
			get
			{
				return this._printSelectedChapterTextCommand = this._printSelectedChapterTextCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedChapter != null)
					{
						this.PrintModel.PrintScenes(this.SelectedChapter);
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectChapterMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 章に含まれるシーンの脚本の印刷設定
		/// </summary>
		private RelayCommand _configPrintSelectedChapterTextCommand;
		public RelayCommand ConfigPrintSelectedChapterTextCommand
		{
			get
			{
				return this._configPrintSelectedChapterTextCommand = this._configPrintSelectedChapterTextCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new ConfigPrintTextPageMessage(this.PrintModel.PageInfo));
				}, (obj) =>
				{
					return this.ViewMode != ChapterTextViewMode.VerticalScenesText;			// ブラウザがダイアログよりも最前面に表示されてしまうので、ブラウザ表示時は印刷設定できないようにする
				});
			}
		}
	}
}
