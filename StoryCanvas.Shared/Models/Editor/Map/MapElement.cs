using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor.Map
{
    /// <summary>
    /// 編集画面のマップに表示するそれぞれの要素
    /// </summary>
    public class MapElement
    {
        /// <summary>
        /// 配置するX座標
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 配置するY座標
        /// </summary>
        public int Y { get; set; }
    }
}
