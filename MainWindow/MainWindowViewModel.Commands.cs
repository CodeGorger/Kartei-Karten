﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;

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

        public ICommand LoadCsvCommand { get; set; }
        private bool canLoadCsv(object parameter)
        {
            return true;
        }

        // Opens file dialogs and loads the progress in same directory if wished.
        private void loadCsvAndStartSession(object parameter)
        {
            // Ask for importable CSV (wordlist to learn)
            string csv_file = FileHelper.AskForFile("csv files (*.csv)|*.csv|All files (*.*)|*.*");
            if ("" == csv_file)
            {
                System.Diagnostics.Debug.WriteLine("File search dialog canceled.");
                return;
            }

            // Load CSV
            var loadedCsv = FileHelper.ImportWordlistCsv(csv_file);
            if (!loadedCsv.Item1)
            {
                System.Diagnostics.Debug.WriteLine("Loading wordlist (csv file) failed.");
                return;
            }

            _progress_dir = Path.GetDirectoryName(csv_file);

            _questionManager.ImportQuestionAndAnswerList(loadedCsv.Item2);
            _questionManager.StartTrainingSession();
            SessionNumber = _questionManager.GetSessionNumber();

            _setNextQna();
        }

        public ICommand LoadProgressCommand { get; set; }
        private bool canLoadProgress(object parameter)
        {
            return true;
        }
        private void selectLoadProgressAndStartSession(object parameter)
        {
            // Ask for loadable progess KKP (KKP = alibi CSV/progress file)
            string progress_file = FileHelper.AskForFile("kkp files (*.kkp)|*.kkp|All files (*.*)|*.*");
            if ("" == progress_file)
            {
                System.Diagnostics.Debug.WriteLine("File search dialog canceled.");
                return;
            }
            LoadProgressAndStartSession(progress_file);
        }

        private string _progress_dir;

        public void LoadProgressAndStartSession(string progress_file)
        {
            // Load KKP
            var loadedProgress = FileHelper.LoadProgress(progress_file);
            if (!loadedProgress.Item1)
            {
                System.Diagnostics.Debug.WriteLine("Loading progress (kkp file) failed.");
                return;
            }
            _progress_dir = Path.GetDirectoryName(progress_file); 

            AddNewRecentFile(progress_file);
            _questionManager.SetProgress(progress_file, loadedProgress.Item2, loadedProgress.Item3);

            if (_questionManager.ProgressFinished())
            {
                MessageBoxResult reset_progress =
                    MessageBox.Show(
                        "Congratulations, you have learnt all cards!\nDo you wish to reset the progress?",
                        "Congratulation, Reset?",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                // Already loaded by progress file
                if (reset_progress == MessageBoxResult.Yes)
                {
                    _questionManager.ResetProgress();
                }
                else
                {
                    return;
                }
            }

            // This must be a loop
            // If in session five the only remaining open questions
            // will be put in box 5 (ie mod 10), then there 
            // are no questions for session 6 but only for 10.
            // sessions will be skipped until we arive that box.
            for(int i=0; i<10; i++ )
            {
                _questionManager.StartTrainingSession();
                SessionNumber = _questionManager.GetSessionNumber();
                if (_questionManager.QuestionsLeft())
                {
                    if(i>0)
                    {
                        MessageBox.Show("Fast forwarded " + i + " sessions.");
                    }
                    // normal path
                    _setNextQna();
                    break;
                }
            }
        }

        private void _setNextQna()
        {
            var qna = _questionManager.NextQuestionAndAnswer();
            bool is_reversed = qna.GetReversed();
            if(is_reversed)
            {
                QuestionText = qna.GetAnswer();
                AnswerText = qna.GetQuestion();
                IsAudioAvailable = false;
                IsAudioReversedAvailable = _tryLoadSoundFile(qna.GetAuestionAudio());
            }
            else
            {
                QuestionText = qna.GetQuestion();
                AnswerText = qna.GetAnswer();
                IsAudioAvailable = _tryLoadSoundFile(qna.GetAuestionAudio());
                IsAudioReversedAvailable = false;
            }
            CardsLeft = _questionManager.GetCardsLeft();
            MainProgramState = ProgramState.question_state;
            CardsBoxOrigin = _questionManager.GetCardBox(is_reversed);
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
            _sound.Play();
        }

        public ICommand AboutPressedCommand { get; set; }
        private bool canAboutPressedCommand(object parameter)
        {
            return true;
        }
        private void aboutPressedCommand(object parameter)
        {
            MessageBox.Show("Flashcard Learning\nBy Simon Poschenrieder\nMIT License");
        }
        

    }
}
