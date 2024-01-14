﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarteiKartenLernen
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        private QuestionManager _questionManager;
        public MainWindowViewModel()
        {
            _questionManager = new QuestionManager();

            RevealCommand = new Command(
                revealCard,
                canRevealCard);

            KnewItCommand = new Command(
                knewIt,
                canKnewIt);

            DidntKnowItCommand = new Command(
                didntKnowIt,
                canDidntKnowIt);

            LoadCsvCommand = new Command(
                loadCsvAndStartSession,
                canLoadCsv);

            LoadProgressCommand = new Command(
                loadProgressAndStartSession,
                canLoadProgress);

            LoadRecentFiles();
        }

        ~MainWindowViewModel()
        {
            //SaveRecentFiles();
        }

        public Action Close { get; set; }

        public void LoadRecentFiles()
        {
            try
            {
                using (StreamReader reader = new StreamReader("./recent_files_memory.txt"))
                {
                    int count = 0;
                    while (!reader.EndOfStream)
                    {
                        RecentFiles.Add(reader.ReadLine());
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
                using (StreamWriter write = new StreamWriter("./recent_files_memory.txt"))
                {
                    int count = 0;
                    foreach(var f in RecentFiles)
                    {
                        write.WriteLine(f);
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
                if(new_recent_file== RecentFiles[i])
                {
                    return;
                }
            }
            RecentFiles.Add(new_recent_file);
            if (RecentFiles.Count>=7)
            {
                RecentFiles.RemoveAt(0);
            }
        }
    }
}
