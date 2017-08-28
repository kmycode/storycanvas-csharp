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
    /// 編集画面のマップ１グループ。入れられる要素は１種類のみ
    /// </summary>
    [DataContract]
    public class SingleEntityMapGroup<E> where E : Entity
    {
        /// <summary>
        /// マップ群
        /// </summary>
        [DataMember]
        private ObservableCollection<SingleEntityMap<E>> _maps = new ObservableCollection<SingleEntityMap<E>>
        {
            new SingleEntityMap<E>
            {
                Name = AppResources.TitleEmpty,
            },
        };
        public ObservableCollection<SingleEntityMap<E>> Maps => this._maps;

        /// <summary>
        /// 現在選択されているマップ
        /// </summary>
        public SingleEntityMap<E> SelectedMap
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
        private SingleEntityMap<E> _selectedMap;

        public SingleEntityMapGroup()
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
