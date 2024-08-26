using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KarteiKartenLernen
{
    class FlashCard
    {
        public FlashCard(List<string> in_sides, List<QuestionProgress> in_progress)
        {
            sides = in_sides;
            progress = in_progress;
        }

        private List<string> sides;
        private List<QuestionProgress> progress;

        public List<string> GetSides()
        {
            return sides;
        }

        public List<QuestionProgress> GetProgresses()
        {
            return progress;
        }

    }
}
