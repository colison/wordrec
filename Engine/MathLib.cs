using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.IntegralTransforms;

namespace Engine
{
    public class MathLib
    {


        public static float CrossCorrelation(List<float> Wave1, List<float> Wave2, int iT)
        {
            float outVal = 0;

            int len = Math.Min(Wave1.Count, Wave2.Count);
            for (int i = 0; i < len; i += 10)
            {
                if (i + iT >= 0 && i + iT < Wave2.Count)
                    outVal += Wave1[i] * Wave2[i + iT];
            }
            return outVal;
        }

        public static float maxCroCor(List<float> Wave1, List<float> Wave2)
        {
            int len = Math.Min(Wave1.Count, Wave2.Count);
            float max = 0;
            float num;
            for (int i = -len + 1; i < len; i++)
            {
                num = CrossCorrelation(Wave1, Wave2, i);
                if (num > max)
                    max = num;
            }
            return max;
        }

        public static float ShapeBaseDis(List<float> Wave1, List<float> Wave2)           //K-shape 距离
        {
            float dis = 0;
            dis = (float)maxCroCor(Wave1, Wave2) / (WaveLen(Wave1, 10) * WaveLen(Wave2, 10));
            return dis;
        }

        public static float WaveLen(List<float> Wave, int interval)             //获取序列的模长
        {
            float len = 0;
            for (int i = 0; i < Wave.Count; i += interval)
            {
                len = len + (Wave[i] * Wave[i]);
            }
            len = (float)Math.Pow(len, 0.5);
            return len;
        }

        public static List<float> Normalization(List<float> Wave)     //归一化
        {
            List<float> NorWave = new List<float>();
            float avg = Wave.Average();

            float dev = (float)Wave.Sum(d => Math.Pow(d - avg, 2));

            dev = (float)Math.Sqrt(dev / Wave.Count);

            for (int i = 0; i < Wave.Count; i++)
            {
                NorWave.Add((Wave[i] - avg) / dev);
            }

            return NorWave;
        }

        public static int DetectBestOverlap(int iInterval, List<float> data1, List<float> data2)   //最佳对齐
        {
            float CrossCorr = 0, BestCrossCorr = 0;
            int bestI = 0;
            for (int i = -data1.Count; i < data1.Count; i++)
            {
                CrossCorr = MathLib.CrossCorrelation(data1, data2, i);
                if (CrossCorr > BestCrossCorr)
                {
                    BestCrossCorr = CrossCorr;
                    bestI = i;
                }
            }


            return bestI;

        }


        public static List<float> Trim(List<float> data, float iMaxVal)   //修剪有效波段
        {
            int oStartMarker, oEndMarker;
            Int32 pos = 0;
            bool mFound = false;
            for (pos = 0; pos < data.Count && !mFound; pos++)
            {
                if (Math.Abs(data[pos]) > iMaxVal) mFound = true;
            }
            oStartMarker = pos;

            mFound = false;
            for (pos = data.Count - 1; pos > 0 && !mFound; pos--)
            {
                if (Math.Abs(data[pos]) > iMaxVal) mFound = true;
            }
            oEndMarker = pos;

            List<float> NewWave = new List<float>();
            for (int i = oStartMarker; i < oEndMarker; i++)
                NewWave.Add(data[i]);
            return NewWave;

        }


        public static List<float> FFT(List<float> inData)
        {
            float[] outArr = new float[inData.Count];
            for (int i = 0; i < inData.Count; i++)
                outArr[i] = inData[i];
            MathNet.Numerics.Complex32[] mathNetComplexArr = new MathNet.Numerics.Complex32[inData.Count];
            for (int i = 0; i < mathNetComplexArr.Length; i++)
            {
                mathNetComplexArr[i] = new MathNet.Numerics.Complex32(outArr[i], 0);
            }
            Fourier.Forward(mathNetComplexArr);//傅里叶变换
            List<float> outData = new List<float>();
            for (int i = 0; i < inData.Count; i++)
            {
                outArr[i] = (float)Math.Pow((Math.Pow(mathNetComplexArr[i].Real, 2) + Math.Pow(mathNetComplexArr[i].Imaginary, 2)), 0.5);
                outData.Add(outArr[i]);
            }
            return outData;
        }

        public static List<float> expand(List<float> inData)
        {
            int i = (int)Math.Log(inData.Count, 2);
            int len = (int)Math.Pow(2, i + 1);
            List<float> outData = inData;
            for (int j = inData.Count; j < len; j++)
                outData.Add(0);
            return outData;
        }

        public static float FFTcompareScore(List<float> wave1, List<float> wave2)
        {
            List<float> data1 = expand(wave1);
            List<float> data2 = expand(wave2);
            List<float> data3 = FFT(data1);
            List<float> data4 = FFT(data2);

            return 10 * (float)Math.Pow(100 * ShapeBaseDis(data3, data4), 0.5);
        }
    }
}