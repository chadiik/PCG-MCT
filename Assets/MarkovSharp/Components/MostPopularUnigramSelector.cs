using System.Collections.Generic;
using System.Linq;

namespace MarkovSharp.Components
{
    public class MostPopularUnigramSelector<T> : IUnigramSelector<T>
    {
        public T SelectUnigram(IEnumerable<T> ngrams)
        {
            return ngrams
                .GroupBy(a => a).OrderByDescending(a => a.Count())
                .FirstOrDefault()
                .FirstOrDefault();
        }
    }
}
