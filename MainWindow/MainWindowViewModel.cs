using System;
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
                selectLoadProgressAndStartSession,
                canLoadProgress);

            SpeakerPressedCommand = new Command(
                speakerPressed,
                canSpeakerPressed);

            AboutPressedCommand = new Command(
                aboutPressedCommand,
                canAboutPressedCommand);

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
