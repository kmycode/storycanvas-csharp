using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using StoryCanvas.Shared.Models.Editor.Map;
using System.Linq;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.Common;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// エンティティをまとめたシンプルなマップを描画するロジック
    /// </summary>
    class SimpleEntityMapCanvasContainer<E> : IEntityEditorCanvasContainer<E>, IEachEntityRelationEditorCanvasContainer<E>
        where E : Entity
    {
        public SimpleEntityMap<E> Map { get; set; } = new SimpleEntityMap<E>();
        public MapEntityElement<E> DraggingElement { get; set; }
        public EntityEditorCanvasBase<E> Canvas { get; set; }
        public bool CanDragMap { get; private set; } = true;
        public EachEntityRelationModel<E> EachRelations { get; set; }
        public IDrawRelationHelper DrawRelationHelper { get; set; }

        public MapEntityElement<E> SelectedElement
        {
            get => this._selectedElement;
            set
            {
                if (this._selectedElement != value)
                {
                    var old = this._selectedElement;
                    this._selectedElement = value;
                    this.SelectedEntityChanged?.Invoke(this, new ValueChangedEventArgs<MapEntityElement<E>>
                    {
                        OldValue = old,
                        NewValue = value,
                    });
                }
            }
        }
        private MapEntityElement<E> _selectedElement;
        
        public EntityRelateBase<E, E> SelectedRelation
        {
            get => this._selectedRelation.Relation;
            set
            {
                if (this._selectedRelation.Relation != value)
                {
                    if (value == null) this._selectedRelation = (null, null, null);
                    else
                    {
                        var r1 = this.Map.Elements.SingleOrDefault(e => e.Entity.Id == value.Entity1.Id);
                        var r2 = this.Map.Elements.SingleOrDefault(e => e.Entity.Id == value.Entity2.Id);
                        this._selectedRelation = (value, r1, r2);
                    }
                }
            }
        }
        private (EntityRelateBase<E, E> Relation, MapElement A, MapElement B) _selectedRelation
        {
            get => this.__selectedRelation;
            set
            {
                if (this.__selectedRelation.Relation != value.Relation)
                {
                    var old = this.__selectedRelation;
                    this.__selectedRelation = value;
                    this.SelectedRelationChanged?.Invoke(this, new ValueChangedEventArgs<EntityRelateBase<E, E>>
                    {
                        OldValue = old.Relation,
                        NewValue = value.Relation,
                    });
                }
            }
        }
        private (EntityRelateBase<E, E> Relation, MapElement A, MapElement B) __selectedRelation;

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

        /// <summary>
        /// 選択されたエンティティの枠とか描画する
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="paint"></param>
        public void DrawFloatingElements(SKCanvas canvas, SKPaint paint)
        {
            // 選択されたエンティティの枠
            if (this.SelectedElement != null)
            {
                paint.IsStroke = true;
                paint.StrokeWidth = 4;
                paint.Color = SKColors.Blue;

                canvas.DrawRect(new SKRect(this.SelectedElement.X - 8,
                                           this.SelectedElement.Y - 8,
                                           this.SelectedElement.X + this.SelectedElement.ViewWidth + 8,
                                           this.SelectedElement.Y + this.SelectedElement.ViewHeight + 8
                                           ), paint);

                paint.IsStroke = false;
            }

            // 選択された関連付け
            if (this._selectedRelation.Relation != null)
            {
                paint.StrokeWidth = 6;
                paint.Color = SKColors.Blue;

                canvas.DrawLine(this._selectedRelation.A.X + 50, this._selectedRelation.A.Y + 50, this._selectedRelation.B.X + 50, this._selectedRelation.B.Y + 50, paint);
            }
        }

        public void OnTapStart(double x, double y)
        {
            // 現在選択中の要素をタップしようとしているか判定
            if (this.SelectedElement is MapEntityElement<E> el)
            {
                var element = this.Canvas.GetEntityFromPosition(this.Map.Elements, x, y);
                if (element?.Entity.Id == el?.Entity.Id)
                {
                    this.DraggingElement = element;
                    this.CanDragMap = false;
                }
                else
                {
                    this.DraggingElement = null;
                }
            }
        }

        public void OnTapEnd(double x, double y)
        {
            // 要素のドラッグが終わったところであれば、要素を再描画
            if (this.DraggingElement != null)
            {
                this.Canvas.RequestRedraw(true);

                if (this.DraggingElement != null)
                {
                    this.CanDragMap = true;
                    this.DraggingElement = null;
                }
            }
        }

        public void OnTapped(double x, double y)
        {
            // 選択された要素を検出する
            this.SelectedElement = this.Canvas.GetEntityFromPosition(this.Map.Elements, x, y);

            if (this.SelectedElement != null)
            {
                this.SelectedRelation = null;
            }
            else
            {
                // 選択された関連付けを検出する
                this._selectedRelation = this.Canvas.GetEntityRelationFromPosition(this.Map.Elements, this.EachRelations.Relations, x, y);
            }

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
        
        /// <summary>
        /// エンティティの選択を変更した時に発行
        /// </summary>
        public event ValueChangedEventHandler<MapEntityElement<E>> SelectedEntityChanged;

        /// <summary>
        /// 関連付けの選択を変更した時に発行
        /// </summary>
        public event ValueChangedEventHandler<EntityRelateBase<E, E>> SelectedRelationChanged;
    }
}
