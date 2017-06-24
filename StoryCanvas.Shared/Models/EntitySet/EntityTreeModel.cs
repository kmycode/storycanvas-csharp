using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using StoryCanvas.Shared.Models.Entities;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;
using System.Collections.ObjectModel;
using System.Collections;

namespace StoryCanvas.Shared.Models.EntitySet
{
	/// <summary>
	/// EntityTreeModelの共変性（ジェネリック部分を基底クラスへキャストすることを許可）を認める
	/// </summary>
	/// <typeparam name="E"></typeparam>
	public interface IEntityTreeModel<out E> where E : TreeEntity
	{
		E Root { get; }
		ObservableCollection<TreeEntityListItem> List { get; }
		event UpdateEntityParentEventHandler TreeChanged;
	}

	[KnownType(typeof(PlaceEntity))]
	[DataContract]
	public class EntityTreeModel<E> : EntitySetModel<E>, IEntityTreeModel<E> where E : TreeEntity
	{
		/// <summary>
		/// ツリーの階層が変更された時に呼び出される
		/// </summary>
		public event UpdateEntityParentEventHandler TreeChanged = delegate { };

		/// <summary>
		/// ルートになるエンティティ
		/// </summary>
		[DataMember]
		private E _root;
		public E Root
		{
			get
			{
				return this._root;
			}
		}

		/// <summary>
		/// 新しいモデルを生成する
		/// </summary>
		/// <param name="root">ルートになるエンティティ</param>
		public EntityTreeModel(E root)
		{
			this._root = root;
			root.ParentChangedOnAllDescendants += (entity, oldParent, newParent) =>
			{
				this.UpdateList();
				this.TreeChanged(entity, oldParent, newParent);
			};
		}

		/// <summary>
		/// エンティティをルートに追加
		/// </summary>
		/// <param name="entity">追加するエンティティ</param>
		public override void Add(E entity, E willNext)
		{
            if (willNext == null)
            {
                this.Add(entity, this.Root);
            }
            else
            {
                this.Insert(willNext, entity, (E)willNext.Parent ?? this.Root);
            }
        }

		/// <summary>
		/// ルートで、指定したエンティティの前に、新しいエンティティを挿入
		/// </summary>
		/// <param name="afterEntity">指定エンティティ</param>
		/// <param name="entity">挿入するエンティティ</param>
		public void Insert(E afterEntity, E entity)
		{
			this.Insert(afterEntity, entity, this.Root);
		}

		/// <summary>
		/// ルートで、指定したエンティティの後に、新しいエンティティを挿入
		/// </summary>
		/// <param name="prevEntity">指定エンティティ</param>
		/// <param name="entity">挿入するエンティティ</param>
		public void InsertAndDown(E prevEntity, E entity)
		{
			this.InsertAndDown(prevEntity, entity, this.Root);
		}

		/// <summary>
		/// エンティティを親を指定して追加
		/// </summary>
		/// <param name="entity">追加するエンティティ</param>
		/// <param name="parent">追加するエンティティの新しい親</param>
		public void AddAsChild(E entity, E parent)
		{
			if (parent == null)
			{
				this.Add(entity);
			}
			else
			{
				parent.Children.Add(entity);
				this.OnEntityAdded(entity);
			}
		}

		/// <summary>
		/// エンティティを指定エンティティの兄弟要素として追加
		/// </summary>
		/// <param name="entity">追加するエンティティ</param>
		/// <param name="brother">追加するエンティティの兄弟</param>
		public void AddAsBrother(E entity, E brother)
		{
			if (brother == null || brother.Parent == null)
			{
				this.Add(entity);
			}
			else
			{
				brother.Parent.Children.Add(entity);
				this.OnEntityAdded(entity);
			}
		}

		/// <summary>
		/// 指定したエンティティの前に、新しいエンティティを挿入
		/// </summary>
		/// <param name="afterEntity">指定エンティティ</param>
		/// <param name="entity">挿入するエンティティ</param>
		/// <param name="parent">親</param>
		public void Insert(E afterEntity, E entity, E parent)
		{
			parent.Children.Insert(afterEntity, entity);
			this.OnEntityAdded(entity);
		}

		/// <summary>
		/// 指定したエンティティの後に、新しいエンティティを挿入
		/// </summary>
		/// <param name="prevEntity">指定エンティティ</param>
		/// <param name="entity">挿入するエンティティ</param>
		/// <param name="parent">親</param>
		public void InsertAndDown(E prevEntity, E entity, E parent)
		{
			if (parent == null)
			{
				this.InsertAndDown(prevEntity, entity);
			}
			else
			{
				parent.Children.InsertAndDown(prevEntity, entity);
				this.OnEntityAdded(entity);
			}
		}

		/// <summary>
		/// エンティティを所定箇所に挿入。
		/// このメソッドの呼出時点で、エンティティはparentの子でなくてもよい。
		/// メソッド内で親子の関連付け処理も行う
		/// </summary>
		/// <param name="index">挿入するインデクス番号</param>
		/// <param name="entity">挿入するエンティティ</param>
		/// <param name="parent">新しく親になるエンティティ。nullならルート</param>
		public void Insert(int index, E entity, E parent = null)
		{
			if (parent == null)
			{
				parent = this.Root;
			}

			/*
			E target = (E)parent.Children[index];

			// 以降のエンティティの順番をリストアップ
			var orders = new Queue<long>();
			for (int i = index; i < parent.Children.Count; i++)
			{
				orders.Enqueue(parent.Children[i].Order);
			}
			orders.Enqueue(entity.Order);
			*/

			// エンティティを挿入
			parent.Children.Insert(index, entity);

			/*
			// 以降のエンティティを１つずつ下にずらす
			for (int i = index; i < parent.Children.Count; i++)
			{
				parent.Children[i].Order = orders.Dequeue();
			}
			*/
		}

		/// <summary>
		/// エンティティを削除する
		/// </summary>
		/// <param name="entity">削除するエンティティ</param>
		public override void Remove(E entity)
		{
			this.OnEntityRemoving(entity);
			this.Remove_Private(entity);
			this.OnEntityRemoved(entity);
		}
		private void Remove_Private(E entity)
		{
			entity?.Parent?.Children.Remove(entity);
		}

		/// <summary>
		/// エンティティを全削除
		/// </summary>
		public override void RemoveAll()
		{
			this.Root.Children.RemoveAll();
		}

		/// <summary>
		/// 指定エンティティをリストの上へ移動
		/// </summary>
		/// <param name="entity">指定エンティティ</param>
		public override void Up(E entity)
		{
			if (entity?.Parent == null)
			{
				return;
			}

            entity.Parent.Children.Up(entity);
		}

		/// <summary>
		/// 指定エンティティをリストの下へ移動
		/// </summary>
		/// <param name="entity">指定エンティティ</param>
		public override void Down(E entity)
		{
			if (entity?.Parent == null)
			{
                return;
			}

            entity.Parent.Children.Down(entity);
        }

		/// <summary>
		/// 指定エンティティをリストの右へ移動。ひとつ兄上のエンティティの子にする
		/// </summary>
		/// <param name="entity">指定エンティティ</param>
		public override void Right(E entity)
		{
			if (entity?.Parent == null)
			{
                return;
			}

			// エンティティのインデクス番号を取得
			int index = entity.Parent.Children.IndexOf(entity);
			if (index < 1)
			{
                return;
			}

			// ひとつ前のエンティティを取得して処理
			E newParent = (E)entity.Parent.Children.ElementAt(index - 1);
			newParent.Children.Add(entity);
		}

		/// <summary>
		/// 指定エンティティをリストの左へ移動。祖父のエンティティの子にする
		/// </summary>
		/// <param name="entity">指定エンティティ</param>
		public override void Left(E entity)
		{
			if (entity?.Parent?.Parent == null)
			{
				return;
			}

			// 親エンティティのインデクス番号を取得
			int index = entity.Parent.Parent.Children.IndexOf(entity.Parent);

			// 挿入
			this.Insert(index + 1, entity, (E)entity.Parent.Parent);
		}

		/// <summary>
		/// 指定したIDのエンティティを取得
		/// </summary>
		/// <param name="id">エンティティのID</param>
		/// <returns>エンティティ。なければnull</returns>
		public override E FindId(long id)
		{
			return (E)this.Root.FindId(id);
		}

		/// <summary>
		/// エンティティの数を取得
		/// </summary>
		public override int Count
		{
			get
			{
				return this.Root.CountDescendants();
			}
		}

		/// <summary>
		/// 指定のエンティティがツリーに含まれるか調べる
		/// </summary>
		/// <param name="entity">調べるエンティティ</param>
		/// <returns>含まれていればtrue</returns>
		public bool Contains(E entity)
		{
			TreeEntity root = entity;
			while (root.Parent != null)
			{
				root = root.Parent;
			}
			return root == this.Root;
		}

		/// <summary>
		/// ツリーを一次元のリストにしたもの
		/// </summary>
		public ObservableCollection<TreeEntityListItem> List { get; private set; } = new ObservableCollection<TreeEntityListItem>();
		
		/// <summary>
		/// ツリーを一次元のリストにする
		/// </summary>
		/// <returns>ツリーが一次元のリストになったもの</returns>
		private ObservableCollection<TreeEntityListItem> UpdateList()
		{
			var list = this.List;
			list.Clear();
			this.ToListForEach(list, this.Root, 0);
			return list;
		}
		private void ToListForEach(ICollection<TreeEntityListItem> collection, TreeEntity parent, int level)
		{
			foreach (TreeEntity entity in parent.Children)
			{
				collection.Add(new TreeEntityListItem { Entity = entity, Level = level });
				this.ToListForEach(collection, entity, level + 1);
			}
		}

		/// <summary>
		/// ツリーの親子関係をチェックし、直すものがあれば直す
		/// （デシリアライズ時、子からの親の参照がnullになるので）
		/// </summary>
		public void CheckParentChildren()
		{
			this.CheckParentChildrenForEach(this.Root);
			this.UpdateList();
		}
		private void CheckParentChildrenForEach(TreeEntity parent)
		{
			foreach (TreeEntity child in parent.Children)
			{
				child.Parent = parent;
				this.CheckParentChildrenForEach(child);
			}
		}

		#region IEnumerable

		public override IEnumerator<E> GetEnumerator()
		{
            return new Enumerator(this);
		}

        private class Enumerator : IEnumerator<E>
        {
            public EntityTreeModel<E> Tree { get; private set; }

            public E Current { get; private set; }

            object IEnumerator.Current => this.Current;

            private Stack<ParentIndex> indices = new Stack<ParentIndex>();

            public Enumerator(EntityTreeModel<E> tree)
            {
                this.Tree = tree;
                this.Current = tree.Root;
                this.indices.Push(new ParentIndex
                {
                    Entity = this.Current,
                    ChildIndex = 0,
                });
            }

            public void Dispose()
            {
                this.Tree = null;
                this.Current = null;
                this.indices = null;
            }

            public bool MoveNext()
            {
                // 自分に子がいるか？
                var entity = (E)this.Current.Children.FirstOrDefault();
                if (entity != null)
                {
                    this.Current = entity;
                    this.indices.Push(new ParentIndex
                    {
                        Entity = entity,
                        ChildIndex = 0,
                    });
                    return true;
                }
                
                // 自分、または親、またはその親、...に弟はいるか？
                while (this.indices.Count > 0)
                {
                    var parent = this.indices.Pop();
                    parent.ChildIndex++;
                    entity = (E)parent.Entity.Children.ElementAtOrDefault(parent.ChildIndex);
                    if (entity != null)
                    {
                        this.Current = entity;
                        this.indices.Push(parent);
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                this.Current = this.Tree.Root;
            }

            private class ParentIndex
            {
                public E Entity { get; set; }
                public int ChildIndex { get; set; }
            }
        }

        #endregion
    }
}
