using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KarteiKartenLernen
{
    public class RecentFileViewModel
    {
        //private int _index;
        private string _fileName;
        private MainWindowViewModel _mwvm;
        //private IFileHandler _fileHandler;

        //public RecentFileViewModel(int index, string fileName, IFileHandler fileHandler)
        public RecentFileViewModel(string fileName, MainWindowViewModel mwvm)
        {
            _fileName = fileName;
            _mwvm = mwvm;
            //_fileHandler = fileHandler;
        }
        
        public string FileName
        {
            get { return _fileName; }
        }

        public ICommand Open
        {
            get
            {
                return new Command(
                    (object parameter) => {
                        //MessageBox.Show(_fileName);
                        _mwvm.LoadSessionProgress(_fileName);
                    }, (object parameter) => { return true; });
                //return MakeCommand
                //    .Do(() => _fileHandler.Open(_fileName));
            }
        }
    }
}
