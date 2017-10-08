﻿using System;
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
                "restart", "update", "open steam", "close steam", "посчитай числа","скажите математическую функцию вы хотите исполнить","сложение",
                "скажите первое число","скажите второе число"
            });

            Choices Numbers = new Choices();
            Words.Add(File.ReadAllLines(@"C:\Users\direc\Desktop\Numbers.txt"));

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




            //if (wake == true)
            //{


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


                if (r == "посчитай числа")
                {
                    say("скажите математическую функцию вы хотите исполнить");
                    if (r == "сложение")
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
                    }

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