using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.Entities;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace StoryCanvas.Shared.Models.EntitySet
{
	public delegate void EntityInsertedEventHandler<E>(int index, E entity) where E : IEntity;

	[DataContract]
	public class EntityListModel<E> : EntitySetModel<E>, IEnumerable<E>, ICollection<E>, INotifyCollectionChanged where E : IEntity
	{
		/// <summary>
		/// 初期化
		/// </summary>
		public EntityListModel()
		{
			this._entities = new ObservableCollection<E>();
			this.Initialize(default(StreamingContext));
		}

		[OnDeserialized]
		private void Initialize(StreamingContext context)
		{
			this.CollectionChanged = delegate { };

			// コレクションのイベントをリスト全体のイベントと結びつける
			this.Entities.CollectionChanged += (s, e) => this.CollectionChanged(s, e);
			this.Entities.CollectionChanged += (sender, e) =>
			{
				if (e.OldItems != null)
				{
					foreach (E entity in e.OldItems)
					{
						this.OnEntityRemoved(entity);
					}
				}
				if (e.NewItems != null)
				{
					foreach (E entity in e.NewItems)
					{
						this.OnEntityAdded(entity);
					}
				}
			};
		}

		/// <summary>
		/// エンティティリスト本体
		/// </summary>
		[DataMember]
		private ObservableCollection<E> _entities;
        protected ObservableCollection<E> Entities => this._entities;

        /// <summary>
        /// エンティティリスト（XAMLで使う専用。UWPではINotifyCollectionChangedの実装だけでは画面上に変更が反映されず、ObservableCollectionのまま使う必要がある）
        /// </summary>
        public object Observable => this._entities;

		#region IEnumerable

		public override IEnumerator<E> GetEnumerator()
		{
			return ((IEnumerable<E>)this.Entities).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<E>)this.Entities).GetEnumerator();
		}

		#endregion

		#region INotifyCollectionChanged

		public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

		#endregion

		/// <summary>
		/// エンティティを追加
		/// </summary>
		/// <param name="entity">追加するエンティティ</param>
        /// <param name="willNext">次となるエンティティ</param>
		public override void Add(E entity, E willNext)
        {
            if (willNext != null)
            {
                var index = this.Entities.IndexOf(willNext);
                if (index < 0)
                {
                    this.Entities.Add(entity);
                }
                else
                {
                    this.Entities.Insert(index - 1, entity);
                }
            }
            else
            {
                this.Entities.Add(entity);
            }
        }

		/// <summary>
		/// エンティティを所定箇所に挿入
		/// </summary>
		/// <param name="index">挿入先インデクス番号</param>
		/// <param name="entity">挿入するエンティティ</param>
		public void Insert(int index, E entity)
		{
			this.Entities.Insert(index, entity);
		}
		public void Insert(E afterEntity, E entity)
		{
			int index = afterEntity != null ? this.IndexOf(afterEntity) : this.Count;
			this.Entities.Insert(index >= 0 ? index : this.Count, entity);
		}
		public void InsertAndDown(E prevEntity, E entity)
		{
			this.Insert(prevEntity, entity);
			this.Down(entity);
		}

		/// <summary>
		/// エンティティを削除
		/// </summary>
		/// <param name="entity">削除するエンティティ</param>
		public override void Remove(E entity)
		{
			this.Remove_EntityListModel(entity);
		}
		private void Remove_EntityListModel(E entity)
		{
			this.OnEntityRemoving(entity);
			this.Remove_Private(entity);
		}
		private void Remove_Private(E entity)
		{
			this.Entities.Remove(entity);
		}

		/// <summary>
		/// 指定条件に該当するエンティティを削除
		/// </summary>
		/// <param name="predicate">条件</param>
		public void RemoveIf(Predicate<E> predicate)
		{
			var removes = from item in this.Entities
						  where predicate(item)
						  select item;
			var removelist = removes.ToArray();
			foreach (var item in removelist)
			{
				this.Entities.Remove(item);
			}
		}

		/// <summary>
		/// すべてのエンティティを削除
		/// </summary>
		public override void RemoveAll()
		{
			this.Entities.Clear();
		}

		/// <summary>
		/// 指定エンティティをリストの上へ移動
		/// </summary>
		/// <param name="entity">指定エンティティ</param>
		public void Up(E entity)
		{
			// ひとつ順番が上のものを探す
			E target = this.Back(entity);

			if (target != null)
			{
                // エンティティを並べ替え
                this.Entities.Remove(entity);
				this.Add(entity, target);
			}
		}

		/// <summary>
		/// 指定エンティティをリストの下へ移動
		/// </summary>
		/// <param name="entity">指定エンティティ</param>
		public void Down(E entity)
		{
			// ひとつ順番が下のものを探す
			E target = this.Next(entity);

			if (target != null)
			{
                target = this.Next(entity);
                this.Entities.Remove(entity);

                if (target != null)
                {
                    // エンティティを並べ替え
                    this.Add(entity, target);
                }
                else
                {
                    // エンティティを最後に移動
                    this.Add(entity);
                }
            }
		}

		/// <summary>
		/// 指定のエンティティの1つ前のorderのエンティティを取得
		/// </summary>
		/// <param name="entity">エンティティ</param>
		/// <returns>1つ前のorderのエンティティ</returns>
		public E Back(E entity)
		{
			int index = this.Entities.IndexOf(entity) - 1;
			if (index < 0)
			{
				return default(E);
			}
			return this.Entities[index];
		}

		/// <summary>
		/// 指定のエンティティの1つ後のorderのエンティティを取得
		/// </summary>
		/// <param name="entity">エンティティ</param>
		/// <returns>1つ後のorderのエンティティ</returns>
		public E Next(E entity)
		{
			int index = this.Entities.IndexOf(entity) + 1;
			if (index <= 0 || index >= this.Entities.Count)
			{
				return default(E);
			}
			return this.Entities[index];
		}

		/// <summary>
		/// 指定したIDのエンティティを取得する
		/// </summary>
		/// <param name="id">エンティティのID</param>
		/// <returns>エンティティ。見つからなければnull</returns>
		public override E FindId(long id)
		{
			foreach (E item in this.Entities)
			{
				if (item.Id == id)
				{
					return item;
				}
			}
			return default(E);
		}

        #region ラップ

        public E this[int index]
        {
            get
            {
                return this.Entities[index];
            }
            set
            {
                this.Entities[index] = value;
            }
        }

        /// <summary>
        /// エンティティの数を取得
        /// </summary>
        public override int Count
		{
			get
			{
				return this.Entities.Count;
			}
		}

        public bool IsReadOnly => ((ICollection<E>)this._entities).IsReadOnly;

        /// <summary>
        /// エンティティを検索
        /// </summary>
        /// <param name="entity">検索するエンティティ</param>
        /// <returns>インデクス番号</returns>
        public int IndexOf(E entity)
		{
			return this.Entities.IndexOf(entity);
		}

		/// <summary>
		/// 指定したエンティティが含まれるか調べる
		/// </summary>
		/// <param name="entity">検索するエンティティ</param>
		/// <returns>エンティティがリストに含まれていればtrue</returns>
		public bool Contains(E entity)
		{
			return this.Entities.Contains(entity);
		}

		/// <summary>
		/// 指定条件に該当するエンティティが存在するか調べる
		/// </summary>
		/// <param name="predicate">条件</param>
		public bool ContainsIf(Predicate<E> predicate)
		{
			var matches = from item in this.Entities
						  where predicate(item)
						  select item;
			return matches.Count() > 0;
		}

		/// <summary>
		/// IDのみを記載した配列を作成する
		/// </summary>
		/// <returns>エンティティのIDだけが載せられたリスト</returns>
		public ICollection<long> ToIdList()
		{
			var list = from item in this.Entities
					   select item.Id;
			return list.ToList();
		}

		/// <summary>
		/// IDのリストから、エンティティのリストを復元する
		/// </summary>
		/// <param name="idList">IDのリスト</param>
		/// <param name="entitySet">エンティティの一覧</param>
		public void FromIdList(ICollection<long> idList, EntitySetModel<E> entitySet)
		{
			foreach (long id in idList)
			{
				E entity = entitySet.FindId(id);
				if (entity != null)
				{
					this.Entities.Add(entity);
				}
			}
		}

		/// <summary>
		/// NotifyCollectionChangedを発火する
		/// （※ほとんどObjectPicker専用メソッド）
		/// </summary>
		public void OnCollectionChanged()
		{
			this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

        public void CopyTo(E[] array, int arrayIndex)
        {
            ((ICollection<E>)this._entities).CopyTo(array, arrayIndex);
        }

        bool ICollection<E>.Remove(E item)
        {
            return ((ICollection<E>)this._entities).Remove(item);
        }

        #endregion
    }
}
