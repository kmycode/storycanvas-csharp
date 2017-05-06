using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;

namespace StoryCanvas.Shared.Models.EntityRelate
{
	public delegate void RelationAddedEventHandler<E1, E2>(object sender, RelationAddedEventArgs<E1, E2> e) where E1 : Entity where E2 : Entity;
	public class RelationAddedEventArgs<E1, E2> : EventArgs where E1 : Entity where E2 : Entity
	{
		public RelationAddedEventArgs(E1 e1, E2 e2)
		{
			this.Entity1 = e1;
			this.Entity2 = e2;
		}
		public E1 Entity1 { get; private set; }
		public E2 Entity2 { get; private set; }
	}

	/// <summary>
	/// エンティティ同士の関連付け
	/// 
	/// TODO
	/// 本当は普通に「EntityRelationModel<E1, E2>」として基底クラスをなしにしたいのだが、
	/// v1.0のStoryModelでデータコントラクトのメンバとして登録してしまったため、
	/// このような複雑な形式になっている
	/// </summary>
    public class EntityRelationModel<R, E1, E2> where R : EntityRelateBase<E1, E2>
		where E1 : Entity where E2 : Entity
    {
		/// <summary>
		/// エンティティ関連付けのリスト
		/// </summary>
		private EntityListModel<R> _relations = new EntityListModel<R>();
		public EntityListModel<R> Relations
		{
			get
			{
				return this._relations;
			}
			// 本当はprivateにしたいのだが、
			// v1.0でStoryModelのデータコントラクトのメンバに設定してしまったので、publicにしている
			set
			{
				this._relations = value;
			}
		}

		/// <summary>
		/// 関連付けが追加された時に発行されるイベント
		/// </summary>
		public event RelationAddedEventHandler<E1, E2> RelationAdded = delegate { };

		public EntityRelationModel()
		{
		}

		/// <summary>
		/// E1に関連付けられているE2を取得
		/// </summary>
		/// <param name="e1">E1</param>
		/// <returns>関連付けのリスト</returns>
		public IEnumerable<R> FindRelated(E1 e1)
		{
			return this.FindRelatedE2(e1);
		}

		/// <summary>
		/// E1に関連付けられているE2を取得
		/// </summary>
		/// <param name="e1">E1</param>
		/// <returns>関連付けのリスト</returns>
		public IEnumerable<R> FindRelatedE2(E1 e1)
		{
			return this.Relations.Where((obj) => obj.Entity1 == e1);
		}

		/// <summary>
		/// E1に関連付けられていないE2を取得
		/// </summary>
		/// <param name="e1">E1</param>
		/// <returns>関連付けられていないE2のリスト</returns>
		public IEnumerable<E2> FindNotRelated(E1 e1, EntitySetModel<E2> e2s)
		{
			return this.FindNotRelatedE2(e1, e2s);
		}

		/// <summary>
		/// E1に関連付けられていないE2を取得
		/// </summary>
		/// <param name="e1">E1</param>
		/// <returns>関連付けられていないE2のリスト</returns>
		public IEnumerable<E2> FindNotRelatedE2(E1 e1, EntitySetModel<E2> e2s)
		{
			var related = this.FindRelatedE2(e1);
			var list = new List<E2>();
			foreach (var item in e2s)
			{
				bool hit = true;
				if (hit)
				{
					foreach (var relation in related)
					{
						if (relation.Entity1 == e1 && relation.Entity2 == item)
						{
							hit = false;
							break;
						}
					}
				}
				if (hit)
				{
					list.Add(item);
				}
			}
			return list;
		}
		
		/// <summary>
		/// E2に関連付けられているE1を取得
		/// </summary>
		/// <param name="e2">E2</param>
		/// <returns>E1の関連付け</returns>
		public IEnumerable<R> FindRelated(E2 e2)
		{
			return this.FindRelatedE1(e2);
		}

		/// <summary>
		/// E2に関連付けられているE1を取得
		/// </summary>
		/// <param name="e2">E2</param>
		/// <returns>E1の関連付け</returns>
		public IEnumerable<R> FindRelatedE1(E2 e2)
		{
			return this.Relations.Where((obj) => obj.Entity2 == e2);
		}

		/// <summary>
		/// E2に関連付けられていないE1を取得
		/// </summary>
		/// <param name="e2">E2</param>
		/// <returns>関連付けられていないE1のリスト</returns>
		public IEnumerable<E1> FindNotRelated(E2 e2, EntitySetModel<E1> e1s)
		{
			return this.FindNotRelatedE1(e2, e1s);
		}
		public IEnumerable<TreeEntityListItem> FindNotRelatedTreeItems(E2 e2, ICollection<TreeEntityListItem> e1s)
		{
			return this.FindNotRelatedE1TreeItem(e2, e1s);
		}
		public IEnumerable<E1> FindNotRelatedTreeItemEntities(E2 e2, ICollection<TreeEntityListItem> e1s)
		{
			return this.FindNotRelatedE1TreeItemEntity(e2, e1s);
		}

		/// <summary>
		/// E2に関連付けられていないE1を取得
		/// </summary>
		/// <param name="e2">E2</param>
		/// <returns>関連付けられていないE1のリスト</returns>
		public IEnumerable<E1> FindNotRelatedE1(E2 e2, EntitySetModel<E1> e1s)
		{
			// ジェネリックでは is が使えないので、TreeEntity特有のメンバをチェック
			if (typeof(E1).GetRuntimeEvent("ParentChanged") != null)
			{
				throw new ArgumentException("EntityRelationModel.FindNotRelated cannout use classes based TreeEntity. Instead of 'FindNotRelatedTreeItem'.");
			}

			var related = this.FindRelatedE1(e2);
			var list = new List<E1>();
			foreach (var item in e1s)
			{
				bool hit = true;
				if (hit)
				{
					foreach (var relation in related)
					{
						if (relation.Entity1 == item && relation.Entity2 == e2)
						{
							hit = false;
							break;
						}
					}
				}
				if (hit)
				{
					list.Add(item);
				}
			}
			return list;
		}
		public IEnumerable<TreeEntityListItem> FindNotRelatedE1TreeItem(E2 e2, ICollection<TreeEntityListItem> e1s)
		{
			var related = this.FindRelatedE1(e2);
			var list = new List<TreeEntityListItem>();
			foreach (var item in e1s)
			{
				bool hit = true;
				if (hit)
				{
					foreach (var relation in related)
					{
						if (relation.Entity1 == item.Entity && relation.Entity2 == e2)
						{
							hit = false;
							break;
						}
					}
				}
				if (hit)
				{
					list.Add(item);
				}
			}
			return list;
		}
		public IEnumerable<E1> FindNotRelatedE1TreeItemEntity(E2 e2, ICollection<TreeEntityListItem> e1s)
		{
			var related = this.FindRelatedE1(e2);
			var list = new List<E1>();
			foreach (var item in e1s)
			{
				bool hit = true;
				if (hit)
				{
					foreach (var relation in related)
					{
						if (relation.Entity1 == item.Entity && relation.Entity2 == e2)
						{
							hit = false;
							break;
						}
					}
				}
				if (hit)
				{
					list.Add(item.Entity as E1);
				}
			}
			return list;
		}

		/// <summary>
		/// 新しい関連付けを追加
		/// </summary>
		/// <param name="e1">E1</param>
		/// <param name="e2">E2</param>
		public void AddRelate(E1 e1, E2 e2)
		{
			if (e1 == null || e2 == null)
			{
				return;
			}
			if (!this.Relations.ContainsIf((item) => item.Entity1 == e1 && item.Entity2 == e2))
			{
				// 順番の付け方が指定されていれば、指定された方法で挿入場所を検索する
				var val = Container<R>.Create(e1, e2);
				this.Relations.Add(val);
				this.RelationAdded(this, new RelationAddedEventArgs<E1, E2>(e1, e2));
			}
		}

		/// <summary>
		/// 関連付けを削除
		/// </summary>
		/// <param name="e1">E1</param>
		/// <param name="e2">E2</param>
		public void RemoveRelate(E1 e1, E2 e2)
		{
			this.Relations.RemoveIf((item) => item.Entity1 == e1 && item.Entity2 == e2);
		}

		/// <summary>
		/// ジェネリックから、引数のあるnewを呼び出すために使うクラス
		/// http://qiita.com/Temarin/items/21650af9edbbbc9ebc97
		/// </summary>
		/// <typeparam name="RR"></typeparam>
		public class Container<RR> where RR : EntityRelateBase<E1, E2>
		{
			private static Func<E1, E2, RR> instanceCreateCache = createInstanceDelegate<E1, E2, RR>();

			public static RR Create(E1 e1, E2 e2) => instanceCreateCache(e1, e2);

			private static Func<T1, T2, TInstance> createInstanceDelegate<T1, T2, TInstance>()
			{
				var info = typeof(TInstance).GetTypeInfo();
				foreach (var ctor in info.DeclaredConstructors)
				{
					if (ctor.GetParameters().Count() == 2 && ctor.IsPublic && !ctor.IsStatic
						&& ctor.GetParameters()[0].ParameterType == typeof(T1)
						&& ctor.GetParameters()[1].ParameterType == typeof(T2))
					{
						var arg1s = Expression.Parameter(typeof(T1));
						var arg2s = Expression.Parameter(typeof(T2));
						return Expression.Lambda<Func<T1, T2, TInstance>>(Expression.New(ctor, arg1s, arg2s), arg1s, arg2s).Compile();
					}
				}
				/*
				var constructor = typeof(TInstance).GetConstructor
					(BindingFlags.Public | BindingFlags.Instance,
					Type.DefaultBinder,
					new[] { typeof(T1), typeof(T2) },
					null);
				var arg1 = Expression.Parameter(typeof(T1));
				var arg2 = Expression.Parameter(typeof(T2));
				return Expression.Lambda<Func<T1, T2, TInstance>>(Expression.New(constructor, arg1, arg2), arg1, arg2).Compile();
				*/
				return (a,b) => default(TInstance);
			}
		}
	}
}
