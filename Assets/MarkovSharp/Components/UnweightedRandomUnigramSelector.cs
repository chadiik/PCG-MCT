using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkovSharp.Components
{
    public class UnweightedRandomUnigramSelector<T> : IUnigramSelector<T>
    {
        public T SelectUnigram(IEnumerable<T> ngrams)
        {
            return ngrams.GroupBy(a => a)
                .Select(a => a.FirstOrDefault())
                .OrderBy(a => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
