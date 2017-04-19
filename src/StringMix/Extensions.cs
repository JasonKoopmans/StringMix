using StringMix.Internal;
using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix
{
    public static class Extensions
    {
        public static List<TaggedToken> Tokenize(this String str, List<LexiconEntry> lexicon)
        {
            return Tokenize(str, lexicon, new StringMixOptions());
        }

        public static List<TaggedToken> Tokenize(this String str, List<LexiconEntry> lexicon, StringMixOptions options) 
        {
            return Tokenize(str, new Tagger(lexicon, options));
        }

        public static List<TaggedToken> Tokenize(this String str, Tagger tagger)
        {
            return tagger.Tag(str);
        }

        public static MatchSet Match(this List<TaggedToken> tokens, string RegExPattern)
        {
            RegexMatcher matcher = new RegexMatcher() { Expression = RegExPattern };
            return Match(tokens, matcher);
        }

        public static MatchSet Match(this List<TaggedToken> tokens, IMatcher matcher)
        {
            return matcher.Match(tokens);
        }
 

        public static T Transform<T>(this MatchSet matches, Func<MatchSet, T> function)
        {
            if (matches.MatchedPatterns.Count() == 0)
            {
                return default(T);
            }

            return function.Invoke(matches);
        }

        public static T Transform<T>(this MatchSet matches, ITransformer<T> transformer) where T: new()
        {
            return transformer.Transform(matches);
        }

        public static T Transform<T>(this String str, List<LexiconEntry> lexicon, string MatchRegEx, ITransformer<T> transformer) where T : new()
        {
            return Tokenize(str, lexicon)
                .Match(MatchRegEx)
                .Transform<T>(transformer);
        }



        
    }
}
