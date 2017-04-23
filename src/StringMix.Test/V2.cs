using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringMix.Model;
using System.Collections.Generic;
using StringMix;
using StringMix.Internal;

namespace StringMix.Test
{
    public class NaiveNameTransformer : ITransformer<List<Name>>
    {
        public List<Name> Transform(MatchSet set)
        {
            var ret = new List<Name>();
            var name = new Name();

            name.First = set.Tokens[0].Value;
            name.Last = set.Tokens[1].Value;

            ret.Add(name);

            name = new Name();

            name.First = set.Tokens[2].Value;
            name.Last = set.Tokens[3].Value;

            ret.Add(name);

            return ret;
        }
    }

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
        public void TryItOutEmptyString()
        {
            var tokens = "".Tokenize(GetBasicNameLex());
            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count == 0);
        }

        [TestMethod]
        public void Match()
        {
            var tokens = "Fred and Wilma Flintstone".Tokenize(GetBasicNameLex());

            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count == 4);
            Assert.IsTrue(tokens[0].Tags.Contains("F"));
            Assert.IsTrue(tokens[2].Tags.Contains("F"));
            Assert.IsTrue(tokens[3].Tags.Contains("L"));

            string regex = @"^F&FL$";

            var matchset = tokens.Match(regex);

            Assert.IsNotNull(matchset);
            Assert.IsTrue(matchset.MatchedPatterns.Count == 1);


        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MatchRegexIsEmptyString()
        {
            var tokens = "Fred and Wilma Flintstone".Tokenize(GetBasicNameLex());

            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count == 4);
            Assert.IsTrue(tokens[0].Tags.Contains("F"));
            Assert.IsTrue(tokens[2].Tags.Contains("F"));
            Assert.IsTrue(tokens[3].Tags.Contains("L"));

            string regex = @"";

            var matchset = tokens.Match(regex);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MatchRegexIsNull()
        {
            var tokens = "Fred and Wilma Flintstone".Tokenize(GetBasicNameLex());

            Assert.IsNotNull(tokens);
            Assert.IsTrue(tokens.Count == 4);
            Assert.IsTrue(tokens[0].Tags.Contains("F"));
            Assert.IsTrue(tokens[2].Tags.Contains("F"));
            Assert.IsTrue(tokens[3].Tags.Contains("L"));

            string regex = null;

            var matchset = tokens.Match(regex);

        }

        [TestMethod]
        public void Transform()
        {
            List<Name> names= "Fred and Wilma Flintstone".Transform(GetBasicNameLex(), "^FLFL$", new NaiveNameTransformer());

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Count == 2);
            Assert.IsTrue(names[0].First == "Fred");
            Assert.IsTrue(names[1].First == "Wilma");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformLexIsNull()
        {
            List<Name> names = "Fred and Wilma Flintstone".Transform(null, "^FLFL$", new NaiveNameTransformer());

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Count == 2);
            Assert.IsTrue(names[0].First == "Fred");
            Assert.IsTrue(names[1].First == "Wilma");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformMatchRegExIsNull()
        {
            List<Name> names = "Fred and Wilma Flintstone".Transform(GetBasicNameLex(), null, new NaiveNameTransformer());

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Count == 2);
            Assert.IsTrue(names[0].First == "Fred");
            Assert.IsTrue(names[1].First == "Wilma");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformMatchRegExIsEmptyString()
        {
            List<Name> names = "Fred and Wilma Flintstone".Transform(GetBasicNameLex(), String.Empty, new NaiveNameTransformer());

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Count == 2);
            Assert.IsTrue(names[0].First == "Fred");
            Assert.IsTrue(names[1].First == "Wilma");

        }



    }
}
