using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarteiKartenLernen
{
    public class CardSide
    {
        private bool _has_text;
        public bool HasText
        {
            get => _has_text;
            set
            {
                _has_text = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private bool _has_audio;
        public bool HasAudio
        {
            get => _has_audio;
            set
            {
                _has_audio = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private string _audio_file;
        public string AudioFile
        {
            get => _audio_file;
            set
            {
                _audio_file = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private bool _has_image;
        public bool HasImage
        {
            get => _has_image;
            set
            {
                _has_image = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private string _image_file;
        public string ImageFile
        {
            get => _image_file;
            set
            {
                _image_file = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private bool _has_video;
        public bool HasVideo
        {
            get => _has_video;
            set
            {
                _has_video = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private string _video_file;
        public string VideoFile
        {
            get => _video_file;
            set
            {
                _video_file = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private string _card_side_image_icon;
        public string CardSideImageIcon
        {
            get => _card_side_image_icon;
            set
            {
                _card_side_image_icon = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }

        private string _card_side_name;
        public string CardSideName
        {
            get => _card_side_name;
            set
            {
                _card_side_name = value;
                //OnPropertyChanged(nameof(HasText));
            }
        }





        public CardSide()
        {
            _has_text = false;
            _text = "";
            _has_audio = false;
            _audio_file = "";
            _has_image = false;
            _image_file = "";
            _has_video = false;
            _video_file = "";
        }
    }
}