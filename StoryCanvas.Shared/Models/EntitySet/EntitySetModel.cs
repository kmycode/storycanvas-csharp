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
        public void Add(E entity) => this.Add(entity, default(E));

        /// <summary>
        /// エンティティを追加
        /// </summary>
        /// <param name="entity">追加するエンティティ</param>
        /// <param name="willNext">１つ次のエンティティ。nullなら一番最後に追加</param>
        public abstract void Add(E entity, E willNext);

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

        /// <summary>
        /// エンティティを上へ移動
        /// </summary>
        /// <param name="entity">移動するエンティティ</param>
        public abstract void Up(E entity);

        /// <summary>
        /// エンティティを下へ移動
        /// </summary>
        /// <param name="entity">移動するエンティティ</param>
        public abstract void Down(E entity);

        /// <summary>
        /// エンティティを左へ移動
        /// </summary>
        /// <param name="entity">移動するエンティティ</param>
        public abstract void Left(E entity);

        /// <summary>
        /// エンティティを右へ移動
        /// </summary>
        /// <param name="entity">移動するエンティティ</param>
        public abstract void Right(E entity);

        /// <summary>
        /// 指定したIDのエンティティを取得する
        /// </summary>
        /// <param name="id">エンティティのID</param>
        /// <returns>エンティティ。見つからなければnull</returns>
        public abstract E FindId(long id);

		/// <summary>
		/// エンティティの数を取得する
		/// </summary>
		public abstract int Count { get; }

		#region IEnumerable

		public abstract IEnumerator<E> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
