using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Internal {
    /// <summary>
    /// An interface that defines a method that needs to accept a List<Mix> (that 
    /// correspond to the patterns that were matched) that return a instance of type T.  Callers of
    /// Translate method can either pass in an object that implements this interface -or- a 
    /// Func<List<List<TaggedToken>>, T>
    /// </summary>
    /// <typeparam name="T">Generic Type that represents the type this class will return in its Translate method</typeparam>
    public interface ITranslator<T> where T : new() {
        T Translate(List<Mix> TokenListLists);
    }

    /// <summary>
    /// using a Translator callers can perform actions on "Mixes" that convert the list of mixes to
    /// an object of type T
    /// </summary>
    public class Translator {
        public Translator() { }

        /// <summary>
        /// The list of Mixes that resulted from a call to Mix().  This is the collection
        /// of items that will translated
        /// </summary>
        public List<Mix> Mixes = new List<Mix>();

        /// <summary>
        /// Translates a list of Mixes to an object of T type.  This offers a way for a caller to express
        /// how these lists should be translated and allow the framework to do the work of translation.
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="translator">a function that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Translate<T>(Func<List<Mix>, T> translator) where T : new() {
            if (Mixes.Count() == 0) {
                return default(T);
            } else {
                return translator.Invoke(Mixes);
            }
        }

        /// <summary>
        /// An alternate to the Translate method that accepts a Func<> for those callers that would 
        /// prefer to work with more permenant (concrete?) types rather that function delegates
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="translator">a class implementing ITranslator<T> that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Translate<T>(ITranslator<T> translator) where T : new() {
            return Translate(translator.Translate);
        }

    }
}
