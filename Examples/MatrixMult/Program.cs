using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMult
{
    public class Program
    {
        public static int N = 2048;
        static async Task Main(string[] args)
        {
            SingleThreadMatrixMult();
            SingleThreadMatrixMult();
            SingleThreadMatrixMult();
            SingleThreadMatrixMult();
            //await MatrixMultiplicationParcsBinary.ParcsMatrixMultiply();
            
            //await MatrixMultiplicationParcsJson.ParcsMatrixMultiply();
            Console.ReadKey();
        }
        
        public static void SingleThreadMatrixMult()
        {
            Matrix matrixA = new Matrix(N, N);
            matrixA.RandomFill(10);
            Matrix matrixB = new Matrix(N, N);
            matrixB.RandomFill(10);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            matrixA.MultiplyBy(matrixB);
            Console.WriteLine(sw.Elapsed);
            File.WriteAllText("RealResult.txt", matrixA.ToString());
            Console.WriteLine("Done");
            //Console.ReadKey();
        }
    }
}
