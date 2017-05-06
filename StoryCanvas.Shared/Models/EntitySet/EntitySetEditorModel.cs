using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.EntitySet
{
	/// <summary>
	/// 要素の一覧を表示して、検索や並べ替え機能などを提供し、編集する要素を選択するためのモデル
	/// </summary>
    public abstract class EntitySetEditorModel<E, PMT> : INotifyPropertyChanged where E : Entity
	{
		public EntitySetEditorModel(string entityType, EntitySetModel<E> entitySet, Func<E, PMT> primaryEditMessageDelegate)
		{
			this.EntityType = entityType;
			this.EntitySet = entitySet;
			this.PrimaryEditMessageDelegate = primaryEditMessageDelegate;

			this.EntitySelection.SelectionChanged += (a, b) =>
			{
				this.OnPropertyChanged("Selected" + this.EntityType);
				Messenger.Default.Send(this, this.PrimaryEditMessageDelegate(a));
			};

			this.SearchModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "Keyword")
				{
					this.OnPropertyChanged("SearchKeyword");
				}
				else if (e.PropertyName == "IsValid")
				{
					this.OnPropertyChanged("IsSearching");
				}
			};
		}

		/// <summary>
		/// エンティティの種類を文字列で表したもの
		/// </summary>
		public string EntityType { get; private set; }

		/// <summary>
		/// 要素の集合
		/// </summary>
		public EntitySetModel<E> EntitySet { get; private set; }

		/// <summary>
		/// 要素の編集画面を開くメッセージを生み出すデリゲート
		/// </summary>
		public Func<E, PMT> PrimaryEditMessageDelegate { get; private set; }

		/// <summary>
		/// リストの中でどの要素が選択されているかのモデル
		/// </summary>
		public EntitySelectionModel<E> EntitySelection { get; private set; } = new EntitySelectionModel<E>();

		/// <summary>
		/// 現在選択されている要素
		/// </summary>
		public E SelectedEntity
		{
			get
			{
				return this.EntitySelection.Selected;
			}
			set
			{
				this.EntitySelection.Selected = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 要素を検索するためのロジック
		/// </summary>
		protected EntitySearchModel SearchModel { get; private set; } = new EntitySearchModel();

		/// <summary>
		/// 検索ワード
		/// </summary>
		public string SearchKeyword
		{
			get
			{
				return this.SearchModel.Keyword;
			}
			set
			{
				this.SearchModel.Keyword = value;
			}
		}

		/// <summary>
		/// 今、検索中であるか
		/// </summary>
		public bool IsSearching
		{
			get
			{
				return this.SearchModel.IsValid;
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}
}
