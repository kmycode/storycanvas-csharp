using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared
{
    static class ArrayExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
        {
            foreach (var el in elements)
            {
                collection.Add(el);
            }
        }
    }
}
