using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Converters
{
    /// <summary>
    /// enumの値が文字列と一致しているか調べる
    /// </summary>
    public class EnumEqualsConverter : ValueConverterBase<Enum, bool, string>
    {
        public override bool Convert(Enum value, string parameter)
        {
            return value.ToString() == parameter;
        }
    }
}
