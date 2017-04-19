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

    }
}
