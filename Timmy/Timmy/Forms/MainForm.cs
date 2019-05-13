﻿using IBatisNet.DataMapper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timmy.Forms;

namespace Timmy
{
    public partial class MainForm : Form
    {
        public IWebDriver driver;
        public ChromeDriverService ser = ChromeDriverService.CreateDefaultService();
        public SpeechSynthesizer ss;
        IList<Site> list = Mapper.Instance().QueryForList<Site>("SelectSite", null);

        public MainForm()
        {
            InitializeComponent();
        }

        private void ttsButton_Click(object sender, EventArgs e)
        {
            internet("naver.com");
            ss = new SpeechSynthesizer();
            string txt = txtView.Text;
            TTS tts = new TTS();
            tts.tts(txt);
            
            if (txt.Contains("켜"))
            {
                //새 탭으로 여는거 추가요망
                for (int i = 0; i < list.Count; i++)
                {
                    if (txt.Contains(list[i].siteName))
                    {
                        ss.SpeakAsync(list[i].siteName + "실행");
                        internet(list[i].url);
                        Console.WriteLine("hello");
                        //이거 바꾸면 됨
                    }
                }
            }
            else if (txt.Contains("꺼"))
            {
                ss.SpeakAsync("인터넷 종료");
                driver.Quit();
            }
            else if (txt.Contains("이동"))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (txt.Contains(list[i].siteName))
                    {
                        ss.SpeakAsync("인터넷 이동");
                        internet(list[i].url);
                    }
                }
            }
            if (txt.Contains("검색"))
            {
                ss.SpeakAsync(txt);
                if (driver.Url.Contains("naver"))
                    naversearch(txt.Replace("검색", ""));
                else if (driver.Url.Contains("google"))
                    googleSearch(txt.Replace("검색", ""));
            }
        }

        public void internet(string url)
        {
            ser.HideCommandPromptWindow = true;

            if (driver == null)
                driver = new ChromeDriver(ser, new ChromeOptions());


            driver.Manage().Window.Maximize();
            driver.Url = ("https://www." + url + "/");

        }
        //구글 검색
        public void googleSearch(string searchword)
        {
            try
            {
                IWebElement q = driver.FindElement(By.Name("q"));
                q.Clear();
                q.SendKeys(searchword);
                q.Submit();
                var wt = driver.FindElement(By.XPath("//*[@id='rhs_block']/div/div[1]/div/div[1]/div[2]/div[2]/div/div[1]/div/div"));
                resultbox.Text = wt.Text;
                ss.SpeakAsync(resultbox.Text);
            }
            catch (Exception e)
            {

            }

            if (searchword.Contains("날씨"))
            {
                IWebElement q = driver.FindElement(By.Name("q"));
                q.Clear();
                q.SendKeys(searchword);
                q.Submit();

                var wt = driver.FindElement(By.CssSelector(".vk_gy.vk_h"));
                resultbox.Text = wt.Text;
                ss.SpeakAsync(resultbox.Text);
            }
        }


        // 네이버 검색
        public void naversearch(string searchword)
        {
            if (searchword.Contains("날씨"))
            {
                Thread.Sleep(1000);
                IWebElement q = driver.FindElement(By.Id("query"));
                q.SendKeys(searchword);
                driver.FindElement(By.Id("search_btn")).Click();
                var wt = driver.FindElement(By.ClassName("cast_txt"));
                resultbox.Text = wt.Text;
                ss.SpeakAsync(resultbox.Text);
            }
            else if (searchword.Contains("최신노래"))
            {
                IWebElement q = driver.FindElement(By.Id("query"));
                q.SendKeys(searchword);
                driver.FindElement(By.Id("search_btn")).Click();
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
                    resultbox.AppendText(i + "위  " + sing + "  -  " + song + Environment.NewLine);
                }
            }
            else
            {
                try
                {
                    IWebElement q = driver.FindElement(By.Id("query"));
                    q.SendKeys(searchword);
                    q.Submit();

                }
                catch (Exception e)
                {
                    IWebElement q = driver.FindElement(By.Id("nx_query"));
                    q.Clear();
                    q.SendKeys(searchword);
                    q.Submit();

                }
            }
        }

        private void tsmLogin_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            SetLoginForm settingForm = new SetLoginForm();
            settingForm.Show();
            
        }

        private void tsmSite_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            SetSiteForm listForm = new SetSiteForm();
            listForm.Show();
        }

        private void tsmHelp_Click(object sender, EventArgs e)
        {
            HelpForm helpForm = new HelpForm();
            helpForm.Show();
        }

        private void tsmInfo_Click(object sender, EventArgs e)
        {
            InformationForm infoForm = new InformationForm();
            infoForm.Show();
        }

        private void btnSpeechStart_Click(object sender, EventArgs e)
        {
            STT.StreamingMicRecognizeAsync(5);
            this.txtView.Text += STT.resultText + "\r\n";
        }
    }
}
