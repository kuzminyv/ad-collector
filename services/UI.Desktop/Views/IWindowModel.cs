using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace UI.Desktop.Views
{
	public interface IWindowModel
	{
		bool WindowClosing();
		EventHandler WindowCloseRequest { get; set; }
	}
}
