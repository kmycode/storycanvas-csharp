using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor.Map
{
    /// <summary>
    /// 編集画面のマップ１枚分
    /// </summary>
    [DataContract]
    public class SimpleEntityMap<E> : INotifyPropertyChanged where E : Entity
    {
        /// <summary>
        /// マップの名前
        /// </summary>
        public string Name
        {
            get => this._name;
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged();
                }
            }
        }
        [DataMember]
        private string _name;

        /// <summary>
        /// 画面に表示する要素群
        /// </summary>
        [DataMember]
        private Collection<MapEntityElement<E>> _elements = new Collection<MapEntityElement<E>>();
        public Collection<MapEntityElement<E>> Elements => this._elements;

        public void LoadEntities(EntitySetModel<E> entities)
        {
            foreach (var el in this.Elements)
            {
                el._entity.FindEntity(entities);
            }
        }
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
