﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;


namespace KarteiKartenLernen
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        public class AzureConfig
        {
            public string azure_key { get; set; }
            public string location { get; set; }
        }

        static AzureConfig LoadAzureConfig(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<AzureConfig>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or parsing {filePath}: {ex.Message}");
                return null;
            }
        }

        void SynthesizeAudioAsync()
        {
            var config = LoadAzureConfig("azure.json");
            if(config!=null && config.azure_key != null && config.location != null)
            {
                _tts_config = SpeechConfig.FromSubscription(config.azure_key, config.location);
                _tts_config.SpeechSynthesisLanguage = "zh-CN";
            }
        }

        private SpeechConfig _tts_config;

        private QuestionManager _questionManager;
        public MainWindowViewModel()
        {
            SynthesizeAudioAsync();

            _questionManager = new QuestionManager();

            RevealCommand = new Command(
                revealCard,
                (object p) => { return true; });

            KnewItCommand = new Command(
                knewIt,
                (object p) => { return true; });

            BoringCommand = new Command(
                boringQuestion,
                (object p) => { return true; });

            DidntKnowItCommand = new Command(
                didntKnowIt,
                (object p) => { return true; });

            //LoadCsvCommand = new Command(
            //    loadCsvAndStartSession,
            //    (object p) => { return true; });

            LoadProgressCommand = new Command(
                selectLoadSessionProgressAndStartSession,
                (object p) => { return true; });

            SpeakerPressedCommand = new Command(
                speakerPressed,
                (object p) => { return true; });

            AboutPressedCommand = new Command(
                aboutPressedCommand,
                (object p) => { return true; });

            LoadRecentFiles();
        }

        ~MainWindowViewModel()
        {
            //SaveRecentFiles();
        }

        public Action Close { get; set; }

        private static string _recent_files_mem_path = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "flashcardlearning", "recent_files_memory.txt");


        public void LoadRecentFiles()
        {
            try
            {
                using (StreamReader reader = new StreamReader(_recent_files_mem_path))
                {
                    int count = 0;
                    while (!reader.EndOfStream)
                    {
                        RecentFiles.Add(new RecentFileViewModel(reader.ReadLine(), this));
                        count++;
                        if (count >= 7)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
            }
        }

        public void SaveRecentFiles()
        {
            try
            {
                string dir = Path.GetDirectoryName(_recent_files_mem_path);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (StreamWriter write = new StreamWriter(_recent_files_mem_path))
                {
                    int count = 0;
                    foreach(var f in RecentFiles)
                    {
                        write.WriteLine(f.FileName);
                        count++;
                        if (count >= 7)
                        {
                            break;
                        }
                    }
                    write.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
            }
        }

        public void AddNewRecentFile(string new_recent_file)
        {
            // Check if it's already in the list, because then ignore it.
            for(int i=0; i< RecentFiles.Count; i++)
            {
                if(new_recent_file== RecentFiles[i].FileName)
                {
                    return;
                }
            }
            RecentFiles.Add(new RecentFileViewModel(new_recent_file, this));
            if (RecentFiles.Count>=7)
            {
                RecentFiles.RemoveAt(0);
            }
        }
    }
}
