using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.BehaviorHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.ViewModels.EntityEditControls
{
    class PersonEditControlViewModel : EntityEditControlViewModelBase<PersonEntity>
    {
        /// <summary>
        /// 関連人物の選択ヘルパ
        /// </summary>
        public SelectEntityHelper<PersonEntity> PersonSelector { get; } = new SelectEntityHelper<PersonEntity>();

        /// <summary>
        /// 性別一覧
        /// </summary>
        public IEnumerable<SexEntity> Sexes => this.StoryModel.Sexes;

        public PersonEditControlViewModel()
        {
            // 関連人物選択時
            this.PersonSelector.EntitySelected += (sender, e) =>
            {
                if (!this.Entity.StoryModel.PersonPersonRelation.IsRelated(this.Entity, e.Entity))
                {
                    this.Entity.StoryModel.PersonPersonRelates.Add(new PersonPersonEntityRelate(this.Entity, e.Entity));
                    this.Entity.UpdateRelations();
                }
            };
        }

        /// <summary>
        /// 人物と人物の関連付けを追加
        /// </summary>
        public RelayCommand AddPersonPersonRelationCommand
        {
            get
            {
                return this._addPersonPersonRelationCommand = this._addPersonPersonRelationCommand ?? new RelayCommand((obj) =>
                {
                    this.PersonSelector.OpenSelection();
                });
            }
        }
        private RelayCommand _addPersonPersonRelationCommand;

        /// <summary>
        /// 人物と人物の関連付けを削除
        /// </summary>
        public RelayCommand<PersonPersonEntityRelate> RemovePersonPersonRelationCommand
        {
            get
            {
                return this._removePersonPersonRelationCommand = this._removePersonPersonRelationCommand ?? new RelayCommand<PersonPersonEntityRelate>((relation) =>
                {
                    this.Entity.StoryModel.PersonPersonRelates.Remove(relation);
                    this.Entity.UpdateRelations();
                });
            }
        }
        private RelayCommand<PersonPersonEntityRelate> _removePersonPersonRelationCommand;
    }
}
