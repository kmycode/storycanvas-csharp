using SkiaSharp;
using StoryCanvas.Shared.Models.Common;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// エンティティの関連付けを描画するロジック
    /// </summary>
    public interface IEachEntityRelationEditorCanvasContainer<E> : IEntityCanvasContainer<E>
        where E : Entity
    {
        EachEntityRelationModel<E> EachRelations { get; set; }

        EntityRelateBase<E, E> SelectedRelation { get; set; }

        /// <summary>
        /// エンティティの選択枠など、更新頻度の高いものを描画する
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        /// <param name="paint">ペイント</param>
        void DrawFloatingElements(SKCanvas canvas, SKPaint paint);

        /// <summary>
        /// 関連付けの選択を変更した時に発行
        /// </summary>
        event ValueChangedEventHandler<EntityRelateBase<E, E>> SelectedRelationChanged;
    }
}
