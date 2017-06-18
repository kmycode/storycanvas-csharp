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

        /// <summary>
        /// 描画を更新する時に呼び出し。ただし、マップをドラッグで動かす場合など、描画内容に変更がない時は呼び出されない
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        /// <param name="paint">ペイント</param>
        void DrawUpdate(SKCanvas canvas, SKPaint paint);

        /// <summary>
        /// エンティティの選択枠など、更新頻度の高いものを描画する
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        /// <param name="paint">ペイント</param>
        void DrawFloatingElements(SKCanvas canvas, SKPaint paint);

        /// <summary>
        /// タップが開始された時に呼び出される
        /// </summary>
        /// <param name="x">座標X</param>
        /// <param name="y">座標Y</param>
        void OnTapStart(double x, double y);

        /// <summary>
        /// タップが終了した時に呼び出される
        /// </summary>
        /// <param name="x">座標X</param>
        /// <param name="y">座標Y</param>
        void OnTapEnd(double x, double y);

        /// <summary>
        /// 一定時間内でタッピングされたときに呼び出される
        /// </summary>
        /// <param name="x">座標X</param>
        /// <param name="y">座標Y</param>
        void OnTapped(double x, double y);

        /// <summary>
        /// ドラッグ中に呼び出される
        /// </summary>
        /// <param name="dx">Xの差分</param>
        /// <param name="dy">Yの差分</param>
        void OnDragging(double dx, double dy);

        /// <summary>
        /// マップの大きさを変更する。結果はCanvas.X、Canvas.Yに格納される
        /// </summary>
        void ResizeMap();

        event EventHandler SelectedEntityChanged;
    }
}
