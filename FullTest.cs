using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace MySeleniumTests
{
    [TestFixture]
    public class FullTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized --scale"); //аргументы хром.драйвера при запуске из консоли
            driver = new ChromeDriver(options); //инициировали драйвер с опциями

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5)); //инициалиация явного ожидания
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); //неявное ожидание
        }
        [Test]
        public void MyFirstFullTest()
        {            
            driver.Navigate().GoToUrl("https://labirint.ru/");
            //Наводим на пункт "Книги" (локатор: booksMenu)
            var booksMenu = By.XPath("//*[@data-toggle='header-genres']"); 
            new Actions(driver).MoveToElement(driver.FindElement(booksMenu)).Build().Perform();

            var allBooks = By.XPath("//div[@id='header-genres']//a[@href='/books/']");
            wait.Until(ExpectedConditions.ElementIsVisible(allBooks)); //Дожидаемся показа "Все книги" (локатор: allBooks)            
            new Actions(driver).Click(driver.FindElement(allBooks)).Build().Perform(); //Кликаем по "Все книги" (локатор: allBooks)            
            Assert.AreEqual("https://www.labirint.ru/books/", driver.Url, "Не перешел на страницу https://www.labirint.ru/books/"); //Проверяем, что перешли на url https://www.labirint.ru/books/

            var allBooklnCart = By.XPath("//div[contains(@class, 'product ')]//a[contains(@id, 'buy')]"); //поправил, чтобы class тоже искался по contains
            new Actions(driver).MoveToElement(driver.FindElement(allBooklnCart)).Click(driver.FindElement(allBooklnCart)).Build().Perform();            
            
            var issueOrder = By.XPath("(//a[contains(@class,'buy-link')][contains(@class,'btn-more')])[1]"); //оформить
            new Actions(driver).MoveToElement(driver.FindElement(issueOrder)).Click(driver.FindElement(issueOrder)).Build().Perform();

            var beginOrder = By.CssSelector("input#basket-default-begin-order"); //Начать оформление
            wait.Until(ExpectedConditions.ElementIsVisible(beginOrder));
            new Actions(driver).Click(driver.FindElement(beginOrder)).Build().Perform();

            var chooseCourierDelivery = By.CssSelector("label.b-radio-delivery-courier span.b-radio-e-bg");
            new Actions(driver).Click(driver.FindElement(chooseCourierDelivery)).Build().Perform();

            var city = By.CssSelector("div.js-dlform-wrap input.js-district");
            new Actions(driver).SendKeys(driver.FindElement(city), "Qwqqqqqq").Build().Perform();
            new Actions(driver).SendKeys(Keys.Tab).Build().Perform();
            var cityError = By.CssSelector("span[id*=jsdistrict] span.b-form-error-e-text"); 
            Assert.IsTrue(driver.FindElement(cityError).Displayed, "Сообщение об ошибке, при вводе некорретных данных в поле город не показывается");
            driver.FindElement(city).Clear();

            new Actions(driver).SendKeys(driver.FindElement(city), "Екатеринбург").Build().Perform();            
            var suggestedCity = By.XPath("//a[contains(@class, 'suggests-item-link')]//*[contains(@class,'suggests-item-txt')]"); 
            new Actions(driver).Click(driver.FindElement(suggestedCity)).Build().Perform();
            
            var street = By.CssSelector("div.js-dlform-wrap input.js-street"); 
            new Actions(driver).SendKeys(driver.FindElement(street), "Тестовая").Build().Perform();

            var building = By.CssSelector("div.js-dlform-wrap input.js-building"); 
            new Actions(driver).SendKeys(driver.FindElement(building), "11").Build().Perform();

            var flat = By.CssSelector("div.js-dlform-wrap input[name*=flat]"); 
            new Actions(driver).SendKeys(driver.FindElement(flat), "716").Build().Perform();

            var loadingPanel = By.CssSelector("loading-panel");
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(loadingPanel));

            var dataHandler = By.CssSelector("[data-handler=selectDay]:nth-child(5)");
            new Actions(driver).Click(driver.FindElement(dataHandler)).Build().Perform();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(loadingPanel));

            var confirm = By.XPath("//div[contains(@class, 'js-dlform-wrap')]//input[@type='submit']"); 
            new Actions(driver).MoveToElement(driver.FindElement(confirm)).Click().Build().Perform();

            var courierDeliveryLightbox = By.CssSelector("div.js-dlform-wrap");   
            Assert.IsFalse(driver.FindElement(courierDeliveryLightbox).Displayed, "Отображается окно курьерской доставки");
        }

        [Test]
        public void MyFirstSelectTest()
        {
            driver.Navigate().GoToUrl("https://www.labirint.ru/guestbook/");
            driver.FindElement(By.Id("aone")).Click();

            driver.FindElement(By.CssSelector("#a_writer [name=name]")).SendKeys("Тестировщик");
            driver.FindElement(By.CssSelector("#a_writer [name=email]")).SendKeys("test@test.ru");
            driver.FindElement(By.Id("send_answer")).Click();
            driver.FindElement(By.Id("guesttextarea")).SendKeys("Тестовое сообщение");
            driver.FindElement(By.Id("guesttextarea")).Clear();

            var selectElement = driver.FindElement(By.Name("theme"));
            var themeSelector = new SelectElement(selectElement);
            themeSelector.SelectByText("Приятные слова");
            Assert.AreEqual("Приятные слова", themeSelector.SelectedOption.Text, "Селектор выбрал не тот элемент обращения");

            driver.FindElement(By.Id("hd")).Click();
            var displayedLightBox = driver.FindElement(By.Id("notForGuestbook")).Displayed;
            Assert.IsTrue(displayedLightBox, "Не отобразился лайтбокс");
        }

        [Test]
        public void MyAdvancedInteractionsApi()
        {
            driver.Navigate().GoToUrl("https://www.labirint.ru/");
            var linkMyLabirint = By.ClassName("js-b-autofade-wrap");
            var linkIntoLabirint = By.CssSelector(".dropdown-block-opened [data-sendto='registration'].b-menu-list-title");
            var registrationLightbox = By.CssSelector("#registration .authorize-main");

            new Actions(driver)
                .MoveToElement(driver.FindElement(linkMyLabirint))
                .Build().
                Perform();

            wait.Until(ExpectedConditions.ElementIsVisible(linkIntoLabirint));
            driver.FindElement(linkIntoLabirint).Click();
            Assert.IsTrue(driver.FindElement(registrationLightbox).Displayed, "Не видно Lightbox");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver = null; //потому что найти null reference exeption найти быстрее в коде
        }
    }
}


