using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using StoryCanvas.Shared.View.Paint.Editor;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Editor.Map;

namespace StoryCanvas.Shared.View.Paint
{
    /// <summary>
    /// 人物マップのキャンバス
    /// </summary>
    public class PersonMapCanvas : EntityEditorCanvasBase<PersonEntity>
    {
        public SimpleEntityMap<PersonEntity> Map
        {
            get => ((SimpleEntityMapCanvasContainer<PersonEntity>)this.Container).Map;
            set => ((SimpleEntityMapCanvasContainer<PersonEntity>)this.Container).Map = value;
        }

        public PersonMapCanvas() : base(new SimpleEntityMapCanvasContainer<PersonEntity>())
        {
        }
    }
}
