using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.ViewTools;
using static StoryCanvas.Shared.Models.EntityRelate.EntityRelationEditorModel;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	/// <summary>
	/// 要素関連付け画面（Xamarin.Forms）のVM
	/// </summary>
    public class EntityRelationViewModel : ViewModelBase
    {
		public EntityRelationViewModel(EntityRelationEditorMessage message)
		{
			this.Model = message.Model;
			this.Model.RelatedEntityItemSelection.SelectionChanged += (a, b) =>
			{
				this.OnPropertyChanged("SelectedRelatedEntityItem");
				this.OnPropertyChanged("IsDummy");
			};
			this.Model.EntityForRelateSelection.SelectionChanged += (a, b) => this.OnPropertyChanged("SelectedEntityForRelate");
		}

		private EntityRelationEditorModel Model;

		/// <summary>
		/// 関連付けの相手の要素はEntity1？Entity2？
		/// </summary>
		public RelationDirection Direction
		{
			get
			{
				return this.Model.Direction;
			}
		}

		/// <summary>
		/// リストから選択された、現在関連付けられている要素
		/// </summary>
		public IRelation SelectedRelatedEntityItem
		{
			get
			{
				return this.Model.RelatedEntityItemSelection.Selected;
			}
			set
			{
				this.Model.RelatedEntityItemSelection.Selected = (IRelationEntity)value;
			}
		}

		/// <summary>
		/// ダミーの画面を表示しているか
		/// </summary>
		public bool IsDummy
		{
			get
			{
				return this.SelectedRelatedEntityItem == null;
			}
		}

		/// <summary>
		/// 関連付けるために選択された、現在関連付けられていない要素
		/// </summary>
		public Entity SelectedEntityForRelate
		{
			get
			{
				return this.Model.EntityForRelateSelection.Selected;
			}
			set
			{
				this.Model.EntityForRelateSelection.Selected = value;
			}
		}

		/// <summary>
		/// 現在関連付けられている要素
		/// </summary>
		public IEnumerable<IRelation> RelatedEntityItems
		{
			get
			{
				return this.Model.RelatedEntityItemsGetter();
			}
		}

		/// <summary>
		/// 現在関連付けられていない要素
		/// </summary>
		public IEnumerable<Entity> NotRelatedEntities
		{
			get
			{
				return this.Model.NotRelatedEntitiesGetter();
			}
		}

		/// <summary>
		/// 関連付け追加
		/// </summary>
		private RelayCommand _addCommand;
		public RelayCommand AddCommand
		{
			get
			{
				return this._addCommand = this._addCommand ?? new RelayCommand((obj) =>
				{
					this.OnPropertyChanged("NotRelatedEntities");
					Messenger.Default.Send(this, new PickNotRelatedEntityRequestedMessage
					{
						PickedAction = (picked) =>
						{
							this.SelectedEntityForRelate = picked as Entity;
							this.Model.AddCommand.Execute(obj);
							this.SelectedRelatedEntityItem = null;
							this.OnPropertyChanged("RelatedEntityItems");
						},
					});
				});
			}
		}

		/// <summary>
		/// 関連付け削除
		/// </summary>
		private RelayCommand _removeCommand;
		public RelayCommand RemoveCommand
		{
			get
			{
				return this._removeCommand = this._removeCommand ?? new RelayCommand((obj) =>
				{
					this.Model.RemoveCommand.Execute(obj);
					this.SelectedRelatedEntityItem = null;
					this.OnPropertyChanged("RelatedEntityItems");
					this.OnPropertyChanged("NotRelatedEntities");
				});
			}
		}
    }
}
