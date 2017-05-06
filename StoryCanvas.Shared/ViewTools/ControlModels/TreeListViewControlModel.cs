using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;

namespace StoryCanvas.Shared.ViewTools.ControlModels
{
	/// <summary>
	/// 自作コントロールTreeListViewのロジックをまとめるモデル
	/// </summary>
    public class TreeListViewControlModel
    {
		private readonly IEntityTreeModel<TreeEntity> TreeModel;
		
		public ObservableCollection<TreeEntityListItem> Entities { get; private set; }

		[Obsolete]
		public class TreeListViewItem
		{
			public TreeEntity Entity { get; set; }
			public int Level { get; set; }
		}
		public class TreeEntityListItem : INotifyPropertyChanged
		{
			private TreeEntity _entity;
			public TreeEntity Entity { get { return this._entity; } set { this._entity = value; this.OnPropertyChanged(); } }
			private int _level;
			public int Level { get { return this._level; } set { this._level = value; this.OnPropertyChanged(); } }
			public override string ToString()
			{
				return this.Entity.ToString();
			}

			#region INotifyPropertyChanged

			public event PropertyChangedEventHandler PropertyChanged;
			protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
			{
				if (PropertyChanged == null) return;

				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}

			#endregion
		}

		public TreeListViewControlModel(IEntityTreeModel<TreeEntity> treeModel)
		{
			this.TreeModel = treeModel;
			this.Entities = this.TreeModel.List;
		}

		// TODO 【高速化】ツリーリストの表示の一部を更新する処理
		/*
		/// <summary>
		/// 表示の一部を更新
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="oldParent"></param>
		/// <param name="newParent"></param>
		private void UpdatePartOfTreeView(TreeEntity entity, TreeEntity oldParent, TreeEntity newParent)
		{
			if (oldParent != null)
			{
				var targets = from item in this.Entities
							  where item.Entity == entity
							  select item;
				foreach (TreeListViewItem item in targets)
				{
					this.Entities.Remove(item);
				}
			}
			if (newParent != null)
			{
				var targets = from item in this.Entities
							  where item.Entity == newParent
							  select item;
				foreach (TreeListViewItem item in targets)
				{
					this.UpdateChildren(item.Entity);
					break;
				}
			}
		}

		/// <summary>
		/// リストで特定の親のところだけ更新する
		/// </summary>
		/// <param name="parent"></param>
		private void UpdateChildren(TreeEntity parent)
		{
			// そもそも親が表示リスト内に存在していなければ戻る
			int parentIndex = -1;
			int parentLevel = -1;
			bool hit = false;
			foreach (TreeListViewItem item in this.Entities)
			{
				parentIndex++;
				if (item.Entity == parent)
				{
					hit = true;
					parentLevel = item.Level;
					break;
				}
			}
			if (!hit)
			{
				return;
			}

			// 親に所属する子を全て削除
			while (this.Entities[parentIndex + 1] != null && this.Entities[parentLevel + 1].Level > parentLevel)
			{
				this.Entities.RemoveAt(parentIndex + 1);
			}

			// 親に所属する子を追加
			foreach (TreeEntity entity in parent.Children)
			{
				this.Entities.Insert(parentIndex + 1, new TreeListViewItem { Entity = entity, Level = parentLevel + 1 });
			}
		}
		*/
	}
}
