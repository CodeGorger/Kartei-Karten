using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace KarteiKartenLernen
{
    class FileHelper
    {
        static public string AskForFile(string filter = "", bool is_save = false)
        {
            string returnFilePath = "";
            if (!is_save)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (filter != "")
                {
                    openFileDialog.Filter = filter;
                }

                if (openFileDialog.ShowDialog() == true)
                {
                    returnFilePath = openFileDialog.FileName;
                    // Process the selected file
                }
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (filter != "")
                {
                    saveFileDialog.Filter = filter;
                }

                if (saveFileDialog.ShowDialog() == true)
                {
                    returnFilePath = saveFileDialog.FileName;
                    // Process the selected file
                }
            }
            return returnFilePath;
        }


        static public (bool, List<(string, string, string)>) ImportWordlistCsv(string filePath)
        {
            List<(string, string, string)> ret_wordlist = new List<(string, string, string)>();
            bool ret_status = false;
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(';'); // Split the line by comma

                        string question = values[0].Trim();
                        string answer = values[1].Trim();
                        string sound_file = "";
                        if(values.Length>2)
                        {
                            sound_file = values[2].Trim();
                        }
                        ret_wordlist.Add((question, answer, sound_file));
                    }
                    ret_status = true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
            }
            return (ret_status, ret_wordlist);
        }


        //static public (bool, int, List<(string, string, string, int, int)>) LoadSessionProgress(string filePath)
        static public (bool, SessionAndProgress) LoadSessionProgress(string filePath)
        {
            SessionAndProgress ret_progress = new SessionAndProgress();
            bool ret_status = false;
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string json_content = reader.ReadToEnd();
                    ret_progress = JsonConvert.DeserializeObject<SessionAndProgress>(json_content);
                                        ret_status = true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                ret_status = false;
            }

            return (ret_status, ret_progress);
        }

        static public void SaveProgress(string filePath, SessionAndProgress progress)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    string json_content = JsonConvert.SerializeObject(progress, Formatting.Indented);
                        
                    writer.Write(json_content); 
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
            }
        }
    }
}
