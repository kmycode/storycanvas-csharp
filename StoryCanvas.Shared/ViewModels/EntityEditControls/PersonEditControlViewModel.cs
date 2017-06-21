using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.ViewModels.EntityEditControls
{
    class PersonEditControlViewModel : EntityEditControlViewModelBase<PersonEntity>
    {
        /// <summary>
        /// 人物と人物の関連付けを追加
        /// </summary>
        public RelayCommand<PersonEntity> AddPersonPersonRelationCommand
        {
            get
            {
                return this._addPersonPersonRelationCommand = this._addPersonPersonRelationCommand ?? new RelayCommand<PersonEntity>((entity) =>
                {
                    this.Entity.StoryModel.PersonPersonRelates.Add(new PersonPersonEntityRelate(this.Entity, entity));
                    this.Entity.UpdateRelations();
                });
            }
        }
        private RelayCommand<PersonEntity> _addPersonPersonRelationCommand;

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
