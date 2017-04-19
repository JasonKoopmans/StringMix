using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Model
{
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
