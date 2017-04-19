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

        public static MixSet Mix(this MatchSet matches, string RegexPattern )
        {
            IMixer mixer = new RegexExtractionMixer() { Expression = RegexPattern };
            return Mix(matches, mixer);
        }

        public static MixSet Mix(this MatchSet matches, IMixer mixer)
        {
            return mixer.Mix(matches);
        } 

        public static T Transform<T>(this MixSet Mixes, ITransformer<T> transformer) where T: new()
        {
            return transformer.Transform(Mixes);
        }

        public static T Transform<T>(this String str, List<LexiconEntry> lexicon, string MatchRegEx, string MixRegEx, ITransformer<T> transformer) where T : new()
        {
            return Tokenize(str, lexicon)
                .Match(MatchRegEx)
                .Mix(MixRegEx)
                .Transform<T>(transformer);
        }



        
    }
}
