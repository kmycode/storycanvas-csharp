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
	public class EntityListModel<E> : EntitySetModel<E>, IEnumerable<E>, IList<E>, IList, INotifyCollectionChanged where E : IEntity
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
		protected ObservableCollection<E> Entities
		{
			get
			{
				return this._entities;
			}
		}

		#region IList

		public override IEnumerator<E> GetEnumerator()
		{
			return ((IEnumerable<E>)this.Entities).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<E>)this.Entities).GetEnumerator();
		}

		public void RemoveAt(int index)
		{
			((IList<E>)this.Entities).RemoveAt(index);
		}

		public void CopyTo(E[] array, int arrayIndex)
		{
			((IList<E>)this.Entities).CopyTo(array, arrayIndex);
		}

		bool ICollection<E>.Remove(E item)
		{
			return ((IList<E>)this.Entities).Remove(item);
		}

		public bool IsReadOnly
		{
			get
			{
				return ((IList<E>)this.Entities).IsReadOnly;
			}
		}

		E IList<E>.this[int index]
		{
			get
			{
				return ((IList<E>)this.Entities)[index];
			}

			set
			{
				((IList<E>)this.Entities)[index] = value;
			}
		}

		public E this[int index]
		{
			get
			{
				return ((IList<E>)this.Entities)[index];
			}

			set
			{
				((IList<E>)this.Entities)[index] = value;
			}
		}

		#endregion

		#region INotifyCollectionChanged

		public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

		#endregion

		/// <summary>
		/// エンティティを、順番通りの位置に追加
		/// 例えば、Order=(1,3,5)のリストにOrder=4のエンティティを追加する時、
		/// 結果はOrder=(1,3,4,5)になる
		/// </summary>
		/// <param name="entity">追加するエンティティ</param>
		private void InsertByOrder(E entity)
		{
			// そもそも同じ順番のエンティティが存在しないか
			foreach (E item in this.Entities)
			{
				if (entity.Order == item.Order)
				{
					entity.Order = Entity.GetNewEntityID();
					break;
				}
			}

			// 挿入位置直後のエンティティを特定
			int index = 0;
			foreach (E item in this.Entities)
			{
				if (item.Order > entity.Order)
				{
					break;
				}
				index++;
			}

			// 挿入
			this.Entities.Insert(index, entity);
		}

		/// <summary>
		/// エンティティを追加
		/// </summary>
		/// <param name="entity">追加するエンティティ</param>
		public override void Add(E entity)
		{
			this.Add_Private(entity);
		}
		private void Add_Private(E entity)
		{
			this.InsertByOrder(entity);
		}

		/// <summary>
		/// 指定した配列の内容をすべて追加する
		/// </summary>
		/// <param name="collection">追加する配列</param>
		public void AddRange(ICollection<E> collection)
		{
			foreach (E item in collection)
			{
				this.Add_Private(item);
			}
		}

		/// <summary>
		/// エンティティを所定箇所に挿入
		/// </summary>
		/// <param name="index">挿入先インデクス番号</param>
		/// <param name="entity">挿入するエンティティ</param>
		public void Insert(int index, E entity)
		{
			this.Insert_Private(index, entity);
		}
		public void Insert(E afterEntity, E entity)
		{
			int index = afterEntity != null ? this.IndexOf(afterEntity) : this.Count;
			this.Insert_Private(index >= 0 ? index : this.Count, entity);
		}
		public void InsertAndDown(E prevEntity, E entity)
		{
			this.Insert(prevEntity, entity);
			this.Down(entity);
		}
		private void Insert_Private(int index, E entity)
		{
			if (this.Entities.Count <= index)
			{
				this.Add(entity);
				return;
			}

			E target = this.Entities[index];
			long entityOrder = target.Order;
			long lastOrder = target.Order > this.Entities.Last().Order ? target.Order : Entity.GetNewEntityID();
			this.ShiftOrder(target, lastOrder);

			entity.Order = entityOrder;
			this.InsertByOrder(entity);
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
				entity.ReplaceOrder(target);
				this.Remove_Private(target);
				this.Add_Private(target);
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
				// エンティティを並べ替え
				entity.ReplaceOrder(target);
				this.Remove_Private(target);
				this.Add_Private(target);
			}
		}

		/// <summary>
		/// エンティティの順番を1つずつ足し算してずらす（ひき算するメソッドは、orderの仕様上意味が無いので作らない）
		/// ただし、指定したエンティティ（startEntity）がリストに登録されていない場合、そのエンティティに対しては処理を行わない
		/// </summary>
		/// <param name="startEntity">指定するエンティティ。orderの値がこのエンティティのもの以上であるエンティティに対して処理される</param>
		[Obsolete]
		public virtual void ShiftOrder(E startEntity)
		{
			EntitySetModel<E>.ShiftOrder(this.Entities, startEntity);
		}

		/// <summary>
		/// エンティティの順番を1つずつずらす。次のエンティティの順番番号を採用し、次のエンティティはそのまた次のエンティティの、の繰り返し。
		/// 最後のエンティティは、別途指定した順番番号を採用する
		/// </summary>
		/// <param name="list">処理を行う対象のリスト</param>
		/// <param name="startEntity">順番をずらす最初のエンティティ。リスト内になくても構わないが、そのときはこのエンティティに対しても処理が行われるので注意</param>
		/// <param name="lastOrder">最後のエンティティに適用する順番番号。最後のエンティティの現在の順番より大きい必要がある</param>
		public virtual void ShiftOrder(E startEntity, long lastOrder)
		{
			EntitySetModel<E>.ShiftOrder(this.Entities, startEntity, lastOrder);
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

		#endregion

		#region IList

		public bool IsFixedSize
		{
			get
			{
				return ((IList)this.Entities).IsFixedSize;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return ((IList)this.Entities).IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return ((IList)this.Entities).SyncRoot;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return ((IList)this.Entities)[index];
			}

			set
			{
				((IList)this.Entities)[index] = value;
			}
		}

		public int Add(object value)
		{
			if (value is E)
			{
				this.InsertByOrder((E)value);
			}
			return -1;
			//return ((IList)this.Entities).Add(value);
		}

		public bool Contains(object value)
		{
			return ((IList)this.Entities).Contains(value);
		}

		public int IndexOf(object value)
		{
			return ((IList)this.Entities).IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			if (value is E)
			{
				this.Insert(index, (E)value);
			}
			//((IList)this.Entities).Insert(index, value);
		}

		public void Remove(object value)
		{
			if (value is E)
			{
				this.Remove_EntityListModel((E)value);
			}
			//((IList)this.Entities).Remove(value);
		}

		public void CopyTo(Array array, int index)
		{
			((IList)this.Entities).CopyTo(array, index);
		}

		#endregion
	}
}
