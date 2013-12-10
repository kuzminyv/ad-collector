using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL
{
    public interface ILogEntriesRepository
    {
        QueryResult<LogEntry> GetList(Query query);

        void AddItem(LogEntry logEntry);
    }
}
