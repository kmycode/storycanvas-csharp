using SkiaSharp;
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
    public interface IEntityEditorCanvasContainer<T>
        where T : Entity
    {
        /// <summary>
        /// 現在選択されている要素
        /// </summary>
        MapElement SelectedElement { get; set; }

        /// <summary>
        /// 現在ドラッグ中の要素
        /// </summary>
        MapElement DraggingElement { get; set; }

        /// <summary>
        /// マップ全体をドラッグ可能であるか
        /// </summary>
        bool CanDragMap { get; }

        /// <summary>
        /// 描画対象のキャンバス
        /// </summary>
        EntityEditorCanvasBase<T> Canvas { get; set; }

        void DrawUpdate(SKCanvas canvas, SKPaint paint);

        void OnTapStart(double x, double y);

        void OnTapEnd(double x, double y);

        void OnTapped(double x, double y);

        void OnDragging(double dx, double dy);

        void ResizeMap();

        event EventHandler SelectedEntityChanged;
    }
}
