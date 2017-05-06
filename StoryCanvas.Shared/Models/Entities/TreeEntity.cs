using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using StoryCanvas.Shared.Models.EntitySet;

namespace StoryCanvas.Shared.Models.Entities
{
	public delegate void UpdateEntityParentEventHandler(TreeEntity entity, TreeEntity oldParent, TreeEntity newParent);

	[DataContract]
	public abstract class TreeEntity : Entity
	{
		/// <summary>
		/// 親エンティティ
		/// </summary>
		private WeakReference<TreeEntity> _parent;
		public TreeEntity Parent
		{
			get
			{
				TreeEntity entity = null;
				this._parent.TryGetTarget(out entity);
				return entity;
			}
			set
			{
				if (this.Parent != value)
				{
					TreeEntity oldParent = this.Parent;

					this.Parent?.Children.Remove(this);
					if (value?.Children.Contains(this) != true)
					{
						value?.Children.Add(this);
					}
					this._parent.SetTarget(value);

					// イベント発行
					this.ParentChanged(this, oldParent, value);
				}
			}
		}

		/// <summary>
		/// 親エンティティが変更された時に発行される
		/// </summary>
		public event UpdateEntityParentEventHandler ParentChanged;

		/// <summary>
		/// 子孫エンティティ（つまり孫以降も含む）のどこかで親エンティティが変更された時に発行される
		/// ParentChangedが発行された時、同時にこれも発行されるようになっている
		/// </summary>
		public event UpdateEntityParentEventHandler ParentChangedOnAllDescendants;

		/// <summary>
		/// 子エンティティ
		/// </summary>
		[DataMember]
		private EntityListModel<TreeEntity> _children;
		public EntityListModel<TreeEntity> Children
		{
			get
			{
				return this._children;
			}
		}

		/// <summary>
		/// 初期化処理。子エンティティのリストを操作した時に、親エンティティを自動的に設定するよう細工
		/// </summary>
		public TreeEntity()
		{
			this._children = new EntityListModel<TreeEntity>();
			this.Initialize(default(StreamingContext));
		}

		[OnDeserialized]
		private new void Initialize(StreamingContext context)
		{
			this._parent = new WeakReference<TreeEntity>(null);
			this.ParentChanged = delegate { };
			this.ParentChangedOnAllDescendants = delegate { };

			this.AddChildrenEntityChangedEvent();

			// デシリアライズ時は、この時点で子がいる
			if (this.Children.Count > 0)
			{
				foreach (TreeEntity child in this.Children)
				{
					child.AddChildrenEntityChangedEvent();
					child.ParentChangedOnAllDescendants += this.CallParentChangedOnAllDescendants;
				}
			}

			// ２つのイベントが同時に起こるようにする
			this.ParentChanged += (a, b, c) => this.ParentChangedOnAllDescendants(a, b, c);
		}

		private void AddChildrenEntityChangedEvent()
		{
			// エンティティの子リストと親を連動させる
			// 子のエンティティ変更イベントが親にも来るようにする
			this.Children.EntityAdded += (entity) =>
			{
				entity.ParentChangedOnAllDescendants += this.CallParentChangedOnAllDescendants;
				entity.Parent = this;
			};
			this.Children.EntityRemoved += (entity) =>
			{
				if (entity.Parent == this)
				{
					entity.Parent = null;
					entity.ParentChangedOnAllDescendants -= this.CallParentChangedOnAllDescendants;
				}
			};
		}

		private void CallParentChangedOnAllDescendants(TreeEntity entity, TreeEntity newParent, TreeEntity oldParent)
		{
			this.ParentChangedOnAllDescendants(entity, newParent, oldParent);
		}

		/// <summary>
		/// 指定したIDを持つエンティティを、自分の子孫から検索する
		/// </summary>
		/// <param name="id">エンティティのID</param>
		/// <returns>エンティティ。見つからなければnull</returns>
		public TreeEntity FindId(long id)
		{
			foreach (TreeEntity item in this.Children)
			{
				if (item.Id == id)
				{
					return item;
				}
				TreeEntity e = item.FindId(id);
				if (e != null)
				{
					return e;
				}
			}
			return null;
		}

		[Obsolete("メソッド名がFindIdに変わりました", true)]
		public TreeEntity Get(long id)
		{
			return this.FindId(id);
		}
		
		/// <summary>
		/// 全ての子孫の総数を返す。自身も含まれる
		/// </summary>
		/// <returns>子孫の総数</returns>
		public int CountDescendants()
		{
			int c = 1;          // 自身を1カウント
			foreach (TreeEntity item in this.Children)
			{
				c += item.CountDescendants();
			}
			return c;
		}

		/// <summary>
		/// 先祖要素の配列を取得
		/// </summary>
		/// <returns>rootの１つ下までの先祖要素の配列。なお自身も含まれる</returns>
		public IEnumerable<TreeEntity> GetAncestors()
		{
			var ancestors = new Stack<TreeEntity>();
			var entity = this;
			while (entity.Parent != null)
			{
				ancestors.Push(entity);
				entity = entity.Parent;
			}
			return ancestors;
		}
	}
}
