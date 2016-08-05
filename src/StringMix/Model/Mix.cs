using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Model {

    /// <summary>
    /// A Mix is created as a result of calling the Mix() method on the Mixer class.  It is 
    /// simply just a container for a list of tokens.  It exists to provide a meaningful model
    /// object to understand the results of a call to that method.  In many cases, this will be 
    /// a list with similar dimensions of the inputs into Mix() [The source tokens 
    /// themselves and the patterns that were matched].  The important piece to understand 
    /// is that it is not *necessary* for these dimensions to be the same.  A consumer may want to use
    /// Match() in the pipeline to match a bigger pattern, but only be interested in a subset 
    /// of the tokens that are captured as a part of the pattern.  
    /// 
    /// Though this is in the model namespace, its really expected that these objects would result from 
    /// calls to Mix() and other methods in the framework.  
    /// </summary>
    public class Mix {
        /// <summary>
        /// The Tokens that are a part of this mix
        /// </summary>
        public List<TaggedToken> Tokens = new List<TaggedToken>();
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Mix() { }

        /// <summary>
        /// Convenience constructor.  Callers can provice 2 tokens to initialize the token list with
        /// </summary>
        /// <param name="token1">Just a token</param>
        /// <param name="token2">Just another token</param>
        public Mix(TaggedToken token1, TaggedToken token2) {
            Tokens.Add(token1);
            Tokens.Add(token2);
        }

        /// <summary>
        /// Like the (TaggedToken, TaggedToken) constuctor, this is a convenience for simplifying initialization
        /// </summary>
        /// <param name="tokens">Some tokens, in a list.</param>
        public Mix(List<TaggedToken> tokens) {
            Tokens = tokens;
        }

        

    }
}
