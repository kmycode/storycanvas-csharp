using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;
using StoryCanvas.Shared.ViewTools.BehaviorHelper;
using StoryCanvas.WPF.View.SubViews;

namespace StoryCanvas.WPF.Behaviors
{
	class ProgressBehavior : Behavior<MainWindow>
	{
		/// <summary>
		/// 進捗をあらわすオブジェクト
		/// </summary>
		public ProgressView Progress
		{
			get { return (ProgressView)GetValue(ProgressProperty); }
			set { SetValue(ProgressProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ProgressProperty =
			DependencyProperty.Register("Progress",
				typeof(ProgressView),
				typeof(ProgressBehavior),
				new UIPropertyMetadata((d, e) =>
				{
					var bh = d as ProgressBehavior;
					if (bh != null)
					{
						var oldValue = e.OldValue as ProgressView;
						var newValue = e.NewValue as ProgressView;
						if (oldValue != null)
						{
							oldValue.StartProcessing -= bh.ProgressStart;
							oldValue.EndProcessing -= bh.ProgressEnd;
						}
						if (newValue != null)
						{
							newValue.StartProcessing += bh.ProgressStart;
							newValue.EndProcessing += bh.ProgressEnd;
						}
					}
				}));

		private ProgressSubView _progressSubView;

		/// <summary>
		/// アタッチされた時
		/// </summary>
		protected override void OnAttached()
		{
			base.OnAttached();
		}

		/// <summary>
		/// でタッチされた時
		/// </summary>
		protected override void OnDetaching()
		{
			base.OnDetaching();
			if (this.Progress != null)
			{
				this.Progress.StartProcessing -= this.ProgressStart;
				this.Progress.EndProcessing -= this.ProgressEnd;
			}
		}

		private void ProgressStart(object sender, EventArgs e)
		{
			this._progressSubView = new ProgressSubView();
			this.AssociatedObject.ShowMetroDialogImplAsync(this._progressSubView, new MahApps.Metro.Controls.Dialogs.MetroDialogSettings { AnimateShow = false, AnimateHide = false, });
		}

		private void ProgressEnd(object sender, EventArgs e)
		{
			this.AssociatedObject.HideMetroDialogImplAsync(this._progressSubView, new MahApps.Metro.Controls.Dialogs.MetroDialogSettings { AnimateHide = false, AnimateShow = false, });
			this._progressSubView = null;
		}
	}
}
