using StringMix.Internal;
using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix
{
    /// <summary>
    /// These are the primary entry point into the API.  Extension methods allow for magical attachment
    /// of methods to exising structures without the need to subtype or use other means.  This technique was 
    /// adopted to eliminate special object ceremony to do something that just seemed natural to call a method on 
    /// and object instance.  
    /// 
    /// "Fred and Wilma Flintstone".Tokenize(lexicon, options);
    /// 
    /// feels way more natural than newing up a bunch of Pipeline and specialized library objects.  Callers still have
    /// to familiarize themselves with some elements of the library, but a good deal of the ceremony has been cut away.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// For a given string and some lexicon, provide a list of tagged tokens.
        /// </summary>
        /// <param name="str">
        /// the string that shall be operated upon
        /// </param>
        /// <param name="lexicon">
        /// See LexiconEntry
        /// <returns>
        /// A list of Tagged Tokens. Think of tokens as the terms seen in text that would
        /// be processed.  If the incoming string were "Fred Flintstone" there are two terms 
        /// --tokens--.  Using the lexicon, the library attaches tags --meaning-- to these terms.
        /// These meanings can be turned into sequences --Patterns-- that then can be further processed
        /// for matches.  
        /// </returns>
        public static List<TaggedToken> Tokenize(this String str, List<LexiconEntry> lexicon)
        {
            return Tokenize(str, lexicon, new StringMixOptions());
        }

        /// <summary>
        /// For a given string, lexicon and processing options, provide a list of tagged tokens.
        /// </summary>
        /// <param name="str">
        /// The string that shall be operated upon
        /// </param>
        /// <param name="lexicon">
        /// See LexiconEntry
        /// </param>
        /// <param name="options">
        /// See StringMixOptions: basically string processing options for delimiters, casing, etc
        /// </param>
        /// <returns>
        /// A list of Tagged Tokens. Think of tokens as the terms seen in text that would
        /// be processed.  If the incoming string were "Fred Flintstone" there are two terms 
        /// --tokens--.  Using the lexicon, the library attaches tags --meaning-- to these terms.
        /// These meanings can be turned into sequences --Patterns-- that then can be further processed
        /// for matches.  
        /// </returns>
        public static List<TaggedToken> Tokenize(this String str, List<LexiconEntry> lexicon, StringMixOptions options) 
        {
            return Tokenize(str, new Tagger(lexicon, options));
        }

        /// <summary>
        /// Given a string to operate on and a tagger, return a list of tagged tokens
        /// </summary>
        /// <param name="str">
        /// The string to be operated upon
        /// </param>
        /// <param name="tagger">
        /// See Tagger class, a library class that uses lexicon and option to product list
        /// of tagged tokens.  Useful in cases where a specific implementation of a tagger 
        /// might be needed.  The library provides a general purpose one, but consuming applications
        /// can easily extend that object to meet specific requirements.
        /// </param>
        /// <returns>
        /// List of tagged Tokens
        /// </returns>
        public static List<TaggedToken> Tokenize(this String str, Tagger tagger)
        {
            return tagger.Tag(str);
        }

        /// <summary>
        /// Given a list of tokens and a regex pattern, return a matchset using the RegExMatcher class
        /// </summary>
        /// <param name="tokens">
        /// A list of tokens that would come out of a call to .Tokenize()
        /// </param>
        /// <param name="RegExPattern">
        /// Just a string, but should represent a valid RegEx.  This expression will be tested
        /// against all of the different patterns of tokens that are present in the list of tokens.
        /// Because a token can be tagged with more than one meaning, a single set of tokens may produce
        /// more than one pattern
        /// </param>
        /// <returns>
        /// A MatchSet -- which is the original set of tokens along with a list of the patterns 
        /// that matched the criteria that was offered.  
        /// </returns>
        public static MatchSet Match(this List<TaggedToken> tokens, string RegExPattern)
        {
            RegexMatcher matcher = new RegexMatcher() { Expression = RegExPattern };
            return Match(tokens, matcher);
        }

        /// <summary>
        /// Given a list of tokens and an IMatcher implementation, return a MatchSet 
        /// </summary>
        /// <param name="tokens">
        /// List of tagged tokens that would come out of a call to .Tokenize()
        /// </param>
        /// <param name="matcher">
        /// An implementation of IMatcher whose purpose is to take in a list of 
        /// tagged tokens and return a matchset.  The library provides a general purpose
        /// regex matcher, but consuming applications could opt to implement their own
        /// purpose-build implementation or extend the in-build regex matcher
        /// </param>
        /// <returns>
        /// A MatchSet -- which is the original set of tokens along with a list of the patterns 
        /// that matched the criteria that was offered. 
        /// </returns>
        public static MatchSet Match(this List<TaggedToken> tokens, IMatcher matcher)
        {
            return matcher.Match(tokens);
        }
 
        /// <summary>
        /// Given a MatchSet and a function delegate, return an object of generic type
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object that should be returned by the call.  Need to match the return type 
        /// of the delegate
        /// </typeparam>
        /// <param name="matches">
        /// A matchset as returned from a call to .Match()
        /// </param>
        /// <param name="function">
        /// A function delegate that accepts a MatchSet and returns a defined generic type.  
        /// This is one way to provide a looser, inline technique for defining the conversion from 
        /// Matched Tokens to an object of type T
        /// </param>
        /// <returns></returns>
        public static T Transform<T>(this MatchSet matches, Func<MatchSet, T> function)
        {
            if (matches.MatchedPatterns.Count() == 0)
            {
                return default(T);
            }

            return function.Invoke(matches);
        }

        /// <summary>
        /// Given a matchset and an implementation of ITransformer<T>, return an object of type T
        /// </summary>
        /// <typeparam name="T">
        /// the type of the object that should be return by the call
        /// </typeparam>
        /// <param name="matches">
        /// A MatchSet as what might be returned from a call to .Match()
        /// </param>
        /// <param name="transformer">
        /// An implementation of ITransformer[T].  This technique is a more OO method for defining
        /// and extending behaviors that transform list of tokens and patterns to an object.
        /// </param>
        /// <returns>
        /// an instance of T
        /// </returns>
        public static T Transform<T>(this MatchSet matches, ITransformer<T> transformer) where T: new()
        {
            return transformer.Transform(matches);
        }

        /// <summary>
        /// A convenience method that performs the full chain of tokenize, tag, Match, and Transform 
        /// to a string object.  Its the equivilent to doing:
        /// 
        /// "Fred Flintstone".Tokenize(lexicon).Match("FL").Transform<Name>(NameTransformer);
        /// 
        /// </summary>
        /// <typeparam name="T">
        /// The type that should be returned by this call
        /// </typeparam>
        /// <param name="str">
        /// The string being operated upon
        /// </param>
        /// <param name="lexicon">
        /// See LexiconEntry 
        /// </param>
        /// <param name="MatchRegEx">
        /// The Regular Expression that will be tested against all of the patterns of the tokens in the 
        /// target string.  Can be thought of as the same as a call to .Match(tokens, [match criteria])
        /// </param>
        /// <param name="transformer">
        /// The ITransformer[T] implementation that will be used to convert the matchset to the target object type
        /// </param>
        /// <returns>
        /// an object of type T
        /// </returns>
        public static T Transform<T>(this String str, List<LexiconEntry> lexicon, string MatchRegEx, ITransformer<T> transformer) where T : new()
        {
            return Tokenize(str, lexicon)
                .Match(MatchRegEx)
                .Transform<T>(transformer);
        }



        
    }
}
