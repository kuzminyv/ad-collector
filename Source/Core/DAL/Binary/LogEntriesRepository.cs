using Core.DAL.Binary.Common;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.Binary
{
    public class LogEntriesRepository : BinaryRepository<LogEntry, int>, ILogEntriesRepository
    {
    }
}
