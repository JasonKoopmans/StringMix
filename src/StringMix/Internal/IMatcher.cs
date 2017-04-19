using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringMix.Internal
{
    public interface IMatcher
    {
        MatchSet Match(List<TaggedToken> tokens);
    }

    public class RegexMatcher : IMatcher
    {
        public string Expression { get; set; }

        public MatchSet Match(List<TaggedToken> tokens)
        {
            MatchSet ret = new MatchSet();
            List<Pattern> candidates = PatternMaker.MakePatterns(tokens);

            ret.Tokens = tokens;
            // Iterate over the patterns, return true if one matches
            ret.MatchedPatterns = candidates.Where(x => Regex.IsMatch(x.PatternText, Expression)).ToList();
            return ret;
        }
    }
}
