using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarteiKartenLernen
{
    public class QuestionDirection
    {
        [JsonProperty("from")]
        public int _from { get; set; }

        [JsonProperty("to")]
        public int _to { get; set; }
    }

    public class FieldData
    {
        [JsonProperty("id")]
        public int _id_side_component { get; set; }

        [JsonProperty("type")]
        public string _type { get; set; }
    }

    public class Side
    {
        [JsonProperty("name")]
        public string _name { get; set; }

        [JsonProperty("icon")]
        public string _icon { get; set; }

        [JsonProperty("fields")]
        public List<FieldData> _fields { get; set; }
    }

    //public class Card
    //{
    //    public List<object> _id_then_values { get; set; }
    //}

    //public class CardProgress
    //{
    //    public string id;
    //    private List<int> _id_then_bin_progress;
    //}

    public class QuestionProgress
    {
        public QuestionProgress(int in_bin, int in_next_session)
        {
            _bin = in_bin;
            _next_session = in_next_session;
        }

        [JsonProperty("bin")]
        public int _bin;
        [JsonProperty("next_session")]
        public int _next_session;
    }

    public class SessionAndProgress
    {
        [JsonProperty("session_counter")]
        public int _session_counter { get; set; }

        [JsonProperty("new_card_limit")]
        public int _new_card_limit { get; set; }

        [JsonProperty("max_fillup_size")]
        public int _max_fillup_size { get; set; }

        [JsonProperty("max_questions")]
        public int _max_questions { get; set; }

        [JsonProperty("sound_dir")]
        public string _sound_dir { get; set; }

        [JsonProperty("bin_repetition")]
        public List<int> _bin_repetition { get; set; }

        [JsonProperty("question_directions")]
        public List<QuestionDirection> _question_directions { get; set; }

        [JsonProperty("sides")]
        public List<Side> _sides { get; set; }

        [JsonProperty("cards")]
        public List<List<string>> _cards { get; set; }

        [JsonProperty("progress")]
        public List<List<QuestionProgress>> _progress { get; set; }

    }
}
