using System.Collections.Generic;
using System.Linq;

namespace MarkovSharp.Components
{
    public class AlphabeticUnigramSelector<T> : IUnigramSelector<T>
    {
        public T SelectUnigram(IEnumerable<T> ngrams)
        {
            return ngrams
                .OrderBy(a => a)
                .FirstOrDefault();
        }
    }
}
