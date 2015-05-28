using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ipz_opt
{
    public class Lexer
    {
        public List<Lexem> all_lexems(string filepath, List<Error> errors)
        {
            string code;
            if (File.Exists(filepath))
            {
                code = File.ReadAllText(filepath);
            }
            else
            {
                throw new FileNotFoundException();
            }
            List<Lexem> lexems = new List<Lexem>();
            int i = 0;
            int lexemCode = 0;
            while (i < code.Length)
            {
                bool skip = false;
                char current = code[i];
                var attribute = Tab.attributes[current];
                var main_position = calc_position(code, i);
                bool is_comment = false;
                if (i + 1 < code.Length && code[i] == '(' && code[i + 1] == '*')
                {
                    is_comment = true;
                }
                if (i + 1 < code.Length && code[i] == '*' && code[i + 1] == ')')
                {
                    errors.Add(new Error { errMessage = "Error comment", position = main_position });
                }
                if (attribute == lexemType.Whitespace)
                {
                    i++;
                    skip = true;
                }
                else if (attribute == lexemType.Identifier)
                {
                    lexemCode = examineIdent(code, ref i);
                }
                else if (attribute == lexemType.Const)
                {
                    lexemCode = examineConst(code, ref i);
                }
                else if (attribute == lexemType.ShortDelimiter && !is_comment)
                {
                    lexemCode = examineSD(code[i]);
                    i++;
                }
                else if (is_comment)
                {
                    skip = true;
                    if (i < code.Length - 1)
                    {
                        i++;
                    }
                    if (code[i] != '*')
                    {
                        errors.Add(new Error { errMessage = "Unaceptable symbol", position = main_position });
                    }
                    else
                    {
                        i++;
                        while (i < code.Length)
                        {
                            if (code[i] == Tab.EndComment[1] && code[i - 1] == Tab.EndComment[0])
                            {
                                i++;
                                break;
                            }
                            i++;
                        }
                        if (i == code.Length)
                        {
                            errors.Add(new Error { errMessage = "Unclosed comment", position = main_position });
                        }
                    }
                }
                else //Unecaptable
                {
                    skip = true;
                    lexems.Add(new Lexem { id = 0, position = main_position });
                    errors.Add(new Error { errMessage = "Unaceptable symbol", position = main_position });
                    i++;
                }
                if (!skip)
                {
                    lexems.Add(new Lexem { id = lexemCode, position = main_position });
                }
            }
            return lexems;
        }

        private int examineIdent(string code, ref int i)
        {
            StringBuilder temp = new StringBuilder();

            while (i < code.Length && (Tab.attributes[code[i]] == lexemType.Identifier
                || Tab.attributes[code[i]] == lexemType.Const) && code[i] < Tab.attributes.Length)
            {
                temp.Append(code[i]);
                i++;
            }

            String ident = temp.ToString();
            if (Tab.getIdByLexem(ident) == null)
            {
                return Tab.registerIdentifier(ident) - 1;
            }
            return (int)Tab.getIdByLexem(ident);
        }

        private int examineConst(string code, ref int i)
        {
            StringBuilder temp = new StringBuilder();

            while (i < code.Length && Tab.attributes[code[i]] == lexemType.Const
                && code[i] < Tab.attributes.Length)
            {
                temp.Append(code[i]);
                i++;
            }

            String constant = temp.ToString();
            if (Tab.getIdByLexem(constant) == null)
            {
                return Tab.registerConstant(constant) - 1;
            }
            return (int)Tab.getIdByLexem(constant);
        }

        private int examineSD(char i)
        {
            return (int)Tab.getIdByLexem(i.ToString());
        }

        private Position calc_position(string code, int currentPosition)
        {
            int _line = 0;
            int _column = 0;
            for (int index = 0; index < currentPosition; index++)
            {
                _column++;
                if (code[index] == '\n')
                {
                    _column = 0;
                    _line++;
                }
            }
            return new Position
            {
                line = _line + 1,
                column = _column + 1
            };
        }
    }
}
