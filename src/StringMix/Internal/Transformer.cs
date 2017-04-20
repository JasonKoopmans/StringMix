using StringMix.Model;
using System;
using System.Linq;

namespace StringMix.Internal
{
    /// <summary>
    /// using a Transformer callers can perform actions on MatchSets that convert the list of matched 
    /// patterns to a Generic type.  Callers of Transform method can either pass in an object that 
    /// implements a translator interface -or- a Func<MatchSet, T>
    /// </summary>
    public class Transformer {
        public Transformer() { }

        /// <summary>
        /// Transforms a MatchSet to an object of T type.  This offers a way for a caller to express
        /// how these lists should be translated and allow the framework to do the work of transformation.
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="transformer">a function that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Transform<T>(MatchSet set, Func<MatchSet, T> transformer) where T : new() {
            if (set.MatchedPatterns.Count() == 0) {
                return default(T);
            } else {
                return transformer.Invoke(set);
            }
        }

        /// <summary>
        /// An alternate to the Transform method that accepts a type for those callers that would 
        /// prefer to work with more formal types rather that function delegates
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="transformer">a class implementing ITransformer<T> that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Transform<T>(MatchSet set, ITransformer<T> transformer) where T : new() {
            return Transform(set, transformer.Transform);
        }
     
    }
}
