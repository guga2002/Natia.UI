using NatiaGuard.BrainStorm.Models;
using NAudio.Wave;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Drawing;
using NAudio.CoreAudioApi;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using Natia.Core.Entities;

namespace NatiaGuard.BrainStorm.Main
{
    public partial class Main
    {
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
                return greetings[index].Text;
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

                    return (weatherData.current.temperature_2m, weatherData.current.wind_speed_10m);
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
                var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex.StackTrace);
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
                var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex.StackTrace);
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
                Console.WriteLine($"An error occurred during audio playback: {ex.Message}");
            }
        }
        #endregion

        #region CheckAndPlayAsync
        public async Task CheckAndPlayAsync(CheckAndPlayModel Model, string filename = "Empty")
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
                var neurall = new Natia.Core.Entities.Neurall()
                {
                    ActionDate = DateTime.Now,
                    ChannelName = Model.ChannelName,
                    IsCritical = Model.IsCritical,
                    IsError = Model.IsError,
                    Priority = Model.Priority,
                    SuggestedSolution = Model.SuggestedSolution,
                    ErrorMessage = Model.ErrorMessage,
                    ErrorDetails = Model.ErrorDetails,
                    Satellite = Model.Satellite,
                    WhatNatiaSaid = Model.WhatNatiaSaid,
                    WhatWasTopic = Model.WhatWasTopic,
                };
                await neuralRepository.AddNewRecord(neurall);
                await Console.Out.WriteLineAsync("daemataa Neuralli");
            }
            string sharedFolderPath = @"\\192.168.1.102\ShearedFolders\Sounds";

            if (!Directory.Exists(sharedFolderPath))
            {
                Directory.CreateDirectory(sharedFolderPath);
            }

            string fileName = $"{EncodeName(Model.WhatNatiaSaid)}.wav";
            if (filename is not "Empty")
            {
                fileName = $"{EncodeName(filename)}.wav";
            }

            string filePath = Path.Combine(sharedFolderPath, fileName);
            

            if (File.Exists(filePath))
            {
                try
                {
                    await PlayAudio(filePath);
                }
                catch (Exception ex)
                {
                    var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex.StackTrace);
                    await _smtpClientRepository.SendMessage(res);
                    await CheckAndPlayAsync(new CheckAndPlayModel
                    {
                        WhatNatiaSaid = "შეცდომა საუბრის დროს გადაამოწმე სასწრაფოდ,გუგას უკვე შევატყობინე",
                        IsCritical = true,
                        IsError = true,
                        Priority = Priority.საშუალო,
                        WhatWasTopic = Topic.დეველოპერისშეცდომა,
                        ErrorDetails = ex.StackTrace,
                        ErrorMessage = ex.Message,
                        ChannelName = "შეცდომა დეველოპმენტის დროს",
                        Satellite = "შეცომა",
                        SuggestedSolution = "შეატყობინე შესაბამის პირს",
                    });
                    Console.WriteLine($"Error playing sound: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    byte[] speakeText = await _makeSound.SpeakNow(Model.WhatNatiaSaid);
                    await SaveAudioToFile(speakeText, filePath);
                    await PlayAudio(filePath);
                }
                catch (Exception ex)
                {
                    var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex.StackTrace);
                    await _smtpClientRepository.SendMessage(res);
                    await CheckAndPlayAsync(new CheckAndPlayModel
                    {
                        WhatNatiaSaid = "შეცდომა საუბრის დროს გადაამოწმე სასწრაფოდ,გუგას უკვე შევატყობინე",
                        IsCritical = true,
                        IsError = true,
                        Priority = Priority.საშუალო,
                        WhatWasTopic = Topic.დეველოპერისშეცდომა,
                        ErrorDetails = ex.StackTrace,
                        ErrorMessage = ex.Message,
                        ChannelName = "შეცდომა",
                        Satellite = "შესცომა საუბრისას",
                        SuggestedSolution = "შეატყობინე შესაბამის პირს"
                    });
                    Console.WriteLine($"Error processing text: {ex.Message}");
                }
            }
        }
        #endregion

        #region PlayAudioAndSave
        private async Task PlayAudioAndSave(byte[] audio, string Name,string text="ნათა მოგესალმებათ")
        {
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
        private async Task SaveAudioToFile(byte[] audioData, string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, audioData);
            }
            catch (Exception ex)
            {
                var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex.StackTrace);
                await _smtpClientRepository.SendMessage(res);
                Console.WriteLine($"An error occurred while saving the audio file: {ex.Message}");
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

        #region CheckMail

        private async Task CheckMail()
        {
            var messages=await _imapService.CheckForNewMessage();

            if(messages.Any())
            {
                await PlayAudioAndSave(await _makeSound.SpeakNow("მეილზე გვაქვს ახალი შეტყობინება", 1), "მეილზეხალი", "მეილზე გვაქვს ახალი შეტყობინება");
                foreach (var item in messages)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow($"გამომგზავნი{item.Name}; თემატიკა:{item.Subject};შეტყობინება:{item.Body}", 1),Guid.NewGuid().ToString(), $"გამომგზავნი{item.Name}; თემატიკა:{item.Subject};შეტყობინება:{item.Body}");
                }
                await PlayAudioAndSave(await _makeSound.SpeakNow("საჭიროების შემთხვევაში შეამოწმე მეილი", 1), "შემოწმება", "საჭიროების შემთხვევაში შეამწმე მეილი");
            }
        }

        private async Task ReplayToUser()
        {

            var message = await _imapService.CheckforReplay();

            foreach (var item in message)
            {
                if (item.Body.ToLower().Contains("report") || item.Body.ToLower().Contains("chanell report") || item.Body.Contains("რეპორტი") || item.Body.Contains("შეცდომა")||item.Body.Contains("პრობლემა"))
                {
                    var info = await _db.Neuralls.Take(10).ToListAsync();
                    await _smtpClientRepository.SendMessage(MakeShablon(info), item.Email, $"{item.Name},გიგზავნი პასუხს,");
                }

                if(item.Body.ToLower().Contains("usage")|| item.Body.ToLower().Contains("data")|| item.Body.ToLower().Contains("მონაცემები"))
                {
                    PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    float availableRAM = ramCounter.NextValue();
                    var axlaiyenebs = 8000 - availableRAM;

                    var percent = axlaiyenebs / 8000 * 100;
                    float cpuUsage = cpuCounter.NextValue();

                    await _smtpClientRepository.SendMessage(ShablonForComputer(percent,cpuUsage), item.Email, $"{item.Name},გიგზავნი ჩემს სასიცოცხლო მონაცემებს");
                }

                if (item.Body.ToLower().Contains("hello") ||
                      item.Body.ToLower().Contains("hi") ||
                      item.Body.ToLower().Contains("greetings") ||
                      item.Body.Contains("გამარჯობა"))
                {
                    string response = $"გამარჯობა, {item.Name}! მე მზად ვარ დაგეხმარო ნებისმიერ კითხვაში ან პრობლემის გადაჭრაში!";
                    await _smtpClientRepository.SendMessage(response, item.Email, "პასუხი შენს შეტყობინებაზე");
                }
                else if (item.Body.ToLower().Contains("uptime"))
                {
                    TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount);
                    string uptimeMessage = $"სისტემა აქტიურია უკვე {uptime.Days} დღე, {uptime.Hours} საათი და {uptime.Minutes} წუთი.";
                    await _smtpClientRepository.SendMessage(uptimeMessage, item.Email, "სისტემის მუშაობის დრო");
                }
                else if (item.Body.ToLower().Contains("status"))
                {
                    string statusMessage = "ყველაფერი კარგად მუშაობს. მე აქ ვარ, რათა დაგეხმარო ნებისმიერ დროს!";
                    await _smtpClientRepository.SendMessage(statusMessage, item.Email, "სისტემის სტატუსი");
                }
                else if (item.Body.ToLower().Contains("version"))
                {
                    string versionMessage = "სისტემა ახლა მუშაობს ვერსია 1.2.0-ზე. ახალი ფუნქციები მალე დაემატება!";
                    await _smtpClientRepository.SendMessage(versionMessage, item.Email, "ვერსიის ინფორმაცია");
                }

                // Assistance Cases
                else if (item.Body.ToLower().Contains("help"))
                {
                    string helpResponse = "თუ დახმარება გჭირდებათ, აქ ვარ! უბრალოდ დაწერეთ და მე შევეცდები პასუხის გაცემას.";
                    await _smtpClientRepository.SendMessage(helpResponse, item.Email, "დახმარების პასუხი");
                }
                else if (item.Body.ToLower().Contains("error"))
                {
                    string errorResponse = "შეცდომა? მე აქ ვარ, რომ გავარკვიოთ და გადავწყვიტოთ. გთხოვთ, დეტალურად აღწეროთ პრობლემა.";
                    await _smtpClientRepository.SendMessage(errorResponse, item.Email, "შეცდომის პასუხი");
                }
                else if (item.Body.ToLower().Contains("settings"))
                {
                    string settingsResponse = "გთხოვთ, მოაწესრიგოთ თქვენი პარამეტრები და მოხსენით პრობლემები. მე აქ ვარ დახმარებისთვის!";
                    await _smtpClientRepository.SendMessage(settingsResponse, item.Email, "პარამეტრების ინფორმაცია");
                }

                // Fun and Interactive Cases
                else if (item.Body.ToLower().Contains("joke") || item.Body.ToLower().Contains("ანეკდოტი"))
                {
                    string joke = "რატომ დადიან ჩიტები სკოლაში? რომ ჰქონდეთ მაგარი განათლება! 😄";
                    await _smtpClientRepository.SendMessage(joke, item.Email, "მხიარული ანეკდოტი");
                }
                else if (item.Body.ToLower().Contains("fact") || item.Body.ToLower().Contains("ფაქტი"))
                {
                    string fact = "იცით თუ არა, რომ მეტროში ჰაერის წნევა ადვილად მოქმედებს სხეულზე? უცნაური, არა?";
                    await _smtpClientRepository.SendMessage(fact, item.Email, "საინტერესო ფაქტი");
                }
                else if (item.Body.ToLower().Contains("quote") || item.Body.ToLower().Contains("ციტატა"))
                {
                    string quote = "ცხოვრება მოკლეა, ამიტომ აკეთეთ ის, რაც გიყვართ! ✨";
                    await _smtpClientRepository.SendMessage(quote, item.Email, "ციტატა");
                }
                else if (item.Body.ToLower().Contains("weather"))
                {
                    string weather = "ამინდის ინფორმაცია ჯერ კიდევ დამუშავების პროცესშია, მაგრამ მწვანე ტყე ყოველთვის ლამაზია!";
                    await _smtpClientRepository.SendMessage(weather, item.Email, "ამინდის ინფორმაცია");
                }
                else if (item.Body.ToLower().Contains("random number"))
                {
                    var random = new Random();
                    int randomNumber = random.Next(1, 100);
                    string randomResponse = $"შემთხვევითი რიცხვი არის: {randomNumber}.";
                    await _smtpClientRepository.SendMessage(randomResponse, item.Email, "შემთხვევითი რიცხვი");
                }

                // Greetings and Personal Interaction
                else if (item.Body.ToLower().Contains("hello") || item.Body.ToLower().Contains("hi") || item.Body.ToLower().Contains("greetings") || item.Body.ToLower().Contains("გამარჯობა"))
                {
                    string response = $"გამარჯობა, {item.Name}! მე მზად ვარ დაგეხმარო ნებისმიერ კითხვაში ან პრობლემაში!";
                    await _smtpClientRepository.SendMessage(response, item.Email, "პასუხი შენი შეტყობინებისთვის");
                }
                else if (item.Body.ToLower().Contains("who am i")||item.Body.Contains("ვინ ვარ"))
                {
                    string whoAmIResponse = $"{item.Name}, თქვენ გამორჩეული ხართ! ეს საკმარისი უნდა იყოს. 😊";
                    await _smtpClientRepository.SendMessage(whoAmIResponse, item.Email, "ვინ ხართ თქვენ");
                }
                else if (item.Body.ToLower().Contains("who are you")||item.Body.Contains("ვინ ხარ"))
                {
                    string systemResponse = "მე ვარ ნათია, თქვენი საიმედო ვირტუალური ასისტენტი!";
                    await _smtpClientRepository.SendMessage(systemResponse, item.Email, "ვინ ვარ მე");
                }

                // Productivity and Practical Tools
                else if (item.Body.ToLower().Contains("reminder"))
                {
                    string reminderResponse = $"შეგახსენებთ, რომ შეგიძლიათ დამიკავშირდეთ ნებისმიერ დროს! მე აქ ვარ თქვენი დახმარებისთვის.";
                    await _smtpClientRepository.SendMessage(reminderResponse, item.Email, "შეგახსენება");
                }
                else if (item.Body.ToLower().Contains("task"))
                {
                    string taskResponse = "შეიტანეთ თქვენი ამოცანები და მე დაგეხმარებით მათ დროულად შესრულებაში!";
                    await _smtpClientRepository.SendMessage(taskResponse, item.Email, "შეტყობინება ამოცანების შესახებ");
                }
                else if (item.Body.ToLower().Contains("calendar")|| item.Body.Contains("დღე")|| item.Body.Contains("თარიღი"))
                {
                    string calendarResponse = "თქვენი კალენდრის მონაცემები ჯერ არ მაქვს, მაგრამ მალე ამასაც დავამატებ!";
                    await _smtpClientRepository.SendMessage(calendarResponse, item.Email, "კალენდრის მონაცემები");
                }
                else if (item.Body.ToLower().Contains("time")||item.Body.Contains("საათი"))
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss");
                    string timeMessage = $"ახლა არის {currentTime}. თუ რამე დაგჭირდეს, აქ ვარ!";
                    await _smtpClientRepository.SendMessage(timeMessage, item.Email, "მიმდინარე დრო");
                }
                else
                {
                    string defaultMessage = "ბოდიშით, თქვენი მოთხოვნა ვერ გავიგე. გთხოვთ, უფრო კონკრეტული ინფორმაციის მოწოდება!";
                    await _smtpClientRepository.SendMessage(defaultMessage, item.Email, "შეტყობინების პასუხი");
                }
            }
        }


        private string ShablonForComputer(float ram,float cpu)
        {
            var res = @$"<!DOCTYPE html>
<html lang=""ka"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>სისტემის შესრულების ანგარიშის</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        h1 {{
            text-align: center;
            color: #333333;
        }}
        p {{
            font-size: 16px;
            color: #666666;
            line-height: 1.6;
        }}
        .highlight {{
            color: #4CAF50;
            font-weight: bold;
        }}
        .critical {{
            color: #ff4d4d;
            font-weight: bold;
        }}
        .table-container {{
            margin-top: 20px;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin: 10px 0;
        }}
        th, td {{
            padding: 12px;
            text-align: left;
            border: 1px solid #dddddd;
        }}
        th {{
            background-color: #4CAF50;
            color: white;
        }}
        tr:nth-child(even) {{
            background-color: #f2f2f2;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>ნათიას სასიცოცხლო მონაცემები</h1>
        <p>
           იხილეთ სისტემის ამჭამინდელი სიცოცხლის მაჩვენებელი:
        </p>
        <div class=""table-container"">
            <table>
                <thead>
                    <tr>
                        <th>მეტრიკა</th>
                        <th>მნიშვნელობა</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>ხელმისაწვდომი RAM</td>
                        <td class=""{{RAMClass}}"">{100 - ram}%</td>
                    </tr>
                    <tr>
                        <td>ამჟამად სისტემა იყენებს</td>
                        <td class=""{{RAMClass}}"">{ram}%</td>
                    </tr>
                    <tr>
                        <td>CPU-ს იყენებს</td>
                        <td class=""{{CPUClass}}"">{cpu}%</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <p>
            დეტალური ინფორმაციისთვის, გთხოვთ, გადაამოწმოთ სისტემის ლოგები ან დაუკავშირდით ადმინისტრატორს.
        </p>
    </div>
</body>
</html>
";
            return res;
        }


        private string MakeShablon(List<Natia.Core.Entities.Neurall>lst)
        {
            var res = @$"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Neurall Data Report</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 800px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        h1 {{
            text-align: center;
            color: #333333;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        th, td {{
            padding: 12px;
            border: 1px solid #dddddd;
            text-align: left;
        }}
        th {{
            background-color: #4CAF50;
            color: white;
        }}
        tr:nth-child(even) {{
            background-color: #f2f2f2;
        }}
        .critical {{
            color: #ff4d4d;
            font-weight: bold;
        }}
        .medium {{
            color: #ffa500;
        }}
        .low {{
            color: #4CAF50;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>სადგურის გათიშვების რეპორტი</h1>
        <table>
            <thead>
                <tr>
                   <th>არხის სახელი</th>
                    <th>თარიღი</th>
                    <th>რა თქვა ნათიამ</th>
                    <th>კრიტიკულია?</th>
                    <th>შეტყობინება</th>
                    <th>დეტალები</th>
                    <th>რჩევა</th>
                </tr>
            </thead>
            <tbody>";

            foreach (var item in lst)
            {
                var kritik= item.IsCritical? "კრიტიკულია":"უსაფრთხოა";
                res += @$"
                <tr>
                    <td>{item.ChannelName}</td>
                    <td>{item.ActionDate}</td>
                    <td>{item.WhatNatiaSaid}</td>
                    <td class=""low"">{kritik}</td>
                    <td>{item.ErrorMessage}</td>
                    <td>{item.ErrorDetails}</td>
                    <td>{item.SuggestedSolution}</td>
                </tr>
";
            }
            res += @"  </tbody>
        </table>
    </div>
</body>
</html>";
            return res;
        }
        #endregion

        #region Encoder and Decode
        private const int MaxNameLength = 20;
        private const int Offset = 3;

        public static string EncodeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var letters = name
                .Where(char.IsLetter)
                .Take(MaxNameLength)
                .Select(c => ShiftChar(c, Offset));

            return new string(letters.ToArray());
        }


        private static char ShiftChar(char c, int offset)
        {
            if (char.IsUpper(c) && c <= 'Z')
                return (char)((((c - 'A') + offset + 26) % 26) + 'A');

            if (char.IsLower(c) && c <= 'z')
                return (char)((((c - 'a') + offset + 26) % 26) + 'a');

            return c;
        }


        public static string DecodeName(string encodedName)
        {
            if (string.IsNullOrEmpty(encodedName)) return encodedName;

            StringBuilder decodedName = new StringBuilder();
            foreach (char c in encodedName)
            {
                decodedName.Append((char)(c - Offset));
            }
            return decodedName.ToString();
        }

        #endregion

    }
}
