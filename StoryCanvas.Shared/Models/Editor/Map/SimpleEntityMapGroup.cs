using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor.Map
{
    /// <summary>
    /// 編集画面のマップ１グループ
    /// </summary>
    [DataContract]
    public class SimpleEntityMapGroup<E> where E : Entity
    {
        /// <summary>
        /// マップ群
        /// </summary>
        [DataMember]
        private ObservableCollection<SimpleEntityMap<E>> _maps = new ObservableCollection<SimpleEntityMap<E>>
        {
            new SimpleEntityMap<E>
            {
                Name = AppResources.TitleEmpty,
            },
        };
        public ObservableCollection<SimpleEntityMap<E>> Maps => this._maps;

        /// <summary>
        /// 現在選択されているマップ
        /// </summary>
        public SimpleEntityMap<E> SelectedMap
        {
            get => this._selectedMap;
            set
            {
                if (this._selectedMap != value)
                {
                    this._selectedMap = value;
                    this.SelectedMapChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        private SimpleEntityMap<E> _selectedMap;

        public SimpleEntityMapGroup()
        {
            this.Maps.CollectionChanged += (sender, e) =>
            {
                if (this.SelectedMap == null || !this.Maps.Contains(this.SelectedMap))
                {
                    this.SelectedMap = this.Maps.FirstOrDefault();
                }
            };
            this.SelectedMap = this.Maps.First();
        }

        public void LoadEntities(EntitySetModel<E> entities)
        {
            foreach (var map in this.Maps)
            {
                map.LoadEntities(entities);
            }
        }

        /// <summary>
        /// 選択中のマップが変更された時に発行
        /// </summary>
        public event EventHandler SelectedMapChanged;
    }
}
