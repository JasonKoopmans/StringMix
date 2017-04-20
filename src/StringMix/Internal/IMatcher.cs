using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Internal
{
    public interface IMatcher
    {
        MatchSet Match(List<TaggedToken> tokens);
    }
}
