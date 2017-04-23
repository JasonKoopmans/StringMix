using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Model
{
    /// <summary>
    /// A MatchSet is the result of a call to IMatcher.Match(List<TaggedToken>).  It provides a list of the 
    /// tokens that were originally provided to the Matcher, and also a set of patterns that were matched according
    /// to the behavior in the Matcher that was called.  From a MatchSet, the list of patterns could be interesting 
    /// to inspect.  An extension method also exists so that holders of this object can Transform this object into 
    /// any other object by using an implementation of ITransformer<T>
    /// </summary>
    public class MatchSet
    {
        
    /// <summary>
    /// Tokens are the list of tokens that resulted from Tagging
    /// </summary>
    public List<TaggedToken> Tokens = new List<TaggedToken>();

    /// <summary>
    /// MatchedPatterns are the pattern values that matched according to the lexicon and the expression 
    /// offered to the Matcher
    /// </summary>
    public List<Pattern> MatchedPatterns = new List<Pattern>();
        
    
    }
}
