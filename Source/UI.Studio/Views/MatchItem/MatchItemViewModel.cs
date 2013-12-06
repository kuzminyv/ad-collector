using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Entities;
using System.Windows.Input;
using System.Web;
using System.Windows;
using Core.Expressions;
using UI.Desktop.Views;

namespace UI.Studio.Views
{
	public class MatchItemViewModel : ViewModel
	{
		private Match _model;

        public Match Parent
        {
            get
            {
                return _model.Parent;
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                if (_text == null)
                {
                    _text = _model.Path.ToUpper() + ": " + _model.Value;
                }
                return _text;
            }
        }

        public MatchItemViewModel(Match model)
		{
			_model = model;
		}
	}
}
