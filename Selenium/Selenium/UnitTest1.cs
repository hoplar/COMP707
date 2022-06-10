using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using System.Net;

namespace Selenium
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1() //Test one google taupo weather
        {
            //opens google then sends keys into the search bar
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://www.google.co.nz");
            driver.FindElement(By.Name("q")).SendKeys("Taupo weather");
            driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);

        }       
        
        [TestMethod]
        public void TestMethod2() //Test two trademe search for IT jobs
        {
            //opens trademe then sends keys into the search bar
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.trademe.co.nz/a/");
            driver.FindElement(By.Id("search")).SendKeys("IT Jobs");
            driver.FindElement(By.Id("search")).SendKeys(Keys.Enter);

        }


        //Code for checking if a link is valid, returns the status code
        public string GetHttpStatus(string url)
        {
            try
            {
                HttpWebRequest webReq;
                webReq = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                return response.StatusCode.ToString();
            }

            //returns an error if it is unable to check the status
            catch (Exception e)
            {
                return e.Message;
            }

        }

        [TestMethod]
        public void TestMethod3() //Test three validating internal links on automationpractice
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://automationpractice.com");

            //retreiving all internal links from the page
            var links = driver.FindElements(By.XPath("//*[contains(@href,'http://automationpractice.com')]"));
           
            //parallel foreach loop to make this faster and use multiple cores
            Parallel.ForEach(links, url =>
            {
                    //calls the http check method
                    var UrlStatus = GetHttpStatus(url.GetAttribute("href")).ToString();
                    if (UrlStatus == "OK")
                    {

                        System.Diagnostics.Debug.WriteLine(url.GetAttribute("href") + " Status : " + UrlStatus);
                    }
                    else
                    {

                        System.Diagnostics.Debug.WriteLine(url.GetAttribute("href") + " Status : " + UrlStatus);
                    }
                
            }
        );
        }


        [TestMethod]
        public void TestMethod4()// Test four adds 3 items to cart then checks price
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://automationpractice.com");

            //retreives a list of all items you can add to the cart, then adds the first three
            var links = driver.FindElements(By.XPath("//*[contains(@class,'ajax_add_to_cart_button')]"));
            for (int i = 0; i < 3; i++)
            {
                driver.Navigate().GoToUrl(links[i].GetAttribute("href"));
                driver.Navigate().Back();
            }

            //navigates back to the cart, finds all links to delete items, then deletes one of the items
            driver.Navigate().GoToUrl("http://automationpractice.com/index.php?controller=order");
            var deletelinks = driver.FindElements(By.XPath("//*[contains(@title,'Delete')]"));
            driver.Navigate().GoToUrl(deletelinks[1].GetAttribute("href"));

            //retreives prices from the page, then adds them together
            var prices = driver.FindElements(By.XPath("//*[contains(@class,'price')][contains(@id,'total_product_price')]"));
            double count = 0;
            for (int i = 0; i < prices.Count; i++)
            {
                try { 
                    count += Double.Parse(prices[i].Text.Substring(1)); 
                }
                catch { }

            }

            //getting shipping and adding it to total
            var priceShipping = driver.FindElements(By.XPath("//*[contains(@class,'price')][contains(@id,'total_shipping')]"));
            Console.WriteLine("$" + count + " calculated total without shipping");
            count += Double.Parse(priceShipping[0].Text.Substring(1));

            //showing calculated price and the price shown on the page
            Console.WriteLine("$" + count + " calculated total with shipping");
            var priceTotal = driver.FindElements(By.XPath("//*[contains(@id,'total_price')]"));
            Console.WriteLine("$" + priceTotal[0].Text.Substring(1) + " is displayed total price");
        }

        [TestMethod]
        public void TestMethod5()// Test five adds four items then removes the most expensive
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://automationpractice.com");

            //retreives a list of all items you can add to the cart, then adds the first four
            var links = driver.FindElements(By.XPath("//*[contains(@class,'ajax_add_to_cart_button')]"));
            for (int i = 0; i < 4; i++)
            {
                driver.Navigate().GoToUrl(links[i].GetAttribute("href"));
                driver.Navigate().Back();
            }

            //navigates back to the cart, then retreives the price of all items in cart
            driver.Navigate().GoToUrl("http://automationpractice.com/index.php?controller=order");
            var prices = driver.FindElements(By.XPath("//*[contains(@class,'price')][contains(@id,'total_product_price')]"));
            double count = 0;
            double highest = 0;
            int higestIndex = 0;

            //finding the highest priced item, and also adding prices together
            for (int i = 0; i < prices.Count; i++)
            {
                try
                {
                    if (Double.Parse(prices[i].Text.Substring(1)) > highest)
                    {
                        highest = Double.Parse(prices[i].Text.Substring(1));
                        higestIndex = i;
                    }
                    count += Double.Parse(prices[i].Text.Substring(1));
                }
                catch { }
            }

            //deletes the most expensive item from the cart then removes its cost from the calculated total
            var deletelinks = driver.FindElements(By.XPath("//*[contains(@title,'Delete')]"));
            count -= highest;
            driver.Navigate().GoToUrl(deletelinks[higestIndex].GetAttribute("href"));

            //adds shipping cost into calculation and displays calculated value vs shown total cost
            var priceShipping = driver.FindElements(By.XPath("//*[contains(@class,'price')][contains(@id,'total_shipping')]"));
            Console.WriteLine("$" + count + " calculated total without shipping");
            count += Double.Parse(priceShipping[0].Text.Substring(1));
            Console.WriteLine("$" + count + " calculated total with shipping");
            var priceTotal = driver.FindElements(By.XPath("//*[contains(@id,'total_price')]"));
            Console.WriteLine("$" + priceTotal[0].Text.Substring(1) + " is displayed total price");
        }


        [TestMethod]
        public void TestMethod6() //test 5 opens trademe then outputs all internal links that contain property or services
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.trademe.co.nz/a/");

            //finding all links on the page
            var propAndServ = driver.FindElements(By.TagName("a"));

            //loop through links, checking that they match the criteria of being internal, property, services
            foreach(var link in propAndServ)
            {
                var href = link.GetAttribute("href");
                if(href == null)
                {
                    continue;
                }
                //outputs links to console
                if (href.Contains("https://www.trademe.co.nz") && href.Contains("property") || href.Contains("services"))
                {
                    Console.WriteLine(href);
                }

            }

        }

    }
    
}
