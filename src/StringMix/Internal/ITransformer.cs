using StringMix.Model;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Internal
{
    /// <summary>
    /// An interface that defines a method that needs to accept a List<Mix> (that 
    /// correspond to the patterns that were matched) that return a instance of type T. 
    /// </summary>
    /// <typeparam name="T">Generic Type that represents what a MatchSet should be transformed to</typeparam>
    public interface ITransformer<T> where T : new() {
        T Transform(MatchSet set);
    }
}
