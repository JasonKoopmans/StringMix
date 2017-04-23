using StringMix.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringMix.Internal
{
    /// <summary>
    /// Reference implementation of the IMatcher interface.  When provided a value for expression
    /// (which is presumed a regular expression) this matcher will use this expression to test the 
    /// list of pattern combinations present in the provided tagged tokens.  If any of the patterns
    /// matches the provided regex, the pattern is added to the MatchSet that is returned to the 
    /// caller
    /// </summary>
    public class RegexMatcher : IMatcher
    {
        public string Expression { get; set; }
        public RegexOptions Options { get; set; } = RegexOptions.None;
        public TimeSpan RegExTimeout { get; set; } = new TimeSpan(0, 0, 5); // 5 Seconds

        public MatchSet Match(List<TaggedToken> tokens)
        {
            MatchSet ret = new MatchSet();
            List<Pattern> candidates = PatternMaker.MakePatterns(tokens);
            
            // For the reference of callers that recieve the MatchSet, the original set of
            // tokens is included.
            ret.Tokens = tokens;
            
            // Use a LINQ .Where() to filter the list of patterns down to only those that 
            // successfully match the Expression provided.
            ret.MatchedPatterns = candidates.Where(x => Regex.IsMatch(x.PatternText, Expression, this.Options, this.RegExTimeout)).ToList();

            return ret;
        }
    }
}
