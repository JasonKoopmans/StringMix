using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringMix.Model;
using System.Collections.Generic;

namespace StringMix.Test {
    [TestClass]
    public class TaggerTest {
        
        /* 
         * Gets a Tagger configured to:
            - Separate with whitespace
            - Eliminate empty values
        */
        public Tagger GetBasicTagger(List<LexiconEntry> lexicon) {
            return new Tagger(lexicon);
        }

        public Tagger GetBasicTagger(List<LexiconEntry> lexicon, StringMixOptions options) {
            return new Tagger(lexicon, options);
        }

        public List<LexiconEntry> GetBasicNameLex() {
            List<LexiconEntry> ret = new List<LexiconEntry>();
            
            ret.Add(new LexiconEntry() {
                Value = "Fred",
                Tags = new List<string> {"F" }
            });

            ret.Add(new LexiconEntry() {
                Value = "Wilma",
                Tags = new List<string> { "F" }
            });

            ret.Add(new LexiconEntry() {
                Value = "Flintstone",
                Tags = new List<string> { "L" }
            });

            ret.Add(new LexiconEntry() {
                Value = "F",
                Tags = new List<string> { "M" }
            });

            ret.Add(new LexiconEntry() {
                Value = "Barney",
                Tags = new List<string> { "F" }
            });

            ret.Add(new LexiconEntry() {
                Value = "Rubble",
                Tags = new List<string> { "L" }
            });

            ret.Add(new LexiconEntry() {
                Value = "Landry",
                Tags = new List<string> { "L", "F" }
            });


            return ret;
        }


        
        [TestMethod]
        public void BasicFirstNameLastName() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            var tokens = t.Tag("Fred Flintstone");

            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(1, tokens[0].Tags.Count);
            Assert.AreEqual("F", tokens[0].Tags[0]);
            Assert.AreEqual(1, tokens[1].Tags.Count);
            Assert.AreEqual("L", tokens[1].Tags[0]);
        }

        [TestMethod]
        public void BasicFirstName_X2_LastName() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            var tokens = t.Tag("Fred and Wilma Flintstone");

            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual(1, tokens[0].Tags.Count);
            Assert.AreEqual("F", tokens[0].Tags[0]);
            Assert.AreEqual(1, tokens[2].Tags.Count);
            Assert.AreEqual("F", tokens[2].Tags[0]);
            Assert.AreEqual(1, tokens[3].Tags.Count);
            Assert.AreEqual("L", tokens[3].Tags[0]);
        }

        [TestMethod]
        public void BasicFirstName_DualClasses() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            var tokens = t.Tag("Landry");

            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(2, tokens[0].Tags.Count);
            Assert.AreEqual("F", tokens[0].Tags[1]);
            Assert.AreEqual("L", tokens[0].Tags[0]);
        }


        [TestMethod]
        public void BasicPatternGenerate() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            var tokens = t.Tag("Fred Flintstone Wilma Flintstone Barney Rubble Landry");
                        
            List<string> patterns = PatternMaker.MakePatterns(tokens);

            
            Assert.IsTrue(patterns.Contains("FLFLFLF"));
            Assert.IsTrue(patterns.Contains("FLFLFLL"));

            Assert.AreEqual(2, patterns.Count);
            
        }

        [TestMethod]
        public void BasicMatchWithSpecialChars() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            t.Options.Separators.Add(";");
            var tokens = t.Tag("Fred; Flintstone");

            List<string> patterns = PatternMaker.MakePatterns(tokens);
            
            Assert.IsTrue(patterns.Contains("FL"));
            Assert.AreEqual(1, patterns.Count);

        }

        [TestMethod]
        public void BasicMatchWithSpecialCharsWithSpace() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            t.Options.Separators.Add(";");
            var tokens = t.Tag("Fred ; Flintstone");

            List<string> patterns = PatternMaker.MakePatterns(tokens);

            Assert.IsTrue(patterns.Contains("FL"));
            Assert.AreEqual(1, patterns.Count);

        }

        [TestMethod]
        public void BasicMatch_With_lowcase() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            var tokens = t.Tag("fred Flintstone");

            List<string> patterns = PatternMaker.MakePatterns(tokens);

            Assert.IsTrue(patterns.Contains("?L"));
            Assert.AreEqual(1, patterns.Count);

        }


        [TestMethod]
        public void BasicMatch_With_lowcase_proper() {
            
            Tagger t = GetBasicTagger(GetBasicNameLex(), new StringMixOptions() { MatchesAreCaseSensitive =false} );
            
            var tokens = t.Tag("fred Flintstone");

            List<string> patterns = PatternMaker.MakePatterns(tokens);

            Assert.IsTrue(patterns.Contains("FL"));
            Assert.AreEqual(1, patterns.Count);

        }


    }
}
