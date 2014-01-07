using Core.BLL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Desktop.Views
{
    public class MetadataViewModel : ViewModel<Metadata, AdItemViewModel>
    {
        public bool? IsFavorite
        {
            get
            {
                return Model.IsFavorite;
            }
            set
            {
                bool val = (value == true);
                if (val != Model.IsFavorite)
                {
                    Model.IsFavorite = val;
                    Managers.MetadataManager.SaveItem(Model);
                    OnPropertyChanged("IsFavorite");
                }
            }
        }

        public MetadataViewModel(AdItemViewModel parent, Metadata model)
            : base(parent, model)
        { 
        }
    }
}
