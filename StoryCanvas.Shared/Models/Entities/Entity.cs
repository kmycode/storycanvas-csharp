using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewTools.Resources;
using StoryCanvas.Shared.Models.Story;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StoryCanvas.Shared.Models.EntitySet;

namespace StoryCanvas.Shared.Models.Entities
{
	public delegate void UpdateEntityOrderEventHandler(Entity entity);

	[DataContract]
	public abstract class Entity : IComparable<Entity>, INotifyPropertyChanged, IEntity
	{
		public static long EntityCount
		{
			get;
			set;
		} = 0;
		private static StoryModel CurrentStory = null;

		#region プロパティ

		/// <summary>
		/// エンティティのID
		/// </summary>
		[DataMember]
		private long _id;
		public long Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// エンティティの名前
		/// </summary>
		[DataMember]
		private string _name;
		public string Name
		{
			get
			{
				return this._name ?? "";
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// エンティティのアイコン
		/// </summary>
		[DataMember]
		private ImageResource _icon;
		public ImageResource Icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				this._icon = value;
				if (value == null || value.IsEmpty)
				{
					this.DisplayIcon = this.DefaultIcon;
				}
				else
				{
					this.DisplayIcon = value;
				}
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// エンティティのデフォルトアイコン（特に画像が設定されていない時のアイコン）
		/// </summary>
		protected ImageResource DefaultIcon
		{
			get
			{
				var ir = new ImageResource
				{
#if WPF
					WPFPath = "pack://application:,,,/" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/EntityIcons/" + this.ResourceName + "_white.png",
#elif WINDOWS_UWP
					UWPPath = "ms-appx:///Assets/EntityIcons/" + this.ResourceName + "_white.png",
#elif XAMARIN_FORMS
					DroidPath = this.ResourceName + ".png",
					IOSPath = "EntityIcons/" + this.ResourceName + ".png",
#endif
					ResourceType = ImageResource.Type.ApplicationResource
				};
				return ir;
			}
		}

		/// <summary>
		/// エンティティ固有のリソース名。例えば人物の場合は「people」が返される
		/// </summary>
		abstract protected string ResourceName { get; }

		/// <summary>
		/// 画面に表示するエンティティのアイコン
		/// </summary>
		private ImageResource _displayIcon;
		public ImageResource DisplayIcon {
			get
			{
				return this._displayIcon ?? this.DefaultIcon;
			}
			private set
			{
				this._displayIcon = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// エンティティの順番
		/// </summary>
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
				if (this._order != value)
				{
					this._order = value;
					this.OrderChanged(this);
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// エンティティの順番が変更された時に発行
		/// </summary>
		public event UpdateEntityOrderEventHandler OrderChanged;

		/// <summary>
		/// エンティティの色
		/// </summary>
		[DataMember]
		private ColorResource _color;
		public ColorResource Color
		{
			get
			{
				return this._color ?? ColorResource.Default;
			}
			set
			{
				this._color = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// エンティティのノート
		/// </summary>
		[DataMember]
		private string _note;
		public string Note
		{
			get
			{
				return this._note;
			}
			set
			{
				this._note = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// エンティティの中身が空であるか
		/// </summary>
		public virtual bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.Name) &&
					this.Icon == null &&
					(this.Color == null || this.Color.Equals(ColorResource.Default)) &&
					string.IsNullOrEmpty(this.Note);
			}
		}

		/// <summary>
		/// エンティティが属するストーリーモデル
		/// </summary>
		private WeakReference<StoryModel> _storyModel;
		public StoryModel StoryModel
		{
			get
			{
				StoryModel storyModel = null;
				this._storyModel.TryGetTarget(out storyModel);
				return storyModel;
			}
			set
			{
				if (this.StoryModel != null)
				{
					this.StoryModel.LoadStreamCompleted -= this.LoadStoryModelCompleted;
				}
				this._storyModel.SetTarget(value);
				if (value != null)
				{
					value.LoadStreamCompleted += this.LoadStoryModelCompleted;
				}
				this.OnPropertyChanged();
			}
		}

		protected virtual void LoadStoryModelCompleted()
		{
		}

		/// <summary>
		/// 検索Searchメソッドでヒットしたか
		/// </summary>
		private bool _isSearchHit = true;
		public bool IsSearchHit
		{
			get
			{
				return this._isSearchHit;
			}
			private set
			{
				this._isSearchHit = value;
				this.OnPropertyChanged();
			}
		}

		#endregion

		#region メソッド

		/// <summary>
		/// これから生成するエンティティが、指定したストーリーに所属したものであることを表す
		/// </summary>
		/// <param name="story">ストーリーオブジェクト</param>
		/// <param name="oldStory">古いストーリーオブジェクト</param>
		private static void SetupStory(StoryModel story, StoryModel oldStory)
		{
			Entity.CurrentStory = story;
			Entity.EntityCount = 0;
		}

		/// <summary>
		/// 新しく生成するエンティティに割り当てるIDを取得
		/// </summary>
		/// <returns>新しく生成するエンティティに割り当てるID</returns>
		public static long GetNewEntityID()
		{
			return Entity.EntityCount++;
		}

		/// <summary>
		/// 指定したエンティティの順番を入れ替え
		/// </summary>
		/// <param name="other">入れ替え対象</param>
		public void ReplaceOrder(IEntity other)
		{
			long tmp = this.Order;
			this.Order = other.Order;
			other.Order = tmp;
		}

		/// <summary>
		/// 検索して、結果をIsHitに格納する
		/// </summary>
		/// <param name="query">検索クエリ</param>
		public void Search(EntitySearchQuery query)
		{
			if (query.IsValid)
			{
				if (query.Mode == SearchMode.And)
				{
					bool result = false;
					foreach (var word in query.Keywords)
					{
						result = this.IsWordExists(word);
						if (!result)
						{
							break;
						}
					}
					this.IsSearchHit = result;
				}
				else if (query.Mode == SearchMode.Or)
				{
					bool result = false;
					foreach (var word in query.Keywords)
					{
						result = this.IsWordExists(word);
						if (result)
						{
							break;
						}
					}
					this.IsSearchHit = result;
				}
			}
			else
			{
				this.IsSearchHit = true;
			}
		}

		/// <summary>
		/// 指定したキーワードが存在するか調べる
		/// </summary>
		/// <param name="word">検索するワード</param>
		/// <returns>検索結果</returns>
		protected virtual bool IsWordExists(string word)
		{
			// キーワードがエンティティ内に含まれていないか調べる
			bool? r = false;
			r |= this.Name?.IndexOf(word) >= 0;
			r |= this.Note?.IndexOf(word) >= 0;
			return r == true;
		}

		#endregion

		#region コンストラクタ

		/// <summary>
		/// staticコンストラクタ
		/// </summary>
		static Entity()
		{
			StoryModel.CurrentStoryChanged += SetupStory;
		}

		protected Entity()
		{
			this.Initialize(default(StreamingContext));
		}

		[OnDeserializing]
		protected void Initialize(StreamingContext context)
		{
			this._storyModel = new WeakReference<StoryModel>(null);

			this.OrderChanged = delegate { };

			this.StoryModel = Entity.CurrentStory;
			long id = Entity.GetNewEntityID();			// ここに書くと、データをロードするたびに、既存要素の数だけIDを新規発行してしまう
			this.Id = id;								// もちろんそれらのIDは後で上書きされるので使われることはない
			this.Order = id;							// しかしここまで正しく動作しているので変えるのが怖いので置いておく
			StoryModel.CurrentStoryChanged += Entity.SetupStory;
		}

		[OnDeserialized]
		private void Initialize2(StreamingContext context)
		{
			this.IsSearchHit = true;

			// 画像が設定されていればDisplayIconにセット
			this.Icon = this.Icon;
		}

		#endregion

		#region 共通メソッド

		/// <summary>
		/// エンティティ同士を比較
		/// </summary>
		/// <param name="other">比較対象</param>
		/// <returns>比較結果</returns>
		public int CompareTo(Entity other)
		{
			return this.Order.CompareTo(other.Order);
		}

		/// <summary>
		/// エンティティの文字列を返す
		/// </summary>
		/// <returns>エンティティの名前</returns>
		public override string ToString()
		{
			return this.Name;
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}

    public enum EntityType
    {
        Person,
        Group,
        Place,
        Scene,
        Chapter,
        Sex,
        Parameter,
        Memo,
        Word,
    }
}
