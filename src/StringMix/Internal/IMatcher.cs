using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Internal
{
    /// <summary>
    /// Simple interface to define the behavior that a matcher should have.  Implementors 
    /// will be classes that provide specific strategies for determining if a set of tagged 
    /// tokens matches some criteria.  The suggested method for implementors to have this 
    /// criteria define is to add properties or public fields to the implementation class.
    /// For Reference see RegexMatcher.cs
    /// </summary>
    public interface IMatcher
    {
        MatchSet Match(List<TaggedToken> tokens);
    }
}
