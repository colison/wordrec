using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Engine
{
    class TXTreader
    {
        public string[,] txtRead(string i)
        {
            string[,] Words = new string[GetLine(i), 3];
            try
            {
                using (StreamReader sr = new StreamReader(i+".txt"))
                {
                    string line;
                    int m = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] wordInfo = line.Split('\t');
                        Words[m,0] = wordInfo[0];
                        Words[m, 1] = wordInfo[1];
                        Words[m, 2] = wordInfo[2];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("文件无法读取");
            }
            return Words;
        }

        public static int GetLine(String i)
        {
            int line = 0;
            StreamReader sr = new StreamReader(i + ".txt");
            while (sr.ReadLine() != null)
                line++;
            return line;

        }
    }
}
