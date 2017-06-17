using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor.Map
{
    /// <summary>
    /// 編集画面のマップ１グループ
    /// </summary>
    [DataContract]
    public class SimpleEntityMapGroup<T> where T : Entity
    {
        /// <summary>
        /// マップ群
        /// </summary>
        [DataMember]
        private ObservableCollection<SimpleEntityMap<T>> _maps = new ObservableCollection<SimpleEntityMap<T>>();
        public ObservableCollection<SimpleEntityMap<T>> Maps => this._maps;

        /// <summary>
        /// 現在選択されているマップ
        /// </summary>
        public SimpleEntityMap<T> SelectedMap
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
        private SimpleEntityMap<T> _selectedMap;

        /// <summary>
        /// 選択中のマップが変更された時に発行
        /// </summary>
        public event EventHandler SelectedMapChanged;
    }
}
