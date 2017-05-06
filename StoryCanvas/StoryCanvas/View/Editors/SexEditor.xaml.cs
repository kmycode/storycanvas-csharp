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
	public partial class SexEditor : ContentView
	{
		public SexEditor()
		{
			InitializeComponent();
			this.BindingContext = new SexEntityViewModel();

			Messenger.Default.Register<EditSexEntityNewMessage>((m) => this.EditNewEntity(m));
			Messenger.Default.Register<EditSexEntityPrimaryMessage>((m) => this.EditPrimaryEntity(m));
		}

		/// <summary>
		/// エンティティを優先して編集するエディタであるかの依存プロパティ
		/// </summary>
		public static readonly BindableProperty IsPrimaryProperty = BindableProperty.Create(
				"IsPrimary",                            // プロパティ名
				typeof(bool),                           // プロパティの型
				typeof(SexEditor),                   // プロパティを持つ View の型
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

		private void EditPrimaryEntity(EditSexEntityNewMessage message)
		{
			if (this.IsPrimary)
			{
				this.EditEntity(message);
			}
		}

		private void EditNewEntity(EditSexEntityNewMessage message)
		{
			if (!this.IsPrimary && !this.IsEditStarted)
			{
				this.EditEntity(message);
				this.IsEditStarted = true;
			}
		}

		private void EditEntity(EditSexEntityNewMessage message)
		{
			var viewModel = this.BindingContext as SexEntityViewModel;
			if (viewModel != null)
			{
				viewModel.TrySetEntity(message);
			}
		}
	}
}
