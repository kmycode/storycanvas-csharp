using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using StoryCanvas.Shared.Models.Editor.Map;
using System.Linq;
using StoryCanvas.Shared.Models.EntityRelate;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// エンティティをまとめたシンプルなマップを描画するロジック
    /// </summary>
    class SimpleEntityMapCanvasContainer<T> : IEntityEditorCanvasContainer<T>
        where T : Entity
    {
        public SimpleEntityMap<T> Map { get; set; } = new SimpleEntityMap<T>();
        public MapElement DraggingElement { get; set; }
        public EntityEditorCanvasBase<T> Canvas { get; set; }
        public bool CanDragMap { get; private set; } = true;
        public EachEntityRelationModel<T> EachRelations { get; set; }
        public IDrawRelationHelper DrawRelationHelper { get; set; }

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

            // 関係を描画
            this.DrawRelations(canvas, paint);

            // 要素群を描画
            foreach (var element in this.Map.Elements)
            {
                EntityEditorCanvasUtil.DrawMapElement(element, canvas, paint);
            }
        }

        /// <summary>
        /// エンティティ同士の関係を描画する
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="paint"></param>
        private void DrawRelations(SKCanvas canvas, SKPaint paint)
        {
            foreach (var data in this.EachRelations.GetRelationRange(this.Map.Elements.Select(e => e.Entity))
                                                   .Join(this.Map.Elements, r => r.Entity1.Id, e => e.Entity.Id, (r, e) => new { Element = e, Relation = r, })
                                                   .Join(this.Map.Elements, r => r.Relation.Entity2.Id, e => e.Entity.Id, (r, e) => new { Element1 = r.Element, Element2 = e, Relation = r.Relation, }))
            {
                if (this.DrawRelationHelper != null)
                {
                    // 関連付けを描画する
                    this.DrawRelationHelper.DrawRelation(canvas, paint, data.Element1, data.Element2, data.Relation);
                }
                else
                {
                    // 線だけひいて終わる
                    canvas.DrawLine(data.Element1.X + 50, data.Element1.Y + 50, data.Element2.X + 50, data.Element2.Y + 50, paint);
                }
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
