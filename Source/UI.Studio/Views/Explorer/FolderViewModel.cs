using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Desktop.Views;
using System.IO;

namespace UI.Studio.Views
{
    public class FolderViewModel : ViewModel
    {
        private string _path;
        public string Name
        {
            get
            {
                return Path.GetFileName(_path);
            }
        }

        private ObservableCollection<FileViewModel> _files;
        public ObservableCollection<FileViewModel> Files
        {
            get
            {
                if (_files == null)
                {
                    string[] files = Directory.GetFiles(_path);
                    _files = new ObservableCollection<FileViewModel>(files.Select(f => new FileViewModel(f)));
                }
                return _files;
            }
        }

        public FolderViewModel(string path)
        {
            _path = path;
        }
    }
}
