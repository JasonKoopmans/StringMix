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
    public interface ITranslator<T> where T : new() {
        T Translate(List<Mix> mixes);
    }

    /// <summary>
    /// An interface that defines a method that needs to accept a single Mix
    /// and return an instance of type T. 
    /// </summary>
    /// <typeparam name="T">Generic type that represents what a Mix should be translated to </typeparam>
    public interface IOneToOneTranslator<T> where T : new() {
        T Translate(Mix mix);
    }

    /// <summary>
    /// using a Translator callers can perform actions on "Mixes" that convert the list of mixes to
    /// an object of type T.  Callers of Translate method can either pass in an object that 
    /// implements a translator interface -or- a Func<List<Mix>, T>
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
        /// An alternate to the Translate method that accepts a type for those callers that would 
        /// prefer to work with more formal types rather that function delegates
        /// </summary>
        /// <typeparam name="T">The type that the function should return as a result of its processing of the matched TokenList</typeparam>
        /// <param name="translator">a class implementing ITranslator<T> that performs the translation</param>
        /// <returns>An instance of T</returns>
        public T Translate<T>(ITranslator<T> translator) where T : new() {
            return Translate(translator.Translate);
        }

        /// <summary>
        /// Translates the list of Mixes.  The function delegate here allows expression of how a single mix maps to a single
        /// items of type T, rather than needed to maintian a list to gather the items that are new.
        /// </summary>
        /// <typeparam name="T">The type that each Mix should be translated to.</typeparam>
        /// <param name="translator">A function delegate that expresses how a mix should be translated to a T</param>
        /// <returns>A List of T.  There should be an equal number of T's for the number of Mixes </returns>
        public List<T> Translate<T>(Func<Mix, T> translator) where T : new() {
            List<T> ret = new List<T>();

            foreach (var mix in Mixes) {
                ret.Add(translator.Invoke(mix));
            }

            return ret;
        }

        /// <summary>
        /// Translates the list of Mixes.  An Alternate form of the Translate method that accepts a type for those callers that
        /// whould prefer to work with more formal types rather than function delegates.  Relieves the constructor of the Translator
        /// class from needing to create a list to accumulate results in.  Instead the Translator just needs to describe how to 
        /// beget a T from a mix.
        /// </summary>
        /// <typeparam name="T">the type that each Mix should be translated to</typeparam>
        /// <param name="translator">a class impletmenting IOneToOneTranslator<T> that performs the translation</param>
        /// <returns>A list of T</returns>
        public List<T> Translate<T>(IOneToOneTranslator<T> translator) where T: new() {
            return Translate(translator.Translate);
        }
        

    }
}
