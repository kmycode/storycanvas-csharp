using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.Entities;
using System.Runtime.Serialization;

namespace StoryCanvas.Shared.Models.EntitySet
{
	[DataContract]
	[Obsolete("未テスト")]
	public class EntityFilteredListModel<E> : EntityListModel<E> where E : Entity
	{
		/// <summary>
		/// 隠されたエンティティも含む全てのエンティティ
		/// </summary>
		[DataMember]
		private readonly ObservableCollection<E> _allEntities = new ObservableCollection<E>();
		protected ObservableCollection<E> AllEntities
		{
			get
			{
				return this._allEntities;
			}
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public EntityFilteredListModel()
		{
			base.EntityRemoved += (item) =>
			{
				this.AllEntities.Remove((E)item);
			};
		}

		/// <summary>
		/// フィルタリングを行い、条件にマッチしたエンティティのみを外部から見えるようにする
		/// </summary>
		/// <param name="filter">フィルタリング条件</param>
		public void Filtering(Predicate<E> filter)
		{
			base.RemoveAll();

			var showItems = from item in this.AllEntities
							where filter(item)
							select item;
			foreach (E item in showItems)
			{
				this.Add(item);
			}
		}

		/// <summary>
		/// 全てのエンティティを隠す。他の処理から見えない状態にする
		/// </summary>
		public void HideAll()
		{
			this.Filtering((item) => false);
		}

		/// <summary>
		/// 全てのエンティティを表示する。他の処理から見える状態にする
		/// </summary>
		public void ShowAll()
		{
			this.Filtering((item) => true);
		}

		/// <summary>
		/// エンティティの順番を1つずつ足し算してずらす（ひき算するメソッドは、orderの仕様上意味が無いので作らない）
		/// ただし、指定したエンティティ（startEntity）がリストに登録されていない場合、そのエンティティに対しては処理を行わない
		/// </summary>
		/// <param name="startEntity">指定するエンティティ。orderの値がこのエンティティのもの以上であるエンティティに対して処理される</param>
		[Obsolete]
		public override void ShiftOrder(E startEntity)
		{
			EntitySetModel<E>.ShiftOrder(this.AllEntities, startEntity);
		}

		/// <summary>
		/// 指定したIDのエンティティを取得する
		/// </summary>
		/// <param name="id">エンティティのID</param>
		/// <returns>エンティティ。見つからなければnull</returns>
		public override E FindId(long id)
		{
			foreach (E item in this.AllEntities)
			{
				if (item.Id == id)
				{
					return item;
				}
			}
			return null;
		}

		/// <summary>
		/// エンティティの総数を取得（隠されたエンティティも含む）
		/// </summary>
		public int CountAll
		{
			get
			{
				return this.AllEntities.Count;
			}
		}
	}
}
