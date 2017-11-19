using System;
using System.Collections.Generic;
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
    public abstract class Map : INotifyPropertyChanged
    {
        /// <summary>
        /// 描画を開始するX座標。画面スクロールで変動
        /// </summary>
        [DataMember]
        public float X { get; set; }

        /// <summary>
        /// 描画を開始するY座標。画面スクロールで変動
        /// </summary>
        [DataMember]
        public float Y { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
