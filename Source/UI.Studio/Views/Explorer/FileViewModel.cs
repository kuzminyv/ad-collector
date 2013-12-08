using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Desktop.Views;
using System.IO;

namespace UI.Studio.Views
{
    public class FileViewModel : ViewModel
    {
        private string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public string Name
        {
            get 
            {
                return Path.GetFileName(_filePath);
            }
        }

        public FileViewModel(string filePath)
        {
            _filePath = filePath;
        }
    }
}
