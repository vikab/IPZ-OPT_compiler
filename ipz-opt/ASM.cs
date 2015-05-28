using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipz_opt
{
    class ASM
    {
        private IDictionary<string, int> _identifiersWithConstants;
        public string createAsmFile(SyntaxTree tree, List<Error> errors, List<int> labels, bool isproc)
        {
            List<string> asmCode = new List<string>();
            programGenerate(tree.mainNode, asmCode, errors, labels, isproc);
            return string.Join("\n", asmCode);
        }

        private void programGenerate(Node programNode, List<string> asmCode, List<Error> errors, List<int> labels, bool isproc)
        {
            _identifiersWithConstants = new Dictionary<string, int>();
            int curIndex = Tab.identifierStartIndex + 1;
            string lexem = Tab.getLexemById(curIndex);
            while (lexem != null)
            {
                _identifiersWithConstants.Add(lexem, 0);
                curIndex++;
                lexem = Tab.getLexemById(curIndex);
            }
            asmCode.Add(".386");
            asmCode.Add("DATA SEGMENT USE16");
            foreach (int label in labels)
            {
                asmCode.Add(string.Format("@{0} label dword", Tab.getLexemById(label)));
            }
            asmCode.Add("DATA ENDS");
            asmCode.Add("CODE SEGMENT USE16");
            asmCode.Add("ASSUME CS:CODE, DS:DATA");
            asmCode.Add("BEGIN:");

            asmCode.Add("mov ax,data");
            asmCode.Add("mov ds,ax");

            if (isproc)
            {
                asmCode.Add(string.Format("{0} proc", Tab.getLexemById(1001)));
                asmCode.Add("nop");
                asmCode.Add("ret");
                asmCode.Add(string.Format("{0} END", Tab.getLexemById(1001)));
            }
            else asmCode.Add("nop");

            asmCode.Add("mov ax,4c00h");
            asmCode.Add("int 21h");
            asmCode.Add("CODE ENDS");
            asmCode.Add("END BEGIN");
        }
    }
}

