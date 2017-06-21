using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Editor;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.Models.EntityRelate;

namespace StoryCanvas.Shared.ViewModels.EntityEditors
{
    /// <summary>
    /// 人物編集のビューモデル
    /// </summary>
    public class PersonEditorViewModel : EntityEditorWithEachRelationViewModelBase<PersonEntity>
    {
        public PersonEditorViewModel() : base(StoryModel.Current.PersonEditorModel)
        {
            this.Editor.EntitySelectionChanged += (sender, e) =>
            {
                this.SelectedEntity?.UpdateRelations();
            };
        }
    }
}
