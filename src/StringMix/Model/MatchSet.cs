using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Model
{
    /// <summary>
    /// Using Mixer callers can perform actions on sets of tokens that match a defined pattern.  
    /// In some cases this could be redundant to what happens in Match().  It does allow for cases
    /// where matching a larger string in Match() provides an opportunity to further refine or map
    /// the tagged tokens into a different list of Mixes.  
    /// </summary>
    public class MatchSet
    {
        
    /// <summary>
    /// Tokens are the list of tokens that resulted from Tagging
    /// </summary>
    public List<TaggedToken> Tokens = new List<TaggedToken>();

    /// <summary>
    /// MatchedPatterns are the pattern values that matched according to the lexicon in the Mixer (via Tagger)
    /// </summary>
    public List<Pattern> MatchedPatterns = new List<Pattern>();
        
    
    }
}
