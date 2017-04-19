using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Internal {

    /// <summary>
    /// Using Mixer callers can perform actions on sets of tokens that match a defined pattern.  
    /// In some cases this could be redundant to what happens in Match().  It does allow for cases
    /// where matching a larger string in Match() provides an opportunity to further refine or map
    /// the tagged tokens into a different list of Mixes.  
    /// </summary>
    public class Mixer {

        /// <summary>
        /// Tokens are the list of tokens that resulted from Tagging
        /// </summary>
        public List<TaggedToken> Tokens = new List<TaggedToken>();

        /// <summary>
        /// MatchedPatterns are the pattern values that matched according to the lexicon in the Mixer (via Tagger)
        /// </summary>
        public List<string> MatchedPatterns = new List<string>();

        /// <summary>
        /// A static result to represent an empty set avoiding Null Reference Exceptions
        /// </summary>
        protected static List<List<TaggedToken>> EmptyResult = new List<List<TaggedToken>>(0);

        /// <summary>
        /// Mix is a method that allows a action to be expressed that turns the TaggedTokens and Patterns into a list of
        /// Matched Pattern Results.  
        /// </summary>
        /// <param name="Action">Delegate (Func<>) that accepts a list of tagged tokens, a list of patterns, and returns a List of List of TaggedTokens</param>
        /// <returns>List of List of TaggedTokens</returns>
        public Translator Mix(Func<List<TaggedToken>, List<string>, List<Mix>> Action) {
            if (MatchedPatterns.Count() == 0) {
                return new Translator();
            } else {
                return new Translator() { Mixes = Action.Invoke(Tokens, MatchedPatterns) };
            }
        }
    }
}
