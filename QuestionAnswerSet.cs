using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace KarteiKartenLernen
{
    class QAEntity
    {
        public QAEntity()
        {
            _value = new List<string>();
            _value_datatypes = new List<string>();
        }
        
        public QAEntity(
            List<string> in_value, 
            List<string> in_value_datatypes, 
            string in_side_name, 
            string in_side_icon)
        {
            _value = in_value;
            _value_datatypes = in_value_datatypes;
            _side_name = in_side_name;
            _side_icon = in_side_icon;
        }

        public List<string> _value;
        public List<string> _value_datatypes;
        public string _side_name;
        public string _side_icon;
    }

    class QuestionAnswerSet
    {
        private QAEntity _answer;
        private QAEntity _question;
        private string _othersides_text;

        // What card is it?
        private int _card_id;

        // What bin is this QA in?
        private int _bin_id;

        // Has this been demoted this session?
        private bool _previously_demoted;

        // What Question direction id is it?
        private int _question_direction_id;

        // When will be the next session this card is relevant?
        private int _next_session;

        public QuestionAnswerSet(
            QAEntity in_answer,
            QAEntity in_question,
            string in_othersides_text,
            int in_card_id,
            int in_bin_id,
            int in_next_session,
            int in_question_direction_id
            )
        {
            _answer = in_answer;
            _question = in_question;
            _othersides_text = in_othersides_text;
            _card_id = in_card_id;
            _bin_id = in_bin_id;
            _next_session = in_next_session;
            _question_direction_id = in_question_direction_id;
            _previously_demoted = false;
        }

        public QAEntity GetQuestion() { return _question; }
        public QAEntity GetAnswer() { return _answer; }
        public int GetCardId() { return _card_id; }
        public int GetBinId() { return _bin_id; }
        public void SetBinId(int in_bin_id) { _bin_id = in_bin_id; }

        public void SetNextBinId() { _bin_id++; }

        public int GetNextSession() { return _next_session; }
        public void SetNextSession(int in_next_session) { _next_session = in_next_session; }
        public int GetQuestionDirectionId() { return _question_direction_id; }

        public void Demote()
        {
            _previously_demoted = true;
            _bin_id = 1;
        }

        public bool WasDemoted()
        {
            return _previously_demoted;
        }


        public string GetOtherCardSideText()
        {
            return _othersides_text;
        }

        public CardSide GetQuestionCardSide(
            string in_kp2_base_dir,
            string in_sound_dir,
            SpeechConfig in_tts_conf,
            out string created_wav )
        {
            created_wav = "";
            CardSide ret = new CardSide();
            ret.CardSideName = _question._side_name;
            ret.CardSideImageIcon = _question._side_icon;
            for (int i = 0; i < _question._value_datatypes.Count; i++)
            {
                switch (_question._value_datatypes[i])
                {
                    case "string":
                        ret.HasText = true;
                        ret.Text = _question._value[i];
                        break;
                    case "audio":
                        ret.AudioFile = _question._value[i]; 
                        if (File.Exists(in_kp2_base_dir + "\\" + ret.AudioFile))
                        {
                            ret.HasAudio = true;
                        }
                        else if (in_tts_conf != null && in_sound_dir != null)
                        {
                            string filename = _getFilename();
                            string hanzi = _getHanzi();
                            Directory.CreateDirectory(Path.Combine(in_kp2_base_dir, in_sound_dir));
                            string p = Path.Combine(in_kp2_base_dir, in_sound_dir, filename);
                            using var audioConfig = AudioConfig.FromWavFileOutput(p);
                            using var synthesizer = new SpeechSynthesizer(in_tts_conf, audioConfig);
                            synthesizer.SpeakTextAsync(hanzi).GetAwaiter().GetResult();
                            ret.HasAudio = true;
                            ret.AudioFile = Path.Combine(in_sound_dir, filename);
                            created_wav = filename;
                        }
                        break;
                    case "image":
                        ret.HasImage = true;
                        ret.ImageFile = _question._value[i];
                        break;
                    case "video":
                        ret.HasVideo = true;
                        ret.VideoFile = _question._value[i];
                        break;
                    default:
                        MessageBox.Show("Unknown type " + _question._value_datatypes[i]);
                        break;
                }
            }
            return ret;
        }

        private string _getHanzi()
        {
            string hanzi = "";
            if (_answer._side_name == "Chinese Character")
            {
                hanzi = _answer._value[0];
            }
            else if (_question._side_name == "Chinese Character")
            {
                hanzi = _question._value[0];
            }
            else
            {
                hanzi = _othersides_text;
            }
            return hanzi;
        }

        private string _getFilename()
        {
            string english = "";
            if (_answer._side_name == "English")
            {
                english = _answer._value[0];
            }
            else if (_question._side_name == "English")
            {
                english = _question._value[0];
            }
            else
            {
                english = _othersides_text;
            }

            return english.Trim()
                .Replace(" ", "_")
                .Replace("'", "_")
                .Replace(",", "_")
                .Replace(";", "_")
                .Replace(":", "_")
                .Replace("*", "_")
                .Replace("?", "_")
                .Replace("\"", "_")
                .Replace("<", "_")
                .Replace(">", "_")
                .Replace("|", "_")
                .Replace("/", "_")
                .Replace("\\", "_") + ".wav";
        }

        public CardSide GetAnswerCardSide(
            string in_kp2_base_dir,
            string in_sound_dir,
            SpeechConfig in_tts_conf,
            out string created_wav)
        {
            created_wav = "";
            CardSide ret = new CardSide();
            ret.CardSideName = _answer._side_name;
            ret.CardSideImageIcon = "/icons/"+_answer._side_icon;
            for (int i=0; i< _answer._value_datatypes.Count; i++)
            {
                switch(_answer._value_datatypes[i])
                {
                    case "string":
                        ret.HasText = true;
                        ret.Text = _answer._value[i];
                        break;
                    case "audio":
                        ret.AudioFile = _answer._value[i];
                        if (File.Exists(in_kp2_base_dir + "\\" + ret.AudioFile))
                        {
                            ret.HasAudio = true;
                        }
                        else if (in_tts_conf != null && in_sound_dir != null)
                        {
                            string filename = _getFilename();
                            string hanzi = _getHanzi();
                            Directory.CreateDirectory(Path.Combine(in_kp2_base_dir, in_sound_dir));
                            string p = Path.Combine(in_kp2_base_dir, in_sound_dir, filename);
                            using var audioConfig = AudioConfig.FromWavFileOutput(p);
                            using var synthesizer = new SpeechSynthesizer(in_tts_conf, audioConfig);
                            synthesizer.SpeakTextAsync(hanzi).GetAwaiter().GetResult();
                            ret.HasAudio = true;
                            ret.AudioFile = Path.Combine(in_sound_dir, filename);
                            created_wav = filename;
                        }
                        break;
                    case "image":
                        ret.HasImage = true;
                        ret.ImageFile = _answer._value[i];
                        break;
                    case "video":
                        ret.HasVideo = true;
                        ret.VideoFile = _answer._value[i];
                        break;
                    default:
                        MessageBox.Show("Unknown type " + _answer._value_datatypes[i]);
                        break;
                }
            }
            return ret;
        }
    }
}
