using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                        string sound_file = values[2].Trim();
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

        static public bool CheckForProgessFile(string filePath)
        {
            if (!filePath.EndsWith(".csv"))
            {
                return false;
            }
            string progressFile = filePath.Replace(".csv", ".kkp");
            return File.Exists(progressFile);
        }


        //static public (bool, List<int>) LoadProgressForKnownCsv(string filePath)
        //{
        //    List<int> ret_progress = new List<int>();
        //    bool ret_status = false;

        //    if (!filePath.EndsWith(".csv"))
        //    {
        //        return (ret_status, ret_progress);
        //    }
        //    string progressFile = filePath.Replace(".csv", ".kkp");

        //    return LoadProgress(progressFile);
        //}


        static public (bool, int, List<(string, string, string, int, int)>) LoadProgress(string filePath)
        {
            List<(string, string, string, int, int)> ret_progress = new List<(string, string, string, int, int)>();
            bool ret_status = false;
            int session_id = 0;
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    if (!int.TryParse(reader.ReadLine(), out session_id))
                    {
                        System.Diagnostics.Debug.WriteLine("Conversion failed session_id.");
                        return (false, session_id, ret_progress);
                    }
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(';');
                        
                        string question = values[0].Trim();
                        string answer = values[1].Trim();
                        string sound_file = values[2].Trim();

                        if (!int.TryParse(values[3], out int box_id))
                        {
                            System.Diagnostics.Debug.WriteLine("Conversion failed box_id.");
                            ret_status = false;
                            break;
                        }
                        if (!int.TryParse(values[4], out int reverse_box_id))
                        {
                            System.Diagnostics.Debug.WriteLine("Conversion failed reverse_box_id.");
                            ret_status = false;
                            break;
                        }
                        ret_progress.Add((question, answer, sound_file, box_id, reverse_box_id));
                    }
                    ret_status = true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                ret_status = false;
            }

            return (ret_status, session_id, ret_progress);
        }

        static public void SaveProgress(string filePath, int session_id, List<(string, string, string, int, int)> progress)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(session_id);
                    foreach(var c in progress)
                    {
                        writer.WriteLine(c.Item1 + ";" + c.Item2 + ";" + c.Item3 + ";" + c.Item4 + ";" + c.Item5);
                    }
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
