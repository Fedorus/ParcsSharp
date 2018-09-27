using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF.Cheats
{
    public static class FileChecksum
    {
        public static string Calculate(byte[] file)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(file));
            }
        }
        public static string Calculate(Stream filestream)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(filestream));
            }
        }
        public static string Calculate(string fileFullName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileFullName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }
    }
}
