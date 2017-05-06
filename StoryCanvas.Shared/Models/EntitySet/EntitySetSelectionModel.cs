using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Models.EntitySet
{
	/// <summary>
	/// 選択が適用された時に呼び出されるデリゲート
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void SelectionAppliedEventHandler(object sender, EventArgs e);

	/// <summary>
	/// 要素の集合から、要素を複数選択できるモデル
	/// </summary>
    public class EntityListSelectionModel<E> : INotifyPropertyChanged where E : Entity
    {
		public EntityListSelectionModel(EntityListModel<E> entitySet)
		{
			foreach (var entity in entitySet)
			{
				this.Cells.Add(new EntityListSelectionCell<E>(entity));
			}
		}

		/// <summary>
		/// 項目一覧
		/// </summary>
		private ICollection<EntityListSelectionCell<E>> _cells = new Collection<EntityListSelectionCell<E>>();
		public ICollection<EntityListSelectionCell<E>> Cells
		{
			get
			{
				return this._cells;
			}
			set
			{
				this._cells = value;
			}
		}

		/// <summary>
		/// 選択を適用
		/// </summary>
		public void Apply()
		{
			this.Applied(this, new EventArgs());
		}

		public event SelectionAppliedEventHandler Applied = delegate { };

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}

	/// <summary>
	/// 要素の集合から、要素を複数選択できるモデル（ツリー版）
	/// </summary>
	/// <typeparam name="E"></typeparam>
	public class EntityTreeSelectionModel<E> : INotifyPropertyChanged where E : TreeEntity
	{
		public EntityTreeSelectionModel(EntityTreeModel<E> entitySet)
		{
			foreach (var item in entitySet.List)
			{
				this.Cells.Add(new EntityTreeSelectionCell<E>((E)item.Entity, item.Level));
			}
		}

		/// <summary>
		/// 項目一覧
		/// </summary>
		private ICollection<EntityTreeSelectionCell<E>> _cells = new Collection<EntityTreeSelectionCell<E>>();
		public ICollection<EntityTreeSelectionCell<E>> Cells
		{
			get
			{
				return this._cells;
			}
			set
			{
				this._cells = value;
			}
		}

		/// <summary>
		/// 選択を適用
		/// </summary>
		public void Apply()
		{
			this.Applied(this, new EventArgs());
		}

		public event SelectionAppliedEventHandler Applied = delegate { };

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}

	/// <summary>
	/// ひとつの項目
	/// </summary>
	public class EntityListSelectionCell<E> : INotifyPropertyChanged where E : Entity
	{
		public EntityListSelectionCell(E entity)
		{
			this._entity = entity;
		}

		/// <summary>
		/// 要素
		/// </summary>
		private E _entity;
		public E Entity
		{
			get
			{
				return this._entity;
			}
		}

		/// <summary>
		/// 現在選択されているか
		/// </summary>
		private bool _isSelected;
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				this._isSelected = value;
				this.OnPropertyChanged();
			}
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

	/// <summary>
	/// ひとつの項目（ツリー）
	/// </summary>
	/// <typeparam name="E"></typeparam>
	public class EntityTreeSelectionCell<E> : EntityListSelectionCell<E> where E : TreeEntity
	{
		public EntityTreeSelectionCell(E entity, int treeLevel) : base(entity)
		{
			this._treeLevel = treeLevel;
		}

		/// <summary>
		/// ツリーの階層の深さ
		/// </summary>
		private int _treeLevel;
		public int TreeLevel
		{
			get
			{
				return this._treeLevel;
			}
		}
	}
}
