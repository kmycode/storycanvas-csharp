using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.ViewModels.Entities;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.View.Editors
{
	/// <summary>
	/// SceneEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class SceneEditor : UserControl
	{
		public SceneEditor()
		{
			InitializeComponent();
			this.DataContext = new SceneEntityViewModel();

			Messenger.Default.Register<EditSceneEntityNewMessage>((m) => this.EditNewEntity(m));
			Messenger.Default.Register<EditSceneEntityPrimaryMessage>((m) => this.EditPrimaryEntity(m));
		}

		/// <summary>
		/// エンティティを優先して編集するエディタであるかの依存プロパティ
		/// </summary>
		public static readonly DependencyProperty IsPrimaryProperty =
			DependencyProperty.Register(
				"IsPrimary",
				typeof(bool),
				typeof(SceneEditor),
				new FrameworkPropertyMetadata(
					false,
					(d, e) =>
					{
					})
			);

		/// <summary>
		/// エンティティを優先して編集するエディタであるか
		/// </summary>
		public bool IsPrimary
		{
			get
			{
				return (bool)GetValue(IsPrimaryProperty);
			}
			set
			{
				SetValue(IsPrimaryProperty, value);
			}
		}

		private bool IsEditStarted = false;

		private void EditPrimaryEntity(EditSceneEntityNewMessage message)
		{
			if (this.IsPrimary)
			{
				this.EditEntity(message);
			}
		}

		private void EditNewEntity(EditSceneEntityNewMessage message)
		{
			if (!this.IsPrimary && !this.IsEditStarted)
			{
				this.EditEntity(message);
				this.IsEditStarted = true;
			}
		}

		private void EditEntity(EditSceneEntityNewMessage message)
		{
			var viewModel = this.DataContext as SceneEntityViewModel;
			if (viewModel != null)
			{
				viewModel.TrySetEntity(message);
				this.FirstEditControl.Focus();
			}
		}
	}
}
