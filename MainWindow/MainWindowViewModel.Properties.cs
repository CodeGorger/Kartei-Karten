using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarteiKartenLernen
{
    public partial class MainWindowViewModel
    {
        private ProgramState _programState;
        public ProgramState MainProgramState
        {
            set
            {
                _programState = value;
                OnPropertyChanged(nameof(IsInactive));
                OnPropertyChanged(nameof(IsActivated));
                OnPropertyChanged(nameof(IsQuestionState));
                OnPropertyChanged(nameof(IsResponseState));
            }
        }

        public bool IsInactive
        {
            get => (_programState == ProgramState.inactive_state);
        }

        public bool IsActivated
        {
            get => (_programState != ProgramState.inactive_state);
        }

        public bool IsQuestionState
        {
            get => (_programState == ProgramState.question_state);
        }
     
        public bool IsResponseState
        {
            get => (_programState == ProgramState.answer_state);
        }

        private string _questionText = "";
        public string QuestionText
        {
            get => _questionText;
            set
            {
                _questionText = value;
                OnPropertyChanged(nameof(QuestionText));
            }
        }

        private string _answerText = "";
        public string AnswerText
        {
            get => _answerText;
            set
            {
                _answerText = value;
                OnPropertyChanged(nameof(AnswerText));
            }
        }

        private string _sessionNumber = "";
        public string SessionNumber
        {
            get => _sessionNumber;
            set
            {
                _sessionNumber = value;
                OnPropertyChanged(nameof(SessionNumber));
            }
        }

        private string _cardsLeft = "";
        public string CardsLeft
        {
            get => _cardsLeft;
            set
            {
                _cardsLeft = value;
                OnPropertyChanged(nameof(CardsLeft));
            }
        }

        private string _cardsBoxOrigin = "";
        public string CardsBoxOrigin
        {
            get => _cardsBoxOrigin;
            set
            {
                _cardsBoxOrigin = value;
                OnPropertyChanged(nameof(CardsBoxOrigin));
            }
        }

        private ObservableCollection<RecentFileViewModel> _recentFiles = new ObservableCollection<RecentFileViewModel>();
        public ObservableCollection<RecentFileViewModel> RecentFiles
        {
            get => _recentFiles;
        }

        private bool _audioAvailable = true;
        public bool IsAudioAvailable
        {
            get => _audioAvailable;
            set
            {
                _audioAvailable = value;
                OnPropertyChanged(nameof(IsAudioAvailable));
            }
        }
    }
}

