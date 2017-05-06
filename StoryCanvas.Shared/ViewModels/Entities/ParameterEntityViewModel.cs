using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class ParameterEntityViewModel : EntityViewModelBase<ParameterEntity>
	{
		protected override ParameterEntity CreateDummyEntity()
		{
			return new ParameterEntity();
		}

		public ParameterEntityViewModel()
		{
			base.EntityChanged += (e, old) =>
			{
				this.OnPropertyChanged("IsForAllPeople");
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditParameter)
				{
					// 画面遷移のタイミングで、表示情報とか更新
					if (!this.IsDummy)
					{
						this.Entity = this.Entity;
					}
				}
			};
		}

		/// <summary>
		/// 全ての人物につけるべきパラメータであるか
		/// </summary>
		public bool IsForAllPeople
		{
			get
			{
				return this.Entity.IsForAllPeople;
			}
			set
			{
				this.Entity.IsForAllPeople = value;
			}
		}

		/// <summary>
		/// 登録済の全ての人物に対してパラメータを設定する、またはその逆
		/// 処理内容はIsForAllPeopleの値に応じて分岐する
		/// </summary>
		private RelayCommand _setParameterToExistingPeopleCommand;
		public RelayCommand SetParameterToExistingPeopleCommand
		{
			get
			{
				return this._setParameterToExistingPeopleCommand = this._setParameterToExistingPeopleCommand ?? new RelayCommand((obj) =>
				{
					if (this.IsForAllPeople)
					{
						this.Entity.StoryModel?.AddParameterToAllPeople(this.Entity);
						Messenger.Default.Send(this, new LightAlertMessage(StringResourceResolver.Resolve("ParameterForAllEntityAppliedTrue")));
					}
					else
					{
						this.Entity.StoryModel?.RemoveEmptyParameterToAllPeople(this.Entity);
						Messenger.Default.Send(this, new LightAlertMessage(StringResourceResolver.Resolve("ParameterForAllEntityAppliedFalse")));
					}
				});
			}
		}
	}
}
