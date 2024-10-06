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

        private CardSide _question;
        public CardSide Question
        {
            get => _question;
            set
            {
                _question = value;
                OnPropertyChanged(nameof(Question));
            }
        }

        private CardSide _answer;
        public CardSide Answer
        {
            get => _answer;
            set
            {
                _answer = value;
                OnPropertyChanged(nameof(Answer));
            }
        }

        private string _other_sides_text;
        public string OtherSidesText
        {
            get => _other_sides_text;
            set
            {
                _other_sides_text = value;
                OnPropertyChanged(nameof(OtherSidesText));
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

        private bool _hasQuestionString = false;
        public bool HasQuestionString
        {
            get => _hasQuestionString;
            set
            {
                _hasQuestionString = value;
                OnPropertyChanged(nameof(HasQuestionString));
            }
        }

        //private bool _hasQuestionAudio = false;
        //public bool HasQuestionAudio
        //{
        //    get => _hasQuestionAudio;
        //    set
        //    {
        //        _hasQuestionAudio = value;
        //        OnPropertyChanged(nameof(HasQuestionAudio));
        //    }
        //}

        private bool _hasAnswerString = false;
        public bool HasAnswerString
        {
            get => _hasAnswerString;
            set
            {
                _hasAnswerString = value;
                OnPropertyChanged(nameof(HasAnswerString));
            }
        }

        //private bool _hasAnswerAudio = false;
        //public bool HasAnswerAudio
        //{
        //    get => _hasAnswerAudio;
        //    set
        //    {
        //        _hasAnswerAudio = value;
        //        OnPropertyChanged(nameof(HasAnswerAudio));
        //    }
        //}
    }
}

