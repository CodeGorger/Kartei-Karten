using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarteiKartenLernen
{
    class FlashCard
    {
        public FlashCard(string in_question, string in_answer, string in_sound_file, bool in_one_directional)
        {
            question = in_question;
            answer = in_answer;
            sound_file = in_sound_file;
            box_id = 0;
            reverse_box_id = 0;
            promote = 0;
            if(in_one_directional)
            {
                reverse_box_id = 6;
            }
            else
            {
                reverse_box_id = 0;
            }
        }

        public FlashCard(
            string in_question, 
            string in_answer, 
            string in_sound_file, 
            int in_box_id,
            int in_reverse_box_id)
        {
            question = in_question;
            answer = in_answer;
            sound_file = in_sound_file;
            box_id = in_box_id;
            reverse_box_id = in_reverse_box_id;
            promote = 0;
            reverse_promote = 0;
        }
        public string question;
        public string answer;
        public string sound_file;

        // 0=new cards; 1...5=normal boxes; 6=learned
        public int box_id;
        public int reverse_box_id;

        // -1=demote; 1=promote
        public int promote;
        public int reverse_promote;
    }
}
