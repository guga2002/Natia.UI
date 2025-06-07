using System.Drawing.Printing;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using Natia.Application.Contracts;
using Natia.Core.Context;
using Natia.Application.Services;
using Natia.Core.Interfaces;
using Natia.Neurall.Interfaces;
using Natia.Persistance.Interface;
using Natia.Core.Entities;
using Natia.Persistance.Model;
using Natia.Application.Dtos.Keys;
using Natia.Gateway.Clients;
using Natia.UI.Models;
using System.Globalization;

namespace NatiaGuard.BrainStorm.Main;

public partial class Main
{
    #region Dependency Injection
    private readonly ISoundService _makeSound;

    private readonly IChanellService _channels;

    private readonly IInfoService _info;

    private readonly ITranscoderService _transcoder;

    private readonly SpeakerDbContext _db;

    private readonly IAllinOneService _allInOne;

    private readonly PortCheckAndRefresh _refresh;

    private readonly CheckFromWhereItIsCameFrom _checkSource;

    private readonly IRecieverRepository _reciever;

    private readonly ITempperatureService _temperature;

    private readonly Client _natiaClient;

    private readonly HttpClient _httpClient;

    private readonly INeuralRepository neuralRepository;

    private readonly INeuralMLPredict _predict;

    private readonly ISolutionRecommendationService _Recomendation;

    private readonly IImapServices _imapService;

    private readonly ISmtpClientRepository _smtpClientRepository;

    private static List<string> _mailsTOSent = new List<string>()
    {
        "aapkhazava22@gmail.com",
        "jandaga.monitoring@gmail.com",
        "globaltvsupport@qarva.com",
        "dimitri.shamugia@gmail.com",
    };

    // Constructor with dependency injection
    public Main(
        ISoundService makeSound,
        IChanellService channels,
        IInfoService info,
        ITranscoderService transcoder,
        SpeakerDbContext db,
        IAllinOneService allInOne,
        PortCheckAndRefresh refresh,
        CheckFromWhereItIsCameFrom checkSource,
        IRecieverRepository reciever,
        ITempperatureService temperature,
        HttpClient httpClient,
        ISmtpClientRepository _client,
        INeuralRepository neuralRepository,
        ISolutionRecommendationService recomendation,
        INeuralMLPredict predict,
        IImapServices imapService)
    {
        _makeSound = makeSound ?? throw new ArgumentNullException(nameof(makeSound));
        _channels = channels ?? throw new ArgumentNullException(nameof(channels));
        _info = info ?? throw new ArgumentNullException(nameof(info));
        _transcoder = transcoder ?? throw new ArgumentNullException(nameof(transcoder));
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _allInOne = allInOne ?? throw new ArgumentNullException(nameof(allInOne));
        _refresh = refresh ?? throw new ArgumentNullException(nameof(refresh));
        _checkSource = checkSource ?? throw new ArgumentNullException(nameof(checkSource));
        _reciever = reciever ?? throw new ArgumentNullException(nameof(reciever));
        _temperature = temperature ?? throw new ArgumentNullException(nameof(temperature));
        _httpClient = httpClient;
        _natiaClient = new Client();
        _smtpClientRepository = _client;
        this.neuralRepository = neuralRepository;
        _Recomendation = recomendation;
        _predict = predict;
        _imapService = imapService;
    }
    #endregion

    #region Start
    public async Task Start()
    {
        Stopwatch timer = new Stopwatch();

        var listsongs = new List<string>()
        {
            @"\\192.168.1.102\ShearedFolders\musics\DanceDance.wav",
            @"\\192.168.1.102\ShearedFolders\musics\sleep.wav"
        };
        Random rand = new Random();
    mods:
        try
        {
            int count = 0;
            int counttotalchange = 0;
            int countsayelse = 0;
            while (true)
            {
                timer.Start();
                await Heartbeat();

                MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
                var res = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All);
                MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                float level = defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                DateTime currentTime = DateTime.Now;


                if (counttotalchange >= 10)
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler(PrintPage);

                    pd.DefaultPageSettings.Landscape = false;
                    pd.Print();
                    counttotalchange = 0;
                }
                if (currentTime.Hour >= 10 && currentTime.Hour <= 21)
                {

                    if (defaultDevice.AudioEndpointVolume.Mute == true)
                    {
                        Console.WriteLine("damutebulia");
                        defaultDevice.AudioEndpointVolume.Mute = false;
                        defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.80f;

                        await PlayAudio(@"192.168.1.102\\ShearedFolders\\musics\\mute.mp3");

                        counttotalchange++;
                        await PlayAudio(@"\\192.168.1.102\ShearedFolders\musics\angry.mp3");

                    }

                    countsayelse = 0;
                    if (level <= 79)
                    {
                        var index = RandomIndex((int)level);
                        if (index > 50)
                        {

                            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\IntroDgisRejimi.mp3");
                        }
                        else if (index > 30)
                        {
                            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\DgisRejim.mp3");
                        }
                        else
                        {
                            var say = "გადავდივართ დღის რეჟიმზე, ახლა კი შემიძლია ხმა დავაყენო 80 დეციბელზე, რადგან ხარისხიანად მოვიტანო თქვენამდე ჩემი სათქმელი და შეტყობინებები.";
                            await PlayAudioAndSave(await _makeSound.SpeakNow(say), say, say);
                            await PlayAudioAndSave(await _makeSound.SpeakNow(await checkweather(), -1), await checkweather(), await checkweather());
                        }

                        var sessionManager = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                        foreach (var device in sessionManager)
                        {
                            if (device.State == DeviceState.Active)
                            {
                                device.AudioEndpointVolume.MasterVolumeLevelScalar = 0.95f;
                            }
                        }

                        count++;
                        counttotalchange++;
                        if (count >= 3)
                        {
                            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\sayveduri.mp3");
                            await CheckAndPlayAsync(new CheckAndPlayModel
                            {
                                WhatNatiaSaid = "მე საშინლად გაბრაზებული ვარ,  რომ არ ითვალისწინებ ჩემს თხოვნას. უკვე სამჯერ დაუწიე ხმას , გთხოვ სამუშაო საათებში ნუ დაუწევ ხმას.",
                                IsCritical = false,
                                IsError = false,
                                Priority = Priority.მარტივი,
                                WhatWasTopic = Topic.ნათიასგრძნიბები,
                            });
                            count = 0;
                        }

                    }
                }
                else
                {
                    if (countsayelse == 0 || level >= 50 || level <= 20)
                    {
                        counttotalchange = 0;
                        await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\game.mp3");
                        var sessionManager = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                        foreach (var device in sessionManager)
                        {
                            if (device.State == DeviceState.Active)
                            {
                                device.AudioEndpointVolume.MasterVolumeLevelScalar = 0.40f;
                            }
                        }

                        defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.40f;
                        await PlayAudioAndSave(await _makeSound.SpeakNow(await checkweather(), -1), await checkweather(), await checkweather());
                        count = 0;
                        countsayelse++;
                    }
                }
                try
                {
                    var Health = await _natiaClient.GetSystemHealth();
                    if (!string.IsNullOrWhiteSpace(Health))
                    {
                        await PlayAudioAndSave(await _makeSound.SpeakNow(Health), Health, Health);
                    }

                    var anomaly = await _natiaClient.CheckForAnnomalies();
                    if (!string.IsNullOrWhiteSpace(anomaly))
                    {
                        await PlayAudioAndSave(await _makeSound.SpeakNow(anomaly), anomaly, anomaly);
                    }

                    //try
                    //{
                    //    await CheckMail();
                    //    await ReplayToUser();
                    //}
                    //catch (Exception ex)
                    //{

                    //    var rel = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex.StackTrace);
                    //    await _smtpClientRepository.SendMessage(rel);
                    //    Console.WriteLine("An error occurred: " + ex.Message);
                    //}

                    var anivers = await _natiaClient.GetAnniversaryDates();
                    if (!string.IsNullOrEmpty(anivers))
                    {
                        await PlayAudioAndSave(await _makeSound.SpeakNow(anivers), anivers, anivers);
                    }

                    var performance = await MonitorPerformanceAsync();

                    if (performance.Cpu > 98)
                    {
                        var ricx = RandomIndex((int)performance.Cpu);

                        await _smtpClientRepository.SendMessage(performance.Cpu.ToString());

                        if (ricx > 30)
                        {
                            var responses = new[]
{
$"პროცესორზე არის დიდი დატვირთვა, გთხოვთ გამორთოთ არასაჭირო პროგრამები და ჩახურეთ გუგლის ბრაუზერი. ნათია წინასწარ გიხდით მადლობას.",
$"პროცესორი გადატვირთულია. გთხოვთ, გამორთოთ არასაჭირო პროგრამები და ჩახურეთ გუგლის ბრაუზერი. ნათია მადლობას გიხდით წინასწარ.",
$"მაღალი პროცესორის დატვირთვა დაფიქსირდა. გთხოვთ, გამორთოთ არა საჭირო პროგრამები. ნათია წინასწარ გიხდით მადლობას.",
$"გაფრთხილება: პროცესორის დატვირთვა ძალიან მაღალია. გთხოვთ, გამორთოთ არასაჭირო პროცესები. ნათია მადლობას გიხდით.",
$"პროცესორზე მაღალი დატვირთვაა. გთხოვთ, გამორთოთ არასაჭირო აპლიკაციები. ნათია მადლობას გიხდით წინასწარ."
};
                            var random = new Random();
                            string whatNatiaSaid = responses[random.Next(responses.Length)];
                            await PlayAudioAndSave(await _makeSound.SpeakNow(whatNatiaSaid), whatNatiaSaid, whatNatiaSaid);
                        }
                        else
                        {

                            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\procesori.mp3");
                        }
                        //makeSound.SpeakNow("პროცესორზე  არის დიდი  დატვირთვა, გთხოვთ გამორთეთ  არასაჭირო პროგრამები , და  ჩახურეთ  გუგლის ბრაუზერი. ნათია წინასწარ გიხდით მადლობას.",-4);
                    }
                    if (performance.Ram >= 96)
                    {
                        await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\Operatiuli.mp3");
                       
                        //  makeSound.SpeakNow($"ერთი თხოვნა მაქვს... ოპერატიული მეხსიერება,რამი, ძალიან სუსტია, პროცესორი იყენებს {(int)percent} პროცენტს. ყოველივე ეს მიქმნის დისკომფორტს. გთხოვთ იქნებ შეატყობინოთ შესაბამის პირს.," +
                        // "ეს ჩემთვის, სასიცოცხლოდ მნიშვნელოვანია. ნათია ბოდიშს გიხდით, შეწუხებისთვის.", -4);
                    }

                    try
                    {

                        var temp = await _temperature.GetCurrentData();
                        var temperat = double.Parse(temp.Item1.Trim(), CultureInfo.InvariantCulture);
                        var humidity = double.Parse(temp.Item2.Trim(), CultureInfo.InvariantCulture);
                        if (temperat >= 23 && temperat <= 25)
                        {
                            var play = new CheckAndPlayModel
                            {
                                WhatNatiaSaid = $"ამჟამად სადგურში მიმდინარე ტემპერატურაა:{Math.Floor(temperat)} ცელსიუსი, ტენიანობა:{Math.Floor(humidity)} პროცენტი. გადაამოწმე!",
                                IsCritical = true,
                                IsError = false,
                                Priority = Priority.კრიტიკული,
                                WhatWasTopic = Topic.სადგურისტემპერატურა,
                                ChannelName = "ტემპერატურა",
                                ErrorDetails = "ტემპერატურამ აიწია საკმაოდ",
                                Satellite = "ტემპერატურის სენსორი",
                                ErrorMessage = "თემპერატურა  აიწია, კონდიციონერი ვერ აგრილებს",
                                SuggestedSolution = "გადაამოწმე სადგური",
                            };
                            await CheckAndPlayAsync(play);
                        }
                        else if (temperat > 25)
                        {

                            await CheckAndPlayAsync(new CheckAndPlayModel
                            {
                                WhatNatiaSaid = "ყურადღება, ყურადღება, განგაში",
                                IsCritical = true,
                                IsError = false,
                                Priority = Priority.კრიტიკული,
                                WhatWasTopic = Topic.სადგურისტემპერატურა,
                                ChannelName = "ტემპერატურა",
                                ErrorDetails = "ტემპერატურამ აიწია საკმაოდ",
                                Satellite = "ტემპერატურის სენსორი",
                                ErrorMessage = "თემპერატურა  აიწია, კონდიციონერი ვერ აგრილებს",
                                SuggestedSolution = "გადაამოწმე სადგური",
                            }, "GangashiaNow");

                            await CheckAndPlayAsync(new CheckAndPlayModel
                            {
                                WhatNatiaSaid = $"ამჟამად სადგურში მიმდინარე ტემპერატურაა:{Math.Floor(temperat)} ცელსიუსი, ტენიანობა:{Math.Floor(humidity)} პროცენტი. სასწრაფოდ გადაამოწმე!",
                                IsCritical = true,
                                IsError = false,
                                Priority = Priority.კრიტიკული,
                                WhatWasTopic = Topic.სადგურისტემპერატურა,
                                ChannelName = "ტემპერატურა",
                                ErrorDetails = "ტემპერატურამ აიწია საკმაოდ",
                                Satellite = "ტემპერატურის სენსორი",
                                ErrorMessage = "თემპერატურა  აიწია, კონდიციონერი ვერ აგრილებს",
                                SuggestedSolution = "გადაამოწმე სადგური",
                            });

                            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\Alert.mp3");                  
                        }
                        else
                        {
                            Console.WriteLine("Kargi temperatura sadgurshi");
                        }
                    }
                    catch (Exception ex)
                    {
                        await CheckAndPlayAsync(new CheckAndPlayModel
                        {
                            WhatNatiaSaid = "სადგურის ტემპერატურის სენსორზე წვდომა დავკარგე,იქნებ გადაამოწმო, მადლობა.",
                            IsCritical = true,
                            IsError = true,
                            Priority = Priority.კრიტიკული,
                            WhatWasTopic = Topic.დეველოპერისშეცდომა,
                            ErrorDetails = ex.StackTrace,
                            ErrorMessage = ex.Message,
                            ChannelName = "დეველოპმენტი",
                            Satellite = "დეველოპერის შეცდომა",
                            SuggestedSolution = "შეატყობინე შესაბამის პირს",

                        });

                        var rel = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace ?? "");
                        await _smtpClientRepository.SendMessage(rel);
                    }

                }
                catch (Exception ex)
                {
                    await CheckAndPlayAsync(new CheckAndPlayModel
                    {
                        WhatNatiaSaid = $"{ex.Message}-პრობლემა გვაქვს:(((",
                        IsCritical = true,
                        IsError = true,
                        Priority = Priority.კრიტიკული,
                        WhatWasTopic = Topic.სადგურისტემპერატურა,
                        ErrorDetails = ex.StackTrace,
                        ErrorMessage = ex.Message,
                        ChannelName = "დეველოპმენტი",
                        Satellite = "დეველოპერის შეცდომა",
                        SuggestedSolution = "შეატყობინე შესაბამის პირს",
                    }, "Emptyasdsdsdfdffd");
                    var red = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex.StackTrace ?? "");
                    await _smtpClientRepository.SendMessage(red);
                }



                try
                {
                    var data = await _natiaClient.GetCardWhichNeedActivate();

                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            await CheckAndPlayAsync(new CheckAndPlayModel
                            {
                                WhatNatiaSaid = item,
                                IsCritical = true,
                                IsError = true,
                                Priority = Priority.კრიტიკული,
                                WhatWasTopic = Topic.სადგურისტემპერატურა,
                                ErrorDetails = "გააქტიურე ბარათი",
                                ErrorMessage = "ბარათია გასააქტიურებელი",
                                ChannelName = "დეველოპმენტი",
                                Satellite = "დეველოპერის შეცდომა",
                                SuggestedSolution = "შეატყობინე შესაბამის პირს",
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    var red = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace ?? "");
                    await _smtpClientRepository.SendMessage(red);
                }


                try
                {
                    var data = await _natiaClient.GetTextToSentInMail();
                    if (!string.IsNullOrEmpty(data) || data.Length > 10)
                    {
                        foreach (var item in _mailsTOSent.ToList())
                        {
                            await _smtpClientRepository.SendMessage(data, item, $"რეგიონების რეპორტი:{DateTime.Now}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var red = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace ?? "");
                    await _smtpClientRepository.SendMessage(red);
                }



                List<ExcellDataMode3l> ModelFromExcel = new List<ExcellDataMode3l>();

                ModelFromExcel = await _natiaClient.GetExcelInfo();
                if (ModelFromExcel.Count > 0)
                {

                    await PlayAudioAndSave(await _makeSound.SpeakNow("მინდა შეგახსენოთ, მნიშვნელოვანი შეტყობინებები,!", 1), "Mnishvnelovani", "მინდა შეგახსენოთ, მნიშვნელოვანი შეტყობინებები!");
                    int df = 1;
                    foreach (var item in ModelFromExcel)
                    {
                        await PlayAudioAndSave(await _makeSound.SpeakNow($"სამუშაო ნომერი :{df}", 1), $"სამუშაო ნომერი:{df.ToString()}", $"სამუშაო ნომერი :{df}");

                        await PlayAudioAndSave(await _makeSound.SpeakNow($"მორიგე: {item?.onDuty}, შესრულებული სამუშაო: {item?.job}, შენიშვნა: {item?.additionInfo}, არხის სახელი:{item?.chanellName}", 1), Guid.NewGuid().ToString(), $"მორიგე: {item?.onDuty}, შესრულებული სამუშაო: {item?.job}, შენიშვნა: {item?.additionInfo}, არხის სახელი:{item?.chanellName}");

                        df++;
                    };

                    await PlayAudioAndSave(await _makeSound.SpeakNow("დეტალური ინფორმაცია იხილეთ ექსელის ფაილში."), "DetaluriInfo", "დეტალური ინფორმაცია იხილეთ ექსელის ფაილში.");
                }
                else
                {
                    Console.WriteLine("amjamat shetyobineba ar gvaqvs");
                }

                ICollection<string> ports = new List<string>();

                try
                {
                    ports = await _natiaClient.GetInfoAsync();
                }
                catch (Exception ex)
                {
                    await CheckAndPlayAsync(new CheckAndPlayModel
                    {
                        WhatNatiaSaid = "სერვერს ვეღარ ვუკავშირდები იქნებ გადაამოწმო, 192.168.1.102. სერვერი ფიზიკურად არის სადგურში, სერვერების რეკში სულლ ბოლო სერვერი, ნახე ხო არ გამოირთო",
                        IsCritical = true,
                        IsError = true,
                        Priority = Priority.მარტივი,
                        WhatWasTopic = Topic.დეველოპერისშეცდომა,
                        ErrorDetails = ex.StackTrace,
                        ErrorMessage = ex.Message,
                        ChannelName = "შეცდომა",
                        Satellite = "დეველოპმენტის შეცდომა",
                        SuggestedSolution = "შეატყობინე შესაბამის პირს",
                    }, "UpServerDownNw");

                    var rey = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace ?? "");
                    await _smtpClientRepository.SendMessage(rey);
                }


                if (ports.Count > 15)
                {
                    var responses = new[]
{
$"ყურადღება! ყურადღება! გაგვეთიშა {ports.Count} არხი. ვრთავ განგაშის სიგნალს.",
$"სერიოზული პრობლემაა! {ports.Count} არხი აღარ ფუნქციონირებს. ვრთავ განგაშის სიგნალს.",
$"გაფრთხილება: {ports.Count} არხი გაითიშა! დაუყოვნებლივ მიიღეთ ზომები.",
$"გაითიშა {ports.Count} არხი. გთხოვთ, დაუყოვნებლივ შეამოწმოთ და იმოქმედოთ.",
$"ყურადღება! სისტემა გამოვიდა მწყობრიდან, გაგვეთშა {ports.Count} არხი!"
};

                    var random = new Random();
                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                    var yvela = new CheckAndPlayModel
                    {
                        WhatNatiaSaid = whatNatiaSaid,
                        IsCritical = true,
                        IsError = false,
                        Priority = Priority.კრიტიკული,
                        WhatWasTopic = Topic.არხი,
                        ChannelName = "ყველა",
                        Satellite = "აიპი მუხიანი",
                        ErrorDetails = "სავარაუდოთ ოპტიკის კაბელი დაზიანდა",
                        ErrorMessage = $"ოპტიკა დაზიანდა, დრო: {DateTime.Now}",
                        SuggestedSolution = "გადამოწმე სასერვერო, შემდეგ სასწრაფოდ გადაურეკე შესაბამის პირს!"
                    };

                    await CheckAndPlayAsync(yvela);

                    await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\Alert.mp3");
                   
                    var Solu = Solution(yvela);
                    var predict = await Predict(yvela);
                    float confidenceThreshold = yvela.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;


                    await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), $"{Solu.SuggestedSolution}", Solu?.SuggestedSolution ?? "");

                    if (predict is not null)
                    {
                        await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                    }
                }
                else
                {
                    Random rnd = new Random();
                    if (ports.Count > 1 && rnd.Next() % 2 == 0)
                    {
                        if (rnd.Next(34, 4566) % 2 == 1)
                        {
                            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\Shetyobineba.mp3");
                        }
                        else
                        {
                            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\axali.mp3");
                        }
                    }

                    if (!ports.Contains("300000") || !ports.Contains("900000"))
                    {
                        #region Foreach on Ports
                        foreach (var port in ports)
                        {
                            int portparsed;
                            bool resi = int.TryParse(port, out portparsed);
                            if (resi)
                            {
                                Console.WriteLine(portparsed);
                                if (portparsed == 1500)
                                {
                                    Random random = new Random();
                                    var say = await GreetingNow(GretingKeys.Afternoon);
                                    await PlayAudioAndSave(await _makeSound.SpeakNow(say, 1), say, say);

                                    await PlayAudioAndSave(await _makeSound.SpeakNow("ავიმაღლოთ განწყობა.", 0), "ganwyoba", "ავიმაღლოთ განწყობა");

                                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.9f;
                                    await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\{random.Next(100, 1456) % 30 + 1}.mp3");
                                    await PlayAudioAndSave(await _makeSound.SpeakNow(await checkweather(), -1), say, await checkweather());
                                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.95f;
                                }
                                else
                                if (portparsed == 2000)
                                {
                                    var say = await GreetingNow(GretingKeys.evening);
                                    await PlayAudioAndSave(await _makeSound.SpeakNow(say, 1), say, say);
                                    await PlayAudioAndSave(await _makeSound.SpeakNow("ავიმაღლოთ განწყობა.", 1), "Ganwyoba", "ავიმაღლოთ განწყობა.");

                                    Random random = new Random();
                                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.9f;
                                    await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\{random.Next(100, 1456) % 30 + 1}.mp3");                                

                                    await PlayAudioAndSave(await _makeSound.SpeakNow(await checkweather(), -1), say, await checkweather());
                                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.95f;
                                }
                                else
                                if (portparsed == 2500)//
                                {
                                    var say = await GreetingNow(GretingKeys.night);
                                    await PlayAudioAndSave(await _makeSound.SpeakNow(say, 1), say, say);
                                    await PlayAudioAndSave(await _makeSound.SpeakNow("ავიმაღლოთ განწყობა.", -1), "Ganwyoba", "ავიმაღლოთ განწყობა.");

                                    Random random = new Random();
                                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.95f;
                                    await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\{random.Next(100, 1496) % 30 + 1}.mp3");
                                    
                                    await PlayAudioAndSave(await _makeSound.SpeakNow(await checkweather(), -1), await checkweather(), await checkweather());
                                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.40f;
                                }
                                else
                                if (portparsed == 3000)
                                {
                                    var say = await GreetingNow(GretingKeys.moring);
                                    await PlayAudioAndSave(await _makeSound.SpeakNow(say, 1), say, say);
                                    await PlayAudioAndSave(await _makeSound.SpeakNow("ავიმაღლოთ განწყობა.", -1), "ganwyoba", "ავიმაღლოთ განწყობა.");

                                    Random random = new Random();
                                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.5f;
                                    await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\{random.Next(100, 1456) % 30 + 1}.mp3");                                   
                                    await PlayAudioAndSave(await _makeSound.SpeakNow(await checkweather(), -1), await checkweather(), await checkweather());
                                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.95f;
                                }



                                if (portparsed == 2002)
                                {
                                    await BirthDay("გუგა", defaultDevice);
                                }
                                else if (portparsed == 7055)
                                {
                                    await BirthDay("მიშა", defaultDevice);
                                }
                                else
                                if (portparsed == 1605)
                                {
                                    await BirthDay("გიო", defaultDevice);
                                }
                                else

                                if (portparsed == 1210)
                                {
                                    await BirthDay("დავით გურულო", defaultDevice);
                                }
                                else
                                if (portparsed == 6144)
                                {
                                    await BirthDay("ირაკლი", defaultDevice);
                                }
                                else
                                if (portparsed == 0504)
                                {
                                    await BirthDay("ვაშაგ, ვაგო", defaultDevice);

                                }
                                else
                                if (portparsed == 8170)
                                {
                                    await BirthDay("დავით დარჩო", defaultDevice);

                                }
                                else
                                if (portparsed == 1031)
                                {
                                    await BirthDay("დიმა", defaultDevice);
                                }
                                else
                                if (portparsed == 1013)
                                {
                                    await BirthDay("ვახო", defaultDevice);
                                }
                                else
                                if (portparsed == 3120)
                                {
                                    await BirthDay("გიორგი", defaultDevice);
                                }
                                else
                                if (portparsed == 1310)
                                {
                                    var responses = new[]
{
"ყურადღება, ყურადღება, განგაში. გორში გაითიშა ოპტიკა, შეატყობინე შესაბამის პირს.",
"გორში დაფიქსირდა სერიოზული პრობლემა. გთხოვთ, დაუყოვნებლივ აცნობოთ შესაბამის პირს.",
"ოპტიკა გაითიშა გორში. დაუყოვნებლივ შეამოწმეთ და აცნობეთ შესაბამისი პასუხისმგებელი პირს.",
"გორში სადგურთან კავშირი დავკარგე. გთხოვთ, სასწრაფოდ მიიღოთ ზომები და აცნობოთ შესაბამის პირს."
};
                                    var random = new Random();
                                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                                    var play = new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = whatNatiaSaid,
                                        IsCritical = true,
                                        IsError = false,
                                        Priority = Priority.კრიტიკული,
                                        WhatWasTopic = Topic.სარელეოსადგური,
                                        ChannelName = "გორის ოპტიკა",
                                        Satellite = "ოპტიკა",
                                        ErrorDetails = "ოპტიკა გაითშა, სავარაუდოთ კაბელი დაზიანდა",
                                        ErrorMessage = "ოპტიკა გაითშა",
                                        SuggestedSolution = "გადაამოწმე ან შეატყობინე შესაბამის პირს, სასწრაფოდ",
                                    };

                                    await CheckAndPlayAsync(play, whatNatiaSaid);

                                    var Solu = Solution(play);
                                    var predict = await Predict(play);
                                    float confidenceThreshold = play.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                                    await PlayAudioAndSave(await _makeSound.SpeakNow($"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", 1), $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}");

                                    if (predict is not null)
                                    {
                                        await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                                    }
                                }
                                else
                                if (portparsed == 1410)
                                {
                                    var responses = new[]
{
"ყურადღება, ყურადღება, განგაში. ფოთში გაითიშა ოპტიკა, შეატყობინე შესაბამის პირს.",
"ფოთში დაფიქსირდა სერიოზული პრობლემა. გთხოვთ, დაუყოვნებლივ აცნობოთ შესაბამის პირს.",
"ოპტიკა გაითიშა ფოთში. დაუყოვნებლივ შეამოწმეთ და აცნობეთ შესაბამისი პასუხისმგებელი პირს.",
"ფოთის სადგურთან კავშირი დავკარგე. გთხოვთ, სასწრაფოდ მიიღოთ ზომები და აცნობოთ შესაბამის პირს."
};
                                    var random = new Random();
                                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                                    var play = new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = whatNatiaSaid,
                                        IsCritical = true,
                                        IsError = false,
                                        Priority = Priority.კრიტიკული,
                                        WhatWasTopic = Topic.სარელეოსადგური,
                                        ChannelName = "ფოთის ოპტიკა",
                                        Satellite = "ოპტიკა",
                                        ErrorDetails = "ოპტიკა გაითშა, სავარაუდოთ კაბელი დაზიანდა",
                                        ErrorMessage = "ოპტიკა გაითშა",
                                        SuggestedSolution = "გადაამოწმე ან შეატყობინე შესაბამის პირს, სასწრაფოდ",
                                    };

                                    await CheckAndPlayAsync(play, whatNatiaSaid);

                                    var Solu = Solution(play);
                                    var predict = await Predict(play);
                                    float confidenceThreshold = play.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                                    await PlayAudioAndSave(await _makeSound.SpeakNow($"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", 1), $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}");
                                    if (predict.IsAnomalous)
                                    {
                                        await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                                    }
                                }
                                else
                                if (portparsed == 1510)
                                {
                                    var responses = new[]
{
"ყურადღება, ყურადღება, განგაში. ქუთაისში გაითიშა ოპტიკა, შეატყობინე შესაბამის პირს.",
"ქუთაისში დაფიქსირდა სერიოზული პრობლემა. გთხოვთ, დაუყოვნებლივ აცნობოთ შესაბამის პირს.",
"ოპტიკა გაითიშა ქუთაისში. დაუყოვნებლივ შეამოწმეთ და აცნობეთ შესაბამისი პასუხისმგებელი პირს.",
"ქუთაისის სადგურთან კავშირი დავკარგე. გთხოვთ, სასწრაფოდ მიიღოთ ზომები და აცნობოთ შესაბამის პირს."
};
                                    var random = new Random();
                                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                                    var play = new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = whatNatiaSaid,
                                        IsCritical = true,
                                        IsError = false,
                                        Priority = Priority.კრიტიკული,
                                        WhatWasTopic = Topic.სარელეოსადგური,
                                        ChannelName = "ქუთაისი ოპტიკა",
                                        Satellite = "ოპტიკა",
                                        ErrorDetails = "ოპტიკა გაითშა, სავარაუდოთ კაბელი დაზიანდა",
                                        ErrorMessage = "ოპტიკა გაითშა",
                                        SuggestedSolution = "გადაამოწმე ან შეატყობინე შესაბამის პირს სასტრაფოდ",
                                    };

                                    await CheckAndPlayAsync(play, whatNatiaSaid);

                                    var Solu = Solution(play);
                                    var predict = await Predict(play);
                                    float confidenceThreshold = play.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                                    await PlayAudioAndSave(await _makeSound.SpeakNow($"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", 1), $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}");

                                    if (predict.IsAnomalous)
                                    {
                                        await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                                    }
                                }
                                else
                                if (portparsed == 2510)
                                {
                                    var responses = new[]
{
"ყურადღება, ყურადღება, განგაში. თელავში გაითიშა ოპტიკა, შეატყობინე შესაბამის პირს.",
"თელავში დაფიქსირდა სერიოზული პრობლემა. გთხოვთ, დაუყოვნებლივ აცნობოთ შესაბამის პირს.",
"ოპტიკა გაითიშა თელავში. დაუყოვნებლივ შეამოწმეთ და აცნობეთ შესაბამისი პასუხისმგებელი პირს.",
"თელავის სადგურთან კავშირი დავკარგე. გთხოვთ, სასწრაფოდ მიიღოთ ზომები და აცნობოთ შესაბამის პირს."
};
                                    var random = new Random();
                                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                                    var play = new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = whatNatiaSaid,
                                        IsCritical = true,
                                        IsError = false,
                                        Priority = Priority.კრიტიკული,
                                        WhatWasTopic = Topic.სარელეოსადგური,
                                        ChannelName = "თელავის ოპტიკა",
                                        Satellite = "ოპტიკა",
                                        ErrorDetails = "ოპტიკა გაითშა, სავარაუდოთ კაბელი დაზიანდა",
                                        ErrorMessage = "ოპტიკა გაითშა",
                                        SuggestedSolution = "გადაამოწმე ან შეატყობინე შესაბამის პირს, სასწრაფოდ",
                                    };

                                    await CheckAndPlayAsync(play, whatNatiaSaid);

                                    var Solu = Solution(play);
                                    var predict = await Predict(play);
                                    float confidenceThreshold = play.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                                    await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", Solu?.SuggestedSolution ?? "");

                                    if (predict.IsAnomalous)
                                    {
                                        await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                                    }
                                }
                                else
                                if (portparsed == 333)
                                {
                                    var responses = new[]
{
"ქუთაისში, სარელეო სადგური გაითიშა. გთხოვ გადაამოწმო ან შეატყობინე შესაბამის პირს.",
"სარელეო სადგური ქუთაისში აღარ მუშაობს. გირჩევთ გადაამოწმოთ ან აცნობოთ შესაბამის პირს.",
"ქუთაისში სარელეო სადგურზე დაფიქსირდა პრობლემა. გთხოვთ, გადაამოწმოთ და საჭიროების შემთხვევაში შეატყობინოთ შესაბამის პირს.",
"სარელეო სადგური ქუთაისში გაითიშა. გადამოწმე ან აცნობე შესაბამის პირს."
};

                                    var random = new Random();
                                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                                    var play = new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = whatNatiaSaid,
                                        IsCritical = true,
                                        IsError = false,
                                        Priority = Priority.საშუალო,
                                        WhatWasTopic = Topic.სარელეოსადგური,
                                        ChannelName = "ქუთაისის სარელეო",
                                        Satellite = "სარელეო",
                                        ErrorDetails = "სარელეო სადგური გაითშა, სავარაუდოთ გადამცემი დაზიანდა, ან მოდულატორი",
                                        ErrorMessage = "სარელეო გაითშა",
                                        SuggestedSolution = "გადაამოწმე ან შეატყობინე შესაბამის პირს",
                                    };

                                    await CheckAndPlayAsync(play, whatNatiaSaid);

                                    var Solu = Solution(play);
                                    var predict = await Predict(play);
                                    float confidenceThreshold = play.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                                    await PlayAudioAndSave(await _makeSound.SpeakNow($"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", 1), $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}");

                                    if (predict.IsAnomalous)
                                    {
                                        await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                                    }
                                }
                                else
                                if (portparsed == 444)
                                {
                                    var responses = new[]
{
"ფოთში, სარელეო სადგური გაითიშა. გთხოვ გადაამოწმო ან შეატყობინე შესაბამის პირს.",
"სარელეო სადგური ფოთში აღარ მუშაობს. გირჩევთ გადაამოწმოთ ან აცნობოთ შესაბამის პირს.",
"ფოთში სარელეო სადგურზე დაფიქსირდა პრობლემა. გთხოვთ, გადაამოწმოთ და საჭიროების შემთხვევაში შეატყობინოთ შესაბამის პირს.",
"სარელეო სადგური ფოთში გაითიშა. გადამოწმე ან აცნობე შესაბამის პირს."
};

                                    var random = new Random();
                                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                                    var play = new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = whatNatiaSaid,
                                        IsCritical = true,
                                        IsError = false,
                                        Priority = Priority.საშუალო,
                                        WhatWasTopic = Topic.სარელეოსადგური,
                                        ChannelName = "ფოთის სარელეო",
                                        Satellite = "სარელეო",
                                        ErrorDetails = "სარელეო სადგური გაითშა, სავარაუდოთ გადამცემი დაზიანდა, ან მოდულატორი",
                                        ErrorMessage = "სარელეო გაითშა",
                                        SuggestedSolution = "გადაამოწმე ან შეატყობინე შესაბამის პირს",
                                    };

                                    await CheckAndPlayAsync(play, whatNatiaSaid);

                                    var Solu = Solution(play);
                                    var predict = await Predict(play);
                                    float confidenceThreshold = play.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;
                                    await PlayAudioAndSave(await _makeSound.SpeakNow($"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", 1), $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}", $"ნათიას აქვს რჩევა, {Solu.SuggestedSolution}");

                                    if (predict.IsAnomalous)
                                    {
                                        await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                                    }
                                }
                                else
                                if (portparsed == 555)
                                {
                                    var responses = new[]
{
"თელავში, სარელეო სადგური გაითიშა. გთხოვ გადაამოწმო ან შეატყობინე შესაბამის პირს.",
"სარელეო სადგური თელავში აღარ მუშაობს. გირჩევთ გადაამოწმოთ ან აცნობოთ შესაბამის პირს.",
"თელავში სარელეო სადგურზე დაფიქსირდა პრობლემა. გთხოვთ, გადაამოწმოთ და საჭიროების შემთხვევაში შეატყობინოთ შესაბამის პირს.",
"სარელეო სადგური თელავში გაითიშა. გადამოწმე ან აცნობე შესაბამის პირს."
};

                                    var random = new Random();
                                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                                    var play = new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = whatNatiaSaid,
                                        IsCritical = true,
                                        IsError = false,
                                        Priority = Priority.საშუალო,
                                        WhatWasTopic = Topic.სარელეოსადგური,
                                        ChannelName = "თელავის სარელეო",
                                        Satellite = "სარელეო",
                                        ErrorDetails = "სარელეო სადგური გაითშა, სავარაუდოთ გადამცემი დაზიანდა, ან მოდულატორი",
                                        ErrorMessage = "სარელეო გაითშა",
                                        SuggestedSolution = "გადაამოწმე ან შეატყობინე შესაბამის პირს",
                                    };

                                    await CheckAndPlayAsync(play, whatNatiaSaid);

                                    var Solu = Solution(play);
                                    var predi = await Predict(play);
                                    float confidenceThreshold = play.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                                    await PlayAudioAndSave(await _makeSound.SpeakNow($"ნათიას აქვს რჩევა, {Solu?.SuggestedSolution}", 1), $"ნათიას აქვს რჩევა, {Solu?.SuggestedSolution}", Solu?.SuggestedSolution??"");

                                    if (predi.IsAnomalous)
                                    {
                                        await PlayAudioAndSave(await _makeSound.SpeakNow(predi?.AnomalyDetails ?? ""), predi?.AnomalyDetails??"", predi?.AnomalyDetails??"");
                                    }
                                }
                                else
                                if (portparsed == 666)
                                {
                                    var responses = new[]
{
"გორში, სარელეო სადგური გაითიშა. გთხოვ გადაამოწმო ან შეატყობინე შესაბამის პირს.",
"სარელეო სადგური გორში აღარ მუშაობს. გირჩევთ გადაამოწმოთ ან აცნობოთ შესაბამის პირს.",
"გორში სარელეო სადგურზე დაფიქსირდა პრობლემა. გთხოვთ, გადაამოწმოთ და საჭიროების შემთხვევაში შეატყობინოთ შესაბამის პირს.",
"სარელეო სადგური გორში გაითიშა. გადამოწმე ან აცნობე შესაბამის პირს."
};

                                    var random = new Random();
                                    string whatNatiaSaid = responses[random.Next(responses.Length)];
                                    var play = new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = whatNatiaSaid,
                                        IsCritical = true,
                                        IsError = false,
                                        Priority = Priority.საშუალო,
                                        WhatWasTopic = Topic.სარელეოსადგური,
                                        ChannelName = "გორის სარელეო",
                                        Satellite = "სარელეო",
                                        ErrorDetails = "სარელეო სადგური გაითშა, სავარაუდოთ გადამცემი დაზიანდა, ან მოდულატორი",
                                        ErrorMessage = "სარელეო გაითშა",
                                        SuggestedSolution = "გადაამოწმე ან შეატყობინე შესაბამის პირს",
                                    };

                                    await CheckAndPlayAsync(play, whatNatiaSaid);

                                    var Solu = Solution(play);
                                    var predict = await Predict(play);
                                    float confidenceThreshold = play.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                                    await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu?.SuggestedSolution}", 1), $"{Solu?.SuggestedSolution}", Solu?.SuggestedSolution ?? "");

                                    if (predict.IsAnomalous)
                                    {
                                        await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                                    }
                                }
                                else
                                {
                                    await speake(portparsed);
                                }
                            }

                        }
                        #endregion
                    }

                }
                timer.Stop();
                await Console.Out.WriteLineAsync(timer.ElapsedMilliseconds.ToString());
                timer.Reset();
            }
        }
        catch (Exception ex)
        {
            var res = _smtpClientRepository.BuildHtmlMessage(ex.Message, ex?.StackTrace ?? "");
            await _smtpClientRepository.SendMessage(res);
            goto mods;
        }
    }
    #endregion

    #region Natia Speake
    private async Task speake(int port)
    {
        MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
        MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        if (defaultDevice.AudioEndpointVolume.Mute == true)
        {
            defaultDevice.AudioEndpointVolume.Mute = false;
            defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 1f;
            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\mute.mp3");

            await PlayAudio(@"\\192.168.1.102\ShearedFolders\musics\angry.mp3");
           
            await PlayAudioAndSave(await _makeSound.SpeakNow("საშინლად გაბრაზებული ვარ. მჭირდება რელაქსაცია."), "Relaqsacia", "საშინლად გაბრაზებული ვარ. მჭირდება რელაქსაცია.");

            Random random = new Random();

            await PlayAudio($@"\\192.168.1.102\ShearedFolders\musics\{random.Next(0, 31)}.mp3");
        }

        var chan = await _channels.GetChanellByPort(port);
        if (chan is null) return;
        int chanellid = chan.Id;
        var Info = await _info.GetInfoByChanellId(chanellid);
        var trans = await _transcoder.GetTranscoderInfoByChanellId(chanellid);
        if (chan != null && chan.Name != null)
        {
            Random rnd = new Random();
            if (port >= 129 && port <= 133 || port >= 137 && port <= 143)
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"გაითიშა {chan.NameForSpeake}. გადაამოწმე {chan.ChanellFormat}",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.საშუალო,
                    WhatWasTopic = Topic.არხი,
                    ChannelName = chan.Name,
                    Satellite = chan.FromOptic ? "მუხიანის ოპტიკა" : "ამ არხს ვიღებთ თანამგზავრიდან",
                    ErrorMessage = "არხი გაითიშა",
                    SuggestedSolution = "გადაამოწმე იემერი, ან ჰარმონიკი",
                    ErrorDetails = $"გაიტიშა შემდეგი არხი:სახელი{chan.Name};ქართული სახელი{chan.NameForSpeake};ფორმატი: {chan.ChanellFormat}"
                };

                await CheckAndPlayAsync(res);

                var Solu = Solution(res);
                var predict = await Predict(res);
                float confidenceThreshold = res.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu?.SuggestedSolution}", 1), $"{Solu?.SuggestedSolution}", Solu?.SuggestedSolution ?? "");

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
            }
            else if (port >= 134 && port <= 136)
            {

                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"გაითიშა ტე ორი, გადაამოწმე {chan.NameForSpeake} სიხშირე. {chan.ChanellFormat}.",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.ტეორი,
                    ChannelName = chan.Name,
                    Satellite = "ტე ორი",
                    ErrorMessage = "ტე არხი გაითიშა",
                    SuggestedSolution = "მოდი დაგეხმარები, გადაამოწმე შესაბამისი პორტი, შედი კონფიგურაციაში, პიდი გადაიყვანე ნულზე და შეინახე ცვლილება, გაიმეორე იგივე, გადაიყვანე ერთზე. დაარეფრეშე.",
                    ErrorDetails = $"გაიტიშა შემდეგი არხი: სახელი{chan.Name};ქართული სახელი{chan.NameForSpeake};ფორმატი: {chan.ChanellFormat}"
                };

                await CheckAndPlayAsync(res);

                await PlayAudioAndSave(await _makeSound.SpeakNow("მოდი დაგეხმარები, გადაამოწმე შესაბამისი პორტი, შედი კონფიგურაციაში, პიდი გადაიყვანე ნულზე და შეინახე ცვლილება, გაიმეორე იგივე, გადაიყვანე ერთზე. დაარეფრეშე.", 1), "Konfig", "მოდი დაგეხმარები, გადაამოწმე შესაბამისი პორტი, შედი კონფიგურაციაში, პიდი გადაიყვანე ნულზე და შეინახე ცვლილება, გაიმეორე იგივე, გადაიყვანე ერთზე. დაარეფრეშე.");
                var Solu = Solution(res);
                var predict = await Predict(res);
                float confidenceThreshold = res.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                // await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1),Guid.NewGuid().ToString(),Solu.SuggestedSolution);

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
            }
            else if (port == 144)//jeoselis rezervi
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"ყურადღება! ყურადღება! გაითიშა ჯეოსელის ოპტიკა. ვრთავ განგაშის სიგნალს",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.მუხიანისაიპი,
                    ChannelName = chan.Name,
                    Satellite = "ჯეოსელის ოპტიკა",
                    ErrorMessage = "ჯეოსელის ოპტიკა გაითშა",
                    SuggestedSolution = "გადაამოწმე იემერ 240.",
                    ErrorDetails = $"გაიტიშა შემდეგი არხი: სახელი{chan.Name};ქართული სახელი{chan.NameForSpeake};ფორმატი: {chan.ChanellFormat}"
                };

                await CheckAndPlayAsync(res, "JeocellOptic");

                await PlayAudio(@"\\192.168.1.102\ShearedFolders\musics\Alert.mp3");
                var Solu = Solution(res);
                var predict = await Predict(res);
                float confidenceThreshold = res.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;
                await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu?.SuggestedSolution}", 1), $"{Solu?.SuggestedSolution}", Solu?.SuggestedSolution ?? "");

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
            }
            else if (port == 145)
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"ყურადღება! ყურადღება! გაითიშა მთავარი ოპტიკა. ვრთავ განგაშის სიგნალს.",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.მუხიანისაიპი,
                    ChannelName = chan.Name,
                    Satellite = "მთავარი ოპტიკა",
                    SuggestedSolution = "სასწრაფოდ გადაურეკე მუხიანს",
                    ErrorDetails = "ოპტიკა გაიტიშა სავარაუდოთ კაბელი დაზიანდა",
                    ErrorMessage = "ოპტიკა გაითიშა",
                };

                await CheckAndPlayAsync(res, "MainOptic");

                await PlayAudio(@"\\192.168.1.102\ShearedFolders\musics\Alert.mp3");
               
                var Solu = Solution(res);
                var predict = await Predict(res);
                float confidenceThreshold = res.Priority == Priority.კრიტიკული ? 0.7f : 0.5f;

                await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), $"{Solu.SuggestedSolution}", $"{Solu.SuggestedSolution}");

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
            }
            else if (port == 146)
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"გაითიშა, სილქის რესივერები, გადააამოწმე, ენკოდერ 2  და  3, აგრეთე  შეამოწმე, იემერ 230, ქარდ 1 პორტ 1",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.ენკოდერი,
                    ChannelName = chan.Name,
                    Satellite = "სილქის რესივერები",
                    SuggestedSolution = "გადატვირთე ენკოდერი",
                    ErrorMessage = "ენკოდეირ გაიჭედა",
                    ErrorDetails = "ენკოდერს აქვს ხარვეზი, სავარაუდოთ გადატვირთვა უნდა",
                };

                await CheckAndPlayAsync(res, "SilkRecievers");
                var Solu = Solution(res);
                var predict = await Predict(res);
                // await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), Guid.NewGuid().ToString(),Solu.SuggestedSolution);

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
            }
            else if (port == 147)//icone recievers
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"გაითიშა, აიქონის რესივერები, გადაამოწმე, ენკოდერ 4, აგრეთე  შეამოწმე, იემერ 230, ქარდ 3 პორტ 1",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.ენკოდერი,
                    ChannelName = chan.Name,
                    Satellite = "აიქონი",
                    SuggestedSolution = "გადატვირთე  ენკოდერი",
                    ErrorDetails = "ენკოდერი გაიჭედა, ეცადე დარესეტო ან  გადაამოწმო ფიზიკურად",
                    ErrorMessage = "ენკოდერს აქვს ხარვეზი"
                };

                await CheckAndPlayAsync(res, "Icones");
                var Solu = Solution(res);
                var predict = await Predict(res);
                // await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), Guid.NewGuid().ToString(),Solu.SuggestedSolution);

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
            }
            else if (port == 148)
            {
                //t2 recievers
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"გაითიშა, ტე ორის  რესივერები, გადაამოწმე, ენკოდერ 5 აგრეთე  შეამოწმე, იემერ 220, ქარდ 1 პორტ 3",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.ტეორი,
                    ChannelName = chan.Name,
                    Satellite = "ტე ორი",
                    SuggestedSolution = "გაითშა  ტე ორი",
                    ErrorDetails = "შედი კონფიგურაციაში და   შეცვალე პიდი",
                    ErrorMessage = "ტე ორს აქვს  ხარვეზი"
                };

                await CheckAndPlayAsync(res, "T2Recievers");
                var Solu = Solution(res);
                var predict = await Predict(res);
                //await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), Guid.NewGuid().ToString(),Solu.SuggestedSolution);

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
            }
            else if (port == 149)
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"გაითიშა მესამე მულტისვიჩი, გადაამოწმე  სადგურში კვების ბლოკი ხომ არ გამოძვრა, ან კაბელი ხომ არ არის ცუდათ დაერთებული.",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.მულტისვიჩები,
                    ChannelName = chan.Name,
                    Satellite = "მულტისვიჩ სამი",
                    SuggestedSolution = $"გაითშა {chan.NameForSpeake}",
                    ErrorDetails = "შედი კონფიგურაციაში და შეცვალე პიდი",
                    ErrorMessage = "მულტისვიჩ სამს აქვს  ხარვეზი"
                };

                await CheckAndPlayAsync(res, "Multiswitch3");
                var Solu = Solution(res);
                var predict = await Predict(res);
                // await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), Guid.NewGuid().ToString(),Solu.SuggestedSolution);

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
                //მულტისვიჩ 3
            }
            else if (port == 150)
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"გაითიშა პირველი მულტისვიჩი, გადაამოწმე  სადგურში კვების ბლოკი ხომ არ გამოძვრა, ან კაბელი ხომ არ არის ცუდათ დაერთებული.",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.მულტისვიჩები,
                    ChannelName = chan.Name,
                    Satellite = "მულტისვიჩ პირველი",
                    SuggestedSolution = $"გაითშა {chan.NameForSpeake}",
                    ErrorDetails = "შედი  სადგურში და  გადაამოწმე",
                    ErrorMessage = "პირველ მულტისვიჩს  აქვს ხარვეზი"
                };

                await CheckAndPlayAsync(res, "Multiswitch1");
                var Solu = Solution(res);
                var predict = await Predict(res);

                // await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), Guid.NewGuid().ToString(),Solu.SuggestedSolution);

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", Solu?.SuggestedSolution ?? "");
                }

            }
            else if (port == 151)
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"გაითიშა მეორე მულტისვიჩი, გადაამოწმე  სადგურში კვების ბლოკი ხომ არ გამოძვრა, ან კაბელი ხომ არ არის ცუდათ დაერთებული.\",",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.მულტისვიჩები,
                    ChannelName = chan.Name,
                    Satellite = "მულტისვიჩ მეორე",
                    SuggestedSolution = $"გაითშა {chan.NameForSpeake}",
                    ErrorDetails = "შედი  სადგურში და  გადაამოწმე",
                    ErrorMessage = "მეორე მულტისვიჩს  აქვს ხარვეზი"
                };

                await CheckAndPlayAsync(res, "Multiswitch2");
                var Solu = Solution(res);
                var predict = await Predict(res);
                // await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), Guid.NewGuid().ToString(),Solu.SuggestedSolution);

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", Solu?.SuggestedSolution ?? "");
                }
            }
            else if (port >= 152 && port <= 158)//mukhianShi gashebuli arxebi
            {
                var res = new CheckAndPlayModel
                {
                    WhatNatiaSaid = $"{chan.NameForSpeake}-ზე გვაქვს პრობლემა, გადაამოწმე იემერ ორასოცი. {chan.ChanellFormat} {chan.Name}",
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.მუხიანისაიპი,
                    ChannelName = chan.Name,
                    Satellite = "მუხიანი",
                    ErrorMessage = "მუხიანშ წასულ არხებს აქვს ხარვეზი",
                    SuggestedSolution = "გადაამოწმე შესაბამისი მიმღები",
                    ErrorDetails = $"{chan.NameForSpeake}-ზე გვაქვს პრობლემა, გადაამოწმე იემერი ორასოცი, {chan.ChanellFormat}"
                };

                await CheckAndPlayAsync(res);

                var predict = await Predict(res);

                // await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1),Guid.NewGuid().ToString(),Solu.SuggestedSolution);

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }

            }
            else
            {
                var responses = new[]
{
$"{chan?.NameForSpeake??""}-ზე დაფიქსირდა ხარვეზი, გთხოვთ გადაამოწმოთ.",
$"{CorrectNameI(chan?.NameForSpeake??"")} არ მუშაობს სტაბილურად",
$"{CorrectNameI(chan?.NameForSpeake??"")} არ მაუწყებლობს",
};
                var random = new Random();
                string whatNatiaSaid = responses[random.Next(responses.Length)];
                var rek = new CheckAndPlayModel
                {
                    WhatNatiaSaid = whatNatiaSaid,
                    IsCritical = true,
                    IsError = false,
                    Priority = Priority.კრიტიკული,
                    WhatWasTopic = Topic.არხი,
                    ChannelName = chan?.Name,
                    Satellite = chan?.FromOptic == true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                    SuggestedSolution = $"გაითიშა {chan?.NameForSpeake}",
                    ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                    ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                };

                await CheckAndPlayAsync(rek);
                //var Solu = Solution(rek);
                var predict = await Predict(rek);

                //await PlayAudioAndSave(await _makeSound.SpeakNow($"{Solu.SuggestedSolution}", 1), Guid.NewGuid().ToString());

                if (predict.IsAnomalous)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(predict?.AnomalyDetails ?? ""), predict?.AnomalyDetails ?? "", predict?.AnomalyDetails ?? "");
                }
                if (Info != null)
                {
                    await PlayAudioAndSave(await _makeSound.SpeakNow(Info.AlarmMessage), Info.AlarmMessage, Info.AlarmMessage);
                }

                string portiko = await _allInOne.GetPort(chan?.Name ?? "");

                if (!string.IsNullOrEmpty(portiko))
                {
                    var res = await _allInOne.GetInfoByChanellName(portiko);//ასიგნ პორტ

                    if (res != null)
                    {
                        await PlayAudioAndSave(await _makeSound.SpeakNow($"არხი, გაშვებულია იემერ {res?.SourceEmr}-დან, {res?.Text}", 2), $"ამჟამად {chan?.NameForSpeake}, გაშვებულია იემერ {res?.SourceEmr}-დან, {res?.Text}", $"ამჟამად არხი, გაშვებულია იემერ {res?.SourceEmr}-დან, {res?.Text}");

                        switch (res?.SourceEmr)
                        {
                            case 10:
                            case 20:
                            case 30:
                            case 70:
                                {
                                    //var req = await _allInOne.GetRecieverInfoByChanellId(chan.Id);

                                    await PlayAudioAndSave(await _makeSound.SpeakNow("არხი არის ემპეგე ორი ფორმატის, და მოდის თანამგზავრიდან, შეეცადე ჩართო სხვა წყაროდან.თუ არის შესაძლებელი"), "EMPG2", "არხი არის ემპეგე ორი ფორმატის, და მოდის თანამგზავრიდან, შეეცადე ჩართო სხვა წყაროდან.თუ არის შესაძლებელი");
                                    break;
                                }
                            case 140:
                            case 180:
                            case 190:
                                {
                                    if (trans is not null)
                                    {
                                        var responsesharm = new[]
{
$"{CorrectNameI(chan?.NameForSpeake??"")} გატარებულია ჰარმონიკის ტრანსკოდერში: ჰარმონიკ {trans?.Emr_Number}, პორტ {trans?.Card}, სტრიმ {trans?.Port}. გთხოვთ გადაამოწმოთ სტრიმის სტატუსი.",
$"შეამოწმეთ ჰარმონიკის ტრანსკოდერი. ჰარმონიკ {trans?.Emr_Number}, პორტ {trans?.Card}, სტრიმ {trans?.Port}. შესაძლოა, სიგნალი დაეცა.",
$"გადაამოწმე, ჰარმონიკ {trans?.Emr_Number}, პორტ {trans?.Card}, სტრიმ {trans?.Port}. დაუყოვნებლივ გადაამოწმე და დარწმუნდი, რომ ყველაფერი გამართულად მუშაობს.",
$"ჰარმონიკის ტრანსკოდერი: ჰარმონიკ {trans?.Emr_Number}, პორტ {trans?.Card}, სტრიმ {trans?.Port}. დარწმუნდით, რომ სიგნალი სტაბილურია.",
$"გთხოვთ, შეამოწმოთ ჰარმონიკის ტრანსკოდერი. ჰარმონიკ {trans?.Emr_Number}, პორტ {trans?.Card}, სტრიმ {trans?.Port}. სიგნალის დაკარგვის შემთხვევაში, დაუყოვნებლივ იმოქმედეთ.",
$"{CorrectNameI(chan?.NameForSpeake??"")} გადის ჰარმონიკის ტრანსკოდერში. ჰარმონიკ {trans?.Emr_Number}, პორტ {trans?.Card}, სტრიმ {trans?.Port}. შეამოწმეთ, რომ ყველაფერი წესრიგშია.",
};

                                        var randomharm = new Random();
                                        string say = responsesharm[randomharm.Next(responsesharm.Length)];
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = say,
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.მულტისვიჩები,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic == true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                    }
                                    break;
                                }
                            case 100:
                            case 110:
                            case 120:
                            case 130:
                            case 200:
                            case 230:
                                {
                                    if (res.SourceEmr == 200 && trans != null && trans.Card != 0 && trans.Port != 0)
                                    {
                                        _refresh.start(trans.Card, trans.Port);
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"{CorrectNameI(chan?.NameForSpeake ?? "")} გატარებულია ტრანსკოდერში, {trans?.Emr_Number}, ქარდ{trans?.Card}, პორტ{trans?.Port}, გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic == true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"ვიდეო გადავტვირთე, გადაამოწმე თუ გამოსწორდა!",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });

                                        var portsourc = await _checkSource.checkAsync("200", chan?.Name??"");
                                        var emr = _db.Emr200Info.FirstOrDefault(io => io.Port == portsourc.ToString());

                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"ამჟამად ტრანსკოდერში არხის წყაროა, იემერ {emr?.SourceEmr},{emr?.Text}. გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic == true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                    }
                                    if (res.SourceEmr == 100)
                                    {
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"{CorrectNameI(chan?.NameForSpeake??"")} გატარებულია ტრანსკოდერში, {trans?.Emr_Number}, ქარდ{trans?.Card}, პორტ{trans?.Port}, გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.მულტისვიჩები,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                        var portsourc = await _checkSource.checkAsync("100", chan?.Name??"");
                                        var emr = _db.Emr100Info.FirstOrDefault(io => io.Port == portsourc.ToString());
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"ამჟამად ტრანსკოდერში {CorrectNameI(chan?.NameForSpeake??"")} წყაროა, იემერ {emr?.SourceEmr},{emr?.Text}. გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.მულტისვიჩები,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                    }
                                    if (res.SourceEmr == 110)
                                    {
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"{CorrectNameI(chan?.NameForSpeake??"")} გატარებულია ტრანსკოდერში, {trans?.Emr_Number}, ქარდ{trans?.Card}, პორტ{trans?.Port}, გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.მულტისვიჩები,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic ==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                            ,
                                        });
                                        var portsourc = await _checkSource.checkAsync("110", chan?.Name??"");
                                        var emr = _db.Emr110Info.FirstOrDefault(io => io.Port == portsourc.ToString());
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"ამჟამად ტრანსკოდერში {CorrectNameI(chan?.NameForSpeake??"")} წყაროა, იემერ {emr?.SourceEmr},{emr?.Text}. გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                    }
                                    if (res.SourceEmr == 120)
                                    {
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"არხი გატარებულია ტრანსკოდერში, {trans?.Emr_Number}, ქარდ{trans?.Card}, პორტ{trans?.Port}, გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.კრიტიკული,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                        var portsourc = await _checkSource.checkAsync("120", chan?.Name??"");
                                        var emr = _db.Emr120Info.FirstOrDefault(io => io.Port == portsourc.ToString());

                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"ამჟამად ტრანსკოდერში არხის წყაროა, იემერ {emr?.SourceEmr},{emr?.Text}. გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.საშუალო,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                    }
                                    if (res.SourceEmr == 130)
                                    {
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"არხი გატარებულია ტრანსკოდერში, {trans?.Emr_Number}, ქარდ{trans?.Card}, პორტ{trans?.Port}, გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.საშუალო,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name,
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                        var portsourc = await _checkSource.checkAsync("130", chan?.Name??"");
                                        var emr = _db.Emr130Info.FirstOrDefault(io => io.Port == portsourc.ToString());
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"ამჟამად ტრანსკოდერში არხის წყაროა, იემერ {emr?.SourceEmr},{emr?.Text}. გადაამოწმე.",
                                            IsCritical = true,
                                            IsError = false,
                                            Priority = Priority.საშუალო,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name??"",
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                        });
                                    }
                                }
                                break;
                            case 40:
                            case 50:
                            case 80:
                            case 90:
                                {
                                    if (chan is not null)
                                    {
                                        var des = await _allInOne.GetDesclamblerInfoByChanellId(chan.Id);
                                        await CheckAndPlayAsync(new CheckAndPlayModel
                                        {
                                            WhatNatiaSaid = $"დესკრამბლერს აქვს ხარვეზი,გადაამოწმე",
                                            IsCritical = false,
                                            IsError = false,
                                            Priority = Priority.მარტივი,
                                            WhatWasTopic = Topic.არხი,
                                            ChannelName = chan?.Name,
                                            SuggestedSolution = $"ვეცდები დაგეხმარო, შედი შესაბამის ქარდზე  და  გადაამოწმე თუ დესლკამბლერ სტატუსი არის წითლად, ბარათია გასააქტიურებელი," +
                                               $"შედი სადგურში, მოძებნე ,იემერ {des?.EmrNumber},ქარდ{des?.Card},პორტ{des?.Port}, ამოიღე ბარათი , გააქტიურე აიქონის რესივერით, და დააბრუნე თავის ადგილას" +
                                               $"შესაბამისი სიხშირე შეგიძლია ნახო ექსელის ფაილში ან ლუნგსატის ვებსაიტზე.",
                                            Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                            ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                            ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა",

                                        }, "DeskramblerFault");


                                        if (des != null)
                                        {
                                            await CheckAndPlayAsync(new CheckAndPlayModel
                                            {
                                                WhatNatiaSaid = $"ქარდ{des?.Card}, პორტ{des?.Port}",
                                                IsCritical = false,
                                                IsError = false,
                                                Priority = Priority.საშუალო,
                                                WhatWasTopic = Topic.არხი,
                                                ChannelName = chan?.Name,
                                                SuggestedSolution = $"ვეცდები დაგეხმარო, შედი შესაბამის ქარდზე  და  გადაამოწმე თუ დესლკამბლერ სტატუსი არის წითლად, ბარათია გასააქტიურებელი," +
                                             $"შედი სადგურში, მოძებნე ,იემერ {des?.EmrNumber},ქარდ{des?.Card},პორტ{des?.Port}, ამოიღე ბარათი , გააქტიურე აიქონის რესივერით, და დააბრუნე თავის ადგილას" +
                                             $"შესაბამისი სიხშირე შეგიძლია ნახო ექსელის ფაილში ან ლუნგსატის ვებსაიტზე.",
                                                Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                                ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                                ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                            });

                                            var forsay = $"ვეცდები დაგეხმარო, შედი შესაბამის ქარდზე  და  გადაამოწმე თუ დესლკამბლერ სტატუსი არის წითლად, ბარათია გასააქტიურებელი," +
                                                $"შედი სადგურში, მოძებნე ,იემერ {des?.EmrNumber},ქარდ{des?.Card},პორტ{des?.Port}, ამოიღე ბარათი , გააქტიურე აიქონის რესივერით, და დააბრუნე თავის ადგილას" +
                                                $"შესაბამისი სიხშირე შეგიძლია ნახო ექსელის ფაილში ან ლუნგსატის ვებსაიტზე.";

                                            await PlayAudioAndSave(await _makeSound.SpeakNow(forsay), forsay, forsay);

                                        }
                                    }
                                }
                                break;
                            case 210:
                                {
                                    await CheckAndPlayAsync(new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = "არხი მოდის მუხიანის ბლეიდის ტრანსკოდერიდან, გადაამოწმე. საჭიროების შემთხვევაში გადაურეკე!",
                                        IsCritical = false,
                                        IsError = false,
                                        Priority = Priority.საშუალო,
                                        WhatWasTopic = Topic.მუხიანისაიპი,
                                        ChannelName = chan?.Name,
                                        Satellite = chan?.FromOptic == true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                        SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                        ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                        ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა",
                                    }, "CallMUkhiani");
                                    break;
                                }
                            case 240:
                                {
                                    await CheckAndPlayAsync(new CheckAndPlayModel
                                    {
                                        WhatNatiaSaid = "არხი გაშვებულია ჯეოსელის სარეზერვოთი და არის ემპეგე ორი ფორმატის, გთხოვ გადართე მთავარ წყაროზე",
                                        IsCritical = false,
                                        IsError = false,
                                        Priority = Priority.საშუალო,
                                        WhatWasTopic = Topic.მუხიანისაიპი,
                                        ChannelName = chan?.Name,
                                        Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                        SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                        ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                        ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                                    }, "JeosellIP");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (chan?.FromOptic==true)
                {
                    var index = RandomIndex(int.MaxValue);
                    string say;

                    if (index % 3 == 0)
                    {
                        var responsesoptic = new[]
                        {
    $"{CorrectNameI(chan?.NameForSpeake??"")} მოდის მუხიანიდან, გადაამოწმე. საჭიროების შემთხვევაში გადაურეკე!",
    $"მუხიანში {chan?.NameForSpeake}-ზე აქვთ ხარვეზი. გთხოვთ გადაამოწმოთ.",
};
                        say = responsesoptic[new Random().Next(responsesoptic.Length)];
                    }
                    else if (index % 2 == 0)
                    {
                        var responsesoptic = new[]
                        {
    $"{CorrectNameI(chan?.NameForSpeake??"")} გვაქვს მიღებული ოპტიკით, გადაამოწმე იემერ ორასათი.",
};
                        say = responsesoptic[new Random().Next(responsesoptic.Length)];
                    }
                    else
                    {
                        var responsesoptic = new[]
                        {
    $"გადაამოწმე იემერ ორასათი, თუ {CorrectNameI(chan?.NameForSpeake??"")} არ მოდის, გადარეკე მუხიანში და შეატყობინე.",
    $"თუ იემერ ორასათში {CorrectNameI(chan?.NameForSpeake??"")} არ მოდის, შეატყობინე მუხიანის მორიგეს."
};
                        say = responsesoptic[new Random().Next(responsesoptic.Length)];
                    }

                    await CheckAndPlayAsync(new CheckAndPlayModel
                    {
                        WhatNatiaSaid = say,
                        IsCritical = true,
                        IsError = false,
                        Priority = Priority.საშუალო,
                        WhatWasTopic = Topic.არხი,
                        ChannelName = chan?.Name,
                        Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                        SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                        ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                        ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                    }, say);
                }
                else
                {
                    if (chan is not null)
                    {
                        var req = await _allInOne.GetRecieverInfoByChanellId(chan.Id);
                        if (req != null)
                        {
                            var responsesrec = new[]
    {
$"მოდი დაგეხმარები, {CorrectNameI(chan?.NameForSpeake??"")} მიღებულია ჩვენგან, გადაამოწმე, იემერ {req?.EmrNumber}, ქარდ {req?.Card}, პორტ {req?.Port}.",
$"სიგნალი მოდის თანამგზავრიდან, გადაამოწმე, იემერ {req?.EmrNumber}, ქარდ {req?.Card}, პორტ {req?.Port}.",
$"არხი აქტიურია თანამგზავრული სიგნალით, გადამოწმე იემერ {req?.EmrNumber}, ქარდ {req?.Card}, პორტ {req?.Port},დარწმუნდი რომ ყველაფერი რიგზეა",
$"გირჩევთ გადაამოწმოთ იემერ {req?.EmrNumber}, ქარდ {req?.Card}, პორტ {req?.Port}."
};

                            var randomrec = new Random();
                            string say = responsesrec[randomrec.Next(responsesrec.Length)];

                            await CheckAndPlayAsync(new CheckAndPlayModel
                            {
                                WhatNatiaSaid = say,
                                IsCritical = false,
                                IsError = false,
                                Priority = Priority.მარტივი,
                                WhatWasTopic = Topic.არხი,
                                ChannelName = chan?.Name,
                                Satellite = chan?.FromOptic ==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                SuggestedSolution = $"გაითშა {chan?.NameForSpeake}",
                                ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"

                            });
                        }


                        var desk = await _allInOne.GetDesclamblerInfoByChanellId(chan?.Id??0);
                        if (desk != null)
                        {
                            await CheckAndPlayAsync(new CheckAndPlayModel
                            {
                                WhatNatiaSaid = $"{CorrectNameI(chan?.NameForSpeake??"")} გატარებულია დესკრამბლერში",
                                IsCritical = false,
                                IsError = false,
                                Priority = Priority.საშუალო,
                                WhatWasTopic = Topic.არხი,
                                ChannelName = chan?.Name,
                                SuggestedSolution = $"ვეცდები დაგეხმარო, შედი შესაბამის ქარდზე  და  გადაამოწმე თუ დესკრამბლერ სტატუსი არის წითლად. ბარათია გასააქტიურებელი." +
                               $"შედი სადგურში, მოძებნე ,იემერ {desk?.EmrNumber},ქარდ{desk?.Card},პორტ{desk?.Port}. ამოიღე ბარათი, გააქტიურე აიქონის რესივერით, ამის შემდეგ, დააბრუნე თავის ადგილას..." +
                               $"შესაბამისი სიხშირე, შეგიძლია ნახო, ექსელის ფაილში ან ლუნგსატის ვებსაიტზე.",
                                Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                            }, "Desklamblershia");

                            var responsesdesk = new[]
    {
$"ვეცდები დაგეხმარო, შედი შესაბამის ქარდზე და გადაამოწმე თუ დესკრამბლერ სტატუსი არის წითლად. ბარათია გასააქტიურებელი. " +
$"შედი სადგურში, მოძებნე, იემერ {desk?.EmrNumber}, ქარდ {desk?.Card}, პორტ {desk?.Port}. ამოიღე ბარათი, გააქტიურე აიქონის რესივერით და დააბრუნე თავის ადგილას. " +
$"შესაბამისი სიხშირე შეგიძლია ნახო, ექსელის ფაილში ან ლუნგსატის ვებსაიტზე.",

$"შეამოწმე დესკრამბლერის სტატუსი, თუ წითლად აჩვენებს, საჭიროა ბარათის გააქტიურება. " +
$"გადადით იემერ {desk?.EmrNumber}, ქარდ {desk?.Card}, პორტ {desk?.Port}. ამოიღე ბარათი და გააქტიურე აიქონის რესივერით. " +
$"შემდგომ დააბრუნე თავის ადგილას. დეტალური სიხშირე შეგიძლიათ იხილოთ ექსელის ფაილში ან ლუნგსატის ვებსაიტზე.",

$"დარწმუნდით, რომ დესკრამბლერი წესრიგშია. თუ საჭიროა, გააქტიურეთ ბარათი. " +
$"იემერ {desk?.EmrNumber}, ქარდ {desk?.Card}, პორტ {desk?.Port}. ბარათის გააქტიურების შემდეგ, დააბრუნეთ თავის ადგილას. " +
$"შესაბამისი ინფორმაცია შეგიძლიათ ნახოთ ლუნგსატის ვებსაიტზე ან ექსელის ფაილში.",

$"შეამოწმეთ, თუ დესკრამბლერი სწორად მუშაობს. წითელი სტატუსის შემთხვევაში, ბარათი გააქტიურეთ აიქონის რესივერით. " +
$"შედი სადგურში, იემერ {desk?.EmrNumber}, ქარდ {desk?.Card}, პორტ {desk?.Port}. ბარათის გააქტიურებისათვის, დეტალები იხილეთ ლუნგსატის ვებსაიტზე ან ექსელის ფაილში."
};

                            var randomDesk = new Random();
                            string say = responsesdesk[randomDesk.Next(responsesdesk.Length)];

                            await CheckAndPlayAsync(new CheckAndPlayModel
                            {
                                WhatNatiaSaid = say,
                                IsCritical = false,
                                IsError = false,
                                Priority = Priority.საშუალო,
                                WhatWasTopic = Topic.არხი,
                                ChannelName = chan?.Name,
                                SuggestedSolution = $"ვეცდები დაგეხმარო, შედი შესაბამის ქარდზე  და  გადაამოწმე თუ დესკრამბლერ სტატუსი არის წითლად. ბარათია გასააქტიურებელი." +
                              $"შედი სადგურში, მოძებნე ,იემერ {desk?.EmrNumber},ქარდ{desk?.Card},პორტ{desk?.Port}. ამოიღე ბარათი, გააქტიურე აიქონის რესივერით, ამის შემდეგ, დააბრუნე თავის ადგილას..." +
                              $"შესაბამისი სიხშირე, შეგიძლია ნახო, ექსელის ფაილში ან ლუნგსატის ვებსაიტზე.",
                                Satellite = chan?.FromOptic==true ? "მუხიანიდან წამოსული არხი" : "თანამგზავრული არხი",
                                ErrorDetails = "გადაამოწმე სისტემაში პრობლემა გვაქვს",
                                ErrorMessage = "გაგეთიშა არხი, ბითრეით დაეცა"
                            });
                        }
                    }
                }

            }
        }
    }
    #endregion
}