using System.Collections.Generic;

namespace Pokedex.ViewModel
{
    interface IContentList
    {
        IEnumerable<string> ContentList { get; }
    }
}
