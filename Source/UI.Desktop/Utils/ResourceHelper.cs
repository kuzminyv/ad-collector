using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Resources;

namespace UI.Desktop.Utils
{
    public class ResourceHelper
    {
        public static Stream GetResourceStream(string resourcePath)
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(resourcePath, UriKind.Relative));
            return streamInfo == null ? null : streamInfo.Stream;
        }

        public static System.Drawing.Icon GetIcon(string resourcePath)
        { 
            using (Stream stream = ResourceHelper.GetResourceStream(resourcePath))
            {
                return new System.Drawing.Icon(stream);
            }
        }
    }
}
