using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Natia.Application.Services;

public class PortCheckAndRefresh
{
    private readonly ILogger<PortCheckAndRefresh> _logger;

    public PortCheckAndRefresh(ILogger<PortCheckAndRefresh> logger)
    {
        _logger = logger;
    }

    public void start(int card, int port)
    {
        IWebDriver? driver = null;
        try
        {
            _logger.LogInformation("Starting port refresh for Card={Card}, Port={Port}", card, port);

            driver = new ChromeDriver();
            string url = $"http://192.168.20.200/C132/port_video_en.asp?slotNo={card - 1}&portNo={port - 1}";
            driver.Navigate().GoToUrl(url);

            _logger.LogInformation("Navigated to {Url}", url);

            IWebElement dropdown = driver.FindElement(By.Id("switch"));
            dropdown.Click();

            var options = dropdown.FindElements(By.TagName("option"));

            // Disable port
            foreach (var option in options)
            {
                if (option.GetAttribute("value") == "0")
                {
                    option.Click();
                    _logger.LogInformation("Port disabled for Card={Card}, Port={Port}", card, port);
                    break;
                }
            }

            IWebElement applyButton = driver.FindElement(By.Id("applyBtn"));
            applyButton.Click();
            _logger.LogInformation("Disabled state applied. Waiting to re-enable...");

            Thread.Sleep(7000); // Wait for refresh

            // Re-enable port
            var enableOptions = dropdown.FindElements(By.TagName("option"));
            foreach (var option in enableOptions)
            {
                if (option.GetAttribute("value") == "1")
                {
                    option.Click();
                    _logger.LogInformation("Port re-enabled for Card={Card}, Port={Port}", card, port);
                    break;
                }
            }

            IWebElement applyButton1 = driver.FindElement(By.Id("applyBtn"));
            applyButton1.Click();
            _logger.LogInformation("Enabled state applied successfully for Card={Card}, Port={Port}", card, port);
        }
        catch (NoSuchElementException ex)
        {
            _logger.LogError(ex, "Failed to locate element while toggling Card={Card}, Port={Port}", card, port);
        }
        catch (WebDriverException ex)
        {
            _logger.LogError(ex, "WebDriver error while toggling Card={Card}, Port={Port}", card, port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while toggling Card={Card}, Port={Port}", card, port);
        }
        finally
        {
            driver?.Quit();
            _logger.LogInformation("ChromeDriver session closed for Card={Card}, Port={Port}", card, port);
        }
    }
}
