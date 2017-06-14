using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.ViewModels.Entities;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.View.Editors
{
	public partial class GroupEditor : ContentView
	{
		public GroupEditor()
		{
			InitializeComponent();
			this.BindingContext = new GroupEntityViewModel();

			Messenger.Default.Register<EditGroupEntityNewMessage>((m) => this.EditNewEntity(m));
			Messenger.Default.Register<EditGroupEntityPrimaryMessage>((m) => this.EditPrimaryEntity(m));
		}

		/// <summary>
		/// エンティティを優先して編集するエディタであるかの依存プロパティ
		/// </summary>
		public static readonly BindableProperty IsPrimaryProperty = BindableProperty.Create(
				"IsPrimary",                            // プロパティ名
				typeof(bool),                           // プロパティの型
				typeof(GroupEditor),                   // プロパティを持つ View の型
				false,                                  // 初期値
				BindingMode.TwoWay,                     // バインド方向
				null,                                   // バリデーションメソッド
														// 変更後イベントハンドラ
				(bindable, oldValue, newValue) =>
				{
				},
				null,                                   // 変更時イベントハンドラ
				null);                                  // BindableProperty.CoerceValueDelegate Xamarin 公式にも説明なしなので用途不明

		/// <summary>
		/// エンティティを優先して編集するエディタであるか
		/// </summary>
		public bool IsPrimary
		{
			get
			{
				return (bool)this.GetValue(IsPrimaryProperty);
			}
			set
			{
				this.SetValue(IsPrimaryProperty, value);
			}
		}

		private bool IsEditStarted = false;

		private void EditPrimaryEntity(EditGroupEntityNewMessage message)
		{
			if (this.IsPrimary)
			{
				this.EditEntity(message);
			}
		}

		private void EditNewEntity(EditGroupEntityNewMessage message)
		{
			if (!this.IsPrimary && !this.IsEditStarted)
			{
				this.EditEntity(message);
				this.IsEditStarted = true;
			}
		}

		private void EditEntity(EditGroupEntityNewMessage message)
		{
			var viewModel = this.BindingContext as GroupEntityViewModel;
			if (viewModel != null)
			{
				viewModel.TrySetEntity(message);
			}
		}
	}
}
