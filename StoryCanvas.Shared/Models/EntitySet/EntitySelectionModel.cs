using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Models.EntitySet
{
	public delegate void EntitySelectionChangedEventHandler<E>(E newEntity, E oldEntity) where E : IEntity;

	public interface IEntitySelection<E> where E : IEntity
	{
		E Selected { get; set; }
		event EntitySelectionChangedEventHandler<E> SelectionChanged;
	}

	public class EntitySelectionModel<E> : IEntitySelection<E> where E : IEntity
	{
		/// <summary>
		/// 現在選択中のエンティティ
		/// </summary>
		private E _selected;
		public E Selected
		{
			get
			{
				return this._selected;
			}
			set
			{
				if (this._selected?.Id != value?.Id)
				{
					var old = this._selected;
					this._selected = value;
					this.SelectionChanged(value, old);
				}
			}
		}

		/// <summary>
		/// 選択中のエンティティを変更した時に発行されるイベント
		/// </summary>
		public event EntitySelectionChangedEventHandler<E> SelectionChanged = delegate { };
	}

	/// <summary>
	/// EntitySelectionModelのラッパ（キャストに使う）
	/// </summary>
	/// <typeparam name="E"></typeparam>
	/// <typeparam name="INNER"></typeparam>
	public class EntitySelectionModelWrapper<E, INNER> : IEntitySelection<E> where E : IEntity where INNER : IEntity
	{
		private EntitySelectionModel<INNER> Inner;

		public EntitySelectionModelWrapper(EntitySelectionModel<INNER> inner)
		{
			this.Inner = inner;
			this.Inner.SelectionChanged += (a, b) => this.SelectionChanged((E)(IEntity)a, (E)(IEntity)b);
		}

		public E Selected
		{
			get
			{
				return (E)(IEntity)this.Inner.Selected;
			}
			set
			{
				this.Inner.Selected = (INNER)(IEntity)value;
			}
		}

		public event EntitySelectionChangedEventHandler<E> SelectionChanged = delegate { };
	}
}
