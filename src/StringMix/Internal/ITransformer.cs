using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Internal {
    /// <summary>
    /// An interface that defines a method that needs to accept a List<Mix> (that 
    /// correspond to the patterns that were matched) that return a instance of type T. 
    /// </summary>
    /// <typeparam name="T">Generic Type that represents what a Mix should be translated to</typeparam>
    public interface ITransformer<T> where T : new() {
        T Transform(MatchSet set);
    }

    /// <summary>
    /// using a Translator callers can perform actions on "Mixes" that convert the list of mixes to
    /// an object of type T.  Callers of Translate method can either pass in an object that 
    /// implements a translator interface -or- a Func<List<Mix>, T>
    /// </summary>
    public class Transformer {
        public Transformer() { }

        /// <summary>
        /// Translates a list of Mixes to an object of T type.  This offers a way for a caller to express
        /// how these lists should be translated and allow the framework to do the work of translation.
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="translator">a function that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Transform<T>(MatchSet set, Func<MatchSet, T> transformer) where T : new() {
            if (set.MatchedPatterns.Count() == 0) {
                return default(T);
            } else {
                return transformer.Invoke(set);
            }
        }

        /// <summary>
        /// An alternate to the Translate method that accepts a type for those callers that would 
        /// prefer to work with more formal types rather that function delegates
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="translator">a class implementing ITranslator<T> that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Transform<T>(MatchSet set, ITransformer<T> transformer) where T : new() {
            return Transform(set, transformer.Transform);
        }
     
    }
}
