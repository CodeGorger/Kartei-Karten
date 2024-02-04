using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarteiKartenLernen
{
    class QuestionAnswerSet
    {
        private string _answer;
        private string _question;
        private string _question_audio;
        private bool _reversed;

        public QuestionAnswerSet(
            string in_question,
            string in_answer,
            string in_question_audio,
            bool in_reversed)
        {
            _question = in_question;
            _answer = in_answer;
            _question_audio = in_question_audio;
            _reversed = in_reversed;
        }

        public string GetQuestion() { return _question; }
        public string GetAnswer() { return _answer; }
        public string GetAuestionAudio() { return _question_audio; }
        public bool GetReversed() { return _reversed; }
    }
}
