using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.ViewTools
{
	public interface IDataContextHolder
	{
		object DataContext { get; set; }
	}
}
