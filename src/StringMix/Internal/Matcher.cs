using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Internal {

    /// <summary>
    /// Matcher is responsible for being a filter on a list of tokens.  Given a list of tokens
    /// and the patterns that have been determined for those tokens, output a list of patterns 
    /// were matched for list.  Allows for more than one pattern to be matched.
    /// </summary>
    public class Matcher {

        /// <summary>
        /// The Tokens to be inspected
        /// </summary>
        public List<TaggedToken> Tokens = new List<TaggedToken>();

        /// <summary>
        /// The Patterns that correspond to these tokens according to the PatternMaker
        /// </summary>
        public List<string> Patterns = new List<string>();

        // Method: When
        // Takes: Delegate that accepts tokens and patterns and returns patterns that match
        // Returns: Object that has properties of tokens and patterns and return value

        /// <summary>
        /// Match method allows for a caller to push in the criteria (via a Func<>) that would be 
        /// used to determine if a list of tokens match the provided patterns to test for.
        /// </summary>
        /// <param name="criteria">a Func<> delegate that accepts a list of tokens, the patterns to test for, a
        /// and returns a list of matching patterns
        /// </param>
        /// <returns>a Mixer object that can be used to further chain calls</returns>
        public Mixer Match(Func<List<TaggedToken>, List<string>, List<string>> criteria) {
            return new Mixer() {
                Tokens = Tokens,
                MatchedPatterns = criteria.Invoke(Tokens, Patterns)
            };
        }

        /// <summary>
        /// Convenience method that allows caller to use common Regex Matching Criteria and Mixing Actions.
        /// </summary>
        /// <param name="MatchRegex">The Regex to use for a test to see if a pattern should be considered a match</param>
        /// <param name="MixRegex">For those patterns that match the "MatchRegex", this regex will create a Mix object for each regex match
        /// matching this parameter in those matches
        /// </param>
        /// <returns>a Translator object that permits further method chaining, particularly the translation from 
        /// List<Mix> to an object of the callers choosing
        /// </returns>
        public Translator RegexMatchAndMix(String MatchRegex, String MixRegex) {
            return Match(MatchCriteria.RegexCriteria(MatchRegex)).Mix(MixActions.RegexExtraction(MixRegex));
        }

        /// <summary>
        /// Convenience method that allows caller to use the same regex pattern as the basis for both Matching and Mixing.  
        /// It is the equal of calling RegexMatchAndMix(Pattern, Pattern)
        /// </summary>
        /// <param name="MatchAndMixPattern">The pattern to apply for both pattern matching as well as token extraction</param>
        /// <returns>a Translator object that permits further method chaining, particularly the translation from 
        /// List<Mix> to an object of the callers choosing</returns>
        public Translator RegexMatchAndMix(String MatchAndMixPattern) {
            return Match(MatchCriteria.RegexCriteria(MatchAndMixPattern)).Mix(MixActions.RegexExtraction(MatchAndMixPattern));
        }

    }
}
