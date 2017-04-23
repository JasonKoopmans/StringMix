using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Model {

    /// <summary>
    /// An options class which allows a caller to express the desired behavior 
    /// of the components of th elibrary
    /// </summary>
    public class StringMixOptions {
        
        /// <summary>
        /// The list of separator string that are used to divide the input string. The default is
        /// "[space], [comma], [semicolon], [period]"
        /// </summary>
        public List<string> Separators { get; set; }

        /// <summary>
        /// The StringSplitOptions that the input string will be split using.  This is directly entered
        /// into String.Split
        /// </summary>
        public StringSplitOptions StringSplitOptions { get; set; }

        /// <summary>
        /// With what string will tokens that do not match an element in the lexicon be 
        /// represented with in generated patterns?  Default: "?"
        /// </summary>
        public string  EmptyTagValue { get; set; }

        /// <summary>
        /// Determines if the pattern makers uses case sensitivity when determining a match.  Default: true
        /// </summary>
        public bool MatchesAreCaseSensitive { get; set; }

        public StringMixOptions() {
            Separators = new List<string> { " ", ";", ",", "." };
            StringSplitOptions = System.StringSplitOptions.RemoveEmptyEntries;
            EmptyTagValue = "?";
            MatchesAreCaseSensitive = true;
        }
        public StringMixOptions(bool MatchesAreCaseSensitive, params string[] AdditionalSeparators) : this() {
            Separators.AddRange(AdditionalSeparators);
            this.MatchesAreCaseSensitive = MatchesAreCaseSensitive;
        }

        /// <summary>
        /// A convenient static field containing a default implementation
        /// </summary>
        public static StringMixOptions DEFAULT = new StringMixOptions();
    }
}
