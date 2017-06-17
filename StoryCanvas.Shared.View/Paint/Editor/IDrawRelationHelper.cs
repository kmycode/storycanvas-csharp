using SkiaSharp;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// エンティティ同士の関連を描画する時のヘルパ
    /// </summary>
    public interface IDrawRelationHelper
    {
        /// <summary>
        /// 関連付けを描画する
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="paint"></param>
        /// <param name="element1"></param>
        /// <param name="element2"></param>
        void DrawRelation<E1, E2>(SKCanvas canvas, SKPaint paint, MapElement element1, MapElement element2, EntityRelateBase<E1, E2> relation)
            where E1 : Entity where E2 : Entity;
    }
}
