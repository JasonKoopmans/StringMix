using StringMix.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringMix.Internal
{
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
