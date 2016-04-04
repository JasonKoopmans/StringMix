using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMix.Model {
    /// <summary>
    /// Class to contain the details about a single element of lexicon.
    /// </summary>
    public class LexiconEntry {
        /// <summary>
        /// The value of the lexicon entry.  For instance, if the problem were to tag
        /// strings containing names, "Fred" could be one value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The tags or classes that this value could be associated with.  For some
        /// lexicon entries, jsut and single tag might apply.  For others, many might 
        /// apply.  For example, if the problem were to tag string containing names, 
        /// the value "Thomas" might be tagged as either a first or last name.  In this case,
        /// the tags list might contain {"F, "L"} to represent these tags.
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
