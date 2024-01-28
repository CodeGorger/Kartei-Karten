using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarteiKartenLernen
{
    class FlashCard
    {
        public FlashCard(string in_question, string in_answer, string in_sound_file)
        {
            question = in_question;
            answer = in_answer;
            sound_file = in_sound_file;
            box_id = 0;
        }
        public FlashCard(string in_question, string in_answer, string in_sound_file, int in_box_id)
        {
            question = in_question;
            answer = in_answer;
            sound_file = in_sound_file;
            box_id = in_box_id;
        }
        public string question;
        public string answer;
        public string sound_file;

        // 0=new cards; 1...5=normal boxes; 6=learned
        public int box_id;

        // -1=demote; 1=promote
        public int promote;
    }
}
