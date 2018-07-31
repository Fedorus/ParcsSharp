using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parcs;

namespace MatrixMult
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            int N = int.Parse(Console.ReadLine());// 1024;
            Matrix matrixA = new Matrix(N, N);
            matrixA.RandomFill(10);
            Matrix matrixB = new Matrix(N, N);
            matrixB.RandomFill(10);
            Stopwatch sw = new Stopwatch();
            //File.WriteAllBytes("A.txt", matrixA.ToByteArray());
            //File.WriteAllBytes("B.txt", matrixB.ToByteArray());
            sw.Start();
            using (ControlSpace cs = new ControlSpace("MatrixMult"))
            {
                await cs.AddDirectoryAsync(Directory.GetCurrentDirectory());
                var point = await cs.CreatePointAsync();
                await point.RunAsync(new PointStartInfo(MatrixMultiplicationParcsMethod.MultStart));
                point.SendAsync(matrixA);
                point.SendAsync(matrixB);
               // Console.WriteLine(matrixA.ToString());
               // Console.WriteLine(matrixB.ToString());
                var matr = await point.GetAsync<Matrix>();
                File.WriteAllText("Result.txt", matr.ToString());


                Console.WriteLine("ParcsResult");
                Console.WriteLine(sw.Elapsed);
                Console.WriteLine("Ordinal result");
                sw.Restart();
                await Task.Factory.StartNew(()=> matrixA.MultiplyBy(matrixB));
                Console.WriteLine(sw.Elapsed);
                File.WriteAllText("RealResult.txt", matrixA.ToString());
                Console.WriteLine("Done");
                Console.ReadKey();
            }
        }
        public class MatrixMultiplicationParcsMethod
        {
            public static async Task MultStart(PointInfo info)
            {
                var matrixA = await info.ParentPoint.GetAsync<Matrix>(); //Matrix.Load(info.CurrentControlSpace.PointDirectory + "A.txt"); // 
                var matrixB = await info.ParentPoint.GetAsync<Matrix>();// Matrix.Load(info.CurrentControlSpace.PointDirectory + "B.txt");  //
                var points = new Point[8];
                var cs = info.CurrentControlSpace;
                for (int i = 0; i < 8; i++)
                {
                    points[i] = await cs.CreatePointAsync("1");
                    if (matrixA.Width / 2 > 100000000)
                    {
                        await points[i].RunAsync(new PointStartInfo(MultStart));
                    }
                    else
                    {
                        await points[i].RunAsync(new PointStartInfo(MatrEnd));
                    }
                }
                points[0].SendAsync(matrixA.SubMatrix(0, 0, matrixA.Width / 2, matrixA.Width / 2));// A11
                points[0].SendAsync(matrixB.SubMatrix(0, 0, matrixB.Width / 2, matrixB.Width / 2));// B11

                points[1].SendAsync(matrixA.SubMatrix(0, matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2));//A12
                points[1].SendAsync(matrixB.SubMatrix(matrixB.Width / 2, 0, matrixB.Width / 2, matrixB.Width / 2));//B21

                points[2].SendAsync(matrixA.SubMatrix(0, 0, matrixA.Width / 2, matrixA.Width / 2));
                points[2].SendAsync(matrixB.SubMatrix(0, matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2));

                points[3].SendAsync(matrixA.SubMatrix(0, matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2));
                points[3].SendAsync(matrixB.SubMatrix(matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2));

                points[4].SendAsync(matrixA.SubMatrix(matrixA.Width / 2, 0, matrixA.Width / 2, matrixA.Width / 2));
                points[4].SendAsync(matrixB.SubMatrix(0, 0, matrixB.Width / 2, matrixB.Width / 2));

                points[5].SendAsync(matrixA.SubMatrix(matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2));
                points[5].SendAsync(matrixB.SubMatrix(matrixB.Width / 2, 0, matrixB.Width / 2, matrixB.Width / 2));

                points[6].SendAsync(matrixA.SubMatrix(matrixA.Width / 2, 0, matrixA.Width / 2, matrixA.Width / 2));
                points[6].SendAsync(matrixB.SubMatrix(0, matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2));

                points[7].SendAsync(matrixA.SubMatrix(matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2));
                points[7].SendAsync(matrixB.SubMatrix(matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2));

                var resultMatrix = new Matrix(matrixA.Height, matrixA.Height);
                resultMatrix.SetSubmatrix(await SumMatrix(points[0], points[1]), 0, 0);
                resultMatrix.SetSubmatrix(await SumMatrix(points[2], points[3]), 0, matrixA.Width / 2);
                resultMatrix.SetSubmatrix(await SumMatrix(points[4], points[5]), matrixA.Width / 2, 0);
                resultMatrix.SetSubmatrix(await SumMatrix(points[6], points[7]), matrixA.Width / 2, matrixA.Width / 2);

                await info.ParentPoint.SendAsync(resultMatrix);
            }
            private static async Task<Matrix> SumMatrix(Point one, Point two)
            {
                var matrix = await one.GetAsync<Matrix>();
                matrix.Add(await two.GetAsync<Matrix>());
                return matrix;
            }
            public static async Task MatrEnd(PointInfo info)
            {
                var matrixA = await info.ParentPoint.GetAsync<Matrix>();
                var matrixB = await info.ParentPoint.GetAsync<Matrix>();

                matrixA.MultiplyBy(matrixB);
                await info.ParentPoint.SendAsync(matrixA);
            }
        }
    }
}
