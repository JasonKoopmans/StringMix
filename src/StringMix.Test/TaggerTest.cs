﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringMix.Model;
using System.Collections.Generic;
using StringMix.Internal;

namespace StringMix.Test {
    [TestClass]


    public class Name {
        public string First;
        public string Middle;
        public string Last;
    }
    
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
            List<string> patterns = PatternMaker.MakePatterns(tokens);

            // Do Patterns Match?!
            Assert.IsTrue(patterns.Contains("F?FL"));

        }

        [TestMethod]
        public void ItsMyFirstTimeWithAMixer() {
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

            MixPipeline pipe = new MixPipeline("Fred and Wilma Flintstone", lex);

            List<string> combos = pipe.CombineAll("F", "L", " "); 
            // Fred Flintstone
            // Wilma Flintstone

            Assert.AreEqual(2, combos.Count);
            Assert.AreEqual("Fred Flintstone", combos[0]);
            Assert.AreEqual("Wilma Flintstone", combos[1]);


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

        [TestMethod]
        public void BasicMixerUse() {
            MixPipeline pipe = new MixPipeline("Fred and Wilma Flintstone", GetBasicTagger(GetBasicNameLex()) );
            List<string> names = pipe.CombineAll("F", "L", " ");

            Assert.AreEqual(2, names.Count);
            Assert.AreEqual("Fred Flintstone", names[0]);
            Assert.AreEqual("Wilma Flintstone", names[1]);

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
        public void Basic_Mixer_RegexWhen_RegexExtractor() {
            MixPipeline pipe = new MixPipeline(GetBasicTagger(GetBasicNameLex()));

            List<Mix> list = pipe.Process("Fred and Wilma Flintstone")
                .Match(MatchCriteria.RegexCriteria("F?FL"))
                .Mix(MixActions.RegexExtraction("FL")).Mixes;

            

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Wilma", list[0].Tokens[0].Value);
        }

        [TestMethod]
        public void Basic_Mixer_RegexMatch_RegexExtractor_Simple() {
            MixPipeline pipe = new MixPipeline(GetBasicTagger(GetBasicNameLex()));

            List<Mix> list = pipe.Process("Fred Flintstone Wilma Flintstone")
                .Match(MatchCriteria.RegexCriteria("FLFL"))
                .Mix(MixActions.RegexExtraction("FL")).Mixes;



            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Fred", list[0].Tokens[0].Value);
            Assert.AreEqual("Wilma", list[1].Tokens[0].Value);
        }

        [TestMethod]
        public void Basic_Mixer_RegexMatch_RegexExtractor_Multiple() {
            MixPipeline pipe = new MixPipeline(GetBasicTagger(GetBasicNameLex()));

            List<Mix> list = pipe.Process("Fred Franklin Flintstone Wilma Jean Flintstone")
                .Match(MatchCriteria.RegexCriteria("FMLFML"))
                .Mix(MixActions.RegexExtraction("FML")).Mixes;



            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Fred", list[0].Tokens[0].Value);
            Assert.AreEqual("Wilma", list[1].Tokens[0].Value);
        }

        [TestMethod]
        public void TranslateToNameObject_ListToList() {
            
            // Setup the pipeline
            MixPipeline pipe = new MixPipeline(GetBasicTagger(GetBasicNameLex()));
            
            // Process this string
            List<Name> list = pipe.Process("Fred Franklin Flintstone Wilma Jean Flintstone")
                
                // Match Patterns where FML exists in the list of tokens, make mixes of all of the FML matches
                // This single string would contain 2 FML sets
                .RegexMatchAndMix("FML")
                
                // With the Mixes, translate them to Names.  t's type is List<Mix>
                .Translate<List<Name>>(t => {
                    
                    // collection type to capture tranlations
                    List<Name> names = new List<Name>();

                    // visit each mix, creating a new Name for each one.
                    foreach (var mix in t) {
                        var name = new Name();
                        name.First = mix.Tokens[0].Value;
                        name.Middle = mix.Tokens[1].Value;
                        name.Last = mix.Tokens[2].Value;
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

            Assert.AreEqual("Wilma", list[0].First);
            Assert.AreEqual("Jean", list[0].Middle);
            Assert.AreEqual("Flintstone", list[0].Last);

        }

        [TestMethod]
        public void TranslateToNameObject_OneToOne() {

            // Setup the pipeline
            MixPipeline pipe = new MixPipeline(GetBasicTagger(GetBasicNameLex()));

            // Process this string
            List<Name> list = pipe.Process("Fred Franklin Flintstone Wilma Jean Flintstone")

                // Match Patterns where FML exists in the list of tokens, make mixes of all of the FML matches
                // This single string would contain 2 FML sets
                .RegexMatchAndMix("FML")

                // define a translation from a single mix to a single name.  The framework worries about accumulating
                .Translate<Name>(mix => {
                    
                    var name = new Name();
                    name.First = mix.Tokens[0].Value;
                    name.Middle = mix.Tokens[1].Value;
                    name.Last = mix.Tokens[2].Value;
                    return name;
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

            Assert.AreEqual("Wilma", list[0].First);
            Assert.AreEqual("Jean", list[0].Middle);
            Assert.AreEqual("Flintstone", list[0].Last);

        }

        

    }
}
