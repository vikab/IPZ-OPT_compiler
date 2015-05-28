using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipz_opt
{
    public enum lexemType
    {
        Whitespace,
        Const,
        Identifier,
        ShortDelimiter,
        BegComment,
        Unacceptable
    }
    public static class Tab
    {
        public static IEnumerable<char> letters;
        public static IEnumerable<char> digits;
        public static IEnumerable<char> delimiters;
        public static IEnumerable<char> whitespaces;
        public static IEnumerable<string> keywords;
        public static lexemType[] attributes;
        public const string BegComment = "(*";
        public const string EndComment = "*)";
        public static IDictionary<int, string> idToLexem;
        public static IDictionary<string, int> lexemToId;
        public const int keywordStartIndex = 401;
        public const int constStartIndex = 501;
        public const int identifierStartIndex = 1001;
        public static int lastIdentifierId = identifierStartIndex;
        public static int lastConstId = constStartIndex;
        public static List<Lexem> symbols = new List<Lexem>();
        public static int? getIdByLexem(string lexem)
        {
            if (lexemToId.ContainsKey(lexem))
            {
                return lexemToId[lexem];
            }
            return null;
        }

        public static string getLexemById(int id)
        {
            if (idToLexem.ContainsKey(id))
            {
                return idToLexem[id];
            }
            return null;
        }

        public static int registerConstant(string lexem)
        {
            lexemToId.Add(lexem, lastConstId);
            idToLexem.Add(lastConstId, lexem);
            lastConstId++;
            return lastConstId;
        }

        public static int registerIdentifier(string lexem)
        {
            lexemToId.Add(lexem, lastIdentifierId);
            idToLexem.Add(lastIdentifierId, lexem);
            lastIdentifierId++;
            return lastIdentifierId;
        }

        public static lexemType CharType(char i)
        {
            if (letters.Contains(i))
            {
                return lexemType.Identifier;
            }
            if (digits.Contains(i))
            {
                return lexemType.Const;
            }
            if (whitespaces.Contains(i))
            {
                return lexemType.Whitespace;
            }
            if (delimiters.Contains(i))
            {
                return lexemType.ShortDelimiter;
            }
            if (i == BegComment[0])
            {
                return lexemType.BegComment;
            }
            return lexemType.Unacceptable;
        }

        private static void fillAttributes()
        {
            idToLexem = new SortedDictionary<int, string>();
            lexemToId = new SortedDictionary<string, int>();
            attributes = new lexemType[256];
            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i] = CharType((char)i);
                if (attributes[i] == lexemType.ShortDelimiter)
                {
                    lexemToId.Add(((char)i).ToString(), i);
                    idToLexem.Add(i, ((char)i).ToString());
                }
            }
            int index = keywordStartIndex;
            foreach (var keyword in keywords)
            {
                lexemToId.Add(keyword, index);
                idToLexem.Add(index, keyword);
                index++;
            }
        }

        private static IEnumerable<char> genEnumChar(char start, char finish)
        {
            return Enumerable.Range(start, (finish - start) + 1).Select(c => (char)c);
        }

        public static void PrintDict<T1, T2>(IDictionary<T1, T2> dict)
        {
            foreach (var p in dict)
            {
                Console.WriteLine("{0}:{1}", p.Key, p.Value);
            }
        }

        static Tab()
        {
            letters = genEnumChar('A', 'Z');
            digits = genEnumChar('0', '9');
            delimiters = new[]
            {
                ';','.','(',')',','
            };
            whitespaces = new[]
            {
                '\x9', //horizontal tab
                '\xA', //LF
                '\xB', //vertical tab
                '\xC', //new page
                '\xD', //CR
                '\x20' //space
            };
            keywords = new[]
            {
                "PROGRAM",
                "PROCEDURE",
                "BEGIN",
                "END",
                "LABEL" 
            };
            fillAttributes();
        }
    }

}

