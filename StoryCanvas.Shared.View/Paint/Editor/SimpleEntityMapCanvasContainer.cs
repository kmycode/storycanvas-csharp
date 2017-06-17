using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using StoryCanvas.Shared.Models.Editor.Map;
using System.Linq;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// エンティティをまとめたシンプルなマップを描画するロジック
    /// </summary>
    class SimpleEntityMapCanvasContainer<T> : IEntityEditorCanvasContainer<T>
        where T : Entity
    {
        public SimpleEntityMap<PersonEntity> Map { get; set; } = new SimpleEntityMap<PersonEntity>();
        public MapElement DraggingElement { get; set; }
        public EntityEditorCanvasBase<T> Canvas { get; set; }
        public bool CanDragMap { get; private set; } = true;

        public MapElement SelectedElement
        {
            get => this._selectedElement;
            set
            {
                if (this._selectedElement != value)
                {
                    this._selectedElement = value;
                    this.SelectedEntityChanged?.Invoke(this, EventArgs.Empty);
                }
                this.CanDragMap = this.SelectedElement == null;
            }
        }
        private MapElement _selectedElement;

        public void DrawUpdate(SKCanvas canvas, SKPaint paint)
        {
            paint.TextSize = 18;

            // 要素群を描画
            foreach (var element in this.Map.Elements)
            {
                EntityEditorCanvasUtil.DrawMapElement(element, canvas, paint);
            }
        }

        public void OnTapStart(double x, double y)
        {
            // 現在選択中の要素をタップしようとしているか判定
            if (this.SelectedElement is MapEntityElement<PersonEntity> el)
            {
                var element = this.Canvas.GetEntityFromPosition(this.Map.Elements, x, y);
                if (element?.Entity.Id == el?.Entity.Id)
                {
                    this.DraggingElement = element;
                }
            }
        }

        public void OnTapEnd(double x, double y)
        {
            // 要素のドラッグが終わったところであれば、要素を再描画
            if (this.DraggingElement != null)
            {
                this.Canvas.RequestRedraw(true);
                this.DraggingElement = null;
            }
        }

        public void OnTapped(double x, double y)
        {
            // 選択された要素を検出する
            var element = this.Canvas.GetEntityFromPosition(this.Map.Elements, x, y);
            this.SelectedElement = element;

            // 再描画を要求
            this.Canvas.RequestRedraw();
        }

        public void OnDragging(double dx, double dy)
        {
            // ドラッグ中の要素があれば座標変更
            if (this.DraggingElement != null)
            {
                this.DraggingElement.X -= (int)dx;
                this.DraggingElement.Y -= (int)dy;
                this.Canvas.RequestRedraw();
            }
        }

        public void ResizeMap()
        {
            var minX = this.Map.Elements.Min(e => e.X);
            var maxX = this.Map.Elements.Max(e => e.X + e.ViewWidth);
            var minY = this.Map.Elements.Min(e => e.Y);
            var maxY = this.Map.Elements.Max(e => e.Y + e.ViewHeight);

            // 要素を移動した結果、要素が今までの枠の外に出た時は、枠のサイズを変更する
            if (minX < 0)
            {
                foreach (var element in this.Map.Elements)
                {
                    element.X -= minX;
                }
                this.Canvas.X += minX;
            }
            else if (minX > 10)
            {
                foreach (var element in this.Map.Elements)
                {
                    element.X -= minX;
                }
                this.Canvas.X += minX;
            }
            if (minY < 0)
            {
                foreach (var element in this.Map.Elements)
                {
                    element.Y -= minY;
                }
                this.Canvas.Y += minY;
            }
            else if (minY > 10)
            {
                foreach (var element in this.Map.Elements)
                {
                    element.Y -= minY;
                }
                this.Canvas.Y += minY;
            }

            // キャンバスの縦幅、横幅を設定する
            // 250: 要素の大きさは描画時に設定されるが、アプリ起動直後に描画する時は値が設定されていないのでとりあえずこれで
            this.Canvas.Width = maxX - minX + 250;
            this.Canvas.Height = maxY - minY + 250;
        }

        public event EventHandler SelectedEntityChanged;
    }
}
