using StringMix.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringMix.Internal
{
    public interface IMixer
    {
        MixSet Mix(MatchSet matches);
    }

    public class RegexExtractionMixer : IMixer
    {

        public string Expression { get; set; }

        public MixSet Mix(MatchSet matches)
        {
            MixSet list = new MixSet();

            foreach (var pattern in matches.MatchedPatterns)
            {
                // Get the matches that occur in the current pattern getting examined
                var matchesOfPattern = Regex.Matches(pattern.PatternText, Expression);

                // for each of those Regexes matched
                foreach (Match match in matchesOfPattern)
                {
                    // Create a Set -- this will be the kernel of a new mix
                    List<TaggedToken> set = new List<TaggedToken>(match.Length);

                    // walk the length of the regex match, adding the corresponding token 
                    // found that that location
                    for (int i = match.Index; i < match.Index + match.Length; i++)
                    {
                        set.Add(matches.Tokens[i]);
                    }

                    // Collect the mix into the set that will be returned.
                    list.Mixes.Add(new Mix(set));
                }
            }
            return list;
        }
    }
}
