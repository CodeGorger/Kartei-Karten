using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace KarteiKartenLernen
{
    class QuestionManager
    {
        public QuestionManager()
        {
            _box_repeat_iterations = new List<int>()
                { -1, 1, 1, 3, 5, 10, -1 };

            _box_one_max_count = 30;
            _new_card_promotion_count = 15;
            _progress_file = "";
            _knew_cards_count = 0;

            _open_question_ids = new List<int>();
            _finished_question_ids = new List<int>();
        }

        // information about how often (every how many training sessions) shall a box be repeated
        // 0:   new pile
        // 1-n: learning piles
        // n+1:   known pile
        private List<int> _box_repeat_iterations;

        // a list of tuples,
        // tuple consists of (all) questions, answer and box
        private List<FlashCard> _cards_and_progress;

        // a counter for the session id, to know which boxes must be learnt
        private int _training_session_id;
        private int _knew_cards_count;
        private int _max_questions;        

        private int _box_one_max_count;
        private int _new_card_promotion_count;

        // If a session started
        private List<int> _open_question_ids;
        private List<int> _finished_question_ids;

        private string _progress_file;

        private List<QuestionAnswerSet> _all_qna_list;
        private List<QuestionDirection> _question_directions;
        List<Side> _sides;

        // After loading a session, the QuestionManager must know the progress
        // Will load all possible questions into the qna_list
        public void SetProgress(
            string in_progress_file,
            SessionAndProgress in_session_progress)
        {
            _progress_file = in_progress_file;

            _max_questions = in_session_progress._max_questions;
            _box_repeat_iterations = in_session_progress._bin_repetition;
            // Box with id 0 is the untouched cards bin
            _box_repeat_iterations.Insert(0, -1);
            // Box with last id is the done cards
            _box_repeat_iterations.Add(-1);
            _box_one_max_count = in_session_progress._max_fillup_size;
            _new_card_promotion_count = in_session_progress._new_card_limit;
            _training_session_id = in_session_progress._session_counter;

            _cards_and_progress = new List<FlashCard>();

            // For each card&progress
            for (int i = 0; i < in_session_progress._progress.Count; i++)
            {
                List<QuestionProgress> tmp_progress = in_session_progress._progress[i];
                List<string> tmp_card = in_session_progress._cards[i];


                List<QuestionProgress> tmp_casted_progresses = new List<QuestionProgress>();
                foreach (var p in tmp_progress)
                {
                    tmp_casted_progresses.Add(p);
                }

                _cards_and_progress.Add(new FlashCard(
                    tmp_card.Select(o => o.ToString()).ToList(),
                    tmp_casted_progresses));

            }

            _question_directions = in_session_progress._question_directions;
            _sides = in_session_progress._sides;

            _all_qna_list = new List<QuestionAnswerSet>();

            for (int card_id = 0;
                card_id < _cards_and_progress.Count;
                card_id++)
            {
                var f = _cards_and_progress[card_id];
                for (int question_direction_id = 0;
                    question_direction_id < f.GetProgresses().Count;
                    question_direction_id++)
                {
                    var tmp_question_progress = f.GetProgresses()[question_direction_id];
                    QuestionDirection qd = _question_directions[question_direction_id];

                    Side tmp_question_side = _sides[qd._from];
                    Side tmp_answer_side = _sides[qd._to];

                    List<string> tmp_question_values = new List<string>();
                    List<string> tmp_question_component_datatype = new List<string>();
                    foreach (FieldData field in tmp_question_side._fields)
                    {
                        tmp_question_values.Add(
                            _cards_and_progress[card_id].GetSides()[field._id_side_component]);
                        tmp_question_component_datatype.Add(
                            field._type);
                    }

                    List<string> tmp_answer_values = new List<string>();
                    List<string> tmp_answer_component_datatype = new List<string>();
                    foreach (FieldData field in tmp_answer_side._fields)
                    {
                        tmp_answer_values.Add(
                            _cards_and_progress[card_id].GetSides()[field._id_side_component]);
                        tmp_answer_component_datatype.Add(
                            field._type);
                    }


                    QAEntity tmp_question = new QAEntity(
                        tmp_question_values,
                        tmp_question_component_datatype,
                        tmp_question_side._name,
                        tmp_question_side._icon);

                    QAEntity tmp_answer = new QAEntity(
                        tmp_answer_values,
                        tmp_answer_component_datatype,
                        tmp_answer_side._name,
                        tmp_answer_side._icon);

                    string tmp_othersides_text = get_othersides_text(card_id, qd._from, qd._to);


                    QuestionAnswerSet tmp_qna_set = new QuestionAnswerSet(
                        tmp_question,
                        tmp_answer,
                        tmp_othersides_text,
                        card_id,
                        tmp_question_progress._bin,
                        tmp_question_progress._next_session,
                        question_direction_id);
                    _all_qna_list.Add(tmp_qna_set);
                }
            }
        }

        private string get_othersides_text(int in_card_id, int in_from, int in_to)
        {
            string ret="";
            Side tmp_question_side = _sides[in_from];
            Side tmp_answer_side = _sides[in_to];

            List<int> used_fields = new List<int>();
            for(int i=0;i<_sides.Count;i++)
            {
                if(i == in_from || i == in_to)
                {
                    continue;
                }
                foreach(var f in _sides[i]._fields)
                {
                    if (f._type == "string")
                    {
                        ret += _cards_and_progress[in_card_id].GetSides()[f._id_side_component]+" ";
                    }
                }
            }

            return ret;
        }


        public List<QuestionDirection> GetQuestionDirections()
        {
            return _question_directions;
        }

        public void StartTrainingSession()
        {
            _knew_cards_count = 0;
            _training_session_id++;
            int box_one_count = _box_count(1);
            if (box_one_count < _box_one_max_count)
            {
                int fillup_count = _box_one_max_count - box_one_count;
                _fillup_box_one(fillup_count);
            }

            _open_question_ids = new List<int>();
            _finished_question_ids = new List<int>();

            // Go through all cards and check if the
            // (correct direction) question must be added 
            for (int i = 0; i < _all_qna_list.Count; i++)
            {
                QuestionAnswerSet qa = _all_qna_list[i];
                if (!(0 < qa.GetBinId() && qa.GetBinId() < _box_repeat_iterations.Count-1)) 
                {
                    continue;
                }

                if (_training_session_id == qa.GetNextSession())
                {
                    _open_question_ids.Add(i);
                }
            }
            _open_question_ids.Shuffle();
        }

        public bool ProgressFinished()
        {
            for (int i = 0; i < _all_qna_list.Count; i++)
            {
                //TODO commented out for now
                //if (_qna_list[i].box_id != 6)
                //{
                //    return false;
                //}
            }
            return true;
        }

        private int _box_count(int to_consider_box_id)
        {
            int ret_count = 0;
            foreach (var qna in _all_qna_list)
            {
                if (qna.GetBinId() == to_consider_box_id)
                {
                    ret_count++;
                }
            }
            return ret_count;
        }

        // Will promote new (unseen) cards to the first box for learning.
        private void _fillup_box_one(int fillup_space_remaining)
        {
            Random random = new Random();

            int fillup_count = _new_card_promotion_count;
            if (fillup_count > fillup_space_remaining)
            {
                fillup_count = fillup_space_remaining;
            }

            int new_cards_remaining = _box_count(0);
            if (fillup_count > new_cards_remaining)
            {
                fillup_count = new_cards_remaining;
            }


            System.Diagnostics.Debug.WriteLine("fillup_count: "+ fillup_count);
            
            // For each card that shall be filled up
            for (int i = 0; i < fillup_count; i++)
            {
                //System.Diagnostics.Debug.WriteLine("i: " + i);
                // Take a random location in the qna_list
                // and go through the list until you find one promotable obj.
                int random_start_id = random.Next(0, _all_qna_list.Count - 1);

                // Still maximally go through the list once, just for (no) bug purposes
                for (int j = 0; j < _all_qna_list.Count+1; j++)
                {
                    //System.Diagnostics.Debug.WriteLine("j: " + j);

                    if (0 == _all_qna_list[(j + random_start_id) % _all_qna_list.Count].GetBinId())
                    {
                        _all_qna_list[(j + random_start_id) % _all_qna_list.Count].SetBinId(1);
                        _all_qna_list[(j + random_start_id) % _all_qna_list.Count].SetNextSession(
                            _training_session_id);
                        break;
                    }
                }
            }
        }

        public QuestionAnswerSet NextQuestionAndAnswer()
        {
            if (_open_question_ids.Count == 0)
            {
                return new QuestionAnswerSet(new QAEntity(), new QAEntity(), "", 0, 0, 0, 0);
            }
            return _all_qna_list[_open_question_ids[0]];
        }

        public string GetCardsLeft()
        {
            return "Cards left: " + _open_question_ids.Count.ToString();
        }

        public bool QuestionsLeft()
        {
            return (_open_question_ids.Count > 0);
        }

        public void ResetProgress()
        {
            _training_session_id = 0;
            //TODO commented out for now
            //for (int j = 0; j < _qna_list.Count; j++)
            //{
            //    _qna_list[j].box_id = 0;
            //}
        }

        public string GetCardBox()
        {
            if(_open_question_ids.Count==0 || _all_qna_list.Count == 0)
            {
                return "";
            }

            return "Card's box: "+ _all_qna_list[_open_question_ids[0]].GetBinId();

        }

        public string GetSessionNumber()
        {
            return "Session number " + _training_session_id;
        }

        public bool Boring()
        {
            _all_qna_list[_open_question_ids[0]].SetNextSession(0);
            _all_qna_list[_open_question_ids[0]].SetBinId(_box_repeat_iterations.Count-1);

            _finished_question_ids.Add(_open_question_ids[0]);
            _open_question_ids.RemoveAt(0);

            if (_open_question_ids.Count == 0)
            {
                _clean_up();
                return true;
            }
            return false;
        }               
        
        public bool KnewIt()
        {
            _knew_cards_count++;
            if (!_all_qna_list[_open_question_ids[0]].WasDemoted())
            {
                // In this session, this question has never been demoted...
                _all_qna_list[_open_question_ids[0]].SetNextBinId();
                int new_bin_id = _all_qna_list[_open_question_ids[0]].GetBinId();
                _all_qna_list[_open_question_ids[0]].SetNextSession(
                    _training_session_id + _box_repeat_iterations[new_bin_id]);
            }

            _finished_question_ids.Add(_open_question_ids[0]);
            _open_question_ids.RemoveAt(0);

            if (_open_question_ids.Count == 0 || _knew_cards_count >= _max_questions)
            {
                _clean_up();
                return true;
            }
            return false;
        }

        public void DidntKnowIt()
        {
            //Demoting causes next session repetion
            _all_qna_list[_open_question_ids[0]].SetNextSession(_training_session_id + 1);
            _all_qna_list[_open_question_ids[0]].Demote();

            if (1 == _open_question_ids.Count)
            {
                return;
            }

            // Reinsert at random location
            int tmp_id = _open_question_ids[0];
            _open_question_ids.RemoveAt(0);
            Random rng = new Random();
            int new_insert_id = rng.Next(1, _open_question_ids.Count+1);
            //System.Diagnostics.Debug.WriteLine("new_insert_id:" + new_insert_id);
            _open_question_ids.Insert(new_insert_id, tmp_id);
        }

        private void _clean_up()
        {
            foreach(int id in _open_question_ids)
            {
                // All open questions must be postponed to next session.
                _all_qna_list[id].SetNextSession(_all_qna_list[id].GetNextSession() + 1);
            }

            if ("" != _progress_file)
            {
                // Already loaded by progress file
                MessageBoxResult result_know_prog_file = MessageBox.Show(
                    "Save to " + _progress_file + "?", 
                    "Save Progress?", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);
                if (result_know_prog_file == MessageBoxResult.Yes)
                {
                    SessionAndProgress tmp_session_progress_to_save =
                        _create_session_and_progress();
                    FileHelper.SaveProgress(_progress_file, tmp_session_progress_to_save);
                    return;
                }
            }

            MessageBoxResult result = MessageBox.Show("Save progress to file?", "Save Progress?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Only possible if it was imported
                string kp2_file = FileHelper.AskForFile("kp2 files (*.kp2)|*.kp2|All files (*.*)|*.*", true);
                _progress_file = kp2_file;
                SessionAndProgress tmp_session_progress_to_save =
                    _create_session_and_progress();
                FileHelper.SaveProgress(_progress_file, tmp_session_progress_to_save);
            }
        }

        private SessionAndProgress _create_session_and_progress()
        {
            SessionAndProgress ret = new SessionAndProgress();
            ret._bin_repetition = _box_repeat_iterations;
            ret._bin_repetition.RemoveAt(0);
            ret._bin_repetition.Reverse();
            ret._bin_repetition.RemoveAt(0);
            ret._bin_repetition.Reverse();

            ret._max_questions = _max_questions;
            ret._max_fillup_size = _box_one_max_count;
            ret._new_card_limit = _new_card_promotion_count;
            ret._session_counter = _training_session_id;

            ret._question_directions = _question_directions;
            ret._sides = _sides;

            int qd_count = _question_directions.Count();
            ret._cards = new List<List<string>>();
            ret._progress = new List<List<QuestionProgress>>();
            for (int card_id = 0;
                card_id < _cards_and_progress.Count;
                card_id++)
            {
                ret._cards.Add(_cards_and_progress[card_id].GetSides());
                List<QuestionProgress> tmp_progress = new List<QuestionProgress>();
                for (int qd_id = 0; qd_id < qd_count; qd_id++)
                {
                    tmp_progress.Add(new QuestionProgress(
                        _all_qna_list[card_id * qd_count + qd_id].GetBinId(),
                        _all_qna_list[card_id * qd_count + qd_id].GetNextSession()));
                }
                ret._progress.Add(tmp_progress);
            }
            return ret;
        }


        public string GetProgressFileName()
        {
            return _progress_file;
        }

        //TODO Progress will be saved very different soon...
        private List<(string, string, string, int, int)> _toProgressCsv()
        {
            List < (string, string, string, int, int) > ret = new List<(string, string, string, int, int)>();

            foreach(var q in _all_qna_list)
            {
                //TODO Dummy entry for now
                //ret.Add((q.question, q.answer, q.sound_file, q.box_id, q.reverse_box_id));
                ret.Add(("","","",0,0));
            }

            return ret;
        }
    }

}
