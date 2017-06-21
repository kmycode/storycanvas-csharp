using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.View.Paint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor
{
    /// <summary>
    /// 人物編集画面のエディタモデル
    /// </summary>
    [DataContract]
    public class PersonEditorModel : EntityEditorWithEachRelationModelBase<PersonEntity>
    {
        public PersonEditorModel(StoryModel story) : base(story, new PersonMapCanvas(story.PersonPersonRelation), story.People)
        {
        }

        public PersonEditorModel(PersonEditorModel other) : base(other)
        {
        }
    }
}
