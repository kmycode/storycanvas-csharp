using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Converters
{
    public class MainMode2NameConverter : ValueConverterBase<MainMode, string>
    {
        public override string Convert(MainMode value)
        {
            var key = value.ToString();

            if (key == "None")
            {
                return "";
            }
            if (key.StartsWith("Edit"))
            {
                return StringResourceResolver.Resolve(key.Substring("Edit".Length));
            }
            if (key.EndsWith("Page"))
            {
                return StringResourceResolver.Resolve(key.Substring(0, key.Length - "Page".Length));
            }

            throw new ArgumentException("指定されたキーはサポートされていません");
        }
    }
}
