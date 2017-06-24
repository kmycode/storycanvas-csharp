using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Models.EntitySet
{
	public delegate void EntityAddedEventHandler<E>(E entity) where E : IEntity;
	public delegate void EntityRemovedEventHandler<E>(E entity) where E : IEntity;
	public delegate void EntitySetChangedEventHandler<E>(E entity) where E : IEntity;

	[DataContract]
	public abstract class EntitySetModel<E> : IEnumerable<E> where E : IEntity
	{
		public EntitySetModel()
		{
			this.Initialize(default(StreamingContext));
		}

		[OnDeserializing]
		private void Initialize(StreamingContext context)
		{
			// AddとRemoveのイベントでChangeのイベントも発行されるようにする
			this.EntitySetChanged = delegate { };
			this.EntityAdded += (e) => this.EntitySetChanged(e);
			this.EntityRemoved += (e) => this.EntitySetChanged(e);
		}

		/// <summary>
		/// 格納されているエンティティに変更があった時に呼び出されるイベント（Add、Removeも含む）
		/// </summary>
		public event EntitySetChangedEventHandler<E> EntitySetChanged;

		/// <summary>
		/// エンティティを追加
		/// </summary>
		/// <param name="entity">追加するエンティティ</param>
		public abstract void Add(E entity);

		/// <summary>
		/// エンティティが追加された時のイベント
		/// </summary>
		public event EntityAddedEventHandler<E> EntityAdded = delegate { };

		/// <summary>
		/// エンティティが追加された時のイベントを呼び出し
		/// </summary>
		/// <param name="entity">追加されたエンティティ</param>
		protected void OnEntityAdded(E entity)
		{
			this.EntityAdded(entity);
		}

		/// <summary>
		/// エンティティを削除
		/// </summary>
		/// <param name="entity">削除するエンティティ</param>
		public abstract void Remove(E entity);

		[Obsolete("Deleteメソッドは、Removeメソッドに名前が変更になりました", true)]
		public void Delete(E entity) { }

		/// <summary>
		/// これからエンティティが削除される時のイベント
		/// </summary>
		public event EntityRemovedEventHandler<E> EntityRemoving = delegate { };

		/// <summary>
		/// エンティティが削除される時のイベントを呼び出し
		/// </summary>
		/// <param name="entity">削除するエンティティ</param>
		protected void OnEntityRemoving(E entity)
		{
			this.EntityRemoving?.Invoke(entity);
		}

		/// <summary>
		/// エンティティが削除された時のイベント
		/// </summary>
		public event EntityRemovedEventHandler<E> EntityRemoved = delegate { };

		/// <summary>
		/// エンティティが削除された時のイベントを呼び出し
		/// </summary>
		/// <param name="entity">削除されたエンティティ</param>
		protected void OnEntityRemoved(E entity)
		{
			this.EntityRemoved(entity);
		}

		/// <summary>
		/// エンティティを全削除
		/// </summary>
		public abstract void RemoveAll();

		[Obsolete("Clearメソッドは、RemoveAllメソッドに名前が変更になりました", true)]
		public void Clear() { }

        /// <summary>
        /// 指定したIDのエンティティを取得する
        /// </summary>
        /// <param name="id">エンティティのID</param>
        /// <returns>エンティティ。見つからなければnull</returns>
        public abstract E FindId(long id);

		[Obsolete("Getメソッドは、FindIdメソッドに名前が変更になりました", true)]
		public E Get(long id) { return default(E); }

		/// <summary>
		/// エンティティの数を取得する
		/// </summary>
		public abstract int Count { get; }

		/// <summary>
		/// エンティティの順番を1つずつ足し算してずらす（ひき算するメソッドは、orderの仕様上意味が無いので作らない）
		/// ただし、指定したエンティティ（startEntity）がリストに登録されていない場合、そのエンティティに対しては処理を行わない
		/// </summary>
		/// <param name="list">処理を行う対象のリスト</param>
		/// <param name="startEntity">指定するエンティティ。リスト内になくても構わないが、その時はこのエンティティに対しては処理は行われない。orderの値がこのエンティティのもの以上であるエンティティに対して処理される</param>
		[Obsolete("順番を+1するだけなので、複数のリストが存在するときなど、場合によっては同じ順番のエンティティが複数できる原因になるかも")]
		public static void ShiftOrder(ObservableCollection<E> list, E startEntity)
		{
			long baseOrder = startEntity.Order;
			var items = from item in list
						where item.Order >= baseOrder
						select item;
			foreach (E item in items)
			{
				item.Order++;
			}
		}

		/// <summary>
		/// エンティティの順番を1つずつずらす。次のエンティティの順番番号を採用し、次のエンティティはそのまた次のエンティティの、の繰り返し。
		/// 最後のエンティティは、別途指定した順番番号を採用する
		/// </summary>
		/// <param name="list">処理を行う対象のリスト</param>
		/// <param name="startEntity">順番をずらす最初のエンティティ。リスト内になくても構わないが、そのときはこのエンティティに対しても処理が行われるので注意</param>
		/// <param name="lastOrder">最後のエンティティに適用する順番番号。最後のエンティティの現在の順番より大きい必要がある</param>
		public static void ShiftOrder(ObservableCollection<E> list, E startEntity, long lastOrder)
		{
			E prevItem = startEntity;
			long baseOrder = startEntity.Order;
			var items = from item in list
						where item.Order >= baseOrder
						select item;
			foreach (E item in items)
			{
				prevItem.Order = item.Order;
				prevItem = item;
			}

			// 最後の要素に対して実行
			prevItem.Order = lastOrder;
		}

		#region IEnumerable

		public abstract IEnumerator<E> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
