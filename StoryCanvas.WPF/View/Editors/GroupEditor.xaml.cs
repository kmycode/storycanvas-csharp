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
	/// GroupEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class GroupEditor : UserControl
	{
		public GroupEditor()
		{
			InitializeComponent();
			this.DataContext = new GroupEntityViewModel();

			Messenger.Default.Register<EditGroupEntityNewMessage>((m) => this.EditNewEntity(m));
			Messenger.Default.Register<EditGroupEntityPrimaryMessage>((m) => this.EditPrimaryEntity(m));
		}

		/// <summary>
		/// エンティティを優先して編集するエディタであるかの依存プロパティ
		/// </summary>
		public static readonly DependencyProperty IsPrimaryProperty =
			DependencyProperty.Register(
				"IsPrimary",
				typeof(bool),
				typeof(GroupEditor),
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
			var viewModel = this.DataContext as GroupEntityViewModel;
			if (viewModel != null)
			{
				viewModel.TrySetEntity(message);
				this.FirstEditControl.Focus();
			}
		}
	}
}
