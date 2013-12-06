using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Expressions
{
    public class StringRange
    {
        public int Start
        {
            get;
            private set;
        }

        public int End
        {
            get;
            private set;
        }

        public int Length
        {
            get
            {
                return (End - Start + 1);
            }
        }


        public StringRange(int start, int end)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException("start", "start must be more than zero!");
            }

            if ((end - start) < -1)
            {
                throw new ArgumentOutOfRangeException("end", "end must be more or equal start!");
            }

            Start = start;
            End   = end;
        }
    }
}
