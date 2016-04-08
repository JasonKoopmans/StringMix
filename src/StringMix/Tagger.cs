using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix {

    /// <summary>
    /// Class whose purpose is to divide an input string into a list of component strings (tokens).
    /// and, given a lexicon, apply tags to the tokens that match that lexicon.
    /// </summary>
    public class Tagger {

        private const int DEFAULT_CAPACITY = 1000;
        private Dictionary<string, LexiconEntry> _lexicon; 
        private StringMixOptions _options;
     
        public Tagger(IList<LexiconEntry> lexicon) : this(lexicon, new StringMixOptions()) { }

        public Tagger(IList<LexiconEntry> lexicon, StringMixOptions options) {
            _lexicon = new Dictionary<string, LexiconEntry>(DEFAULT_CAPACITY);
            _options = options;

            string itemvalue = String.Empty;

            foreach (var item in lexicon) {
                itemvalue = _options.MatchesAreCaseSensitive ? item.Value : item.Value.ToLower();
                if (_lexicon.ContainsKey(itemvalue)) {
                    _lexicon[itemvalue].Tags.AddRange(item.Tags);
                } else {
                    _lexicon.Add(itemvalue, item);
                }
            }

        }

        /// <summary>
        /// Accessor for the Option that this Tagger uses
        /// </summary>
        public StringMixOptions Options { 
            get { return _options; }
            private set { ;} 
        }

        /// <summary>
        /// Given an input string, split into composite tokens.  In the process, consult the lexicon for matches and 
        /// apply the tags from the lexicon to the split tokens.
        /// </summary>
        /// <param name="In">
        /// The string to be tokenized and tagged
        /// </param>
        /// <returns>
        /// A list of Tagged Tokens
        /// </returns>
        public List<TaggedToken> Tag(String In) {
            List<TaggedToken> ret = new List<TaggedToken>();
            
            // Split string using the configured separators and split options
            string[] rawtokens = In.Split(_options.Separators.ToArray<string>(), _options.StringSplitOptions);

            // For each token
            for (int i = 0; i < rawtokens.LongCount(); i++) {

                // Make a new model object 
                var token = new TaggedToken() {
                    // Value of the token
                    Value = rawtokens[i],
                    // position of the token in the input string (the nth token)
                    TokenSequence = i
                };
                
                // Attempt to search the lexicon for a matching token.
                LexiconEntry found;
                string sought = _options.MatchesAreCaseSensitive ? token.Value : token.Value.ToLower();
                if (_lexicon.TryGetValue(sought, out found)) {
                    token.Tags = found.Tags;
                } else {
                    // Apply a placeholder tag for tokens not in the lexicon
                    token.Tags.Add(_options.EmptyTagValue);
                }

                // Add the token to the list
                ret.Add(token);
            }

            // return the list
            return ret;

        }



         
    }
}