using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Editor;
using StoryCanvas.Shared.Models.Story;

namespace StoryCanvas.Shared.ViewModels.EntityEditors
{
    /// <summary>
    /// 人物編集のビューモデル
    /// </summary>
    public class PersonEditorViewModel : EntityEditorViewModelBase<PersonEntity>
    {
        public PersonEditorViewModel() : base(StoryModel.Current.PersonEditorModel)
        {
        }
    }
}
