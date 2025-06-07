using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Natia.Application.Services;

public class PortCheckAndRefresh
{
    public void start(int card, int port)
    {
        Console.WriteLine(card);
        Console.WriteLine(port);
        IWebDriver driver = new ChromeDriver();

        string url = $"http://192.168.20.200/C132/port_video_en.asp?slotNo={card - 1}&portNo={port - 1}";
        driver.Navigate().GoToUrl(url);


        IWebElement dropdown = driver.FindElement(By.Id("switch"));

        dropdown.Click();

        var options = dropdown.FindElements(By.TagName("option"));

        foreach (var option in options)
        {
            if (option.GetAttribute("value") == "0")
            {
                option.Click();
                break;
            }
        }

        IWebElement applyButton = driver.FindElement(By.Id("applyBtn"));

        applyButton.Click();

        Thread.Sleep(7000);

        var optionschartva = dropdown.FindElements(By.TagName("option"));

        foreach (var option in options)
        {
            if (option.GetAttribute("value") == "1")
            {
                option.Click();
                break;
            }
        }

        IWebElement applyButton1 = driver.FindElement(By.Id("applyBtn"));

        applyButton1.Click();

        driver.Quit();
    }
}
