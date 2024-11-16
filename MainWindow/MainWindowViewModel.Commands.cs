using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Microsoft.CognitiveServices.Speech;

namespace KarteiKartenLernen
{
    public partial class MainWindowViewModel
    {
        
        public ICommand RevealCommand { get; set; }
        private bool canRevealCard(object parameter)
        {
            return true;
        }
        private void revealCard(object parameter)
        {
            MainProgramState = ProgramState.answer_state;
        }

        public ICommand KnewItCommand { get; set; }
        private bool canKnewIt(object parameter)
        {
            return true;
        }
        private void knewIt(object parameter)
        {
            bool is_last_question = _questionManager.KnewIt();
            if (is_last_question)
            {
                AddNewRecentFile(_questionManager.GetProgressFileName());
                SessionNumber = "";
                MainProgramState = ProgramState.inactive_state;
            }
            else
            {
                _setNextQna();
            }
        }

        public ICommand BoringCommand { get; set; }
        private void boringQuestion(object parameter)
        {
            bool is_last_question = _questionManager.Boring();
            if (is_last_question)
            {
                AddNewRecentFile(_questionManager.GetProgressFileName());
                SessionNumber = "";
                MainProgramState = ProgramState.inactive_state;
            }
            else
            {
                _setNextQna();
            }
        }

        public ICommand DidntKnowItCommand { get; set; }
        private bool canDidntKnowIt(object parameter)
        {
            return true;
        }
        private void didntKnowIt(object parameter)
        {
            _questionManager.DidntKnowIt();
            _setNextQna();
        }

        //public ICommand LoadCsvCommand { get; set; }
        //private bool canLoadCsv(object parameter)
        //{
        //    return true;
        //}

        //// Opens file dialogs and loads the progress in same directory if wished.
        //private void loadCsvAndStartSession(object parameter)
        //{
        //    // Ask for importable CSV (wordlist to learn)
        //    string csv_file = FileHelper.AskForFile("csv files (*.csv)|*.csv|All files (*.*)|*.*");
        //    if ("" == csv_file)
        //    {
        //        System.Diagnostics.Debug.WriteLine("File search dialog canceled.");
        //        return;
        //    }

        //    // Ask if reverse order shall be asked too.
        //    MessageBoxResult result_ask_reversed = MessageBox.Show(
        //        "Also ask reversed direction? (Take the answer as question.)", 
        //        "Reversed Order?", 
        //        MessageBoxButton.YesNo, 
        //        MessageBoxImage.Question);
            

        //    // Load CSV
        //    var loadedCsv = FileHelper.ImportWordlistCsv(csv_file);
        //    if (!loadedCsv.Item1)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Loading wordlist (csv file) failed.");
        //        return;
        //    }

        //    _progress_dir = Path.GetDirectoryName(csv_file);

        //    _questionManager.ImportQuestionAndAnswerList(loadedCsv.Item2, (result_ask_reversed != MessageBoxResult.Yes));
        //    _questionManager.StartTrainingSession();
        //    SessionNumber = _questionManager.GetSessionNumber();

        //    _setNextQna();
        //}

        public ICommand LoadProgressCommand { get; set; }
        private bool canLoadProgress(object parameter)
        {
            return true;
        }
        private void selectLoadSessionProgressAndStartSession(object parameter)
        {
            // Ask for loadable progess KP2 (KP2 = alibi CSV/progress file)
            string progress_file = FileHelper.AskForFile("kp2 files (*.kp2)|*.kp2|All files (*.*)|*.*");
            if ("" == progress_file)
            {
                System.Diagnostics.Debug.WriteLine("File search dialog canceled.");
                return;
            }
            LoadSessionProgress(progress_file);
        }

        private string _progress_dir;

        public void LoadSessionProgress(string progress_file)
        {
            // Load KP2
            var loadedProgress = FileHelper.LoadSessionProgress(progress_file);
            if (!loadedProgress.Item1)
            {
                System.Diagnostics.Debug.WriteLine("Loading progress (kp2 file) failed.");
                return;
            }
            _progress_dir = Path.GetDirectoryName(progress_file); 

            AddNewRecentFile(progress_file);
            _questionManager.SetProgress(progress_file, loadedProgress.Item2);

            _questionManager.StartTrainingSession();
            _setNextQna();


            //TODO the rest was commented out
            //if (_questionManager.ProgressFinished())
            //{
            //    MessageBoxResult reset_progress =
            //        MessageBox.Show(
            //            "Congratulations, you have learnt all cards!\nDo you wish to reset the progress?",
            //            "Congratulation, Reset?",
            //            MessageBoxButton.YesNo,
            //            MessageBoxImage.Question);
            //    // Already loaded by progress file
            //    if (reset_progress == MessageBoxResult.Yes)
            //    {
            //        _questionManager.ResetProgress();
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            // This must be a loop
            // If in session five the only remaining open questions
            // will be put in box 5 (ie mod 10), then there 
            // are no questions for session 6 but only for 10.
            // sessions will be skipped until we arive that box.
            //for (int i = 0; i < 100; i++)
            //{
            //    _questionManager.StartTrainingSession();
            //    SessionNumber = _questionManager.GetSessionNumber();
            //    if (_questionManager.QuestionsLeft())
            //    {
            //        if (i > 0)
            //        {
            //            MessageBox.Show("Fast forwarded " + i + " sessions.");
            //        }
            //        // normal path
            //        _setNextQna();
            //        break;
            //    }
            //}
        }

        private void _setNextQna()
        {
            string sound_file="";
            var qna = _questionManager.NextQuestionAndAnswer();
            Question = qna.GetQuestionCardSide(
                _questionManager.GetProgressFileNameBaseDir(),
                _questionManager.GetSoundDir(),
                _tts_config,
                out sound_file);
            if(sound_file!="")
            {
                _questionManager.SetSoundFile(qna.GetCardId(), sound_file);
            }
            Answer = qna.GetAnswerCardSide(
                _questionManager.GetProgressFileNameBaseDir(),
                _questionManager.GetSoundDir(),
                _tts_config,
                out sound_file);
            if (sound_file != "")
            {
                _questionManager.SetSoundFile(qna.GetCardId(), sound_file);
            }
            OtherSidesText = qna.GetOtherCardSideText();
            MainProgramState = ProgramState.question_state;

            CardsLeft = _questionManager.GetCardsLeft();
            CardsBoxOrigin = _questionManager.GetCardBox();
        }

        private SoundWrapper _sound;

        private bool _tryLoadSoundFile(string sound_file)
        {
            try
            {
                if(sound_file.EndsWith(".wav"))
                {
                    _sound = new SoundWrapper(_progress_dir + "/" + sound_file);
                    return true;
                }
                else if(sound_file.EndsWith(".mp3"))
                {
                    _sound = new SoundWrapper(_progress_dir + "/" + sound_file);
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
            }

            _sound = null;
            return false;
        }

        public ICommand SpeakerPressedCommand { get; set; }
        private bool canSpeakerPressed(object parameter)
        {
            return true;
        }
        private void speakerPressed(object parameter)
        {
            if(Answer.HasAudio)
            {
                _tryLoadSoundFile(Answer.AudioFile);
            }
            else if(Question.HasAudio)
            {
                _tryLoadSoundFile(Question.AudioFile);
            }

            _sound?.Play();
        }

        public ICommand AboutPressedCommand { get; set; }
        private bool canAboutPressedCommand(object parameter)
        {
            return true;
        }
        private void aboutPressedCommand(object parameter)
        {
            MessageBox.Show("Flashcard Learning\nBy Simon Poschenrieder\nMIT License", "About");
        }
        

    }
}
