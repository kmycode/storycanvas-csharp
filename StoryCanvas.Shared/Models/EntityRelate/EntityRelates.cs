using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Story;
using System.Linq;
using System.Collections.ObjectModel;

namespace StoryCanvas.Shared.Models.EntityRelate
{
	[DataContract]
	public class PersonSceneEntityRelate : EntityRelateBase<PersonEntity, SceneEntity>
	{
		public PersonSceneEntityRelate(PersonEntity entity1, SceneEntity entity2) : base(entity1, entity2)
		{
		}

		/// <summary>
		/// 人物の年齢が計算できる状態にあるか
		/// </summary>
		public bool CanCalcPersonAge
		{
			get
			{
				return this.Entity1.BirthDay?.Date != null && this.Entity2.StartDateTime != null;
			}
		}

		/// <summary>
		/// シーン開始日当時の人物の年齢
		/// </summary>
		public int PersonAge
		{
			get
			{
				if (this.CanCalcPersonAge)
				{
					//var d = this.Entity2.StartDateTime.Date - this.Entity1.BirthDay.Date;
					//return d.Year;
					var startDate = this.Entity2.StartDateTime.Date;
					var birthDay = this.Entity1.BirthDay.Date;
					var year_d = startDate.Year - birthDay.Year - 1 + (startDate.Month > birthDay.Month ? 1 : ((startDate.Month == birthDay.Month && startDate.Day >= birthDay.Day) ? 1 : 0));
					return year_d;
				}
				return 0;
			}
		}
	}

	[DataContract]
	public class PlaceSceneEntityRelate : EntityRelateBase<PlaceEntity, SceneEntity>
	{
		public PlaceSceneEntityRelate(PlaceEntity entity1, SceneEntity entity2) : base(entity1, entity2)
		{
		}
	}

	[DataContract]
	public class SceneChapterEntityRelate : EntityRelateBase<SceneEntity, ChapterEntity>
	{
		public SceneChapterEntityRelate(SceneEntity entity1, ChapterEntity entity2) : base(entity1, entity2)
		{
		}
	}

	[DataContract]
	public class SexPersonEntityRelate : EntityRelateBase<SexEntity, PersonEntity>
	{
		public SexPersonEntityRelate(SexEntity entity1, PersonEntity entity2) : base(entity1, entity2)
		{
		}
	}

	[DataContract]
	public class WordPersonEntityRelate : EntityRelateBase<WordEntity, PersonEntity>
	{
		public WordPersonEntityRelate(WordEntity entity1, PersonEntity entity2) : base(entity1, entity2)
		{
		}
	}

	[DataContract]
	public class WordSceneEntityRelate : EntityRelateBase<WordEntity, SceneEntity>
	{
		public WordSceneEntityRelate(WordEntity entity1, SceneEntity entity2) : base(entity1, entity2)
		{
		}
	}

	[DataContract]
	public class GroupPersonEntityRelate : EntityRelateBase<GroupEntity, PersonEntity>
	{
		public GroupPersonEntityRelate(GroupEntity entity1, PersonEntity entity2) : base(entity1, entity2)
		{
		}

		[DataMember]
		private ObservableCollection<ParameterPair> _parameterPairs;
		public ICollection<ParameterPair> ParameterPairs
		{
			get
			{
				if (this._parameterPairs == null)
				{
					this._parameterPairs = new ObservableCollection<ParameterPair>();
				}
				return this._parameterPairs;
			}
		}

		/// <summary>
		/// XF専用
		/// </summary>
		public ICollection<PersonParameterAtGroupEntityRelate> AllParameters
		{
			get
			{
				var parameters = new Collection<PersonParameterAtGroupEntityRelate>();
				foreach (var pair in this.ParameterPairs)
				{
					foreach (var parameter in pair.Parameters)
					{
						parameters.Add(parameter);
					}
				}
				return parameters;
			}
		}

		/// <summary>
		/// パラメータと集団のペア
		/// （集団、親集団、祖父集団、・・・というのを想定）
		/// </summary>
		[DataContract]
		public class ParameterPair
		{
			[DataMember]
			private EntityReferenceModel<GroupEntity> _groupReference = new EntityReferenceModel<GroupEntity>();
			public GroupEntity Group
			{
				get
				{
					return this._groupReference.Entity;
				}
				set
				{
					this._groupReference.Entity = value;
				}
			}
			[DataMember]
			public EntityListModel<PersonParameterAtGroupEntityRelate> Parameters { get; private set; } = new EntityListModel<PersonParameterAtGroupEntityRelate>();

			public void GiveGroupSet(EntitySetModel<GroupEntity> groups)
			{
				this._groupReference.FindEntity(groups);

				// 自分の集団インスタンスが入手できたら、パラメータの中身にパラメータインスタンス本体を設定する
				foreach (var param in this.Parameters)
				{
					param.GiveParameterEntitySet(this.Group.RelatedPersonParameters);
				}
			}
		}

		public override void GiveEntitySet(EntitySetModel<GroupEntity> e1, EntitySetModel<PersonEntity> e2)
		{
			base.GiveEntitySet(e1, e2);

			// 集団とパラメータ中身のペアに、集団インスタンスを設定
			foreach (var pair in this.ParameterPairs)
			{
				pair.GiveGroupSet(e1);
			}
		}

		/// <summary>
		/// パラメータ情報を最新の状態に更新する
		/// </summary>
		public void ReloadParameters()
		{
			// rootの手前まで遡って、先祖パラメータを列挙する
			var groups = this.Entity1.GetAncestors();

			// 存在しないペアを除外
			{
				var pairsCount = this.ParameterPairs.Count;
				for (int i = 0; i < pairsCount; i++)
				{
					var pair = this.ParameterPairs.ElementAt(i);
					if (!groups.Any((group) => group != null && group.Id == pair.Group.Id))
					{
						this.ParameterPairs.Remove(pair);
						i--;
						pairsCount--;
					}
				}
			}

			// 存在するペアを追加、並べ替え
			{
				var newPairs = new Collection<ParameterPair>();
				foreach (GroupEntity group in groups)
				{
					var pair = this.ParameterPairs.Where((p) => p != null && p.Group.Id == group.Id);
					if (pair.Count() == 0)
					{
						var newPair = new ParameterPair { Group = group, };
						newPairs.Add(newPair);
					}
					else
					{
						newPairs.Add(pair.ElementAt(0));
					}
				}
				this.ParameterPairs.Clear();
				foreach (var pair in newPairs)
				{
					this.ParameterPairs.Add(pair);
				}
			}

			foreach (var pair in this.ParameterPairs)
			{
				// 現在集団に登録されていないパラメータを除外
				var removeParameters = from param in pair.Parameters
									   where param.Entity2 == null || !pair.Group.RelatedPersonParameters.Any(p => p != null && p.Id == param.Entity2.Id)
									   select param;
				pair.Parameters.RemoveIf(param => removeParameters.Any(p => p != null && param.Id == p.Id));

				// 現在集団に登録されているパラメータを追加
				var addParameters = from param in pair.Group.RelatedPersonParameters
									where !pair.Parameters.Any(p => p != null && p.Entity2.Id == param.Id)
									select param;
				foreach (var param in addParameters)
				{
					pair.Parameters.Add(new PersonParameterAtGroupEntityRelate(param));
				}
			}

#if XAMARIN_FORMS
			// XFはAllParametersを使うので、変更を通知する
			this.OnPropertyChanged("AllParameters");
#endif
		}
	}

	[DataContract]
	public class PersonPersonEntityRelate : FocusableEntityRelateBase<PersonEntity, PersonEntity>
	{
		/// <summary>
		/// 1が2を呼ぶときの呼び方
		/// </summary>
		[DataMember]
		private string _call1to2;
		public string Call1to2
		{
			get
			{
				return this._call1to2;
			}
			set
			{
				this._call1to2 = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 2が1を呼ぶときの呼び方
		/// </summary>
		[DataMember]
		private string _call2to1;
		public string Call2to1
		{
			get
			{
				return this._call2to1;
			}
			set
			{
				this._call2to1 = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// フォーカスされている人物がされていない人物を呼ぶときの呼び方
		/// </summary>
		public string FocusedPersonCallNotFocused
		{
			get
			{
				if (this.FocusedEntity.Id == this.Entity1.Id)
				{
					return this.Call1to2;
				}
				return this.Call2to1;
			}
			set
			{
				if (this.FocusedEntity.Id == this.Entity1.Id)
				{
					this.Call1to2 = value;
				}
				else
				{
					this.Call2to1 = value;
				}
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// フォーカスされていない人物がされている人物を呼ぶときの呼び方
		/// </summary>
		public string NotFocusedPersonCallFocused
		{
			get
			{
				if (this.FocusedEntity.Id == this.Entity1.Id)
				{
					return this.Call2to1;
				}
				return this.Call1to2;
			}
			set
			{
				if (this.FocusedEntity.Id == this.Entity1.Id)
				{
					this.Call2to1 = value;
				}
				else
				{
					this.Call1to2 = value;
				}
				this.OnPropertyChanged();
			}
		}

		public PersonPersonEntityRelate(PersonEntity entity1, PersonEntity entity2) : base(entity1, entity2)
		{
		}
	}

	[DataContract]
	public abstract class ParameterEntityRelateBase<E> : EntityRelateBase<E, ParameterEntity> where E : IEntity
	{
		public ParameterEntityRelateBase(E entity1, ParameterEntity entity2) : base(entity1, entity2)
		{
		}

		/// <summary>
		/// 論理値
		/// </summary>
		[DataMember]
		private bool _booleanValue;
		public bool BooleanValue
		{
			get
			{
				return this._booleanValue;
			}
			set
			{
				this._booleanValue = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 文字列値
		/// </summary>
		[DataMember]
		private string _stringValue;
		public string StringValue
		{
			get
			{
				return this._stringValue;
			}
			set
			{
				this._stringValue = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 選択肢のID
		/// </summary>
		[DataMember]
		private long _selectValue;
		public long SelectValue
		{
			get
			{
				return this._selectValue;
			}
			set
			{
				this._selectValue = value;
				this.OnPropertyChanged();
			}
		}
	}

	[DataContract]
	public class PersonParameterEntityRelate : ParameterEntityRelateBase<PersonEntity>
	{
		public PersonParameterEntityRelate(PersonEntity entity1, ParameterEntity entity2) : base(entity1, entity2)
		{
		}
	}

	/// <summary>
	/// ある集団に関連付けられた人物に設定するパラメータのデータ
	/// </summary>
	[DataContract]
	public class PersonParameterAtGroupEntityRelate : ParameterEntityRelateBase<Entity>
	{
		protected PersonParameterAtGroupEntityRelate(Entity entity1, ParameterEntity entity2) : base(entity1, entity2)
		{
		}

		public PersonParameterAtGroupEntityRelate(ParameterEntity entity2) : base(new NullEntity(), entity2)
		{
		}

		public void GiveParameterEntitySet(EntitySetModel<ParameterEntity> parameters)
		{
			this.Entity2Reference.FindEntity(parameters);
		}
	}

	[DataContract]
	public class EntityRelate<E1, E2> : FocusableEntityRelateBase<E1, E2> where E1 : IEntity where E2 : IEntity
	{
		public EntityRelate(E1 entity1, E2 entity2) : base(entity1, entity2)
		{
		}
	}

	/// <summary>
	/// エンティティ関連付けのインターフェイス
	/// </summary>
	/// <typeparam name="E1"></typeparam>
	/// <typeparam name="E2"></typeparam>
	public interface IEntityRelate<out E1, out E2> where E1 : IEntity where E2 : IEntity
	{
		E1 Entity1
		{
			get;
		}
		
		E2 Entity2
		{
			get;
		}
	}

	public interface IRelation
	{
		IEntity Entity1
		{
			get;
		}

		IEntity Entity2
		{
			get;
		}
	}

	public interface IRelationEntity : IRelation, IEntity
	{
	}

	/// <summary>
	/// エンティティ関連付け
	/// abstractにしたほうがのぞましいのだが、ジェネリックにnew制約がついてしまう
	/// </summary>
	[DataContract]
	public class EntityRelateBase<E1, E2> : IRelationEntity, IEntityRelate<E1, E2>, INotifyPropertyChanged where E1 : IEntity where E2 : IEntity
	{
		[DataMember]
		private long _id;
		public long Id
		{
			get
			{
				return this._id;
			}
			private set
			{
				this._id = value;
			}
		}

		[DataMember]
		private long _order;
		public long Order
		{
			get
			{
				return this._order;
			}
			set
			{
				this._order = value;
				this.OnPropertyChanged();
			}
		}

		[DataMember]
		private EntityReferenceModel<E1> _entity1 = new EntityReferenceModel<E1>();
		public E1 Entity1
		{
			get
			{
				return this._entity1.Entity;
			}
		}
		protected EntityReferenceModel<E1> Entity1Reference
		{
			get
			{
				return this._entity1;
			}
		}

		[DataMember]
		private EntityReferenceModel<E2> _entity2 = new EntityReferenceModel<E2>();
		public E2 Entity2
		{
			get
			{
				return this._entity2.Entity;
			}
		}
		protected EntityReferenceModel<E2> Entity2Reference
		{
			get
			{
				return this._entity2;
			}
		}

		[DataMember]
		private string _note;
		public virtual string Note
		{
			get
			{
				return this._note ?? "";
			}
			set
			{
				this._note = value;
				this.OnPropertyChanged();
			}
		}

		IEntity IRelation.Entity1
		{
			get
			{
				return this._entity1.Entity;
			}
		}

		IEntity IRelation.Entity2
		{
			get
			{
				return this._entity2.Entity;
			}
		}

		public EntityRelateBase(E1 entity1, E2 entity2)
		{
			this.Id = this.Order = Entity.GetNewEntityID();
			this._entity1.Entity = entity1;
			this._entity2.Entity = entity2;
		}

		public virtual void GiveEntitySet(EntitySetModel<E1> e1, EntitySetModel<E2> e2)
		{
			this._entity1.FindEntity(e1);
			this._entity2.FindEntity(e2);
		}

		public bool IsValid
		{
			get
			{
				return this.Entity1 != null && this.Entity2 != null;
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

	[DataContract]
	public abstract class FocusableEntityRelateBase<E1, E2> : EntityRelateBase<E1, E2>, INotifyPropertyChanged where E1 : IEntity where E2 : IEntity
	{
		public FocusableEntityRelateBase(E1 entity1, E2 entity2) : base(entity1, entity2)
		{
			this.FocusedEntity = entity1;
		}

		/// <summary>
		/// フォーカスをあわせるエンティティ
		/// </summary>
		[DataMember]
		private EntityReferenceModel<IEntity> _focusedEntity = new EntityReferenceModel<IEntity>();
		public IEntity FocusedEntity
		{
			get
			{
				return this._focusedEntity.Entity;
			}
			set
			{
				this._focusedEntity.Entity = value;
			}
		}

		/// <summary>
		/// フォーカスのあっていないエンティティ
		/// </summary>
		public IEntity NotFocusedEntity
		{
			get
			{
				if (this.IsSameEntity(this.FocusedEntity, this.Entity1))
				{
					return this.Entity2;
				}
				else if (this.IsSameEntity(this.FocusedEntity, this.Entity2))
				{
					return this.Entity1;
				}
				return null;
			}
		}

		// _note      : Entity1 の note
		// _otherNote : Entity2 の note
		[DataMember]
		private string _otherNote;

		/// <summary>
		/// フォーカスに応じて返す値を変える
		/// </summary>
		[DataMember]
		public override string Note
		{
			get
			{
				if (this.IsSameEntity(this.FocusedEntity, this.Entity1))
				{
					return base.Note;
				}
				else if (this.IsSameEntity(this.FocusedEntity, this.Entity2))
				{
					return this._otherNote;
				}
				return null;
			}
			set
			{
				if (this.IsSameEntity(this.FocusedEntity, this.Entity1))
				{
					base.Note = value;
				}
				else if (this.IsSameEntity(this.FocusedEntity, this.Entity2))
				{
					this._otherNote = value;
				}
				this.OnPropertyChanged("Note");
                this.OnPropertyChanged("OtherNote");
            }
        }
        public string OtherNote
        {
            get
            {
                if (this.IsSameEntity(this.FocusedEntity, this.Entity1))
                {
                    return this._otherNote;
                }
                else if (this.IsSameEntity(this.FocusedEntity, this.Entity2))
                {
                    return base.Note;
                }
                return null;
            }
            set
            {
                if (this.IsSameEntity(this.FocusedEntity, this.Entity1))
                {
                    this._otherNote = value;
                }
                else if (this.IsSameEntity(this.FocusedEntity, this.Entity2))
                {
                    base.Note = value;
                }
                this.OnPropertyChanged("OtherNote");
                this.OnPropertyChanged("Note");
            }
        }

        private bool IsSameEntity(IEntity e1, IEntity e2)
		{
			return e1 != null && e2 != null && e1.Id == e2.Id && e1.GetType() == e2.GetType();
		}
	}

	/// <summary>
	/// 要素の種類と関連付けオブジェクトを渡せば、もう一方の要素をRelatedEntityに入れてくれる
	/// 例えば、PersonEntityとSceneEntityの関連付けを、EにPersonEntityを指定して渡すと、
	/// EntityプロパティにはPersonEntity、RelatedEntityプロパティにはSceneEntityが入る
	/// </summary>
	/// <typeparam name="E"></typeparam>
	public class OnesideEntityRelationWrapper<E> where E : Entity
	{
		public OnesideEntityRelationWrapper(IEntityRelate<Entity,Entity> relate)
		{
			if (relate.Entity1.GetType() == typeof(E))
			{
				this.Entity = (E)relate.Entity1;
				this.RelatedEntity = relate.Entity2;
			}
			else
			{
				this.Entity = (E)relate.Entity2;
				this.RelatedEntity = relate.Entity1;
			}
		}
		public E Entity { get; private set; }
		public IEntity RelatedEntity { get; private set; }
	}
}
