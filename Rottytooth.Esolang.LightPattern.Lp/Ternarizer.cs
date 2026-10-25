using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rottytooth.Esolang.LightPattern.Lp
{
    /// <summary>
    /// Converts binary file into ternary data
    /// </summary>
    public static class Ternarizer
    {

        public static string FromBinary(string filePath)
        {
            StringBuilder hexString = new StringBuilder();

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                byte[] currByte;
                while((currByte = reader.ReadBytes(1)).Length > 0)
                    hexString.AppendFormat("{0:x2}", currByte[0]);
            }

            string ternarized = BaseConverter.Convert(16, 3, hexString.ToString());

            StringBuilder formattedString = new StringBuilder();
            StringReader ternReader = new StringReader(ternarized);
            for (int index = 0; index < ternarized.Length - 2; index += 3)
            {
                char[] buffer = new char[3];
                ternReader.ReadBlock(buffer, 0, 3);
                formattedString.Append(buffer[0].ToString());
                formattedString.Append(buffer[1].ToString());
                formattedString.AppendLine(buffer[2].ToString());
            }

            return formattedString.ToString();
        }

        public static string FromString(string input)
        {
            throw new NotImplementedException();
        }

        public static string Deternarize(string filePath)
        {
            return null;
        }
    }
}
