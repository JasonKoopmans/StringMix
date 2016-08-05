using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringMix.Model {

    /// <summary>
    /// Model class to represent a Token that has optionally been tagged with the given lexicon as a guide.
    /// </summary>
    public class TaggedToken {

        public TaggedToken() {
            Tags = new List<string>();
        }

        /// <summary>
        /// Numeric signifying where in the input string this token appeared
        /// </summary>
        public long TokenSequence { get; set; }
        
        /// <summary>
        /// The string value of the token
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The list of tags that have been applied to this token.  Given a lexicon, a matching
        /// value will apply all of the classes of that lexicon to the matching token.
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
