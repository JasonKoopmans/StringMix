using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix {
    public class PatternMaker {

        /// <summary>
        /// Static Method to take an input list of tagged tokens and provide all of the 
        /// Patterns that represent the tokens given the lexicon they were tagged with
        /// </summary>
        /// <param name="tokens">
        /// The Tokens that patterns should be made for
        /// </param>
        /// <returns>
        /// A list of patterns that represent the list of tokens provided.  It is possible to 
        /// have more than one pattern in cases where any of the matched tokens are tagged with 
        /// more than one tag.  "Thomas" > First and/or Last Name "F" and "L" Tag
        /// </returns>
        public static List<string> MakePatterns(List<TaggedToken> tokens) {
            return internalMakePatterns(tokens, new List<string>(), 0);
        }

        /// <summary>
        /// Internal recursive function for visiting all of the tokens in the correct order and 
        /// creating the patterns
        /// </summary>
        /// <param name="tokens">
        /// List of tokens to be patterned
        /// </param>
        /// <param name="patterns">
        /// The list of patterns created so far
        /// </param>
        /// <param name="targetToken">
        /// The number token that should be worked on (visited)
        /// </param>
        /// <returns>
        /// RECURSIVE:
        /// eventually returns the complete set of patterns that the list of tokens can be represented by.
        /// </returns>
        private static List<string> internalMakePatterns(List<TaggedToken> tokens, IList<string> patterns, int targetToken) {
            TaggedToken t = tokens[targetToken];
            List<string> newList = new List<string>();

            foreach (var item in t.Tags) {
                if (targetToken == 0) {
                    newList.Add(item);
                }
                foreach (var pattern in patterns) {
                    string newitem = pattern + item;
                    if (!newList.Contains(newitem)) {
                        newList.Add(newitem);
                    }
                }
            }
            
            if (targetToken + 1   ==  tokens.Count()) {
                return newList;
            } else{ 
                return internalMakePatterns(tokens, newList, targetToken + 1);
            }

        }
        
    }
}
