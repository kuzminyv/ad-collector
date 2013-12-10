using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Entities;
using System.Windows.Input;
using UI.Desktop.Commands;
using System.Web;

namespace UI.Desktop.Views
{
	public class AdHistoryItemViewModel : ViewModel
	{
		private AdHistoryItem _model;

        public double Price
        {
            get
            {
                return _model.Price;
            }
        }

        public DateTime AdDate
        {
            get
            {
                return _model.AdPublishDate;
            }
        }

        public AdHistoryItemViewModel(AdHistoryItem model)
		{
			_model = model;
		}
	}
}
