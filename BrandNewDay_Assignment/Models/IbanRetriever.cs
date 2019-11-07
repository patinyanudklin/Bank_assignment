using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandNewDay_Assignment.Models
{
    class IbanRetriever : IIbanRetriver
    {
        public string GetIban()
        {
            string webDestination = @"http:\\randomiban.com\?country=Netherlands";
            string xPath = "//*[@id='demo']";

            var chromeDriver = new ChromeDriver();

            chromeDriver.Navigate().GoToUrl(webDestination);
            var number = chromeDriver.FindElementByXPath(xPath);
            var output = number.Text;
            chromeDriver.Close();

            return output;
        }
    }
    public interface IIbanRetriver
    {
        string GetIban();
    }
}
