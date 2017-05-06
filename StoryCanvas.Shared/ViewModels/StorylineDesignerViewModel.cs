using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.Messages.StorylineDesigner;
using StoryCanvas.Shared.Messages.EditEntity;
using System.Collections.Specialized;

namespace StoryCanvas.Shared.ViewModels
{
	public class StorylineDesignerViewModel : ViewModelBase
	{
		public StoryModel Story;

		/// <summary>
		/// ストーリーラインデザイナで表示する編
		/// </summary>
		private readonly EntitySelectionModel<PartEntity> _viewPartSelection = new EntitySelectionModel<PartEntity>();
		public EntitySelectionModel<PartEntity> ViewPartSelection
		{
			get
			{
				return this._viewPartSelection;
			}
		}
		public PartEntity ViewPart
		{
			get
			{
				return this.ViewPartSelection.Selected;
			}
			set
			{
				this.ViewPartSelection.Selected = value;
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public StorylineDesignerViewModel()
		{
			this.Story = StoryModel.Current;
			this.ViewPartSelection.SelectionChanged += this.OnPartChanged;
		}

		/// <summary>
		/// 表示される編が変更になった時
		/// </summary>
		/// <param name="entity">編</param>
		private void OnPartChanged(PartEntity entity, PartEntity oldEntity)
		{
			this.OnPropertyChanged("Storylines");
		}

		/// <summary>
		/// 編の一覧
		/// </summary>
		public INotifyCollectionChanged Parts
		{
			get
			{
				return this.Story.Parts;
			}
		}

		/// <summary>
		/// ストーリーラインの一覧
		/// </summary>
		public INotifyCollectionChanged Storylines
		{
			get
			{
				return this.Story.Storylines;
			}
		}
	}
}
