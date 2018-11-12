using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using Venus.AI.ConsoleClient.Utils;
using Venus.AI.SDK.Components;

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

                /*
                /////////////DEBUG
                var config = JsonConvert.DeserializeObject<SDK.Components.Configurations.YandexCompmnentConfig>(File.ReadAllText(Environment.CurrentDirectory + "/appconfig.json"));
                YandexSttComponent yandexStt = new YandexSttComponent();
                var res = yandexStt.Process(new SDK.Components.Messages.VoiceMessage()
                {
                    Id = 1,
                    Language = SDK.Core.Enums.Language.Russian,
                    Vioce = inputMessage.VoiceData
                });

                Console.WriteLine(">" + res.Text);
                Console.ReadLine();
                /////////////
                */

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
                    var jsonRequest = Utils.JsonConverter.ToJson(inputMessage);
                    var jsonRespone = RestApiClient.Post(jsonRequest);
                    ApiRespone outputMessage = Utils.JsonConverter.FromJson<ApiRespone>(jsonRespone);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"\nuser   > ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{outputMessage.InputText}\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"venus.ai> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{outputMessage.OutputText}\n");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($"intent  : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"{outputMessage.IntentName}\n\n");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    File.WriteAllBytes("answer.wav", outputMessage.VoiceData);
                    SoundPlayer.Play("answer.wav");

                    if (outputMessage.IntentName == "personalize_intent")
                    {
                        RestApiClient.Сonfigure(@"http://192.168.88.150:50567/api/intent");
                        var jsonIntents = RestApiClient.Get();
                        var intents = JsonConvert.DeserializeObject<IntentList>(jsonIntents);
                        Console.WriteLine("Intents:\n#\t| Intent Name");
                        for (int i = 0; i< intents.Intents.Count; i++)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write($"{i}\t| ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine(intents.Intents[i]);
                        }
                        Console.Write("Enter Intent number: ");
                        var n = int.Parse(Console.ReadLine());

                        Console.Write($"Intent {intents.Intents[n]} chosen! Say your command. Press <Enter> to start recording.");
                        Console.ReadLine();
                        Recorder.StartRecord();
                        Console.Write("Recording... \nPress <Enter> to stop.");
                        Console.ReadLine();
                        Recorder.StopRecord();
                        Thread.Sleep(100);

                        IntentModificationRequest intentModificationRequest = new IntentModificationRequest()
                        {
                            Id = 1,
                            IntentName = intents.Intents[n],
                            Language = "rus",
                            VoiceData = System.IO.File.ReadAllBytes("demo.wav")
                        };

                        var jsonResponeIntent = RestApiClient.Post(JsonConvert.SerializeObject(intentModificationRequest));
                        var r = JsonConvert.DeserializeObject<IntentModificationRespone>(jsonResponeIntent);

                        File.WriteAllBytes("answer.wav", r.VoiceData);
                        SoundPlayer.Play("answer.wav");
                    }
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
