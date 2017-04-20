using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Model
{
    /// <summary>
    /// A class to more specifically identify Patterns.  Prior to this, much of the API
    /// just passed around List<string> which in essence *is* what a pattern is.  It became 
    /// confusing since the libraries purpose is to work on string just when a method was returning
    /// patterns vs some other structure.  Using a defined pattern class was a way to make the code more
    /// readable, easier to approach, and use.  
    /// </summary>
    public class Pattern
    {
        public Pattern()
        {

        }
        public Pattern(string thePatternText)
        {
            this.PatternText = thePatternText;
        }

        public string PatternText { get; set; }
    }
}
