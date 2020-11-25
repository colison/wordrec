using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace EStudio.Service
{
    public class WordRec
    {
        public static string word;

        public static System.Windows.Forms.Label label;
        public static void doRec(string path)
        {
            RecognizeFileAsync(path);
        }
        public static void RecognizeFileAsync(string path)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            try
            {
                var config = SpeechConfig.FromSubscription("c2da40e1d2364c86a12db793b3a5027d", "westus");
                var stopRecognition = new TaskCompletionSource<int>();
                string fullpath = Path.GetFullPath(path);

                //读取要识别的语音文件
                using (var audioInput = AudioConfig.FromWavFileInput(fullpath))
                {
                    //创建识别器对象
                    using (var recognizer = new SpeechRecognizer(config, audioInput))
                    {
                        // 识别中 （每识别一个词都会执行一次）
                        recognizer.Recognizing += (s, e) =>
                        {
                            Console.WriteLine($"识别中:{e.Result.Text}");
                        };
                        // 识别完成后 （整段语音识别完成后会执行一次）
                        recognizer.Recognized += (s, e) =>
                        {
                            if (e.Result.Reason == ResultReason.RecognizedSpeech)
                            {
                                Console.WriteLine($"识别完成: {e.Result.Text}");
                                word = e.Result.Text.Split('.')[0].ToLower();
                                Console.WriteLine(word);
                                //label.Text = word;


                            }
                            else if (e.Result.Reason == ResultReason.NoMatch)
                            {
                                Console.WriteLine($"没有识别到语音");

                            }
                        };
                        //识别取消时执行
                        recognizer.Canceled += (s, e) =>
                        {
                            Console.WriteLine($"取消识别: Reason={e.Reason}");

                            if (e.Reason == CancellationReason.Error)
                            {
                                Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                                Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                                Console.WriteLine($"CANCELED: Did you update the subscription info?");
                            }

                        };
                        //开始时执行
                        recognizer.SessionStarted += (s, e) =>
                        {
                            Console.WriteLine("\n   开始识别.");
                        };
                        //结束时执行
                        recognizer.SessionStopped += (s, e) =>
                        {
                            Console.WriteLine("\n    识别结束.");
                            stopRecognition.TrySetResult(0); //结束时添加一个异步任务
                        };

                        // 开始连续识别
                        recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                        //保证至少一个任务完成（等待到结束时间执行后再结束）
                        Task.WaitAny(new[] { stopRecognition.Task });

                        // 结束持续识别
                        recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {

                return;
            }

        }
    }
}

