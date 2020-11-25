using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Engine
{
    public class Filereader
    {
        const int dataPosition = 40;

        public static List<float> generate(string path)
        {
            List<float> data = new List<float>();
            byte[] length = new byte[4];
            BinaryReader bI = new BinaryReader(new FileStream(path, FileMode.Open));
            bI.BaseStream.Position = 34;
            int bps = bI.ReadInt16();
            bI.BaseStream.Position = dataPosition;

            while (bI.BaseStream.Position < bI.BaseStream.Length - 16)
            {
                int j = bI.ReadInt16();
                data.Add(j);
            }
            bI.Close();
            return data;
        }


        public static Bitmap getImg(int width, int height, List<float> list)
        {

            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawLine(new Pen(Color.Black, 2), new Point(0, height/2), new Point(width, height/2));

            int k = list.Count/width;

            for (int i = 0; i < list.Count; i += 20)
            {
                g.DrawLine(new Pen(Color.Green, 1), new Point(i / k, height / 2), new Point(i / k, height / 2 + (int)(list[i]) / 300));
            }
            return bitmap;
        }
    }
}