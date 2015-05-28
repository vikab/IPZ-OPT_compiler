using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ipz_opt
{
    public class Syntaxer
    {
        public bool isproc = false;
        Node procParamListNode;
        Node declarations_listNode;

        public List<int> labels = new List<int>();
        public SyntaxTree createSyntaxTree(List<Lexem> lexems, List<Error> errors)
        {
            return new SyntaxTree { mainNode = Program(lexems, errors) };
        }

        private Node Program(List<Lexem> lexems, List<Error> errors)
        {
            if (lexems[0].id == Tab.getIdByLexem("PROGRAM"))
            {
                if (!isCorrectToken("PROGRAM", lexems, errors)) return null;
            }
            else if (lexems[0].id == Tab.getIdByLexem("PROCEDURE"))
            {
                isproc = true;
                if (!isCorrectToken("PROCEDURE", lexems, errors)) return null;
            }
            else
            {
                errors.Add(new Error
                {
                    errMessage = "PROCEDURE or PROGRAM extendet! ",
                    position = lexems[0].position
                });
                return null;
            }

            Node procIdentifierNode = procIdentifier(lexems, errors);
            if (isproc)
            {
                declarations_listNode = declarations_list(lexems, errors);
                procParamListNode = procParamList(lexems, errors);
            }

            if (!isCorrectToken(";", lexems, errors)) return null;

            Node declarationsNode = declarations(lexems, errors);

            Node blockNode = block(lexems, errors);

            if (blockNode != null)
            {
                if (isproc)
                {
                    if (!isCorrectToken(";", lexems, errors)) return null;
                    return new Node
                    {
                        type = "Procedure",
                        children = new List<Node> { procIdentifierNode, procParamListNode, declarationsNode, blockNode },
                        lexem = null
                    };
                }
                else
                {
                    if (!isCorrectToken(".", lexems, errors)) return null;
                    return new Node
                    {
                        type = "Program",
                        children = new List<Node> { procIdentifierNode, declarationsNode, blockNode },
                        lexem = null
                    };
                }
            }
            else
            {
                if (isproc)
                {
                    return new Node
                    {
                        type = "Procedure",
                        children = new List<Node> { procIdentifierNode, procParamListNode, declarationsNode },
                        lexem = null
                    };
                }
                else
                {
                    return new Node
                    {
                        type = "Program",
                        children = new List<Node> { procIdentifierNode, declarationsNode },
                        lexem = null
                    };
                }
            }

        }
        private bool isCorrectToken(string correctLexem, List<Lexem> lexems, List<Error> errors)
        {
            if (lexems[0].id != Tab.getIdByLexem(correctLexem))
            {
                errors.Add(new Error
                {
                    errMessage = string.Format("Expected: {0}, found {1}", correctLexem, Tab.getLexemById(lexems[0].id)),
                    position = lexems[0].position
                });
                return false;
            }

            lexems.RemoveAt(0);
            return true;
        }
        private Node declarations(List<Lexem> lexems, List<Error> errors)
        {
            if (Tab.getLexemById(lexems[0].id).Equals("BEGIN")) return null;
            Node declarationsNode = label_declarations(lexems, errors);
            if (!uniqueLabels(labels))
            {
                errors.Add(new Error
                {
                    errMessage = "The same label's identifier!",
                    position = lexems[0].position
                });
                return null;
            }
            if (!isCorrectToken(";", lexems, errors)) return null;

            return new Node
            {
                type = "Declarations",
                children = new List<Node> { declarationsNode },
                lexem = null
            };
        }
        private Node declarations_list(List<Lexem> lexems, List<Error> errors)
        {
            Lexem curLexem = new Lexem();
            return new Node
            {
                type = "Declarations-list empty",
                children = null,
                lexem = curLexem
            };
        }
        private Node procParamList(List<Lexem> lexems, List<Error> errors)
        {
            Lexem curLexem = lexems[0];
            Lexem curLexem1 = lexems[1];

            if (curLexem.id == 40)
            {
                lexems.RemoveAt(0);
                lexems.RemoveAt(0);
                if (curLexem1.id == 41)
                {
                    return new Node
                    {
                        type = "Parameters-list",
                        children = new List<Node> { declarations_listNode },
                        lexem = null
                    };
                }
                else
                {
                    errors.Add(new Error
                    {
                        errMessage = "Parameters-list is not emty!",
                        position = curLexem.position
                    });
                }
            }
            return null;
        }
        private Node procIdentifier(List<Lexem> lexems, List<Error> errors)
        {
            Lexem curLexem = lexems[0];
            lexems.RemoveAt(0);
            if (curLexem.id >= Tab.identifierStartIndex)
            {
                return new Node
                {
                    type = "Procedure Identifier",
                    children = null,
                    lexem = curLexem
                };
            }
            errors.Add(new Error
            {
                errMessage = "Identifier expected",
                position = curLexem.position
            });
            return null;
        }
        bool isEmpty(List<Lexem> lexems, List<Error> errors, Lexem curLexem)
        {
            if (lexems.Count == 0)
            {
                errors.Add(new Error { errMessage = "Synatax's elemen(s) expected", position = curLexem.position });
                return true;
            }
            return false;
        }
        private Node block(List<Lexem> lexems, List<Error> errors)
        {
            Lexem curLexem = lexems[0];
            if (!isCorrectToken("BEGIN", lexems, errors)) return null;
            if (isEmpty(lexems, errors, curLexem)) return null;
            Node stmntsListNode = statementsList(lexems, errors);
            if (!isCorrectToken("END", lexems, errors)) return null;
            if (!isEmpty(lexems, errors, curLexem))
                return new Node
                {
                    type = "Block",
                    children = new List<Node> { stmntsListNode },
                    lexem = null
                };
            else return null;
        }
        bool uniqueLabels(List<int> labels)
        {
            IEnumerable<int> distincLabels = labels.Distinct();
            int i = 0;
            foreach (int l in distincLabels) i++;
            if (i == labels.Count()) return true;
            else return false;
        }
        private Node label_declarations(List<Lexem> lexems, List<Error> errors)
        {
            if (Tab.getLexemById(lexems[0].id).Equals("BEGIN")) return null;
            if (!isCorrectToken("LABEL", lexems, errors)) return null;
            if (lexems[0].id == 59)
            {
                errors.Add(new Error
                {
                    errMessage = "UNSIGNED-INTERGER labels exented!",
                    position = lexems[0].position
                });
                return null;
            }
            Node labelDeclarationsNode = label_list(lexems, errors);
            return new Node
            {
                type = "Label-Declarations",
                children = new List<Node> { labelDeclarationsNode },
                lexem = null
            };
        }
        private Node label_list(List<Lexem> lexems, List<Error> errors)
        {
            Node labelDeclarationNode = label(lexems, errors);

            if (labelDeclarationNode == null)
            {
                return new Node
                {
                    type = "Labels-list",
                    children = null,
                    lexem = null
                };
            }
            Node labelDeclarationsListNode = label_list(lexems, errors);
            List<Node> childrenLabelDeclarationNodes = new List<Node> { labelDeclarationNode };
            if (labelDeclarationsListNode.children != null)
            {
                childrenLabelDeclarationNodes.AddRange(labelDeclarationsListNode.children);
            }

            return new Node
            {
                type = "Labels-list",
                children = childrenLabelDeclarationNodes,
                lexem = null
            };
        }
        private Node label(List<Lexem> lexems, List<Error> errors)
        {
            Lexem curLexem = lexems[0];

            if (curLexem.id >= Tab.constStartIndex && curLexem.id < Tab.identifierStartIndex)
            {
                lexems.RemoveAt(0);

                if (lexems[0].id != 44 && lexems[0].id != 59)
                {
                    errors.Add(new Error
                    {
                        errMessage = "Skipped delimeter ',' or ';' ",
                        position = curLexem.position
                    });
                    return null;
                }
                else if (lexems[0].id == 44 && lexems[1].id != 59)
                    if (!isCorrectToken(",", lexems, errors)) { lexems.RemoveAt(0); return null; }
                labels.Add(curLexem.id);
                return new Node
                {
                    type = "Label",
                    children = null,
                    lexem = curLexem
                };
            }

            return null;
        }
        private Node statementsList(List<Lexem> lexems, List<Error> errors)
        {
            Node statementNode = new Node();
            Lexem curLexem = lexems[0];

            statementNode = statement(lexems, errors, curLexem);
            if (statementNode == null)
            {
                errors.Add(new Error
                {
                    errMessage = "Statements-list is NOT EMPTY ",
                    position = curLexem.position
                });
                return null;
            }
            else
                return new Node
                {
                    type = "Statements-List",
                    children = new List<Node> { statementNode },
                    lexem = null
                };
        }
        private Node statement(List<Lexem> lexems, List<Error> errors, Lexem curLexem)
        {
            if (lexems[0].id == 0) return null;
            if (Tab.getLexemById(lexems[0].id).Equals("END")) return new Node();
            return null;
        }

    }
}
