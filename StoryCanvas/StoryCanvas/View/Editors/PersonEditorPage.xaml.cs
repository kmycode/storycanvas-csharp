using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace StoryCanvas.View.Editors
{
	public partial class PersonEditorPage : ContentPage
	{
		private static FileImageSource _parameterIcon = ImageSource.FromFile("edit_parameter.png") as FileImageSource;
		private static FileImageSource _personIcon = ImageSource.FromFile("edit_person.png") as FileImageSource;
		
		private enum PersonEditorMode
		{
			EditPersonBase,
			EditParameter,
		}

		private PersonEditorMode _mode = PersonEditorMode.EditPersonBase;

		public PersonEditorPage()
		{
			InitializeComponent();
			this.ParameterIcon.Icon = _parameterIcon;
			this.ParameterIcon.Clicked += this.ParameterIcon_Click;
		}

		/// <summary>
		/// エンティティを優先して編集するエディタであるかの依存プロパティ
		/// </summary>
		public static readonly BindableProperty IsPrimaryProperty = BindableProperty.Create(
				"IsPrimary",                            // プロパティ名
				typeof(bool),                           // プロパティの型
				typeof(PersonEditorPage),                   // プロパティを持つ View の型
				false,                                  // 初期値
				BindingMode.TwoWay,                     // バインド方向
				null,                                   // バリデーションメソッド
														// 変更後イベントハンドラ
				(bindable, oldValue, newValue) =>
				{
					var view = bindable as PersonEditorPage;
					if (view != null)
					{
						view.Editor.IsPrimary = (bool)newValue;
					}
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

		/// <summary>
		/// 人物の基本情報編集画面と、パラメータ編集画面の切り替え
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ParameterIcon_Click(object sender, EventArgs e)
		{
			switch (this._mode)
			{
				case PersonEditorMode.EditPersonBase:
					this._mode = PersonEditorMode.EditParameter;
					this.ParameterIcon.Icon = _personIcon;
					this.Editor.OpenEditParameter();
					break;
				case PersonEditorMode.EditParameter:
					this._mode = PersonEditorMode.EditPersonBase;
					this.ParameterIcon.Icon = _parameterIcon;
					this.Editor.OpenEditPersonBase();
					break;
			}
		}
	}
}
