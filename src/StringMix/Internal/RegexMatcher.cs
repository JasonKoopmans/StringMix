using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringMix.Internal
{
    /// <summary>
    /// Reference implementation of the AbstractBaseMatcher class.  When provided a value for expression
    /// (which is presumed a regular expression) this matcher will use this expression to test each created 
    /// pattern by the base class based on the provided tagged tokens.  If any of the patterns
    /// matches the provided regex, the pattern is added to the MatchSet that is returned to the 
    /// caller
    /// </summary>
    public class RegexMatcher : AbstractBaseMatcher
    {
        public string Expression { get; set; }
        public RegexOptions Options { get; set; } = RegexOptions.None;
        public TimeSpan RegExTimeout { get; set; } = new TimeSpan(0, 0, 5); // 5 Seconds

        public override bool IsMatch(Pattern Candidate)
        {
            return Regex.IsMatch(Candidate.PatternText, Expression, this.Options, this.RegExTimeout);
        }

        public override MatcherValidation ValidateMatcher()
        {
            var ret = new MatcherValidation();

            if (string.IsNullOrEmpty(this.Expression))
            {
                ret.IsValid = false;
                ret.ValidationMessages.Add("The Expression for this matcher is empty or null");
            }
            return ret;
        }
    }
}
