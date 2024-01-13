using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            _programState = ProgramState.answer_state;
        }

        public ICommand KnewItCommand { get; set; }
        private bool canKnewIt(object parameter)
        {
            return true;
        }
        private void knewIt(object parameter)
        {
            bool is_last_question=_questionManager.KnewIt();
            if(is_last_question)
            {
                SessionNumber = "";
                _programState = ProgramState.inactive_state;
            }
            else
            {
                var qna = _questionManager.NextQuestionAndAnswer();
                QuestionText = qna.Item1;
                AnswerText = qna.Item2;
                _programState = ProgramState.inactive_state;
                CardsLeft =_questionManager.GetCardsLeft();
                CardsBoxOrigin = _questionManager.GetCardBox();
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
            var qna = _questionManager.NextQuestionAndAnswer();
            QuestionText = qna.Item1;
            AnswerText = qna.Item2;
            _programState = ProgramState.question_state;
            CardsBoxOrigin = _questionManager.GetCardBox();
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
            string csv_file=FileHelper.AskForFile("csv files (*.csv)|*.csv|All files (*.*)|*.*");
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
            _questionManager.ImportQuestionAndAnswerList(loadedCsv.Item2);
            _questionManager.StartTrainingSession();

            // for now we have flash cards to ask for. if a progress will be loaded
            // later (in this routine or by user action) we can just call 
            //NextQuestionAndAnswer again.
            var qna = _questionManager.NextQuestionAndAnswer();
            QuestionText = qna.Item1;
            AnswerText = qna.Item2;
            _programState = ProgramState.question_state;
            CardsLeft = _questionManager.GetCardsLeft();
            CardsBoxOrigin = _questionManager.GetCardBox();
        }

        public ICommand LoadProgressCommand { get; set; }
        private bool canLoadProgress(object parameter)
        {
            return true;
        }
        private void loadProgressAndStartSession(object parameter)
        {
            // Ask for loadable progess KKP (KKP = alibi CSV/progress file)
            string progress_file = FileHelper.AskForFile("kkp files (*.kkp)|*.kkp|All files (*.*)|*.*");
            if ("" == progress_file)
            {
                System.Diagnostics.Debug.WriteLine("File search dialog canceled.");
                return;
            }

            // Load KKP
            var loadedProgress = FileHelper.LoadProgress(progress_file);
            if (loadedProgress.Item1)
            {
                System.Diagnostics.Debug.WriteLine("Loading progress (kkp file) failed.");
                return;
            }
            _questionManager.SetProgressFile(progress_file);
            _questionManager.SetTrainingSessionId(loadedProgress.Item2);
            _questionManager.SetProgress(loadedProgress.Item3);
            var qna = _questionManager.NextQuestionAndAnswer();
            QuestionText = qna.Item1;
            AnswerText = qna.Item2;
            _programState = ProgramState.question_state;
            CardsLeft = _questionManager.GetCardsLeft();
            CardsBoxOrigin = _questionManager.GetCardBox();
            _questionManager.StartTrainingSession();
        }
    }
}
