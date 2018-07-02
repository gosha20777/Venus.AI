using System;
using System.IO;
using System.Threading;
using Venus.AI.ConsoleClient.Utils;

namespace Venus.AI.ConsoleClient
{
    class Program
    {
        private const bool SHOW_DEBUG_INFO = true;
        static void Main(string[] args)
        {
            Console.WriteLine("Venus.AI console client");
            Console.WriteLine("press ctrl+c to exit");

            while (true)
            {
                Console.Write("Press <Enter> to start recording.");
                Console.ReadLine();
                Recorder.StartRecord();
                Console.Write("Recording... \nPress <Enter> to stop.");
                Console.ReadLine();
                Recorder.StopRecord();
                Thread.Sleep(100);

                ApiRequest inputMessage = new ApiRequest();
                inputMessage.Language = "rus";
                inputMessage.RequestType = "voice";
                inputMessage.Id = 1;
                inputMessage.VoiceData = System.IO.File.ReadAllBytes("demo.wav");
                RestApiClient.Сonfigure(@"http://192.168.88.150:50567/api/request");
                string err;
                bool isConnect = RestApiClient.Connect(out err);

                if (!isConnect)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("[FAIL]\n\t{0}.", err);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("Press eny key to exit...");
                    Console.Read();
                    return;
                }

                try
                {
                    var jsonRequest = JsonConverter.ToJson(inputMessage);
                    var jsonRespone = RestApiClient.Post(jsonRequest);
                    ApiRespone outputMessage = JsonConverter.FromJson<ApiRespone>(jsonRespone);
                    Console.WriteLine(outputMessage.OuputText);

                    File.WriteAllBytes("answer.wav", outputMessage.VoiceData);
                    SoundPlayer.Play("answer.wav");
                    if (SHOW_DEBUG_INFO)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("REQUEST TO SERVER: ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("\n" + jsonRequest);

                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("RESPONE FROM SERVER: ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("\n" + jsonRespone);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
