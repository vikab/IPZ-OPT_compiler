using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ipz_opt
{
    public class SyntaxTree
    {
        public Node mainNode { get; set; }
        public override string ToString()
        {
            return string.Format("SyntaxTree: {0}", mainNode);
        }
    }
}
