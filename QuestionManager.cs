﻿using System;
using System.Collections.Generic;
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
        }

        // information about how often (every how many training sessions) shall a box be repeated
        // 0:   new pile
        // 1-5: learning piles
        // 6:   known pile
        private List<int> _box_repeat_iterations;

        // a list of tuples,
        // tuple consists of question, answer and box
        private List<FlashCard> _qna_list;

        // a counter for the session id, to know which boxes must be learnt
        private int _training_session_id;

        private int _box_one_max_count;
        private int _new_card_promotion_count;

        // If a session started
        private List<int> _open_question_ids;
        private List<int> _finished_question_ids;

        private string _progress_file;

        public void ImportQuestionAndAnswerList(List<(string, string)> in_qna_list)
        {
            _training_session_id = 0;
            _qna_list = new List<FlashCard>();
            foreach (var qna in in_qna_list)
            {
                _qna_list.Add(new FlashCard(qna.Item1, qna.Item2));
            }
        }

        public void SetProgress(
            string in_progress_file, 
            int in_training_session_id, 
            List<(string, string, int)> in_progress)
        {
            _progress_file = in_progress_file;
            _training_session_id = in_training_session_id;
            _qna_list = new List<FlashCard>();
            foreach (var c in in_progress)
            {
                _qna_list.Add(new FlashCard(c.Item1, c.Item2, c.Item3));
            }
        }

        public void StartTrainingSession()
        {
            _training_session_id++;
            int box_one_count = _box_count(1);
            int box_two_count = _box_count(2);
            if ((box_one_count + box_two_count) >= _box_one_max_count)
            {
                return;
            }
            int fillup_count = _box_one_max_count - (box_one_count + box_two_count);
            _fillup_box_one(fillup_count);

            _open_question_ids = new List<int>();
            _finished_question_ids = new List<int>();
            for (int i = 0; i < _qna_list.Count; i++)
            {
                FlashCard c = _qna_list[i];
                if (!(0 < c.box_id && c.box_id < 6))
                {
                    continue;
                }
                
                if (0==(_training_session_id% _box_repeat_iterations[c.box_id]))
                {
                    _open_question_ids.Add(i);
                }
            }
            _open_question_ids.Shuffle();
        }

        private int _box_count(int to_consider_box_id)
        {
            int ret_count = 0;
            foreach (var qna in _qna_list)
            {
                if (qna.box_id == to_consider_box_id)
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
                System.Diagnostics.Debug.WriteLine("i: " + i);
                // Take a random location in the qna_list
                // and go through the list until you find one promotable obj.
                int random_start_id = random.Next(0, _qna_list.Count - 1);

                // Still maximally go through the list once, just for (no) bug purposes
                for (int j = 0; j < _qna_list.Count+1; j++)
                {
                    System.Diagnostics.Debug.WriteLine("j: " + j);
                    if (0 == _qna_list[(j+random_start_id) % _qna_list.Count].box_id)
                    {
                        _qna_list[(j + random_start_id) % _qna_list.Count].box_id = 1;
                        break;
                    }
                }
            }
        }

        public (string, string) NextQuestionAndAnswer()
        {
            if (_open_question_ids.Count == 0)
            {
                return ("", "");
            }
            return (_qna_list[_open_question_ids[0]].question, _qna_list[_open_question_ids[0]].answer);
        }

        public string GetCardsLeft()
        {
            return "Cards left: " + _open_question_ids.Count.ToString();
        }

        public string GetCardBox()
        {
            if(-1==_qna_list[_open_question_ids[0]].promote)
            {
                return "Card's box: 1 (mod 1)";
            }
            else
            {
                int b = _qna_list[_open_question_ids[0]].box_id;
                int m = _box_repeat_iterations[b];

                return "Card's box: " + b + " (mod " + m + ")";
            }
        }

        public string GetSessionNumber()
        {
            return "Session number " + _training_session_id;
        }

        public bool KnewIt()
        {
            if (_qna_list[_open_question_ids[0]].promote == 0)
            {
                _qna_list[_open_question_ids[0]].promote = 1;
            }
            _finished_question_ids.Add(_open_question_ids[0]);
            _open_question_ids.RemoveAt(0);

            if (_open_question_ids.Count == 0)
            {
                _clean_up();
                return true;
            }
            return false;
        }

        public void DidntKnowIt()
        {
            _qna_list[_open_question_ids[0]].promote = -1;
            if(1==_open_question_ids.Count)
            {
                return;
            }
            int tmp_id = _open_question_ids[0];
            _open_question_ids.RemoveAt(0);
            Random rng = new Random();
            int new_insert_id = rng.Next(1, _open_question_ids.Count+1);
            System.Diagnostics.Debug.WriteLine("new_insert_id:" + new_insert_id);
            _open_question_ids.Insert(new_insert_id, tmp_id);
        }

        private void _clean_up()
        {
            for (int i = 0; i < _finished_question_ids.Count; i++)
            {
                if (-1 == _qna_list[_finished_question_ids[i]].promote && _qna_list[_finished_question_ids[i]].box_id > 1)
                {
                    _qna_list[_finished_question_ids[i]].box_id--;
                    _qna_list[_finished_question_ids[i]].promote = 0;
                }
                if (1 == _qna_list[_finished_question_ids[i]].promote)
                {
                    _qna_list[_finished_question_ids[i]].box_id++;
                    _qna_list[_finished_question_ids[i]].promote = 0;
                }
            }

            if ("" != _progress_file)
            {
                // Already loaded by progress file
                MessageBoxResult result_know_prog_file = MessageBox.Show("Save to " + _progress_file + "?", "Save Progress?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result_know_prog_file == MessageBoxResult.Yes)
                {
                    FileHelper.SaveProgress(_progress_file, _training_session_id, _toProgressCsv());
                    return;
                }
            }

            MessageBoxResult result = MessageBox.Show("Save progress to file?", "Save Progress?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Only possible if it was imported
                string kkp_file = FileHelper.AskForFile("kkp files (*.kkp)|*.kkp|All files (*.*)|*.*", true);
                _progress_file = kkp_file;
                FileHelper.SaveProgress(kkp_file, _training_session_id, _toProgressCsv());
            }
        }

        public string GetProgressFileName()
        {
            return _progress_file;
        }

        private List<(string, string, int)> _toProgressCsv()
        {
            List < (string, string, int) > ret = new List<(string, string, int)>();

            foreach(var q in _qna_list)
            {
                ret.Add((q.question, q.answer, q.box_id));
            }

            return ret;
        }
    }

}
