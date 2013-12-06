using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities
{
    [DataContractAttribute]
    [Serializable]
    public class LogEntry : Entity<int>
    {
        public SeverityLevel Severity
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string Details
        {
            get;
            set;
        }
    }
}
