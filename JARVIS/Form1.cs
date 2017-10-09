using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;


namespace JARVIS
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }
        SpeechSynthesizer s = new SpeechSynthesizer();
        Boolean wake = true;
        Boolean math_add = false;

        string temp;
        string condition;
        string high;
        string low;


        private void Form1_Shown(object sender, EventArgs e)
        {
            

            s.SelectVoice("Microsoft Pavel Mobile");
            //s.SpeakAsync("привет");
            s.Rate = 3;
            s.SpeakAsync("1");


            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ru-RU");
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);


            Choices Words = new Choices();
            Words.Add(new string[] {
                "привет", "здравствуй","здравствуйте", "добрый день","дратути","дратути","дороу","однако зравствуйте","ку","как дела","как настроение", "чё как",
                "Сколько время","Сколько времяни", "what is today?", "open google", "turn off", "wake up",
                "restart", "update", "open steam", "close steam", "сложи числа","скажите первое число","скажите второе число", "прогноз погоды",
                "погода", "какая на сегодня погода"
            });

            
            //Words.Add(File.ReadAllLines(@"C:\Users\direc\Desktop\Numbers.txt"));

            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = ci;
            gb.Append(Words);



            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);

            sre.RecognizeAsync(RecognizeMode.Multiple);





            Console.WriteLine("Installed voices -");
            foreach (InstalledVoice voice in s.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                Console.WriteLine(" Voice Name: " + info.Name);
            }
        }

        public String GetWeather(String input)
        {
            String query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='los gatos, ca')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");

            XmlDocument wData = new XmlDocument();
            wData.Load(query);

            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("query/results/channel");
            try
            {
                temp = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;
                condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
                high = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["high"].Value;
                low = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"].Value;
                if (input == "cond")
                {
                    return condition;
                }
                if (input == "temp")
                {
                    return temp;
                    
                }
                if (input == "high")
                {
                    return high;
                }
                if (input == "low")
                {
                    return low;
                }
                
            }
            catch
            {
                return "Error Reciving data";
            }
            return "error";
        }


        public void say(String h)
        {
            s.SpeakAsync(h);
        }

        public void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;

            //What you say
            if (r == "wake up")
            {
                say("Good Morning!");
                wake = true;
            }

            if (r == "turn off")
            {
                say("Ok, sleeping now!");
                wake = false;
            }




            /////////
            //Weather
            /////////
            {
                if (r == "прогноз погоды" || r == "погода" || r == "какая на сегодня погода" )
                {
                    Console.WriteLine(condition);
                    //What it says
                    say("Сегодня будет" + GetWeather("cond"));// + "Средняя температура" + GetWeather("temp") + "градусов" + "Температура днем:" + GetWeather("high") + "градусов" + "Температура ночью: " + GetWeather("low") + "градусов");
                }

            }





                //What you say
                if (r == "Открой (ПРОГРАММА)")
            {
                //What it says
                say("Opening App");
                Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
            }


            //What you say
            if (r == "close steam")
            {
                //What it says
                say("Closing App");
                SendKeys.Send("% +{F4}");
            }



            /////////////
            //Приветствия
            /////////////
            {
                //What you say
                if (r == "привет" || r == "здравствуй" || r == "здравствуйте" || r == "добрый день" || r == "дратути" || r == "дороу" || r == "однако зравствуйте" || r == "ку")
                {
                    //What it says
                    say("Добрый день, хозяин.");
                }


                //What you say
                if (r == "как дела" || r == "как настроение" || r == "че как")
                {
                    //What it says
                    say("Всё в норме. Состояние материнской платы - отличное. Процессор в порядке. А как у вас?");
                }
            }

            ////////////
            //Mathmatics
            ////////////
            {
                int line1 = 0;
                int line2 = 0;


                if (r == "сложи числа")
                {

                    math_add = true;
                    wake = false;

                }

                if (math_add == true)
                {
                    say("скажите первое число");
                    line1 = int.Parse(r);
                    say("Скажите второе число");
                    line2 = int.Parse(r);
                    double output;

                    output = line1 + line2;
                    string OutputText;
                    OutputText = output.ToString();

                    say(OutputText);
                    wake = true;
                    math_add = false;
                    
                }
            }


            //What you say
            if (r == "Сколько времяни" || r == "Сколько время")
            {
                //What it says
                say(DateTime.Now.ToString("h:mm tt"));
            }


            //What you say
            if (r == "what is today?")
            {
                //What it says
                say(DateTime.Now.ToString("M/d/yyyy"));
            }


            //What you say
            if (r == "open google")
            {
                //What it says
                say("OK");
                Process.Start("https://google.com");
            }
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


    }
}