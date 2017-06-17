using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.View.Paint;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor
{
    /// <summary>
    /// 人物編集画面のエディタモデル
    /// </summary>
    [DataContract]
    public class PersonEditorModel : EntityEditorModelBase<PersonEntity>
    {
        [DataMember]
        private SimpleEntityMapGroup<PersonEntity> _personMapGroup = new SimpleEntityMapGroup<PersonEntity>();
        public SimpleEntityMapGroup<PersonEntity> PersonMapGroup => this._personMapGroup;

        public PersonMapCanvas PersonMapCanvas { get; }

        public PersonEditorModel(StoryModel story) : base(story)
        {
            this.PersonMapCanvas = new PersonMapCanvas(story.PersonPersonRelation);

            this.PersonMapGroup.SelectedMapChanged += (sender, e) =>
            {
                this.PersonMapCanvas.Map = this.PersonMapGroup.SelectedMap;
                this.PersonMapCanvas.RequestRedraw(true);
            };
        }
    }
}
