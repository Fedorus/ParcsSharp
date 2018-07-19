using Parcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcsSharp
{
    public class SimpleClass
    {
        public SimpleClass()
        {
            Console.WriteLine("Simple Class Constructor Called");
        }
        public void Method(PointInfo info)
        {
            Console.WriteLine("Method called");
        }
        public static void Method2(PointInfo info)
        {
            Console.WriteLine("Static Method called");
        }
    }
}
