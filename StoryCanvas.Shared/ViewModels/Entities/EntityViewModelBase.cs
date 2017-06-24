using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools.Resources;
using StoryCanvas.Shared.Messages.Picker;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public delegate void EntityChangedEventHandler(Entity entity, Entity oldEntity);

	public abstract class EntityViewModelBase<E> : ViewModelBase where E : Entity
	{
		private E _entity;
		public E Entity
		{
			get
			{
				return this._entity;
			}
			set
			{
				var oldEntity = this._entity;
				if (this._entity != null)
				{
					this.SpendModel(this._entity);
					if (this._entity.GetType() != typeof(SexEntity))
					{
						this._entity.StoryModel.LoadStreamCompleted -= this.StoryModelLoadCompleted;
					}
					else
					{
						// 1.3で追加された性別だけ特別に対応（1.2以前の内部対応に不備がありバグが起きるので）
						this._entity.StoryModel = StoryModel.Current;
					}
				}

				if (value == null)
				{
					value = this.CreateDummyEntity();
					this.IsDummy = true;
				}
				else
				{
					this.IsDummy = false;
					if (value.StoryModel != null)
					{
						value.StoryModel.LoadStreamCompleted += this.StoryModelLoadCompleted;
					}
					this.StoreModel(value);
				}
				this._entity = value;

				this.OnPropertyChanged("Id");
				this.OnPropertyChanged("Entity");
				this.OnPropertyChanged("Name");
				this.OnPropertyChanged("DisplayIcon");
				this.OnPropertyChanged("Icon");
				this.OnPropertyChanged("Color");
				this.OnPropertyChanged("Order");
				this.OnPropertyChanged("Note");
				
				this.EntityChanged(value, oldEntity);
			}
		}
		protected event EntityChangedEventHandler EntityChanged = delegate { };
		public void TrySetEntity(EditEntityMessageBase<E> message)
		{
			this.Entity = message.Entity;
		}

		protected abstract E CreateDummyEntity();

		/// <summary>
		/// 初期化
		/// </summary>
		public EntityViewModelBase()
		{
			this.Entity = null;
		}

		/// <summary>
		/// ストーリーモデルのロード（ファイル／ネット）が完了した時に呼び出し
		/// </summary>
		private void StoryModelLoadCompleted()
		{
			// 編集画面でどのエンティティも選択されていない状態にする
			this.Entity = null;
		}

		/// <summary>
		/// 設定されたエンティティは、（NULL参照を防ぐ目的の）ダミーのものであるか
		/// </summary>
		private bool _isDummy = true;
		public bool IsDummy
		{
			get
			{
				return this._isDummy;
			}
			protected set
			{
				this._isDummy = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// ID
		/// </summary>
		public long Id
		{
			get
			{
				return this.Entity.Id;
			}
		}

		/// <summary>
		/// エンティティ名
		/// </summary>
		public string Name
		{
			get
			{
				return this.Entity.Name;
			}
			set
			{
				this.Entity.Name = value;
			}
		}

		/// <summary>
		/// 画面に表示するためのアイコン
		/// </summary>
		public ImageResource DisplayIcon
		{
			get
			{
				return this.Entity.DisplayIcon;
			}
		}

		/// <summary>
		/// アイコン
		/// </summary>
		public ImageResource Icon
		{
			get
			{
				return this.Entity.Icon;
			}
			set
			{
				this.Entity.Icon = value;
			}
		}

		/// <summary>
		/// 色
		/// </summary>
		public ColorResource Color
		{
			get
			{
				return this.Entity.Color;
			}
			set
			{
				this.Entity.Color = value;
			}
		}

		/// <summary>
		/// 色選択画面の表示
		/// </summary>
		private RelayCommand _colorPickerCommand;
		public RelayCommand ColorPickerCommand
		{
			get
			{
				return this._colorPickerCommand = this._colorPickerCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new ColorPickerMessage(this.Color, (color) =>
					{
						this.Color = color;
					}));
				});
			}
		}

		/// <summary>
		/// ノート
		/// </summary>
		public string Note
		{
			get
			{
				return this.Entity.Note;
			}
			set
			{
				this.Entity.Note = value;
			}
		}
	}
}
