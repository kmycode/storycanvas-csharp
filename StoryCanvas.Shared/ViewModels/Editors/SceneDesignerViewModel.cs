using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using System.Linq;

namespace StoryCanvas.Shared.ViewModels.Editors
{
    public class SceneDesignerViewModel : ViewModelBase
	{
		private static SceneDesignerViewModel _default;
		public static SceneDesignerViewModel Default
		{
			get
			{
				if (_default == null)
				{
					_default = new SceneDesignerViewModel();
				}
				return _default;
			}
		}

		private StoryModel StoryModel = StoryModel.Current;
		private StoryEditorModel EditorModel = new StoryEditorModel();

		public SceneDesignerViewModel()
		{
			this.StoreModel(this.StoryModel);
			this.StoreModel(this.EditorModel);
			this.EditorModel.ChapterSelection.SelectionChanged += (s, e) =>
			{
				this.OnPropertyChanged("SceneItems");
				this.OnPropertyChanged("IsSceneSelected");
			};

			// 章と脚本の画面を開いた時、内容を更新
			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.SceneDesignerPage)
				{
#if XAMARIN_FORMS
					// XFのObjectPickerでは、NameをBindingできない
					this.Chapters.OnCollectionChanged();
#endif
					this.OnPropertyChanged("SceneItems");
					this.OnPropertyChanged("IsSceneSelected");
					this.OnPropertyChanged("SelectedSceneItem");
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
					return relates.Select(r => new SceneRelatedItem(r.Entity1));
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
	}
}
