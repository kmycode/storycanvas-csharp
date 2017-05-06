using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools.ControlModels;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;

namespace StoryCanvas.Shared.Models.EntitySet
{
    public class EntityTreeEditorModel<E, PMT> : EntitySetEditorModel<E, PMT> where E : TreeEntity, new()
	{
		public EntityTreeEditorModel(string entityType, EntityTreeModel<E> entitySet, Func<E, PMT> primaryEditMessageDelegate)
			: base(entityType, entitySet, primaryEditMessageDelegate)
		{
			this.TreeControlModel = new TreeListViewControlModel(entitySet);
			this.Commands = new EntityTreeCommandModel<E>(() => new E(),
															() => this.SelectedEntity,
															(entity) => this.SelectedEntity = entity,
															this.EntityTree);
			this.EntitySelection.SelectionChanged += (a, b) =>
			{
				this.OnPropertyChanged("SelectedItem");
				this.OnPropertyChanged("Selected" + this.EntityType + "Item");
			};

			this.SearchModel.EntitySearchRequested += (sender, e) =>
			{
				StoryModel.Current.SearchTree(entitySet, e.Query);
				this.OnPropertyChanged("SearchResult");
				this.OnPropertyChanged(this.EntityType + "SearchResult");
			};
			this.SearchModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "IsValid")
				{
					this.Commands.IsEnabled = !this.SearchModel.IsValid;
				}
			};
		}

		/// <summary>
		/// 要素の集合をツリーに変換
		/// </summary>
		private EntityTreeModel<E> EntityTree
		{
			get
			{
				return (EntityTreeModel<E>)this.EntitySet;
			}
		}

		/// <summary>
		/// 要素のツリーアイテムの一次元リスト
		/// </summary>
		public ICollection<TreeEntityListItem> EntityItems
		{
			get
			{
				return this.EntityTree.List;
			}
		}
		
		/// <summary>
		/// ツリー形状の要素をコントロールするモデル
		/// </summary>
		public TreeListViewControlModel TreeControlModel { get; private set; }

		/// <summary>
		/// ツリーで現在選択されているアイテム
		/// </summary>
		public TreeEntityListItem SelectedItem
		{
			get
			{
				return this.TreeControlModel.Entities.Where((item) => item.Entity == this.SelectedEntity).SingleOrDefault();
			}
			set
			{
				this.SelectedEntity = (E)value?.Entity;
			}
		}

		/// <summary>
		/// コマンド一覧
		/// </summary>
		public EntityTreeCommandModel<E> Commands { get; private set; }

		/// <summary>
		/// 検索結果
		/// </summary>
		public IEnumerable<TreeEntityListItem> SearchResult
		{
			get
			{
				return (IEnumerable<TreeEntityListItem>)
					   (from item in this.EntityTree.List
					    where item.Entity.IsSearchHit
					    select item);
			}
		}
	}
}
