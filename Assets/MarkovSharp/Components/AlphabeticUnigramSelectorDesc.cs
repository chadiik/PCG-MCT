using System.Collections.Generic;
using System.Linq;

namespace MarkovSharp.Components
{
    public class AlphabeticUnigramSelectorDesc<T> : IUnigramSelector<T>
    {
        public T SelectUnigram(IEnumerable<T> ngrams)
        {
            return ngrams
                .OrderByDescending(a => a)
                .FirstOrDefault();
        }
    }
}
