using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor.Map
{
    /// <summary>
    /// 編集画面のマップに表示するエンティティ要素
    /// </summary>
    [DataContract]
    public class MapEntityElement<T> : MapElement where T : Entity
    {
        /// <summary>
        /// 対象エンティティ
        /// </summary>
        public T Entity
        {
            get => this._entity.Entity;
            set => this._entity.Entity = value;
        }

        [DataMember]
        public EntityReferenceModel<T> _entity { get; set; } = new EntityReferenceModel<T>();
    }
}
