using StringMix.Internal;
using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringMix {
    public class MixPipeline {
        /// <summary>
        /// The list of tagged tokens that would result from tokenizing and tagging.  Either populated by Tag() or 
        /// a constructor that calls Tag()
        /// </summary>
        private List<TaggedToken> _tokens;

        /// <summary>
        /// The Tokenizer/Tagger that is getting used to break the input string into tokens and then label them
        /// according to lexicon.
        /// </summary>
        private Tagger _tagger;

        /// <summary>
        /// Constructor that will tokenize and tag the input string given lexicon and default options.  Shortcut for 
        /// new Mixer(lexicon).Tag(input)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lexicon"></param>
        public MixPipeline(string input, List<LexiconEntry> lexicon) : this(input, lexicon, new StringMixOptions()) { }

        /// <summary>
        /// Constructor that will tokenize and tag the input string given lexicon and options. Shortcut for
        /// new Mixer(lexicon, options).Tag(input)
        /// </summary>
        /// <param name="input">The string to be tagged</param>
        /// <param name="lexicon">The list of expected tokens and their "tag" descriptors</param>
        /// <param name="options">Options for the Tagger/Tokenizer</param>
        public MixPipeline(string input, List<LexiconEntry> lexicon, StringMixOptions options) : this(input, new Tagger(lexicon, options) ) { }
        
        /// <summary>
        /// Constructor that will tokenize a given string with the provided tagger.  A shortcut to new Mixer(Tagger).Tag(input);
        /// </summary>
        /// <param name="input"></param>
        /// <param name="tagger"></param>
        public MixPipeline(string input, Tagger tagger): this(tagger.Tag(input)) {
            _tagger = tagger;
        }

        /// <summary>
        /// Constructor that will build up a Mixer accepting the provided lexicon and default Options
        /// </summary>
        /// <param name="lexicon">A list of lexicon entries that describe the expected tokens and 
        /// their "tag" descriptors
        /// </param>
        public MixPipeline(List<LexiconEntry> lexicon) : this(lexicon, new StringMixOptions()) { }

        /// <summary>
        /// Constructor that will build up a new Mixer using the provided lexicon and options
        /// </summary>
        /// <param name="lexicon">A list of lexicon entries that describe the expected tokens and 
        /// their "tag" descriptors
        /// </param>
        /// <param name="options">An options object that controls the method by which input strings 
        /// will be tokenize and tagged
        /// </param>
        public MixPipeline(List<LexiconEntry> lexicon, StringMixOptions options) : this(new Tagger(lexicon, options)) { }
        
        /// <summary>
        /// Constructor for cases when a tagger has already been pre-configured
        /// </summary>
        /// <param name="tagger">An already configured tagger</param>
        public MixPipeline(Tagger tagger) {
            _tagger = tagger;
        }

        /// <summary>
        /// Constructor accepting a list of TaggedTokens that were either constructed elsewhere or pre-processed
        /// </summary>
        /// <param name="tokens">List of Tagged Tokens</param>
        public MixPipeline(List<TaggedToken> tokens) {
            _tokens = tokens;
        }

        /// <summary>
        /// Tag() provides a simple method to process a string and internally store the tagged tokens
        /// </summary>
        /// <param name="input"></param>
        public void Tag(string input) {
            if (_tagger == null) {
                throw new InvalidOperationException("There is no tagger defined for this mixer.  Please choose a different constructor if you wish to Tag() with this mixer.");
            }
            _tokens = _tagger.Tag(input);
        }

        /// <summary>
        /// CombineAll() is a method for simple mixing allowing for all of the instances of tagname1
        /// to be combined with all of the instances of tagname2, using separator as a string to join them
        /// </summary>
        /// <param name="tagname1">the tagname that should appear first in all returned combinations</param>
        /// <param name="tagname2">the tagname that should appear second in all returned combinations</param>
        /// <param name="separator">the string that should be placed between the tokens in each of the combinations</param>
        /// <returns></returns>
        public List<string> CombineAll(string tagname1, string tagname2, string separator) {
            List<string> ret = new List<string>();

            foreach (var tag1 in _tokens.Where(x=> x.Tags.Contains(tagname1))) {
                foreach (var tag2 in _tokens.Where(x => x.Tags.Contains(tagname2) )) {
                    ret.Add(String.Join(separator, tag1.Value, tag2.Value));
                }
            }
            return ret;
        }

        /// <summary>
        /// With()
        /// </summary>
        /// <param name="input">the string to be tokenized, tagged, patterned, and later processed</param>
        /// <returns>WithAnchor: an object from which the call chain can continue to Mix()</returns>
        public Matcher With(string input) {
            Matcher ret = new Matcher();

            ret.Tokens = _tokens = _tagger.Tag(input);
            ret.Patterns = PatternMaker.MakePatterns(_tokens);

            return ret;
        }

        /// <summary>
        /// With() Convenience method that would allow for further processing (Mix/Translate) though tagging is not needed.
        /// </summary>
        /// <param name="tokens">a List<TaggedToken> </param>
        /// <returns>WithAnchor: an object from which the call chain can continue to Mix()</returns>
        public Matcher With(List<TaggedToken> tokens) {
            Matcher ret = new Matcher();
            ret.Tokens = tokens;
            ret.Patterns = PatternMaker.MakePatterns(_tokens);
            return ret;
        }

        

  


    }
}
