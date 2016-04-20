using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringMix.Internal {
    
    
    /// <summary>
    /// A class that contains common implementations of Match Criteria that would be passed into the Match() method
    /// </summary>
    public class MatchCriteria {

        /// <summary>
        /// Using Regex expression iterate over all of the patterns.  Return all patterns that match the regex expression
        /// </summary>
        /// <param name="expression">The regex expression to compare each pattern to</param>
        /// <returns>a List<string> representing all of the patterns that matched this criteria
        /// </returns>
        public static Func<List<TaggedToken>, List<string>, List<string>> RegexCriteria(string expression) {
            Func<List<TaggedToken>, List<string>, List<string>> ret = (t, p) => {
                // Iterate over the patterns, return true if one matches
                return p.Where(x => Regex.IsMatch(x, expression)).ToList();
            };
            return ret;
        }

        /// <summary>
        /// Used simple String.Equals() comparision to compare the patterns to
        /// </summary>
        /// <param name="expression">The comparison patterns to seek</param>
        /// <returns>
        /// A List<string> representing all of the patterns that matched the expression
        /// </returns>
        public static Func<List<TaggedToken>, List<string>, List<string>> ExactMatch(string expression) {
            Func<List<TaggedToken>, List<string>, List<string>> ret = (t, p) => {
                // Iterate over the patterns, return true if one matches
                return p.Where(x => x.Equals(expression)).ToList();
            };
            return ret;
        }
    }
}
