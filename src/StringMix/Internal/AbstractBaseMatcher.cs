using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringMix.Model;

namespace StringMix.Internal
{
    /// <summary>
    /// Reference base class of the IMatcher interface.  This implementation allows implementors a simple
    /// base class to inherit that takes care of the housekeeping of slotting matches and nonmatches in the 
    /// right collection on a matchset.  This allows derived classes to stay focused on concerns of the implementation:
    /// Is the matcher in a valid state, and the criteria that determines if a match exists. 
    /// </summary>
    public abstract class AbstractBaseMatcher : IMatcher
    {
        public MatchSet Match(List<TaggedToken> tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException("tokens");
            }

            var validation = this.ValidateMatcher();
            if ( !validation.IsValid )
            {
                var ex = new InvalidOperationException("The Matcher is in an invalid state.  Check the Data Collection for more details");
                ex.Data["messages"] = validation.ValidationMessages;
                throw ex;
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
                if ( IsMatch(candidate))
                {
                    ret.MatchedPatterns.Add(candidate);
                }
                else
                {
                    ret.UnmatchedPatterns.Add(candidate);
                }
            }

            return ret;
        }

        /// <summary>
        /// A validation method that subclasses need to implement.  Allows subclass to test internal structure for validity, 
        /// return list of messages to indicate what went wrong
        /// </summary>
        /// <returns></returns>
        public abstract MatcherValidation ValidateMatcher();

        /// <summary>
        /// Derived classes are required to implement this method which accepts a candidate patterns and, based on the 
        /// properties and purpose of the matcher simply express whether that candidate is a match or not.  The Base class
        /// does the rest of the housekeeping to put the cadidate in the right collection (matched vs unmatched).  
        /// </summary>
        /// <param name="Candidate"></param>
        /// <returns></returns>
        public abstract bool IsMatch(Pattern Candidate);

    }

    public class MatcherValidation
    {
        public bool IsValid { get; set; } = true;
        public List<string> ValidationMessages { get; set; } = new List<string>();
    }
}
