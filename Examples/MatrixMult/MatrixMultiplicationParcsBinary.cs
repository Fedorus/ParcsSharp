using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parcs;

namespace MatrixMult
{
    /*
    public class MatrixMultiplicationParcsBinary
    {
        public static async Task ParcsMatrixMultiply()
        {
            int N = Program.N;
            Matrix matrixA = new Matrix(N, N);
            matrixA.RandomFill(10);
            Matrix matrixB = new Matrix(N, N);
            matrixB.RandomFill(10);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            using (ControlSpace cs = new ControlSpace("MatrixMult"))
            {
                await cs.AddDirectoryAsync(Directory.GetCurrentDirectory());
                var point = await cs.CreatePointAsync();
                point.Serializer = new ParcsBinarySerializer();
                await point.RunAsync(new PointStartInfo(MatrixMultiplicationParcsBinary.MultStart));
                await point.SendAsync(matrixA);
                await point.SendAsync(matrixB);
                var matr = await point.GetAsync<Matrix>();
                //File.WriteAllText("Result.txt", matr.ToString());
                Console.WriteLine("Parcs Binary Result:");
                Console.WriteLine(sw.Elapsed);
            }
        }

        public static async Task MultStart(PointInfo info)
        {
            info.ParentPoint.Serializer = new ParcsBinarySerializer();
            var matrixA =
                await info.ParentPoint
                    .GetAsync<Matrix>(); //Matrix.Load(info.CurrentControlSpace.PointDirectory + "A.txt"); // 
            var matrixB =
                await info.ParentPoint
                    .GetAsync<Matrix>(); // Matrix.Load(info.CurrentControlSpace.PointDirectory + "B.txt");  //
            var points = new Point[8];
            var cs = info.CurrentControlSpace;
            for (int i = 0; i < 8; i++)
            {
                points[i] = await cs.CreatePointAsync("1");
                points[i].Serializer = new ParcsBinarySerializer();
                if (matrixA.Width / 2 > 40000000000)
                {
                    await points[i].RunAsync(new PointStartInfo(MultStart));
                }
                else
                {
                    await points[i].RunAsync(new PointStartInfo(MatrEnd));
                }
                
            }

            points[0].SendAsync(matrixA.SubMatrix(0, 0, matrixA.Width / 2, matrixA.Width / 2)); // A11
            points[0].SendAsync(matrixB.SubMatrix(0, 0, matrixB.Width / 2, matrixB.Width / 2)); // B11

            points[1].SendAsync(matrixA.SubMatrix(0, matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2)); //A12
            points[1].SendAsync(matrixB.SubMatrix(matrixB.Width / 2, 0, matrixB.Width / 2, matrixB.Width / 2)); //B21

            points[2].SendAsync(matrixA.SubMatrix(0, 0, matrixA.Width / 2, matrixA.Width / 2));
            points[2].SendAsync(matrixB.SubMatrix(0, matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2));

            points[3].SendAsync(matrixA.SubMatrix(0, matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2));
            points[3].SendAsync(matrixB.SubMatrix(matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2,
                matrixB.Width / 2));

            points[4].SendAsync(matrixA.SubMatrix(matrixA.Width / 2, 0, matrixA.Width / 2, matrixA.Width / 2));
            points[4].SendAsync(matrixB.SubMatrix(0, 0, matrixB.Width / 2, matrixB.Width / 2));

            points[5].SendAsync(matrixA.SubMatrix(matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2,
                matrixA.Width / 2));
            points[5].SendAsync(matrixB.SubMatrix(matrixB.Width / 2, 0, matrixB.Width / 2, matrixB.Width / 2));

            points[6].SendAsync(matrixA.SubMatrix(matrixA.Width / 2, 0, matrixA.Width / 2, matrixA.Width / 2));
            points[6].SendAsync(matrixB.SubMatrix(0, matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2));

            points[7].SendAsync(matrixA.SubMatrix(matrixA.Width / 2, matrixA.Width / 2, matrixA.Width / 2,
                matrixA.Width / 2));
            points[7].SendAsync(matrixB.SubMatrix(matrixB.Width / 2, matrixB.Width / 2, matrixB.Width / 2,
                matrixB.Width / 2));

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
            info.ParentPoint.Serializer = new ParcsBinarySerializer();
            var matrixA = await info.ParentPoint.GetAsync<Matrix>();
            var matrixB = await info.ParentPoint.GetAsync<Matrix>();

            matrixA.MultiplyBy(matrixB);
            await info.ParentPoint.SendAsync(matrixA);
        }
    }

    public class ParcsBinarySerializer : IParcsSerializer
    {
        public string Serialize<T>(T item)
        {
            var formatter = new BinaryFormatter();
            formatter.Binder = new Version1ToVersion2DeserializationBinder();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, item);;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public T Deserialize<T>(string item)
        {
            Console.WriteLine(item.Length);
            byte[] bytes = Convert.FromBase64String(item);
            Console.WriteLine(bytes.Length);
            var formatter = new BinaryFormatter();
            formatter.Binder = new Version1ToVersion2DeserializationBinder();
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return (T)formatter.Deserialize(stream);
            }
        }
    }
    sealed class Version1ToVersion2DeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;
            String assemVer1 = Assembly.GetExecutingAssembly().FullName;
            String typeVer1 = "Version1Type";
            if (assemblyName == assemVer1 && typeName == typeVer1)
            {

                typeName = "Version2Type";
            }
            // The following line of code returns the type.
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                typeName, assemblyName));

            return typeToDeserialize;
        }
    }*/
}
