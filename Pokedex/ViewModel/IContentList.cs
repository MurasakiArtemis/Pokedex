using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.ViewModel
{
    interface IContentList
    {
        IEnumerable<string> ContentList { get; }
    }
}
