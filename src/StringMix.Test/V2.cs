using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringMix.Model;
using System.Collections.Generic;
using StringMix;
using StringMix.Internal;

namespace StringMix.Test
{
    [TestClass]
    public class SomethingCompletelyDifferent
    {
        /* 
 * Gets a Tagger configured to:
    - Separate with whitespace
    - Eliminate empty values
*/
        public Tagger GetBasicTagger(List<LexiconEntry> lexicon)
        {
            return new Tagger(lexicon);
        }

        public Tagger GetBasicTagger(List<LexiconEntry> lexicon, StringMixOptions options)
        {
            return new Tagger(lexicon, options);
        }

        public List<LexiconEntry> GetBasicNameLex()
        {
            List<LexiconEntry> ret = new List<LexiconEntry>();

            ret.Add(new LexiconEntry()
            {
                Value = "Fred",
                Tags = new List<string> { "F" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "and",
                Tags = new List<string> { "&" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "Franklin",
                Tags = new List<string> { "F", "M" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "Jean",
                Tags = new List<string> { "F", "M" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "Wilma",
                Tags = new List<string> { "F" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "Flintstone",
                Tags = new List<string> { "L" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "F",
                Tags = new List<string> { "M" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "Barney",
                Tags = new List<string> { "F" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "Rubble",
                Tags = new List<string> { "L" }
            });

            ret.Add(new LexiconEntry()
            {
                Value = "Landry",
                Tags = new List<string> { "L", "F" }
            });


            return ret;
        }

        [TestMethod]
        public void TryItOut()
        {
            var tokens = "Fred".Tokenize(GetBasicNameLex());
            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count == 1);
            Assert.IsTrue(tokens[0].Tags.Contains("F"));
        }

        [TestMethod]
        public void Mix()
        {
            var tokens = "Fred and Wilma Flintstone".Tokenize(GetBasicNameLex());

            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count == 4);
            Assert.IsTrue(tokens[0].Tags.Contains("F"));
            Assert.IsTrue(tokens[2].Tags.Contains("F"));
            Assert.IsTrue(tokens[3].Tags.Contains("L"));

            string regex = @"^F&FL$";

            var mixset = tokens.Match(regex).Mix(regex);

            Assert.IsNotNull(mixset);
            Assert.IsTrue(mixset.Mixes.Count == 1);


        }
    }
}
