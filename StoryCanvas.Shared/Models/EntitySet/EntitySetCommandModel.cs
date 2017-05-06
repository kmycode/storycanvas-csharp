using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.EntitySet
{
	/// <summary>
	/// エンティティのリストに対する操作のモデル
	/// </summary>
    public class EntityListCommandModel<E> where E : Entity
	{
		public EntityListCommandModel(
			Func<E> newEntityDelegate,
			Func<E> selectedEntityDelegate,
			Action<E> selectedEntityChangeDelegate,
			object entities)
		{
			this.NewEntityDelegate = newEntityDelegate;
			this.SelectedEntityDelegate = selectedEntityDelegate;
			this.SelectedEntityChangeDelegate = selectedEntityChangeDelegate;
			this.Entities = (EntityListModel<E>)entities;
		}

		public Func<E> NewEntityDelegate { get; private set; }
		public Func<E> SelectedEntityDelegate { get; private set; }
		public Action<E> SelectedEntityChangeDelegate { get; private set; }
		public EntityListModel<E> Entities { get; private set; }

		/// <summary>
		/// 各コマンドが有効であるか（現在、ユーザからの操作によって使える状態にあるか）
		/// </summary>
		private bool _isEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				this._isEnabled = value;
				this.AddNewEntityCommand.OnCanExecuteChanged();
				this.RemoveEntityCommand.OnCanExecuteChanged();
				this.UpEntityCommand.OnCanExecuteChanged();
				this.DownEntityCommand.OnCanExecuteChanged();
			}
		}

		/// <summary>
		/// 新しいエンティティを追加
		/// </summary>
		private RelayCommand _addNewEntityCommand;
		public RelayCommand AddNewEntityCommand
		{
			get
			{
				return this._addNewEntityCommand = this._addNewEntityCommand ?? new RelayCommand((obj) =>
				{
					var entity = this.NewEntityDelegate();
					this.Entities.InsertAndDown(this.SelectedEntityDelegate(), entity);
					this.SelectedEntityChangeDelegate(entity);
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}

		/// <summary>
		/// 選択中のエンティティを削除
		/// </summary>
		private RelayCommand _removeEntityCommand;
		public RelayCommand RemoveEntityCommand
		{
			get
			{
				return this._removeEntityCommand = this._removeEntityCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedEntityDelegate() != null)
					{
						if (!this.SelectedEntityDelegate().IsEmpty)
						{
							Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("DeleteEntityMessage"), AlertMessageType.YesNo, (r) =>
							{
								if (r == AlertMessageResult.Yes)
								{
									this.DeleteEntity();
								}
							}));
						}
						else
						{
							this.DeleteEntity();
						}
					}
					else
					{
						Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
					}
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}

		private void DeleteEntity()
		{
			var afterSelected = this.Entities.Next(this.SelectedEntityDelegate()) ??
											this.Entities.Back(this.SelectedEntityDelegate());
			this.Entities.Remove(this.SelectedEntityDelegate());
			this.SelectedEntityChangeDelegate(afterSelected);
		}

		/// <summary>
		/// 選択中のエンティティを上へ移動
		/// </summary>
		private RelayCommand _upEntityCommand;
		public RelayCommand UpEntityCommand
		{
			get
			{
				return this._upEntityCommand = this._upEntityCommand ?? new RelayCommand((obj) =>
				{
					var afterSelect = this.SelectedEntityDelegate();
					if (this.SelectedEntityDelegate() != null)
					{
						this.Entities.Up(this.SelectedEntityDelegate());
					}
					else
					{
						Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
					}
					this.SelectedEntityChangeDelegate(afterSelect);
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}

		/// <summary>
		/// 選択中のエンティティを下へ移動
		/// </summary>
		private RelayCommand _downEntityCommand;
		public RelayCommand DownEntityCommand
		{
			get
			{
				return this._downEntityCommand = this._downEntityCommand ?? new RelayCommand((obj) =>
				{
					var afterSelect = this.SelectedEntityDelegate();
					if (this.SelectedEntityDelegate() != null)
					{
						this.Entities.Down(this.SelectedEntityDelegate());
					}
					else
					{
						Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
					}
					this.SelectedEntityChangeDelegate(afterSelect);
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}
	}

	/// <summary>
	/// エンティティのツリーに対する操作のモデル
	/// </summary>
	public class EntityTreeCommandModel<E> where E : TreeEntity
	{
		public EntityTreeCommandModel(
			Func<E> newEntityDelegate,
			Func<E> selectedEntityDelegate,
			Action<E> selectedEntityChangeDelegate,
			object entities)
		{
			this.NewEntityDelegate = newEntityDelegate;
			this.SelectedEntityDelegate = selectedEntityDelegate;
			this.SelectedEntityChangeDelegate = selectedEntityChangeDelegate;
			this.Entities = (EntityTreeModel<E>)entities;
		}

		public Func<E> NewEntityDelegate { get; private set; }
		public Func<E> SelectedEntityDelegate { get; private set; }
		public Action<E> SelectedEntityChangeDelegate { get; private set; }
		public EntityTreeModel<E> Entities { get; private set; }

		/// <summary>
		/// 各コマンドが有効であるか（現在、ユーザからの操作によって使える状態にあるか）
		/// </summary>
		private bool _isEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				this._isEnabled = value;
				this.AddNewEntityCommand.OnCanExecuteChanged();
				this.RemoveEntityCommand.OnCanExecuteChanged();
				this.UpEntityCommand.OnCanExecuteChanged();
				this.DownEntityCommand.OnCanExecuteChanged();
				this.LeftEntityCommand.OnCanExecuteChanged();
				this.RightEntityCommand.OnCanExecuteChanged();
			}
		}

		/// <summary>
		/// 新しいエンティティを追加
		/// </summary>
		private RelayCommand _addNewEntityCommand;
		public RelayCommand AddNewEntityCommand
		{
			get
			{
				return this._addNewEntityCommand = this._addNewEntityCommand ?? new RelayCommand((obj) =>
				{
					var entity = this.NewEntityDelegate();
					this.Entities.InsertAndDown(this.SelectedEntityDelegate(), entity, (E)this.SelectedEntityDelegate()?.Parent);
					this.SelectedEntityChangeDelegate(entity);
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}

		/// <summary>
		/// 選択中のエンティティを削除
		/// </summary>
		private RelayCommand _removeEntityCommand;
		public RelayCommand RemoveEntityCommand
		{
			get
			{
				return this._removeEntityCommand = this._removeEntityCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedEntityDelegate() != null)
					{
						if (!this.SelectedEntityDelegate().IsEmpty)
						{
							Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("DeleteEntityMessage"), AlertMessageType.YesNo, (r) =>
							{
								if (r == AlertMessageResult.Yes)
								{
									this.DeleteEntity();
								}
							}));
						}
						else
						{
							this.DeleteEntity();
						}
					}
					else
					{
						Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
					}
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}

		private void DeleteEntity()
		{
			var afterSelected = (E)this.SelectedEntityDelegate()?.Parent?.Children.Next(this.SelectedEntityDelegate()) ??
										(E)this.SelectedEntityDelegate()?.Parent?.Children.Back(this.SelectedEntityDelegate()) ??
										(E)this.SelectedEntityDelegate()?.Parent;
			if (afterSelected == this.Entities.Root)
			{
				afterSelected = null;
			}
			this.Entities.Remove(this.SelectedEntityDelegate());
			this.SelectedEntityChangeDelegate(afterSelected);
		}

		/// <summary>
		/// 選択中のエンティティを上へ移動
		/// </summary>
		private RelayCommand _upEntityCommand;
		public RelayCommand UpEntityCommand
		{
			get
			{
				return this._upEntityCommand = this._upEntityCommand ?? new RelayCommand((obj) =>
				{
					var afterSelect = this.SelectedEntityDelegate();
					if (this.SelectedEntityDelegate() != null)
					{
						this.Entities.Up(this.SelectedEntityDelegate());
					}
					else
					{
						Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
					}
#if XAMARIN_FORMS
					this.SelectedEntityChangeDelegate(null);
#endif
					this.SelectedEntityChangeDelegate(afterSelect);
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}

		/// <summary>
		/// 選択中のエンティティを下へ移動
		/// </summary>
		private RelayCommand _downEntityCommand;
		public RelayCommand DownEntityCommand
		{
			get
			{
				return this._downEntityCommand = this._downEntityCommand ?? new RelayCommand((obj) =>
				{
					var afterSelect = this.SelectedEntityDelegate();
					if (this.SelectedEntityDelegate() != null)
					{
						this.Entities.Down(this.SelectedEntityDelegate());
					}
					else
					{
						Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
					}
#if XAMARIN_FORMS
					this.SelectedEntityChangeDelegate(null);
#endif
					this.SelectedEntityChangeDelegate(afterSelect);
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}

		/// <summary>
		/// 選択中のエンティティを右へ移動
		/// </summary>
		private RelayCommand _rightEntityCommand;
		public RelayCommand RightEntityCommand
		{
			get
			{
				return this._rightEntityCommand = this._rightEntityCommand ?? new RelayCommand((obj) =>
				{
					var afterSelect = this.SelectedEntityDelegate();
					if (this.SelectedEntityDelegate() != null)
					{
						this.Entities.Right(this.SelectedEntityDelegate());
					}
					else
					{
						Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
					}
#if XAMARIN_FORMS
					this.SelectedEntityChangeDelegate(null);
#endif
					this.SelectedEntityChangeDelegate(afterSelect);
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}

		/// <summary>
		/// 選択中のエンティティを左へ移動
		/// </summary>
		private RelayCommand _leftEntityCommand;
		public RelayCommand LeftEntityCommand
		{
			get
			{
				return this._leftEntityCommand = this._leftEntityCommand ?? new RelayCommand((obj) =>
				{
					var afterSelect = this.SelectedEntityDelegate();
					if (this.SelectedEntityDelegate() != null)
					{
						this.Entities.Left(this.SelectedEntityDelegate());
					}
					else
					{
						Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("SelectAnyEntityMessage")));
					}
#if XAMARIN_FORMS
					this.SelectedEntityChangeDelegate(null);
#endif
					this.SelectedEntityChangeDelegate(afterSelect);
				},
				(obj) =>
				{
					return this.IsEnabled;
				});
			}
		}
	}
}
