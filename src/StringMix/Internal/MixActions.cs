using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringMix.Internal {
    
    /// <summary>
    /// A class containing common implementations of Actions that would be used to map List of Tokens
    /// and their Patterns into a list of Mixes
    /// </summary>
    public class MixActions {


        /// <summary>
        /// A simple Action that maps all of the tokens that contain a certain tag and combines all
        /// of them with all of the tokens that contain the second tag name.  Meant for simple 
        /// combination
        /// </summary>
        /// <param name="tagname1">the first tag to search for</param>
        /// <param name="tagname2">the second tag to search for</param>
        /// <returns>a list of mixes containing all of the combinations of the first tag and the second tag</returns>
        public static Func<List<TaggedToken>, List<string>, List<Mix>> CombineAll(string tagname1, string tagname2) {
            Func<List<TaggedToken>, List<string>, List<Mix>> ret = (t, p) => {
                List<Mix> list = new List<Mix>();
                foreach (var tag1 in t.Where(x => x.Tags.Contains(tagname1))) {
                    foreach (var tag2 in t.Where(x => x.Tags.Contains(tagname2))) {
                        list.Add(new Mix(tag1, tag2));
                    }
                }
                return list;
            };
            return ret;
        }

        /// <summary>
        /// Use regular expressions to perform mappings from tokens and patterns to list of mixes
        /// </summary>
        /// <param name="pattern">The regex pattern to match to the list of patterns</param>
        /// <returns>a list of mixes.  Each mix will contain the tokens that correspond to 
        /// the tags that matched within the pattern.  Consider:
        /// Tokens(Tags): Fred(F) Franklin(F,M) Flintstone(L)
        /// Patterns: [2] FFL, FML
        /// RegEx Match Pattern: F?L
        /// RegEx Extraction Pattern: F
        /// Mixes: [2] Fred Franklin, Fred
        /// </returns>
        public static Func<List<TaggedToken>, List<string>, List<Mix>> RegexExtraction(string pattern) {
            Func<List<TaggedToken>, List<string>, List<Mix>> ret = (t, ps) => {
                List<Mix> list = new List<Mix>();

                foreach (var p in ps) { // pattern in patters

                    // Get the matches that occur in the current pattern getting examined
                    var matches = Regex.Matches(p, pattern); 

                    // for each one of those Regex matched
                    foreach (Match match in matches) {
                        
                        // Create a Set -- this will be the kernel of a new mix
                        List<TaggedToken> set = new List<TaggedToken>(match.Length);

                        // walk the length of the regex match, adding the corresponding token 
                        // found that that location
                        for (int i = match.Index; i < match.Index + match.Length; i++) {
                            set.Add(t[i]);
                        }

                        // Collect the mix into the set that will be returned.
                        list.Add(new Mix(set));
                    }
                }
                return list;
            };
            return ret; // delegate
        }
    }
}
