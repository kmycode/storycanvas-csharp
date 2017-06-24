using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;

// 本来は StoryCanvas.Shared.Models.EntityRelate フォルダ内にあるのだが、
// IDEの機能だとリファクタリングに手間がかかりそうなので、名前空間はこのままにしてる
namespace StoryCanvas.Shared.Models.EntitySet
{
	/// <summary>
	/// シーンに関連付けられた要素などを取得するためのモデル
	/// 章と脚本画面のシーン１つ分などに使う
	/// </summary>
    public class SceneRelatedItem : INotifyPropertyChanged
    {
		public SceneRelatedItem(SceneEntity scene)
		{
			this.Scene = scene;
		}

		/// <summary>
		/// シーン
		/// </summary>
		public SceneEntity Scene { get; private set; }

		/// <summary>
		/// 関連付けられている人物
		/// </summary>
		public IEnumerable<PersonSceneEntityRelate> RelatedPeople
		{
			get
			{
                return StoryModel.Current.PersonSceneRelation.FindRelated(this.Scene);
			}
		}

		/// <summary>
		/// 関連付けられている場所
		/// </summary>
		public IEnumerable<PlaceSceneEntityRelate> RelatedPlaces
		{
			get
			{
                return StoryModel.Current.PlaceSceneRelation.FindRelated(this.Scene);
			}
		}

		/// <summary>
		/// 関連付けられている章
		/// </summary>
		public IEnumerable<SceneChapterEntityRelate> RelatedChapters
		{
			get
			{
                return StoryModel.Current.SceneChapterRelation.FindRelated(this.Scene);
			}
		}

		/// <summary>
		/// 関連付けられている全てのエンティティ（章を除く）
		/// </summary>
		public IEnumerable<OnesideEntityRelationWrapper<SceneEntity>> RelatedEntitiesExceptForChapters
		{
			get
			{
				var entities = new Collection<OnesideEntityRelationWrapper<SceneEntity>>();
				foreach (var item in this.RelatedPeople)
				{
					entities.Add(new OnesideEntityRelationWrapper<SceneEntity>(item));
				}
				foreach (var item in this.RelatedPlaces)
				{
					entities.Add(new OnesideEntityRelationWrapper<SceneEntity>(item));
				}
				return entities;
			}
		}

		/// <summary>
		/// リストから選択されている、シーンの関連要素
		/// </summary>
		private OnesideEntityRelationWrapper<SceneEntity> _selectedRelatedEntity;
		public OnesideEntityRelationWrapper<SceneEntity> SelectedRelatedEntity
		{
			get
			{
				return this._selectedRelatedEntity;
			}
			set
			{
				this._selectedRelatedEntity = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// シーンを編集
		/// </summary>
		private RelayCommand _editSceneCommand;
		public RelayCommand EditSceneCommand
		{
			get
			{
				return this._editSceneCommand = this._editSceneCommand ?? new RelayCommand((obj) =>
				{
					var message = new StartEditSceneMessage(() =>
					{
						Messenger.Default.Send(this, new EditSceneEntityNewMessage(this.Scene));
					});
					message.EditorClosed += (sender, e) =>
					{
						// 章と脚本画面を更新
						this.OnPropertyChanged("RelatedEntitiesExceptForChapters");
					};
					Messenger.Default.Send(this, message);
				});
			}
		}

		/// <summary>
		/// 関連要素を編集
		/// </summary>
		private RelayCommand _editSelectedRelatedEntityCommand;
		public RelayCommand EditSelectedRelatedEntityCommand
		{
			get
			{
				return this._editSelectedRelatedEntityCommand = this._editSelectedRelatedEntityCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedRelatedEntity == null)
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
						return;
					}
					if (this.SelectedRelatedEntity.RelatedEntity is PersonEntity)
					{
						var message = new StartEditPersonMessage(() =>
						{
							Messenger.Default.Send(this, new EditPersonEntityNewMessage((PersonEntity)this.SelectedRelatedEntity.RelatedEntity));
						});
						message.EditorClosed += (sender, e) =>
						{
							// 章と脚本画面を更新
							this.OnPropertyChanged("RelatedEntitiesExceptForChapters");
						};
						Messenger.Default.Send(this, message);
					}
					else if (this.SelectedRelatedEntity.RelatedEntity is PlaceEntity)
					{
						var message = new StartEditPlaceMessage(() =>
						{
							Messenger.Default.Send(this, new EditPlaceEntityNewMessage((PlaceEntity)this.SelectedRelatedEntity.RelatedEntity));
						});
						message.EditorClosed += (sender, e) =>
						{
							// 章と脚本画面を更新
							this.OnPropertyChanged("RelatedEntitiesExceptForChapters");
						};
						Messenger.Default.Send(this, message);
					}
				});
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged = delegate { };
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}
}
