using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Converters
{
    public class MainMode2BooleanConverter : ValueConverterBase<MainMode, bool, string>
    {
        public override bool Convert(MainMode value, string parameter)
        {
            return value.ToString() == parameter;
        }
    }
}
