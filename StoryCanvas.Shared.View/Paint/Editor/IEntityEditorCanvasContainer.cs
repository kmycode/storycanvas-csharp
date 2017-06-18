﻿using SkiaSharp;
using StoryCanvas.Shared.Models.Common;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// エンティティのキャンバスを描画するロジック
    /// </summary>
    public interface IEntityEditorCanvasContainer<E> : IEntityCanvasContainer<E>
        where E : Entity
    {
        /// <summary>
        /// 現在選択されている要素
        /// </summary>
        MapEntityElement<E> SelectedElement { get; set; }

        /// <summary>
        /// 現在ドラッグ中の要素
        /// </summary>
        MapEntityElement<E> DraggingElement { get; set; }

        /// <summary>
        /// エンティティの選択枠など、更新頻度の高いものを描画する
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        /// <param name="paint">ペイント</param>
        void DrawFloatingElements(SKCanvas canvas, SKPaint paint);

        /// <summary>
        /// エンティティの選択を変更した時に発行
        /// </summary>
        event ValueChangedEventHandler<MapEntityElement<E>> SelectedEntityChanged;
    }
}
