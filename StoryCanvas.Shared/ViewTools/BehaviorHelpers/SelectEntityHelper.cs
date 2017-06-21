using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.ViewTools.BehaviorHelpers
{
    public interface IEntitySelectionOpener
    {
        /// <summary>
        /// 選択の開始が要求された時に発行
        /// </summary>
        event EventHandler SelectionRequested;
    }

    /// <summary>
    /// エンティティの選択画面を表示するためのヘルパ
    /// </summary>
    public class SelectEntityHelper<E> : IEntitySelectionOpener where E : Entity
    {
        /// <summary>
        /// 選択画面を開く
        /// </summary>
        public void OpenSelection()
        {
            this.SelectionRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// エンティティが選択されたことを通知する
        /// </summary>
        /// <param name="entity">選択されたエンティティ</param>
        public void NotifySelected(E entity)
        {
            if (entity != null)
            {
                this.EntitySelected?.Invoke(this, new EntitySelectedEventArgs<E>
                {
                    Entity = entity,
                });
            }
        }

        /// <summary>
        /// 選択の開始が要求された時に発行
        /// </summary>
        public event EventHandler SelectionRequested;

        /// <summary>
        /// エンティティが選択された時に発行
        /// </summary>
        public event EventHandler<EntitySelectedEventArgs<E>> EntitySelected;
    }
    
    public class EntitySelectedEventArgs<E> : EventArgs where E : Entity
    {
        public E Entity { get; set; }
    }
}
