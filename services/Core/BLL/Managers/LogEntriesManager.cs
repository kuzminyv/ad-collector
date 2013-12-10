using Core.DAL;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.BLL
{
    public class LogEntriesManager
    {
        public void AddItem(SeverityLevel severity, string message)
        {
            AddItem(severity, message, null);
        }

        public void AddItem(SeverityLevel severity, string message, string details)
        {
            LogEntry logEntry = new LogEntry() {Severity = severity, Message = message, Time = DateTime.Now, Details = details };
            Repositories.LogEntriesRepository.AddItem(logEntry);
        }

        public List<LogEntry> GetList()
        {
            Query query = new Query();
            query.AddSort("Time", SortOrder.Descending);
            return Repositories.LogEntriesRepository.GetList(query).Items;
        }
    }
}
