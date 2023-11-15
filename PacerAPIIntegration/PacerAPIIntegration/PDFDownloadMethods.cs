/* 
 * Title: Pacer API Integration class
 * Authors: Carlos Gross-Martinez 
 * Description: Downloads specified docket document with Pacer link
 */

//library import to be used in methods
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using System.Threading;
using WindowsInput.Native;
using WindowsInput;
//using System.Windows.Input;

public class PDFRetrival
{
    //testing function for downloading docket file
    public int SavePDF(string url)
        {
            string fileName = @"C:\Users\cagrossmartinez\source\repos\PacerAPIIntegration\PacerAPIIntegration\Credentials.txt";

            var CredentialContent = File.ReadAllLines(fileName);

            string pacerUserName = CredentialContent[0];

            string pacerPassword = CredentialContent[1];




            var driver = new EdgeDriver();

            driver.Navigate().GoToUrl(url);

            driver.Manage().Window.Maximize();

            var userName = driver.FindElement(By.Id("loginForm:loginName"));
            userName.SendKeys(pacerUserName);

            var password = driver.FindElement(By.Id("loginForm:password"));
            password.SendKeys(pacerPassword);
            
            var login = driver.FindElement(By.Id("loginForm:fbtnLogin"));
            login.Click();            

            Thread.Sleep(5000);



            var ReportLink = driver.FindElements(By.TagName("a"));

            Thread.Sleep(5000);

            ReportLink[13].Click();



            var runReport = driver.FindElement(By.Name("button1"));

            Thread.Sleep(5000);

            runReport.Click();


            var docketLinks = driver.FindElements(By.TagName("a"));

            Thread.Sleep(5000);

            docketLinks[5].Click();

            Thread.Sleep(5000);

            
            var viewDocument = driver.FindElement(By.CssSelector("input[type='submit']"));

            Thread.Sleep(5000);

            viewDocument.Click();



            Thread.Sleep(5000);

            Actions savePDF = new Actions(driver);

            savePDF.KeyDown(Keys.Control);

            savePDF.SendKeys("s").Build().Perform();

            savePDF.KeyUp(Keys.Control).Build().Perform();

            Thread.Sleep(5000);



            InputSimulator keyPress = new InputSimulator();

            keyPress.Keyboard.KeyPress(VirtualKeyCode.RETURN);

            Thread.Sleep(5000);

            driver.Close();

            return 1;         
        }

    //method that downloads a specific docket file from Pacer search results URL
    public int SaveMultiplePDFs(string url, int docketNumber)
        {
            //saving path where text file with login credentials for pacer are located
            string fileName = @"C:\Users\cagrossmartinez\source\repos\PacerAPIIntegration\PacerAPIIntegration\Credentials.txt";

            //reading information from credential text file
            var CredentialContent = File.ReadAllLines(fileName);

            //opening browser, navigating to URL, and maximizing browser window
            var driver = new EdgeDriver();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
            Thread.Sleep(2000);

            //entering username and password to page and logging in
            driver.FindElement(By.Id("loginForm:loginName")).SendKeys(CredentialContent[0]);
            driver.FindElement(By.Id("loginForm:password")).SendKeys(CredentialContent[1]);
            driver.FindElement(By.Id("loginForm:fbtnLogin")).Click();
            Thread.Sleep(2000);

            //identifying the link for the docket reports and clicking on it
            var ReportLink = driver.FindElements(By.TagName("a"));
            ReportLink[13].Click();
            Thread.Sleep(2000);

            //identifying button to run reports and clicking on it
            driver.FindElement(By.Name("button1")).Click();
            Thread.Sleep(2000);           

            //idetifying all the links with in the page and saving to variable
            var docketLinks = driver.FindElements(By.TagName("a"));
            var index = 0;

            //loop that finds the index of the first link that has a doc to download
            for (index = 5; index < docketLinks.Count; index++)
                {
                    //clickin on links to see if naviation occurs to next page
                    docketLinks[index].Click();
                    Thread.Sleep(2000);

                    //identifying if a new page with a document link was attained
                    try { driver.FindElement(By.CssSelector("input[type='submit']")); }
                    catch (Exception) { continue; }

                    //navigating top rior page if valid docket link and obtaining all the links from page 
                    driver.Navigate().Back();
                    Thread.Sleep(2000);
                    docketLinks = driver.FindElements(By.TagName("a"));
                    break;
                }
            
            //adjustment of index value to click on the proper docket file
            index += docketNumber - 1;

            //clicking on the proper doc to download and navigating to the page
            docketLinks[index].Click();
            Thread.Sleep(2000);

            //indetifying the button to open the document on browser and clicking on it
            driver.FindElement(By.CssSelector("input[type='submit']")).Click();
            Thread.Sleep(3000);

            //code that sends the "ctrl + s" key combination to open save dialoge box in browser
            Actions savePDF = new Actions(driver);
            savePDF.KeyDown(Keys.Control);
            savePDF.SendKeys("s").Build().Perform();
            savePDF.KeyUp(Keys.Control).Build().Perform();            
            Thread.Sleep(3000);

            //pressing the "Enter" key to select save in dialoge box and download file
            InputSimulator key = new InputSimulator();                    
            key.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            Thread.Sleep(3000);

            //code that closes the browser
            driver.Close();

            //return value if class if process completed successfully
            return 1;
        }



}