using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringMix {

    /// <summary>
    /// An interface that defines a method that needs to accept a List of List of TaggedTokens (that 
    /// correspond to the patterns that were matched) that return a instance of type T.  Callers to the 
    /// MixAnchor.Translate method can either pass in an object that implements this interface -or- a 
    /// Func<List<List<TaggedToken>>, T
    /// </summary>
    /// <typeparam name="T">Generic Type that represents the type this class will return in its Translate method</typeparam>
    public interface ITranslator<T> where T: new() {
        T Translate(List<Mix> TokenListLists);
    }

    /// <summary>
    /// using a Translator callers can perform actions on "Mixes" that are the  matched lists of token lists that 
    /// result from performing Mix() on an original list of tagged tokens and a list of patterns that should be matched
    /// </summary>
    public class Translator {
        public Translator() { }

        /// <summary>
        /// The list of Matched Lists of Tokens that resulted from a call to Mix().  This is the collection
        /// of items that will translated
        /// </summary>
        public List<Mix> Mixes = new List<Mix>();

        /// <summary>
        /// Translates a list of matched TokenLists an object of T type.  This offers a way for a caller to express
        /// how these lists should be translated and allow the framework to do the work of translation.
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="translator">a function that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Translate<T>(Func< List<Mix>, T> translator)  where T: new() {
            if (Mixes.Count() == 0) {
                return default(T);
            } else {
                return translator.Invoke(Mixes);
            }
        }

        /// <summary>
        /// An alternate to the Translate method that accepts a Func<> for those callers that would 
        /// prefer to work with more permenant types rather that function delegates
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="translator">a class implementing ITranslator<T> that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Translate<T>(ITranslator<T> translator) where T : new() {
            return Translate(translator.Translate);
        }

    }

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
        public static List<List<TaggedToken>> EmptyResult = new List<List<TaggedToken>>(0);
        
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
                return new Translator() { Mixes =  Action.Invoke(Tokens, MatchedPatterns)};
            }
        }
    }

    public class Mix {

        public List<TaggedToken> Tokens = new List<TaggedToken>();
        public Mix (TaggedToken token1, TaggedToken token2) {
            Tokens.Add(token1);
            Tokens.Add(token2);
	    }  

        public Mix (List<TaggedToken> tokens) {
            Tokens = tokens;
	    }

        public Mix () {

	    }

    }
    
    public class Matcher {
        public List<TaggedToken> Tokens = new List<TaggedToken>();
        public List<string> Patterns = new List<string>();

        // Method: When
        // Takes: Delegate that accepts tokens and patterns and returns patterns that match
        // Returns: Object that has properties of tokens and patterns and return value
        public Mixer Match(Func<List<TaggedToken>, List<string>, List<string>> criteria) {
            return new Mixer() {
                Tokens = Tokens,
                MatchedPatterns = criteria.Invoke(Tokens, Patterns)
            };
        }

        public Translator RegexMatchAndMix(String WhenRegex, String MixRegex) {
            return Match(MatchCriteria.RegexCriteria(WhenRegex)).Mix(MixActions.RegexExtraction(MixRegex));
        }
        public Translator RegexMatchAndMix(String MatchAndExtractPattern) {
            return Match(MatchCriteria.RegexCriteria(MatchAndExtractPattern)).Mix(MixActions.RegexExtraction(MatchAndExtractPattern));
        }

    }

    public class MatchCriteria {
        
        public static Func<List<TaggedToken>, List<string>, List<string>> RegexCriteria(string expression) {
            Func<List<TaggedToken>, List<string>, List<string>> ret = (t, p) => {
                // Iterate over the patterns, return true if one matches
                return p.Where(x => Regex.IsMatch(x, expression)).ToList();
            };
            return ret;
        }

        public static Func<List<TaggedToken>, List<string>, List<string>> ExactMatch(string expression) {
            Func<List<TaggedToken>, List<string>, List<string>> ret = (t, p) => {
                // Iterate over the patterns, return true if one matches
                return p.Where(x => x.Equals(expression) ).ToList();
            };
            return ret;
        }
    }

    public class MixActions {

        public static Func<List<TaggedToken>, List<string>, List<Mix>> CombineAll(string tagname1, string tagname2) {
            Func<List<TaggedToken>, List<string>,  List<Mix>> ret = (t, p) => {
                List<Mix> list = new List<Mix>();
                foreach (var tag1 in t.Where(x => x.Tags.Contains(tagname1))) {
                    foreach (var tag2 in t.Where(x => x.Tags.Contains(tagname2))) {
                        list.Add(new Mix(tag1, tag2) );
                    }
                }
                return list;
            };
            return ret;
        }

        public static Func<List<TaggedToken>, List<string>, List<Mix>> RegexExtraction(string pattern) {
            Func<List<TaggedToken>, List<string>, List<Mix>> ret = (t, ps) => {
                List<Mix> list = new List<Mix>();

                foreach (var p in ps) {
                    var matches = Regex.Matches(p, pattern);

                    foreach (Match match in matches) {
                        List<TaggedToken> set = new List<TaggedToken>(match.Length);
                        for (int i = match.Index; i < match.Index + match.Length; i++) {
                            set.Add(t[i]);
                        }
                        list.Add( new Mix(set) );
                    }
                }
                return list;
            };
            return ret; // delegate
        }

        public static Func<List<TaggedToken>, List<string>, List<Mix>> GatherSets(string tagname1, string tagname2) {
            Func<List<TaggedToken>, List<string>, List<Mix>> ret = (t,ps) => {
                List<Mix> list = new List<Mix>();

                foreach (var p in ps) {
                    // find the first tn1
                    int firstmatch = p.IndexOf(tagname1);

                    // its possible we don't find any, we should test for that
                    if (firstmatch == -1) {
                        return list;
                    }

                    List<TaggedToken> set = new List<TaggedToken>(2);
                    for (int i = firstmatch; i < p.Length; i++) {

                        // get the tag from the current location
                        string tag = p.Substring(i);
                        TaggedToken token = t[i];

                        // is the set full?
                        if (set.Count == 2) {
                            // Add it and reset
                            list.Add(new Mix(set));
                            set = new List<TaggedToken>(2);
                        }

                        // watch for something strange
                        if (tag.Equals(tagname1) && set.Count == 1) {
                            // found two tn1s in a row.  Problem
                        }

                        if (tag.Equals(tagname2) && set.Count == 0) {
                            // leading off with tn2.  Problem.
                        }
                    }


                    foreach (var tag1 in t.Where(x => x.Tags.Contains(tagname1))) {
                        foreach (var tag2 in t.Where(x => x.Tags.Contains(tagname2))) {
                            list.Add(new Mix( tag1, tag2 ));
                        }
                    }    
                }

                return list;
            };
            return ret;
        }


    }

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

        // Method: Match
            // Takes: string for input
            // Returns: Object onto which to call Match() with the state of patterns and tokens made from the input string.
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
