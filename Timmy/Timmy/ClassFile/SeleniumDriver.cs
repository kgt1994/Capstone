﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using IBatisNet.DataMapper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Timmy.Forms;
using OpenQA.Selenium.Interactions;

namespace Timmy.ClassFile
{
    public class SeleniumDriver
    {
        private static SeleniumDriver selenium;
        MainForm main;
        Animation ani;
        public delegate void dgtSetBox(string result);
        string siteName;

        private SeleniumDriver()
        {
            Thread t1 = new Thread(new ThreadStart(Run));
            t1.Start();
        }

        public static SeleniumDriver Instance
        {
            get
            {
                if (selenium == null)
                {
                    selenium = new SeleniumDriver();
                }
                return selenium;
            }
        }


        public ChromeDriver driver;
        public ChromeDriverService ser = ChromeDriverService.CreateDefaultService();
        public SpeechSynthesizer ss;
        IList<Site> list = Mapper.Instance().QueryForList<Site>("SelectSite", null);


        [DllImport("kernel32.dll")]
        public static extern bool Beep(int n, int m);
        // 도 = 256Hz
        // 레 = 도 * 9/8 = 288Hz
        // 미 = 레 * 10/9 = 320Hz
        // 파 = 미 * 16/15 = 341.3Hz
        // 솔 = 파 * 9/8 = 384Hz
        // 라 = 솔 * 10/9 = 426.6Hz
        // 시 = 라 * 9/8 = 480Hz
        // 도 = 시 * 16/15 = 512Hz (= 처음 도의 2배)
        // 2배 = 높은음, 1/2배 = 낮은음

        public static string input = "";

        public void setInput(string intxt)
        {
            input += intxt;
            Console.WriteLine(input + "setText");

        }
        string result = "";


        public void googlelogin(string id, string pw)
        {

            try
            {
                driver.Url = ("https://www.google.com/");
                driver.FindElement(By.XPath("//*[@id='gb_70']")).Click();
                Thread.Sleep(1000);
                driver.FindElement(By.Id("identifierId")).SendKeys(id);
                Thread.Sleep(1000);
                driver.FindElement(By.XPath("//*[@id='identifierNext']/span/span")).Click();
                Thread.Sleep(1000);
                driver.FindElement(By.Name("password")).SendKeys(pw);
                Thread.Sleep(1500);
                driver.FindElement(By.XPath("//*[@id='passwordNext']/span/span")).Click();
            }
            catch (Exception)
            {
                MessageBox.Show("아이디와 비번이 입력되지 않았습니다");

            }

            input = "";
        }
        public void daumlogin(string id, string pw)
        {
            try
            {
                driver.Url = ("https://logins.daum.net/accounts/signinform.do?url=https%3A%2F%2Fwww.daum.net%2F");
                Thread.Sleep(1000);
                driver.FindElement(By.Id("id")).SendKeys(id);
                driver.FindElement(By.Name("pw")).SendKeys(pw);
                driver.FindElement(By.XPath("//*[@id='loginBtn']")).Click();
            }
            catch (Exception)
            {
                MessageBox.Show("아이디와 비번이 입력되지 않았습니다");

            }
            input = "";
        }
        public void daummail()
        {
            try
            {
                driver.FindElement(By.XPath("//*[@id='mArticle']/div[1]/div[2]/ul/li[1]/a")).Click();
                Thread.Sleep(2000);
                var unread = driver.FindElement(By.ClassName("count_unread"));
                var mail = driver.FindElement(By.ClassName("count_total"));
                string a = ("총" + mail.Text + "개의 메일중" + unread.Text + "개의 읽지않은 메일이 있습니다");
                ss.SpeakAsync(a);
            }
            catch (Exception)
            {

                ss.SpeakAsync("먼저 로그인을 해주세요");
            }
            input = "";
        }
        public void googlemail()
        {
            try
            {
                string a;
                driver.Url = ("https://mail.google.com/");
                Thread.Sleep(3000);
                var list = driver.FindElements(By.ClassName("yW"));
                var cont = list.Count;
                result += "총 " + cont + "개의 메일이 있습니다" + Environment.NewLine;
                for (int i = 0; i < cont; i++)
                {
                    var q = list[i];
                    result += q.Text + Environment.NewLine;
                }
                input = "";
            }
            catch (Exception e)
            {
                ss.SpeakAsync("먼저 로그인을 해주세요");
            }
        }
        public void close(string url) // 탭 끄기
        {
            try
            {
                for (int i = 0; i < driver.WindowHandles.Count(); ++i)
                {
                    if (driver.SwitchTo().Window(driver.WindowHandles.ElementAt(i)).Url.Contains(url))
                    {
                        driver.SwitchTo().Window(driver.WindowHandles.ElementAt(i));
                        driver.Close();
                    }
                }
                if (driver.WindowHandles.Count() != 0)
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            input = "";
        }
        public void swit(string url) //탭 이동
        {
            try
            {
                for (int i = 0; i < driver.WindowHandles.Count(); i++)
                {
                    if (driver.SwitchTo().Window(driver.WindowHandles.ElementAt(i)).Url.Contains(url))
                    {
                        driver.SwitchTo().Window(driver.WindowHandles.ElementAt(i));
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            input = "";
        }

        public void internet(string url)
        {
            if (main == null)
                main = (MainForm)Singleton.getMainInstance();
            if (ani == null)
                ani = (Animation)Singleton.getAnimationInstance();
            Console.WriteLine("new ChromeDriver");
            ser.HideCommandPromptWindow = true;
            try
            {
                Process[] pro = Process.GetProcessesByName("chrome");

                if (driver == null)
                {
                    driver = new ChromeDriver(ser, new ChromeOptions());
                    driver.Manage().Window.Maximize();
                    driver.Url = ("https://www." + url + "/");
                    input = "";
                }
                else
                {
                    try
                    {
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Navigate().GoToUrl("https://www." + url + "/");
                        input = "";
                    }
                    catch (Exception)
                    {
                        driver.Quit();
                        driver = new ChromeDriver(ser, new ChromeOptions());
                        driver.Manage().Window.Maximize();
                        driver.Url = ("https://www." + url + "/");
                        input = "";
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                MessageBox.Show(e.ToString());
                Console.WriteLine("셀레니움 드라이버 생성 오류");
            }
            input = "";
        }
        //구글 검색
        public void googleSearch(string searchword)
        {
            if (main.resultbox.InvokeRequired)
            {
                IWebElement q = driver.FindElementByName("q");
                q.Clear();
                if (searchword.Contains("날씨"))
                {
                    try
                    {
                        q.SendKeys(searchword);
                        Actions builder = new Actions(driver);
                        builder.SendKeys(OpenQA.Selenium.Keys.Enter);

                        var wt = driver.FindElement(By.CssSelector(".vk_gy.vk_h"));
                        var wt2 = driver.FindElement(By.XPath("//*[@id='wob_tm']"));
                        ss.SpeakAsync(wt.Text + "현재온도 " + wt2.Text + "도");
                        result = wt.Text + "현재온도 " + wt2.Text + "도";
                        input = "";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    input = "";

                }
                else
                {
                    try
                    {
                        q.SendKeys(searchword);
                        Actions builder = new Actions(driver);
                        builder.SendKeys(OpenQA.Selenium.Keys.Enter);
                        var wt = driver.FindElement(By.XPath("//*[@id='rhs']/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div/div[1]/div/div/div/div/span"));
                        result = wt.Text;
                        ss.SpeakAsync(result);
                        input = "";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    input = "";
                }
                dgtSetBox dgt = new dgtSetBox(googleSearch);
                main.Invoke(dgt, new object[] { searchword });
            }
            else
            {
                main.resultbox.Text = result;
            }

            if(!result.Equals(""))
                ani.anitext(result);

            input = "";

        }




        // 네이버 검색
        public void naversearch(string searchword)
        {
            if (main.resultbox.InvokeRequired)
            {
                if (searchword.Contains("날씨"))
                {
                    Console.WriteLine("날씨 검색중");
                    Thread.Sleep(1000);
                    IWebElement q = driver.FindElementByName("query");
                    q.Clear();
                    q.SendKeys(searchword);
                    Actions builder = new Actions(driver);
                    builder.SendKeys(OpenQA.Selenium.Keys.Enter);
                    var wt = driver.FindElement(By.ClassName("cast_txt"));
                    result = wt.Text.Replace("˚ ", "도"); ;
                    ss.SpeakAsync(result);
                }
                else if (searchword.Contains("최신 노래"))
                {
                    result = "";
                    IWebElement q = driver.FindElementByName("query");
                    q.Clear();
                    q.SendKeys(searchword);
                    Actions builder = new Actions(driver);
                    builder.SendKeys(OpenQA.Selenium.Keys.Enter);
                    Thread.Sleep(3000);

                    var rank = driver.FindElement(By.ClassName("list_top_music"));
                    string song;
                    string sing;
                    for (int i = 1; i <= 10; i++)
                    {
                        var ranksong = rank.FindElement(By.XPath("//*[@id='main_pack']/div[2]/div[2]/ol/li[" + i + "]/div/div[1]/div[2]/div[1]/a"));
                        var ranksinger = rank.FindElement(By.XPath("//*[@id='main_pack']/div[2]/div[2]/ol/li[" + i + "]/div/div[1]/div[2]/div[2]/a[1]"));

                        song = ranksong.Text;
                        sing = ranksinger.Text;
                        result += i + "위  " + sing + "  -  " + song + Environment.NewLine;
                    }
                    ss.SpeakAsync(result);
                }
                else
                {
                    try
                    {
                        IWebElement q = driver.FindElement(By.CssSelector("[id$=query]"));
                        q.Clear();
                        q.SendKeys(searchword);
                        Actions builder = new Actions(driver);
                        builder.SendKeys(OpenQA.Selenium.Keys.Enter);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        MessageBox.Show(e.ToString());
                        Console.WriteLine("네이버 검색 오류");
                    }
                }
                dgtSetBox dgt = new dgtSetBox(googleSearch);
                main.Invoke(dgt, new object[] { searchword });
            }
            else
            {
                main.resultbox.Text = result;
            }
            ani.anitext(result);
            input = "";
        }
        public void daumSearch(string searchword)
        {

            try
            {
                IWebElement q = driver.FindElement(By.Name("q"));
                q.Clear();
                q.SendKeys(searchword);
                Actions builder = new Actions(driver);
                builder.SendKeys(OpenQA.Selenium.Keys.Enter);
                input = "";
            }
            catch (Exception e)
            {
                ss.SpeakAsync("결과값이 정확하지 않습니다");
            }
            input = "";

        }

        public void textChange()
        {
            if (input != "")
            {
                Beep(1280, 50);
                Beep(1024, 50);
            }

            ss = new SpeechSynthesizer();
            string txt = input;
            TTS tts = new TTS();
            tts.tts(txt);

            if (txt.Contains("Google"))
            {
                txt = txt.Replace("Google", "구글");
            }

            if (txt.Contains("켜") || txt.Contains("실행"))
            {
                if (txt.Contains("인터넷"))
                {
                    ss.SpeakAsync("인터넷 실행");
                    internet("google.com");
                }
                else
                {
                    int sitecount = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (txt.Contains(list[i].siteName))
                        {
                            ss.SpeakAsync(list[i].siteName + "실행");
                            internet(list[i].url);
                            if (!list[i].siteName.Equals(""))
                            {
                                sitecount++;
                            }
                        }
                        else if (sitecount == 0)
                        {
                            if (i == list.Count - 1)
                                ss.SpeakAsync("등록되지 않은 사이트 입니다.");

                        }
                    }
                }
            }
            else if (txt.Contains("꺼") || txt.Contains("종료"))
            {
                if (txt.Contains("인터넷"))
                {
                    ss.SpeakAsync("인터넷 종료");
                    driver.Quit();
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (txt.Contains(list[i].siteName))
                    {
                        ss.SpeakAsync(list[i].siteName + "종료");

                        close(list[i].url);
                    }
                }
            }
            else if (txt.Contains("이동"))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (txt.Contains(list[i].siteName))
                    {
                        ss.SpeakAsync("인터넷 이동");
                        swit(list[i].url);
                    }
                }
            }
            else if (txt.Contains("검색"))
            {
                ss.SpeakAsync(txt);
                if (driver.Url.Contains("naver"))
                {
                    naversearch(txt.Replace("검색", ""));
                    input = "";
                }
                else if (driver.Url.Contains("google"))
                {
                    googleSearch(txt.Replace("검색", ""));
                    input = "";
                }
                else if (driver.Url.Contains("daum"))
                {
                    daumSearch(txt.Replace("검색", ""));
                    input = "";
                }
                else
                {
                    ss.SpeakAsync("지원하지 않는 페이지 입니다.");
                }
            }
            else if (txt.Contains("로그인"))
            {

                if (txt.Contains("구글") || txt.Contains("Google"))
                {
                    siteName = "google";
                    IList<Login> listLogin = Mapper.Instance().QueryForList<Login>("SelectSiteLogin", siteName);
                    googlelogin(listLogin[0].id, listLogin[0].pw);
                    ss.SpeakAsync(txt);
                }
                else if (txt.Contains("다음"))
                {
                    siteName = "daum";
                    IList<Login> listLogin = Mapper.Instance().QueryForList<Login>("SelectSiteLogin", siteName);
                    daumlogin(listLogin[0].id, listLogin[0].pw);
                    ss.SpeakAsync(txt);
                }
                else
                {
                    MessageBox.Show("아이디,패스워드 추가요망");
                }
            }
            else if (txt.Contains("메일"))
            {
                if (txt.Contains("구글"))
                {
                    googlemail();
                    ss.SpeakAsync(txt);
                }
                else if (txt.Contains("다음"))
                {
                    daummail();
                    ss.SpeakAsync(txt);
                }
            }
            else if (txt.Equals("새로고침"))
            {
                driver.Navigate().Refresh();
            }
            else if (txt.Equals("앞으로"))
            {
                driver.Navigate().Forward();
            }
            else if (txt.Equals("뒤로"))
            {
                driver.Navigate().Back();
            }
            else if (txt.Contains("현재 페이지 등록"))
            {
                ss.SpeakAsync("무슨 이름으로 등록 할까요?");

                addSite(2);
            }
            else
            {
                ss.SpeakAsync("무슨 말인지 잘 모르겠네요");
                input = "";
            }

            async void addSite(int time)
            {
                await Task.Delay(time * 1000);
                input = "";
                stt();
                await Task.Delay(6 * 1000);
                ss.SpeakAsync(input + "로 등록 되었습니다.");

                try
                {
                    Site site = new Site();

                    site.siteName = input;
                    site.url = driver.Url;

                    Mapper.Instance().Insert("setSiteIns", site);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    input = "";
                }

            }
            input = "";
        }
        private void stt()
        {
            ss = new SpeechSynthesizer();

            STT.StreamingMicRecognizeAsync(5);
            ResultText(5);
        }

        async void ResultText(int time)
        {
            await Task.Delay(time * 1000);
            input += STT.resultText + "\r\n";
        }


        static void Run()
        {
            while (true)
            {
                if (!input.Equals(""))
                {
                    selenium.textChange();
                    Console.WriteLine(input + "스레드");
                }
            }
        }
    }
}