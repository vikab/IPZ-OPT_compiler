using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ipz_opt
{
    class Program
    {
        static void Main(string[] args)
        {
            const string filename = "test.txt";
            Lexer lexer = new Lexer();
            if (!File.Exists(filename))
            {
                Console.WriteLine("Doesn't exist");
                return;
            }
            var errorList = new List<Error>();
            var lexemTable = lexer.all_lexems(filename, errorList);

            Tab.PrintDict(Tab.idToLexem);
            Console.WriteLine("\n ============Lexical");
            int index = 0, currentline = lexemTable[0].position.line;
            Console.Write("Line {0}:", currentline);
            while (index < lexemTable.Count)
            {
                if (currentline == lexemTable[index].position.line)
                {
                    Console.Write("{0} ", lexemTable[index].id);
                }
                else
                {
                    currentline++;
                    Console.Write("\nLine {0}: {1} ", currentline, lexemTable[index].id);
                }
                index++;
            }

            //for (int i = 0; i < lexemTable.Count - 1; i++)
            //{
            //    if (lexemTable[i].position.line == lexemTable[i + 1].position.line)
            //        Console.Write(" At line {1}  {0} ", lexemTable[i].id, lexemTable[i].position.line);
            //}

            Console.WriteLine("\nErrors:");
            if (errorList.Count == 0)
            {
                Console.WriteLine("No lexical errors!");
            }
            else
            {
                for (int i = 0; i < errorList.Count; i++)
                {
                    Console.WriteLine("Error:{0} at line:{1}, at column:{2}", errorList[i].errMessage, errorList[i].position.line,
                        errorList[i].position.column);
                }
            }

            Console.WriteLine("\n ============Syntaxer");

            var syntaxErr = new List<Error>();
            Syntaxer parser = new Syntaxer();

            var tree = parser.createSyntaxTree(lexemTable, syntaxErr);
            if (syntaxErr.Count == 0)
            {
                Console.WriteLine("No syntax errors!");
                ASM codeGen = new ASM();
                string asmFile = codeGen.createAsmFile(tree, errorList, parser.labels, parser.isproc);
                using (var wrighter = File.CreateText("asm_code.asm"))
                {
                    wrighter.WriteLine(asmFile);
                }
                if (errorList.Count == 0)
                {
                    Console.WriteLine("No symantic errors!");
                }
                else
                {
                    foreach (var item in errorList)
                    {
                        Console.WriteLine("{0}", item.errMessage);
                    }
                }
            }
            else
            {
                Console.WriteLine("Error:{0} at line:{1}, at column:{2}", syntaxErr[0].errMessage, syntaxErr[0].position.line,
                        syntaxErr[0].position.column);
            }

            string syntaxTreeString = tree.ToString();
            Console.WriteLine(syntaxTreeString);
            StringBuilder finalSyntaxTree = new StringBuilder();
            int tabCount = 0;
            for (int i = 0; i < syntaxTreeString.Length; i++)
            {
                if (syntaxTreeString[i] == '(')
                {
                    tabCount++;
                    finalSyntaxTree.Append('\n');
                    for (int j = 0; j < tabCount; j++) finalSyntaxTree.Append('\t');
                }
                else if (syntaxTreeString[i] == ')') tabCount--;
                finalSyntaxTree.Append(syntaxTreeString[i]);
            }
            using (var wrighter = File.CreateText("tree.txt"))
            {
                wrighter.WriteLine(finalSyntaxTree.ToString());
            }




        }
    }
}

