using StringMix.Model;
using System;
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
            if (string.IsNullOrEmpty(this.Expression))
            {
                throw new ArgumentNullException("Expression", "The Expression for this matcher is empty or null");
            }

            if (tokens == null)
            {
                throw new ArgumentNullException("tokens");
            }

            MatchSet ret = new MatchSet();
            List<Pattern> candidates = PatternMaker.MakePatterns(tokens);
            
            // For the reference of callers that recieve the MatchSet, the original set of
            // tokens is included.
            ret.Tokens = tokens;

            // Iterate over all of the candidate.  For matches, include them in a matched list.  For
            // Unmatched items, put in the unmatched list.
            foreach (var candidate in candidates)
            {
                if (Regex.IsMatch(candidate.PatternText, Expression, this.Options, this.RegExTimeout))
                {
                    ret.MatchedPatterns.Add(candidate);
                } else
                {
                    ret.UnmatchedPatterns.Add(candidate);
                }
            }

            return ret;
        }
    }
}
