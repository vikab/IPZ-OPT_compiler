using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ipz_opt
{
    public class Node
    {
        public Lexem lexem { get; set; }
        public List<Node> children { get; set; }
        public string type { get; set; }
        public override string ToString()
        {
            if (children != null)
            {
                StringBuilder childStr = new StringBuilder();
                foreach (var item in children)
                {
                    childStr.AppendFormat("{0} ", item);
                }
                return string.Format("({0} {1})", type, childStr);
            }
            if (lexem != null)
            {
                return string.Format("({0} {1})", type, Tab.getLexemById(lexem.id));
            }
            else
            {
                return "";
            }
        }
    }
}
