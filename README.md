# StringMix
String tokenizer,  part-of-string tagger, and transformation library for .net

---

### What:

This library performs the following feats

1. Simple Tokenizing
1. Given a set of definitions or **lexicon**; apply a category, meaning, or type
to each token using **tag** concepts
1. Summarize a list of tokens into **pattern**s that are concatinations of the tags
that are applied to a token
1. Allow for expressing match criteria that patterns should be tested against
1. For patterns that have matched, allow for transformation into another concrete .net
class using a **tranformer**
1. Provide a simple means to substitute key portions of the pipeline.  In cases where a more
specialized tokenizer is desired, that component can be created outside the library and injected in.


### Why:

To parse, catagorize, and transform relatively small strings.

Consider inputs like:

- Fred Flintstone
- Flintstone, Fred
- Fred and Wilma Flintstone
- Fred Flintstone;Wilma Flintstone
- Flintstone, Fred and Wilma
- Flintstone, Fred;Flintstone, Wilma

Handling these unstructured strings as proper names is tough as they are.
But if they were broken them up into word parts (tokenizing) and tags are applied to those
parts (categorizing) it gets much easier to process them, even turning them into another 
class.

### Terms:

- **Tokenizing**: The process of separating an input string into its component parts
- **Token**: In most tokenizing schemes (including the default for this library) its roughly analogous
to a word
- **`LexiconEntry`**: An identification of expected input and the tags that should be 
applied to it if found.  Ex: Fred, F [For First Name].  Each lexicon entry can 
define more than one tag that could apply.  In the case of processing names, "Thomas"
could be either a first or last name.  Both "F" and "L" tags could be applied.
- **Lexicon**: A collection of Lexicon Entries.  Together, describing all of the expected
values that tokens could be an what their meaning is/could be
- **Tag**: A placeholder for a token in a pattern, a simple descriptor for that 
token in the collection of tokens.  Determining what tags get applied to what tokens 
is controlled by the lexicon.  Once a string is tokenized, a PatternMaker component 
is responsible for walking over each of the tokens and attempting to match each of 
the tokens to an entry in the lexicon.  When a token matches, the tag(s) from the 
lexicon are applied to the patterns already identified.  Since lexicon entries can 
have more than one tag assigned, a single input string might have several different 
patterns that apply.
- **Pattern**: A sequence of tags that represent the meaning or type of the 
token represented in the original input string.  Patterns are a way to summarize 
this meaning and allow for tokens of a type or meaning to be selected, rearranged, or 
processed in a meaningful way
- **Patterns**: In cases where the lexicon contains entries that have more than one tag 
assigned, it is possible for a single input string to result in Lists of tokens that 
are summarized by more than one pattern.  Consider a lexicon that tags "Sarah" 
with "F" and "Thomas" with "F" and "L". Also consider the input of "Sarah Thomas"
The patterns that would summarize this input would be "FF" and "FL"


### Other concepts:

Identification is a first step, which is covered in the readme pretty well.  More advanced 
topics are covered in the [wiki](https://github.com/JasonKoopmans/StringMix/wiki)
 

### To consider:

- The tokenizer is a simple wrapper over `String.split()`.  `StringSplitOptions.Separators` 
which can be passed into the `.Tokenize()` method contains the characters that will be split by default.
This property can be overridden for desired results.
- Proper nouns like 'Eiffel Tower' using this library would yield two tokens. There's really 
no way currently to post process the tokens, apply rules to join them, and re-pattern.  
Initial focus on the libary is on a simpler single tokens.  Pull requests welcomed. 
- At present, it makes the most sense that tags are defined single characters.  The library could
be enhanced to use multicharacter tags to be more descriptive or to offer subclassing of
tags [ex: Verb-Transitive (VT)] that is common in some other AI-focused part of speech 
(POS) tagging.  Since Regex is the heart of the matching and extraction scheme
thoses efforts are much simpler with single character representations in patterns.  

### How:

The primary API for the library is accessible via extension methods that attach to the string class
as well as the other model classes contained within the library.  Several of the internal components
of the library are extendable and replacable with overloads of these extension methods.

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

        // To Tokenize:
        List<TaggedToken> tokens = "Fred Flintstone Wilma Flintstone".Tokenize(lexicon); 

        // Find Matches
        MatchSet matches = tokens.Match(@"^FLFL$");

        // Transform a matchset to an object (even collections of objects)
        List<Name> names1 = matches.Transform(new NameTransformer<List<Name>>() );
        
        /*
        : Results :
        names[0] = {First=Fred, Last=Flintstone};
        names[1] = {First=Wilma, Last=Flintstone}
        */

        // Transforming using a convenience method to Transform right from a string
        List<Name> names = "Fred Flintstone Wilma Flintstone".Transform(lexicon, "^FLFL$", new NameTransformer<List<Name>>()) ;

        /*
        : Results :
        names[0] = {First=Fred, Last=Flintstone};
        names[1] = {First=Wilma, Last=Flintstone}
        */


For other uses check out the [wiki](https://github.com/JasonKoopmans/StringMix/wiki)


