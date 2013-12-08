using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Studio.Views
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style ConnectorStyle
        {
            get;
            set;
        }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is ConnectorViewModel)
                return ConnectorStyle;

            return base.SelectStyle(item, container);
        }
    }
}
