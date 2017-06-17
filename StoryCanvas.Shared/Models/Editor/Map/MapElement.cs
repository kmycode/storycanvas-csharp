using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor.Map
{
    /// <summary>
    /// 編集画面のマップに表示するそれぞれの要素
    /// </summary>
    [DataContract]
    public class MapElement
    {
        /// <summary>
        /// 配置するX座標
        /// </summary>
        [DataMember]
        public int X { get; set; }

        /// <summary>
        /// 配置するY座標
        /// </summary>
        [DataMember]
        public int Y { get; set; }

        /// <summary>
        /// 横幅。この値はビュー側によって設定される
        /// </summary>
        public int ViewWidth { get; set; }

        /// <summary>
        /// 縦幅。この値はビュー側によって設定される
        /// </summary>
        public int ViewHeight { get; set; }
    }
}
