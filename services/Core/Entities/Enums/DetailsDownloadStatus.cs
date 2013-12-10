using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Enums
{
    public enum DetailsDownloadStatus
    {
        NotDownloaded = 0,
        Downloaded = 1,
        ParserError = -1,
        PageNotAccessable = -2
    }
}
