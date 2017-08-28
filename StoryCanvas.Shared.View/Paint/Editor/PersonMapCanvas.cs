using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using StoryCanvas.Shared.View.Paint.Editor;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntityRelate;

namespace StoryCanvas.Shared.View.Paint
{
    /// <summary>
    /// 人物マップのキャンバス
    /// </summary>
    public class PersonMapCanvas : EntityEditorCanvasWithSingleEntityMapBase<PersonEntity>
    {
        public PersonMapCanvas(EachEntityRelationModel<PersonEntity> relations) : base(new SingleEntityMapCanvasContainer<PersonEntity>
        {
            EachRelations = relations,
            DrawRelationHelper = new DrawEachPersonRelationHelper(),
        })
        {
        }

        public PersonMapCanvas(PersonMapCanvas other) : this(((SingleEntityMapCanvasContainer<PersonEntity>)other.Container).EachRelations)
        {
        }
    }
}
