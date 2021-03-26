using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMult
{
    [Serializable]
    public class Matrix
    {
        public float[,] mfMatrix { get; set; }
        public int mHeight { get; set; }
        public int mWidth { get; set; }

        public Matrix(String filename) { Load(filename); }
        [OnDeserializing]
        private void SetCountryRegionDefault(StreamingContext sc)
        {
            Console.WriteLine( sc.Context);
        }
        public Matrix(float[,] matr)
        {
            mfMatrix = matr;
            mHeight = mfMatrix.GetLength(0);
            mWidth = mfMatrix.GetLength(1);
        }

        public Matrix(int iHeight, int iWidth)
        {
            mHeight = iHeight;
            mWidth = iWidth;
            mfMatrix = new float[mHeight, mWidth];
        }
        private Matrix()
        { }
        public void SetItem(int y, int x, float fValue)
        {
            mfMatrix[y, x] = fValue;
        }

        public float Item(int y, int x)
        {
            return mfMatrix[y, x];
        }

        public Matrix SubMatrix(int iTop, int iLeft, int iHeight, int iWidth)
        {
            Matrix mSubMatrix;
            int iX, iY;
            if ((iTop >= 0) && (iLeft >= 0) && (iTop + iHeight <= mHeight) && (iLeft + iWidth <= mWidth))
            {
                mSubMatrix = new Matrix(iHeight, iWidth);
                for (iY = 0; iY < iHeight; iY++)
                {
                    for (iX = 0; iX < iWidth; iX++)
                    {
                        mSubMatrix.SetItem(iY, iX, mfMatrix[iTop + iY, iLeft + iX]);
                    }
                }
            }
            else
            {
                mSubMatrix = null;
            }
            return mSubMatrix;
        }
        public void SetSubmatrix(Matrix mat, int iTop, int iLeft)
        {
            for (int iY = 0; iY < mat.Height; iY++)
            {
                for (int iX = 0; iX < mat.mWidth; iX++)
                {
                    this.mfMatrix[iTop + iY, iLeft + iX] = mat.mfMatrix[iY, iX];
                }
            }
        }

        public int Height { get =>  mHeight; }

        public int Width { get => mWidth;}

        public void Add(Matrix mMatrix)
        {
            int iX, iY;
            if ((mMatrix.Height == this.Height) && (mMatrix.Width == this.Width))
            {
                for (iY = 0; iY < mHeight; iY++)
                {
                    for (iX = 0; iX < mWidth; iX++)
                    {
                        mfMatrix[iY, iX] = mfMatrix[iY, iX] + mMatrix.Item(iY, iX);
                    }
                }
            }
        }

        public void Assign(Matrix mMatrix)
        {
            int iY, iX;

            mHeight = mMatrix.Height;
            mWidth = mMatrix.Width;
            mfMatrix = new float[mHeight, mWidth];

            for (iY = 0; iY < mHeight; iY++)
            {
                for (iX = 0; iX < mWidth; iX++)
                {
                    mfMatrix[iY, iX] = mMatrix.Item(iY, iX);
                }
            }
        }

        public void MultiplyBy(Matrix mMatrix)
        {
            Matrix mResultMatrix;
            int newX, newY, iPos;
            if (this.Width == mMatrix.Height)
            {
                mResultMatrix = new Matrix(mHeight, mMatrix.Width);
                for (newY = 0; newY < mHeight; newY++)
                {
                    for (newX = 0; newX < mMatrix.Width; newX++)
                    {
                        mResultMatrix.SetItem(newY, newX, 0);
                        for (iPos = 0; iPos < mWidth; iPos++)
                        {
                            mResultMatrix.SetItem(newY, newX, mResultMatrix.Item(newY, newX) + mfMatrix[newY, iPos] * mMatrix.Item(iPos, newX));
                        }
                    }
                }
                this.Assign(mResultMatrix);
            }
        }

        public byte[] ToByteArray()
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (MemoryStream fStream = new MemoryStream())
            {
                binFormat.Serialize(fStream, this);
                return fStream.ToArray();
            }
        }
        public static Matrix FromByteArray(byte[] arrray)
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            using (MemoryStream fStream = new MemoryStream(arrray))
            {
                return (Matrix)binFormat.Deserialize(fStream);
            }
        }
        public void Save(string sFileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            using (Stream fStream = File.OpenWrite(sFileName))
            {
                binFormat.Serialize(fStream, this);
            }
        }

        public static Matrix Load(string sFileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            using (Stream fStream = File.OpenRead(sFileName))
            {
                return (Matrix)binFormat.Deserialize(fStream);
            }
        }

        public void RandomFill(int iMaxValue)
        {
            Random rand;
            int iY, iX;
            rand = new Random();
            for (iY = 0; iY < mHeight; iY++)
            {
                for (iX = 0; iX < mWidth; iX++)
                {
                    mfMatrix[iY, iX] = rand.Next(iMaxValue);
                }
            }
        }
        public void RandomFill(int iMinValue, int iMaxValue)
        {
            Random rand;
            int iY, iX;
            rand = new Random();
            for (iY = 0; iY < mHeight; iY++)
            {
                for (iX = 0; iX < mWidth; iX++)
                {
                    mfMatrix[iY, iX] = rand.Next(iMinValue, iMaxValue);
                }
            }
        }
        public void SystemOutput()
        {
            int iY, iX;

            for (iY = 0; iY < mHeight; iY++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                for (iX = 0; iX < mWidth; iX++)
                {
                    sb.Append(string.Format("{0,4} ", mfMatrix[iY, iX]));
                }
                sb.Append(']');
                Console.WriteLine(sb.ToString());
            }
        }
        public override string ToString()
        {
            int iY, iX;

            StringBuilder sb = new StringBuilder();
            for (iY = 0; iY < mHeight; iY++)
            {
                for (iX = 0; iX < mWidth; iX++)
                {
                    sb.Append(string.Format("{0,4} ", mfMatrix[iY, iX]));
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        public void FillSubMatrix(Matrix mSource, int iTop, int iLeft)
        {
            int iY, iX;
            if ((iTop + mSource.Height <= mHeight) && (iLeft + mSource.Width <= mWidth))
            {
                for (iY = 0; iY < mSource.Height; iY++)
                {
                    for (iX = 0; iX < mSource.Width; iX++)
                    {
                        mfMatrix[iTop + iY, iLeft + iX] = mSource.Item(iY, iX);
                    }
                }
            }
        }
    }
}
