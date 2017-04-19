using StringMix.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Model
{
    public class TransformConfig<T> where T: new()
    {
        public MatchCriteria  MatchCriteria { get; set; }
        public MixActions MixAction { get; set; }
        public ITranslator<T> Translator { get; set; }
    }
}
