using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntityRelate;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;
using StoryCanvas.Shared.Common;
using System.Collections.ObjectModel;

namespace StoryCanvas.Shared.Models.Entities
{
	[DataContract]
	public class PersonEntity : Entity
	{
		/// <summary>
		/// 要素を新規追加する場合にのみ処理（データからロードする時は処理されない）
		/// </summary>
		public PersonEntity()
		{
			// パラメータで「すべての人物に付与する」設定がされているものを走査、必要であれば追加
			if (this.StoryModel != null)
			{
				foreach (var parameter in this.StoryModel.Parameters)
				{
					if (parameter.IsForAllPeople)
					{
						this.StoryModel.PersonParameterRelation.AddRelate(this, parameter);
					}
				}
			}
		}

		protected override void LoadStoryModelCompleted()
		{
			//this._sex.ToEntity(this.StoryModel.Sexes);
		}

		/// <summary>
		/// 姓
		/// </summary>
		[DataMember]
		private string _lastName;
		public string LastName
		{
			get
			{
				return this._lastName;
			}
			set
			{
				this._lastName = value;
				this.UpdateName();
			}
		}

		/// <summary>
		/// 名
		/// </summary>
		[DataMember]
		private string _firstName;
		public string FirstName
		{
			get
			{
				return this._firstName;
			}
			set
			{
				this._firstName = value;
				this.UpdateName();
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		/// <summary>
		/// 欧米人であるか？（FirstNameを先にするか？）
		/// </summary>
		[DataMember]
		private bool _isWesternerName = bool.Parse(StringResourceResolver.Resolve("LocationSetting_IsWesternerName"));
		public bool IsWesternerName
		{
			get
			{
				return this._isWesternerName;
			}
			set
			{
				this._isWesternerName = value;
				this.OnPropertyChanged();
				this.UpdateName();
			}
		}

		/// <summary>
		/// 性別（関連付けという形で保存）
		/// </summary>
		public SexEntity Sex
		{
			get
			{
				return this.StoryModel?.SexPersonRelation.FindRelated(this).SingleOrDefault()?.Entity1;
			}
			set
			{
				this.StoryModel?.SexPersonRelation.AddRelate(value, this);
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 誕生日
		/// </summary>
		[DataMember]
		private StoryDateTime _birthDay;
		public StoryDateTime BirthDay
		{
			get
			{
				return this._birthDay;
			}
			set
			{
				this._birthDay = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 死亡日
		/// </summary>
		[DataMember]
		private StoryDateTime _deathDay;
		public StoryDateTime DeathDay
		{
			get
			{
				return this._deathDay;
			}
			set
			{
				this._deathDay = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 関連付けられた人物
		/// </summary>
		public ObservableCollection<PersonPersonEntityRelate> RelatedPeople
		{
			get
			{
                return this._relatedPeople = this._relatedPeople ?? new ObservableCollection<PersonPersonEntityRelate>();
			}
		}
        private ObservableCollection<PersonPersonEntityRelate> _relatedPeople;

		/// <summary>
		/// 関連付けられた集団
		/// </summary>
		public IEnumerable<GroupPersonEntityRelate> RelatedGroups
		{
			get
			{
				return this.StoryModel.GroupPersonRelation.FindRelated(this);
			}
		}

		/// <summary>
		/// 関連付けられていない集団（ツリーアイテム）
		/// </summary>
		public IEnumerable<TreeEntityListItem> NotRelatedGroupTreeItems
		{
			get
			{
				return this.StoryModel.GroupPersonRelation.FindNotRelatedTreeItems(this, this.StoryModel.Groups.List);
			}
		}

		/// <summary>
		/// 関連付けられていない集団
		/// </summary>
		public IEnumerable<GroupEntity> NotRelatedGroups
		{
			get
			{
				return this.StoryModel.GroupPersonRelation.FindNotRelatedTreeItemEntities(this, this.StoryModel.Groups.List);
			}
		}

		/// <summary>
		/// 関連付けられたパラメータ
		/// </summary>
		public IEnumerable<PersonParameterEntityRelate> RelatedParameters
		{
			get
			{
				return this.StoryModel.PersonParameterRelation.FindRelated(this);
			}
		}

		/// <summary>
		/// 関連付けられていないパラメータ
		/// </summary>
		public IEnumerable<ParameterEntity> NotRelatedParameters
		{
			get
			{
				return this.StoryModel.PersonParameterRelation.FindNotRelated(this, this.StoryModel.Parameters);
			}
		}

		/// <summary>
		/// 関連付けられた用語
		/// </summary>
		public IEnumerable<WordPersonEntityRelate> RelatedWords
		{
			get
			{
                return this.StoryModel.WordPersonRelation.FindRelated(this);
			}
		}

		/// <summary>
		/// 関連付けられていない用語（ツリーアイテム）
		/// </summary>
		public IEnumerable<TreeEntityListItem> NotRelatedWordTreeItems
		{
			get
			{
				return this.StoryModel.WordPersonRelation.FindNotRelatedTreeItems(this, this.StoryModel.Words.List);
			}
		}

		/// <summary>
		/// 関連付けられていない用語
		/// </summary>
		public IEnumerable<WordEntity> NotRelatedWords
		{
			get
			{
				return this.StoryModel.WordPersonRelation.FindNotRelatedTreeItemEntities(this, this.StoryModel.Words.List);
			}
		}

		/// <summary>
		/// エンティティの名前を、登場人物の姓名を組み合わせたものにする
		/// </summary>
		private void UpdateName()
		{
			base.Name = this.ToString();
		}

		/// <summary>
		/// リソース名
		/// </summary>
		protected override string ResourceName
		{
			get
			{
				return "person";
			}
		}

		/// <summary>
		/// 中身が空であるか
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				return base.IsEmpty &&
					this.BirthDay == null &&
					this.DeathDay == null;
			}
		}

        /// <summary>
        /// 関連付け内容を更新する
        /// </summary>
        public void UpdateRelations()
        {
            this.RelatedPeople.Clear();
            this.RelatedPeople.AddRange(this.StoryModel.PersonPersonRelation
                                                       .FindRelated(this)
                                                       .Cast<PersonPersonEntityRelate>());
        }

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="query"></param>
		protected override bool IsWordExists(string word)
		{
			if (!base.IsWordExists(word))
			{
				bool? r = false;
				r |= this.Sex?.Name?.IndexOf(word) >= 0;
				foreach (var item in this.RelatedPeople)
				{
					r |= ((PersonEntity)item.NotFocusedEntity).Name?.IndexOf(word) >= 0;
					r |= item.Note?.IndexOf(word) >= 0;
				}
				foreach (var item in this.RelatedGroups)
				{
					r |= item.Entity1.Name?.IndexOf(word) >= 0;
					r |= item.Note?.IndexOf(word) >= 0;
				}
				foreach (var item in this.RelatedParameters)
				{
					r |= item.StringValue?.IndexOf(word) >= 0;
				}

				return r == true;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// 文字列に変換
		/// </summary>
		/// <returns>文字列</returns>
		public override string ToString()
		{
			string s = "";
			if (this.LastName != null && this.FirstName != null)
			{
				if (this.IsWesternerName)
				{
					s = $"{this.FirstName} {this.LastName}";
				}
				else
				{
					s = $"{this.LastName} {this.FirstName}";
				}
			}
			else if (this.LastName != null)
			{
				s = this.LastName;
			}
			else if (this.FirstName != null)
			{
				s = this.FirstName;
			}
			return s;
		}
	}
}
