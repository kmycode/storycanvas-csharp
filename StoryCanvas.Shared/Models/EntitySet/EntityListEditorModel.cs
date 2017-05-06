using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;

namespace StoryCanvas.Shared.Models.EntitySet
{
	public class EntityListEditorModel<E, PMT> : EntitySetEditorModel<E, PMT> where E : Entity, new()
	{
		public EntityListEditorModel(string entityType, EntityListModel<E> entitySet, Func<E, PMT> primaryEditMessageDelegate)
			: base(entityType, entitySet, primaryEditMessageDelegate)
		{
			this.Commands = new EntityListCommandModel<E>(() => new E(),
															() => this.SelectedEntity,
															(entity) => this.SelectedEntity = entity,
															this.EntityList);

			this.SearchModel.EntitySearchRequested += (sender, e) =>
			{
				StoryModel.Current.Search(entitySet, e.Query);
				this.OnPropertyChanged("SearchResult");
				this.OnPropertyChanged(this.EntityType + "SearchResult");
			};
			this.SearchModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "IsValid")
				{
					this.Commands.IsEnabled = !this.SearchModel.IsValid;
				}
			};
		}

		/// <summary>
		/// 要素の集合をリストに変換
		/// </summary>
		private EntityListModel<E> EntityList
		{
			get
			{
				return (EntityListModel<E>)this.EntitySet;
			}
		}

		/// <summary>
		/// 要素のリストを取得
		/// </summary>
		public EntityListModel<E> Entities
		{
			get
			{
				return this.EntityList;
			}
		}

		/// <summary>
		/// コマンド一覧
		/// </summary>
		public EntityListCommandModel<E> Commands { get; private set; }

		/// <summary>
		/// 検索結果
		/// </summary>
		public IEnumerable<E> SearchResult
		{
			get
			{
				return from item in this.EntityList
					   where item.IsSearchHit
					   select item;
			}
		}
	}
}
