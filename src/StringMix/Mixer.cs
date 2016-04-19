using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringMix {

    public interface ITranslator<T> where T: new() {
        T Translate(List<List<TaggedToken>> TokenListLists);
    }

    public class MixAnchor {
        public MixAnchor() { }
        public List<List<TaggedToken>> Mixes = new List<List<TaggedToken>>();

        public T Translate<T>(Func< List<List<TaggedToken>>, T> translator)  where T: new() {
            if (Mixes.Count() == 0) {
                return default(T);
            } else {
                return translator.Invoke(Mixes);
            }
        }

        public T Translate<T>(ITranslator<T> translator) where T : new() {
            return Translate(translator.Translate);
        }

    }


    public class WhenAnchor {
        public List<TaggedToken> Tokens = new List<TaggedToken>();
        public List<string> MatchedPatterns = new List<string>();
        public static List<List<TaggedToken>> EmptyResult = new List<List<TaggedToken>>(0);

        // Method: Mix
        // Takes: Delegate that accepts tokens
        // Returns: List<string>
        
        public MixAnchor Mix(Func<List<TaggedToken>, List<string>, List<List<TaggedToken>>> Action) {
            if (MatchedPatterns.Count() == 0) {
                return new MixAnchor();
            } else {
                return new MixAnchor() { Mixes =  Action.Invoke(Tokens, MatchedPatterns)};
            }
        }

    }
    
    public class WithAnchor {
        public List<TaggedToken> Tokens = new List<TaggedToken>();
        public List<string> Patterns = new List<string>();

        // Method: When
        // Takes: Delegate that accepts tokens and patterns and returns patterns that match
        // Returns: Object that has properties of tokens and patterns and return value
        public WhenAnchor When(Func<List<TaggedToken>, List<string>, List<string>> criteria) {
            return new WhenAnchor() {
                Tokens = Tokens,
                MatchedPatterns = criteria.Invoke(Tokens, Patterns)
            };
        }

        public MixAnchor RegexMatchAndMix(String WhenRegex, String MixRegex) {
            return When(WhenCriteria.RegexCriteria(WhenRegex)).Mix(MixActions.RegexExtraction(MixRegex));
        }
        public MixAnchor RegexMatchAndMix(String MatchAndExtractPattern) {
            return When(WhenCriteria.RegexCriteria(MatchAndExtractPattern)).Mix(MixActions.RegexExtraction(MatchAndExtractPattern));
        }

    }

    public class WhenCriteria {
        
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

        public static Func<List<TaggedToken>, List<string>, List<List<TaggedToken>>> CombineAll(string tagname1, string tagname2) {
            Func<List<TaggedToken>, List<string>,  List<List<TaggedToken>>> ret = (t, p) => {
                List<List<TaggedToken>> list = new List<List<TaggedToken>>();
                foreach (var tag1 in t.Where(x => x.Tags.Contains(tagname1))) {
                    foreach (var tag2 in t.Where(x => x.Tags.Contains(tagname2))) {
                        list.Add(new List<TaggedToken>(2) { tag1, tag2 });
                    }
                }
                return list;
            };
            return ret;
        }

        public static Func<List<TaggedToken>, List<string>, List<List<TaggedToken>>> RegexExtraction(string pattern) {
            Func<List<TaggedToken>, List<string>, List<List<TaggedToken>>> ret = (t, ps) => {
                List<List<TaggedToken>> list = new List<List<TaggedToken>>();

                foreach (var p in ps) {
                    var matches = Regex.Matches(p, pattern);

                    foreach (Match match in matches) {
                        List<TaggedToken> set = new List<TaggedToken>(match.Length);
                        for (int i = match.Index; i < match.Index + match.Length; i++) {
                            set.Add(t[i]);
                        }
                        list.Add(set);
                    }
                }
                return list;
            };
            return ret; // delegate
        }

        public static Func<List<TaggedToken>, List<string>, List<List<TaggedToken>>> GatherSets(string tagname1, string tagname2) {
            Func<List<TaggedToken>, List<string>, List<List<TaggedToken>>> ret = (t,ps) => {
                List<List<TaggedToken>> list = new List<List<TaggedToken>>();

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
                            list.Add(set);
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
                            list.Add(new List<TaggedToken>(2) { tag1, tag2 });
                        }
                    }    
                }

                return list;
            };
            return ret;
        }


    }

    public class Mixer {

        private List<TaggedToken> _tokens;
        private Tagger _tagger;


        public Mixer(string input, List<LexiconEntry> lexicon) : this(input, lexicon, new StringMixOptions()) { }

        public Mixer(string input, List<LexiconEntry> lexicon, StringMixOptions options) : this(input, new Tagger(lexicon, options) ) { }
        
        public Mixer(string input, Tagger tagger): this(tagger.Tag(input)) {
            _tagger = tagger;
        }

        public Mixer(List<LexiconEntry> lexicon) : this(lexicon, new StringMixOptions()) { }

        public Mixer(List<LexiconEntry> lexicon, StringMixOptions options) : this(new Tagger(lexicon, options)) { }
        

        public Mixer(Tagger tagger) {
            _tagger = tagger;
        }


        public Mixer(List<TaggedToken> tokens) {
            _tokens = tokens;
        }

        public void Tag(string input) {
            _tokens = _tagger.Tag(input);
        }

        public List<string> CombineAll(string tagname1, string tagname2, string separator) {
            List<string> ret = new List<string>();

            foreach (var tag1 in _tokens.Where(x=> x.Tags.Contains(tagname1))) {
                foreach (var tag2 in _tokens.Where(x => x.Tags.Contains(tagname2) )) {
                    ret.Add(String.Join(separator, tag1.Value, tag2.Value));
                }
            }
            return ret;
        }

        // Method: With
            // Takes: string for input
            // Returns: Object onto which to call When() with the state of patterns and tokens made from the input string.
        public WithAnchor With(string input) {
            WithAnchor ret = new WithAnchor();

            ret.Tokens = _tokens = _tagger.Tag(input);
            ret.Patterns = PatternMaker.MakePatterns(_tokens);

            return ret;
        }

        

  


    }
}
