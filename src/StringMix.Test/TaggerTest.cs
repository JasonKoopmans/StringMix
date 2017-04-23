using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringMix.Model;
using System.Collections.Generic;
using StringMix.Internal;

namespace StringMix.Test {
    


    public class Name {
        public string First;
        public string Middle;
        public string Last;
    }

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
                Value = "Franklin",
                Tags = new List<string> { "F", "M" }
            });
            
            ret.Add(new LexiconEntry() {
                Value = "Jean",
                Tags = new List<string> { "F", "M" }
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
        public void ItsMyFirstTime() {
            // Define some Lexicon
            List<LexiconEntry> lex = new List<LexiconEntry>();

            lex.Add(new LexiconEntry() {
                Value = "Fred",
                Tags = new List<string> { "F" } // For FirstName
            });

            lex.Add(new LexiconEntry() {
                Value = "Wilma",
                Tags = new List<string> { "F" } // For FirstName
            });

            lex.Add(new LexiconEntry() {
                Value = "Flintstone",
                Tags = new List<string> { "L" } // For LastName
            });

            // New Up a Tagger
            Tagger tagger = new Tagger(lex);

            List<TaggedToken> tokens = tagger.Tag("Fred and Wilma Flintstone");

            // Get Patterns from these tokens
            List<Pattern> patterns = PatternMaker.MakePatterns(tokens);

            // Do Patterns Match?!
            Assert.IsTrue(patterns.Exists(p => p.PatternText == "F?FL"));

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
                        
            List<Pattern> patterns = PatternMaker.MakePatterns(tokens);

            
            Assert.IsTrue(patterns.Exists(p => p.PatternText.Equals("FLFLFLF")));
            Assert.IsTrue(patterns.Exists(p => p.PatternText.Equals("FLFLFLL")));

            Assert.AreEqual(2, patterns.Count);
            
        }

        [TestMethod]
        public void BasicMatchWithSpecialChars() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            t.Options.Separators.Add(";");
            var tokens = t.Tag("Fred; Flintstone");

            List<Pattern> patterns = PatternMaker.MakePatterns(tokens);
            
            Assert.IsTrue(patterns.Exists(p => p.PatternText.Equals("FL")));
            Assert.AreEqual(1, patterns.Count);

        }

        [TestMethod]
        public void BasicMatchWithSpecialCharsWithSpace() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            t.Options.Separators.Add(";");
            var tokens = t.Tag("Fred ; Flintstone");

            List<Pattern> patterns = PatternMaker.MakePatterns(tokens);

            Assert.IsTrue(patterns.Exists(p => p.PatternText.Equals("FL")));
            Assert.AreEqual(1, patterns.Count);

        }

        [TestMethod]
        public void BasicMatch_With_lowcase() {
            Tagger t = GetBasicTagger(GetBasicNameLex());
            var tokens = t.Tag("fred Flintstone");

            List<Pattern> patterns = PatternMaker.MakePatterns(tokens);

            Assert.IsTrue(patterns.Exists(p => p.PatternText.Equals("?L")));
            Assert.AreEqual(1, patterns.Count);

        }


        [TestMethod]
        public void BasicMatch_With_lowcase_proper() {
            
            Tagger t = GetBasicTagger(GetBasicNameLex(), new StringMixOptions() { MatchesAreCaseSensitive =false} );
            
            var tokens = t.Tag("fred Flintstone");

            List<Pattern> patterns = PatternMaker.MakePatterns(tokens);

            Assert.IsTrue(patterns.Exists(p => p.PatternText.Equals("FL")));
            Assert.AreEqual(1, patterns.Count);

        }


        [TestMethod]
        public void Overlap_In_Lex_Tags() {
            List<LexiconEntry> lex = new List<LexiconEntry>();

            lex.Add(new LexiconEntry() { Value="Landry", Tags=new List<string>() { "F" } });
            lex.Add(new LexiconEntry() { Value = "Landry", Tags = new List<string>() { "L" } });

            Tagger t = new Tagger(lex);
            var tokens = t.Tag("Landry");

            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(2, tokens[0].Tags.Count);

        }
        
        [TestMethod]
        public void TranslateToNameObject_ListToList() {
            
            // Process this string
            List<Name> list = "Fred Franklin Flintstone Wilma Jean Flintstone"

                // Tokenize the string, using the name lexicon and a default tokenizer
                .Tokenize(GetBasicNameLex())

                // Match Patterns where the sequence of "F", "M", "L" tags exist in the list of tokens,
                .Match("FMLFML")
                
                // Transform
                .Transform<List<Name>>( ms =>
                {
                    // collection type to capture tranlations
                    List<Name> names = new List<Name>();

                    // visit each mix, creating a new Name for each one.
                    foreach (var match in ms.MatchedPatterns)
                    {
                        var name = new Name();
                        name.First = ms.Tokens[0].Value;
                        name.Middle = ms.Tokens[1].Value;
                        name.Last = ms.Tokens[2].Value;
                        names.Add(name);

                        name = new Name();
                        name.First = ms.Tokens[3].Value;
                        name.Middle = ms.Tokens[4].Value;
                        name.Last = ms.Tokens[5].Value;
                        names.Add(name);
                    }
                    return names;
                });


            /* List = 
             * [0] => First:Fred,Middle:Franklin,Last:Flintstone
             * [1] => First:Wilma,Middle:Jean,Last:Flintstone
            */ 
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Fred", list[0].First);
            Assert.AreEqual("Franklin", list[0].Middle);
            Assert.AreEqual("Flintstone", list[0].Last);

            Assert.AreEqual("Wilma", list[1].First);
            Assert.AreEqual("Jean", list[1].Middle);
            Assert.AreEqual("Flintstone", list[1].Last);

        }
        

    }
}
