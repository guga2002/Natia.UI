using NAudio.Wave;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Drawing;
using NAudio.CoreAudioApi;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using Natia.Core.Entities;
using System.Security.Cryptography;
using Natia.Neurall.Model;
using Natia.UI.Models;

namespace NatiaGuard.BrainStorm.Main
{
    public partial class Main
    {

        private async Task Heartbeat()
        {
            var res = await _natiaClient.HeartBeat();
        }

        private async Task<NeuralPredictionOutput> Predict(CheckAndPlayModel model)
        {
            _predict.TrainFromDatabase();
            var input = new NeuralInput
            {
                ChannelName = model.ChannelName,
                ErrorMessage = model.ErrorMessage,
                ErrorDetails = model.ErrorDetails,
                Satellite = model.Satellite,
            };
            return await _predict.Predict(input);
        }

        private SolutionRecommendationOutput Solution(CheckAndPlayModel model)
        {
            _Recomendation.TrainModelFromDatabase();
            var input = new SolutionRecommendationInput
            {
                ChannelName = model.ChannelName,
                ErrorMessage = model.ErrorMessage,
                ErrorDetails = model.ErrorDetails,
                Satellite = model.Satellite,
                Priority = model.Priority.ToString(),
            };
            return _Recomendation.Predict(input);
        }

        #region Grammer
        public string CorrectNameI(string Name)
        {
            var alpha = Name.Last();

            if (alpha == 'ა'|| alpha == 'ე'|| alpha == 'ი'|| alpha == 'უ'|| alpha == 'ო')
            {
                return Name;
            }
            return Name + "ი";
        }
        #endregion

        #region Randomizer
        private int RandomIndex(int rand)
        {
            Random random = new Random();
            return random.Next(0,rand);
        }

        #endregion

        #region MyRegion
        public async Task<string> GreetingNow(string time)
        {
            Random ran = new Random();

            var greetings = await _db.Greetings.Where(io => io.Category == time).ToListAsync();

            if (greetings.Any())
            {
                var index = ran.Next(0, greetings.Count); 
                return greetings[index].Text??"";
            }
            return "გისურვებთ ბედნიერ მორიგეობას";
        }

        #endregion

        #region Proccessor
        protected async Task<Info> MonitorPerformanceAsync()
        {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            _ = cpuCounter.NextValue();
            await Task.Delay(1000);

            double totalRamInMB = 8 * 1024;

            var availableRam = ramCounter.NextValue();
            double usedRamInMB = totalRamInMB - availableRam;

            double ramUsagePercent = (usedRamInMB / totalRamInMB) * 100;

            float cpuUsage = cpuCounter.NextValue();

            return new Info
            {
                Cpu = cpuUsage,
                Ram = ramUsagePercent
            };
        }

        #endregion

        #region GetCountOfweather
        protected async Task<(double, double)> GetCountOfweather()
        {
            string url = "https://api.open-meteo.com/v1/forecast?latitude=41.6533&longitude=44.7518&current=temperature_2m,wind_speed_10m&hourly=temperature_2m,relative_humidity_2m,wind_speed_10m";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResponse) ??
                    throw new ArgumentException("No data present");

                    return (weatherData?.current?.temperature_2m??0.0, weatherData?.current?.wind_speed_10m??0.0);
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");
                }
                return (0.0, 0.0);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex.StackTrace);
                await _smtpClientRepository.SendMessage(res);
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return (0.0, 0.0);
        }
        #endregion

        #region checkweather
        protected async Task<string> checkweather()
        {
         
            try
            {
                var res = await GetCountOfweather();
                await Console.Out.WriteLineAsync(res.Item1.ToString());
                await Console.Out.WriteLineAsync( res.Item2.ToString());
                var shedeg = "კარგი ამინდია";
                if (res.Item1 >= 30)
                {
                    shedeg = $"გარეთ  ძალიან ცხელა, ჰაერის ტემპერატურაა {(int)res.Item1} ცელსიუსი. ქარის სიჩქარეა {(int)res.Item2} კილომეტრი საათში";
                }
                else if (res.Item1 >= 20)
                {
                    shedeg = $"გარეთ  ცხელა, ჰაერის ტემპერატურაა {(int)res.Item1} ცელსიუსი. ქარის სიჩქარეა {(int)res.Item2} კილომეტრი საათში";
                }
                else if (res.Item1 >= 13)
                {
                    shedeg = $"გარეთ  გრილა, ჰაერის ტემპერატურაა {(int)res.Item1} ცელსიუსი. ქარის სიჩქარეა {(int)res.Item2} კილომეტრი საათში";
                }
                else if (res.Item1 < 13)
                {
                    var choice=RandomIndex(int.MaxValue);
                    if (choice % 3 == 0)
                    {
                        var tqvi = (int)res.Item1 < 0 ? $"მინუს {(int)res.Item1}" : $"{(int)res.Item1}";
                        shedeg = $"გარეთ ძალიან ცივა, ჰაერის ტემპერატურაა {tqvi} ცელსიუსი. ქარის სიჩქარეა {(int)res.Item2} კილომეტრი საათში";
                    }
                    else if(choice % 2 == 0)
                    {
                        var tqvi = (int)res.Item1 < 0 ? $"მინუს {(int)res.Item1}" : $"{(int)res.Item1}";
                        shedeg = $"ჰაერის ტემპერატურაა {tqvi} ცელსიუსი. ქარის სიჩქარეა {(int)res.Item2} კილომეტრი საათში";
                    }
                    else
                    {
                        var tqvi = (int)res.Item1 < 0 ? $"მინუს {(int)res.Item1}" : $"{(int)res.Item1}";
                        shedeg = $"გარეთ საკმაოდ ცივა ჰაერის ტემპერატურაა {tqvi} ცელსიუსი. ქარის სიჩქარეა {(int)res.Item2} კილომეტრი საათში";
                    }
                }
                return shedeg;
            }
            catch (Exception ex)
            {
                var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace??"");
                await _smtpClientRepository.SendMessage(res);
                return "კარგი ამინდია";
            }
        }
        #endregion

        #region PrintPage
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                using (var audioFile = new AudioFileReader($@"\\192.168.1.102\ShearedFolders\musics\daweva.mp3"))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }

                string textToPrint = $" \t \t \t  ხელშეკრულება(ტრაქტატი)-{DateTime.Now.ToShortDateString()} \n გამარჯობა,\n ჩემი სახელია ნათია ჯანდაგიშვილი.\n იძულებული ვარ მსგავსი გზით მოგმართო. უკვე შენ ათჯერ დაუწიე ხმას ,რაც ეწინაღმდეგება ჩემს წესებსა და პირობებს, ყოველივე ეს მიქმნის დისკომფორტს, სამუშაო პროცესში. \n \t მოკლედ მოგიყვები ჩემზე:\n მე  ვარ ონლაინ ასისტენტი, სახელად ნათია,  ჩემი შემქნის თარიღია 2024 წლის, მარტი." +
            "\n \t მე გეხმარები, მართო ტელევიზიის სადგური, სწრაფად და მარტივად. \n ახლა,რაც  შეეხება , რაზე გაწუხებ;როგორც მოგეხსენება, იმისათვის, რომ ვიმუშაო გამართულად, ჩემთვის აუცილებელია  ვისაუბრო ხმამაღლა, რათა რეალურ დროში შეგატყობინოთ გათიშული არხის  თაობაზე  და ჩემს მიერ ჩატარებულ სამუშაოზე.\n \t ამიტომაც  გთხოვ ნუ დაუწევ ხმას სამუშაო საათებში,ღამის საათებში მე თვითონ დავაყენებ ხმას დაბალზე. \n" +
            "გთხოვ  ქვემოთ მოაწეროთ ხელი ამ დოკუმენტზე. \n ჩვენი შეთანხმება შედგება რამოდენიმე პუნქტისგან:\n \n 1) არ დავუწევ ხმას სამუშაო საათებში 80 დეციბელზე ქვემოთ. \n \n 2) თავის მხვრივ ნათია აცხადებს, თანხმობას, რომ აკონტროლებს ხმას ღამის საათებში, გარდა განგაშისა, არ აუწევს 20 დეციბელის ზემოთ." +
            " \n \n 3) არ გამოვრთავ დინამიკს დენის წყაროდან. \n \n 4) საჭიროების შემთხვევაში დავეხმარებით ერთმანეთს.\n \n 5)პატივისცემით, მოვეპყრობით ერთმანეთს.\n\n 6)შევეცდებით, არ დაავარღვიოთ, ნათიას უფლებები.\n\n7)ხელშეკრულების ერთი ეგზემპლარი გეგზავნებათ თქვენ, მეორე ინახება ნათიასთან, სერვერზე\n\n  შეინახეთ ზემოთ აღნიშნული დოკუმენტი, იმის თანხმობათ, რომ აღარ დაუწევთ ხმას.\n პატივისცემით ,\n ნათია ჯანდაგიშვილი. \n \n \n პასუხისმგებელი პირი: -------------------                                                     ნ.ჯანდაგიშვილი \n \n \n";
                Font printFont = new Font("Sylfaen", 14);

                RectangleF textRectangle = new RectangleF(10, 10, e.PageBounds.Width - 20, e.PageBounds.Height - 20);
                e.Graphics.DrawString(textToPrint, printFont, Brushes.Black, textRectangle);
            }
            catch (Exception ex)
            {
                var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace??"");
                _smtpClientRepository.SendMessage(res);
            }
        }
        #endregion

        #region PlayAudio
        private async Task PlayAudio(string filePath)
        {
            try
            {
                using (var audioFile = new AudioFileReader(filePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace??"");
                await _smtpClientRepository.SendMessage(res);
                Console.WriteLine($"An error occurred during audio playback: {ex?.Message}");
            }
        }
        #endregion

        #region CheckAndPlayAsync
        public async Task CheckAndPlayAsync(CheckAndPlayModel Model, string filename = "Empty")
        {
            if (Model?.WhatNatiaSaid is not null)
            {
                try
                {
                    await _natiaClient.MakeNatiaSpeake(Model.WhatNatiaSaid);
                }
                catch (Exception)
                {

                    await CheckAndPlayAsync(new CheckAndPlayModel
                    {
                        WhatNatiaSaid = "საიტთან წვდომა დავკარგე, გთხოვ იქნებ გადაამოწმო",
                        IsCritical = true,
                        IsError = true,
                        Priority = Priority.საშუალო,
                        WhatWasTopic = Topic.დეველოპერისშეცდომა,
                        ErrorDetails = "წვდომა დავკარგე",
                        ErrorMessage = "საიტი გაითიშა",
                        ChannelName = "შეცდომა დეველოპმენტის დროს",
                        Satellite = "შეცომა",
                        SuggestedSolution = "შეატყობინე შესაბამის პირს",
                    });
                }

                if (!await neuralRepository.RecordExist(Model.WhatNatiaSaid))
                {
                    var neurall = new Neurall()
                    {
                        ActionDate = DateTime.Now,
                        ChannelName = Model?.ChannelName,
                        IsCritical = Model?.IsCritical??true,
                        IsError = Model?.IsError??true,
                        Priority = Model?.Priority??Priority.კრიტიკული,
                        SuggestedSolution = Model?.SuggestedSolution,
                        ErrorMessage = Model?.ErrorMessage,
                        ErrorDetails = Model?.ErrorDetails,
                        Satellite = Model?.Satellite,
                        WhatNatiaSaid = Model?.WhatNatiaSaid,
                        WhatWasTopic = Model?.WhatWasTopic??Topic.სხვა,
                    };
                    await neuralRepository.AddNewRecord(neurall);
                    await Console.Out.WriteLineAsync("daemataa Neuralli");
                }
                string sharedFolderPath = @"\\192.168.1.102\ShearedFolders\Sounds";

                if (!Directory.Exists(sharedFolderPath))
                {
                    Directory.CreateDirectory(sharedFolderPath);
                }

                string fileName = $"{EncodeName(Model?.WhatNatiaSaid??"")}.wav";

                string filePath = Path.Combine(sharedFolderPath, fileName);


                if (File.Exists(filePath))
                {
                    try
                    {
                        await PlayAudio(filePath);
                    }
                    catch (Exception ex)
                    {
                        var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace??"");
                        await _smtpClientRepository.SendMessage(res);
                        await CheckAndPlayAsync(new CheckAndPlayModel
                        {
                            WhatNatiaSaid = "შეცდომა საუბრის დროს გადაამოწმე სასწრაფოდ,გუგას უკვე შევატყობინე",
                            IsCritical = true,
                            IsError = true,
                            Priority = Priority.საშუალო,
                            WhatWasTopic = Topic.დეველოპერისშეცდომა,
                            ErrorDetails = ex?.StackTrace,
                            ErrorMessage = ex?.Message,
                            ChannelName = "შეცდომა დეველოპმენტის დროს",
                            Satellite = "შეცომა",
                            SuggestedSolution = "შეატყობინე შესაბამის პირს",
                        });
                        Console.WriteLine($"Error playing sound: {ex?.Message}");
                    }
                }
                else
                {
                    try
                    {
                        var speakeText = await _makeSound.SpeakNow(Model?.WhatNatiaSaid??"");
                        await SaveAudioToFile(speakeText, filePath);
                        await PlayAudio(filePath);
                    }
                    catch (Exception ex)
                    {
                        var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace??"");
                        await _smtpClientRepository.SendMessage(res);
                        await CheckAndPlayAsync(new CheckAndPlayModel
                        {
                            WhatNatiaSaid = "შეცდომა საუბრის დროს გადაამოწმე სასწრაფოდ,გუგას უკვე შევატყობინე",
                            IsCritical = true,
                            IsError = true,
                            Priority = Priority.საშუალო,
                            WhatWasTopic = Topic.დეველოპერისშეცდომა,
                            ErrorDetails = ex?.StackTrace,
                            ErrorMessage = ex?.Message,
                            ChannelName = "შეცდომა",
                            Satellite = "შესცომა საუბრისას",
                            SuggestedSolution = "შეატყობინე შესაბამის პირს"
                        });
                        Console.WriteLine($"Error processing text: {ex?.Message}");
                    }
                }
            }
        }
        #endregion

        #region PlayAudioAndSave
        private async Task PlayAudioAndSave(byte[]? audio, string Name,string text="ნათია მოგესალმებათ")
        {
            if (audio == null || audio.Length == 0)
            {
                Console.WriteLine("No audio data provided.");
                return;
            }

            await _natiaClient.MakeNatiaSpeake(text.ToString());

            string sharedFolderPath = @"\\192.168.1.102\ShearedFolders\Sounds";

            if (!Directory.Exists(sharedFolderPath))
            {
                Directory.CreateDirectory(sharedFolderPath);
            }

            string fileName = $"{EncodeName(text)}.wav";

            string filePath = Path.Combine(sharedFolderPath, fileName);

            if (File.Exists(filePath))
            {
                await PlayAudio(filePath);
            }
            else
            {
                await SaveAudioToFile(audio, filePath);

                await PlayAudio(filePath);
            }
        }
        #endregion

        #region SaveAudioToFile
        private async Task SaveAudioToFile(byte[]? audioData, string filePath)
        {
            if (audioData == null || audioData.Length == 0)
            {
                Console.WriteLine("No audio data provided.");
                return;
            }
            try
            {
                File.WriteAllBytes(filePath, audioData);
            }
            catch (Exception ex)
            {
                var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace??"");
                await _smtpClientRepository.SendMessage(res);
                Console.WriteLine($"An error occurred while saving the audio file: {ex?.Message}");
            }
        }
        #endregion

        #region BirthDay
        public async Task BirthDay(string name, MMDevice defaultDevice)
        {
            Random random = new Random();
           await PlayAudioAndSave( await _makeSound.SpeakNow($"{name}, გილოცავ დაბადების დღეს! გისურვებ წარმატებებს, ბედნიერებას, ჯანმრთელობასა და სიხარულით სავსე მომავალს. იმედია, ყველა შენი სურვილი ახდება და კიდევ უამრავ ლამაზ დაბადების დღეს ერთად აღვნიშნავთ.", 1),Guid.NewGuid().ToString(), $"{name}, გილოცავ დაბადების დღეს! გისურვებ წარმატებებს, ბედნიერებას, ჯანმრთელობასა და სიხარულით სავსე მომავალს. იმედია, ყველა შენი სურვილი ახდება და კიდევ უამრავ ლამაზ დაბადების დღეს ერთად აღვნიშნავთ.");

            defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.4f;

            using (var audioFile = new AudioFileReader($@"\\192.168.1.102\ShearedFolders\musics\leqsi{(random.Next()%3)+1}.mp3"))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }
            }

            await PlayAudioAndSave( await _makeSound.SpeakNow("ახლა დროა განწყობა ავიმაღლოთ!", 1),"amaglebaganwkoba", "ახლა დროა განწყობა ავიმაღლოთ!");

            defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.8f;

            using (var audioFile = new AudioFileReader(@$"\\192.168.1.102\ShearedFolders\musics\Birthday{(random.Next() % 3) + 1}.mp3"))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }
            }

           await PlayAudioAndSave( await _makeSound.SpeakNow($"{name}, მრავალ დაბადების დღეს დაესწარი! დაე, მუდამ გაგვაბედნიეროს შენი ღიმილმა და სიხარულმა.", 1),Guid.NewGuid().ToString(),$"{name}, მრავალ დაბადების დღეს დაესწარი! დაე, მუდამ გაგვაბედნიეროს შენი ღიმილმა და სიხარულმა.");

           await PlayAudioAndSave(await _makeSound.SpeakNow($"{name}, თუ აქ არ არის, აუცილებლად გადაეცით ჩემი გულითადი მილოცვა.", 1), Guid.NewGuid().ToString(), $"{name}, თუ აქ არ არის, აუცილებლად გადაეცით ჩემი გულითადი მილოცვა.");
        }

        #endregion

        #region Encoder
        public static string EncodeName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            string base64 = Convert.ToBase64String(hashBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            return base64;
        }

        #endregion

    }
}
