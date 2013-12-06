using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.Common
{
    public class Query
    {
        private List<Filter> _filters;
        public List<Filter> Filters
        {
            get
            {
                return _filters;
            }
        }

        private List<Sort> _sorts;
        public List<Sort> Sorts
        {
            get
            {
                return _sorts;
            }
        }

        private List<string> _optionalFields;
        public List<string> OptionalFields
        {
            get
            {
                return _optionalFields;
            }
        }

        public int? Start
        {
            get;
            set;
        }

        public int? Limit
        {
            get;
            set;
        }

        public bool HasOptionalFields
        {
            get
            {
                return OptionalFields != null && OptionalFields.Count > 0;
            }
        }

        public bool HasFilters
        {
            get
            {
                return Filters != null && Filters.Count > 0;
            }
        }

        public bool HasSorts
        {
            get
            {
                return Sorts != null && Sorts.Count > 0;
            }
        }

        public Query AddSort(string name, SortOrder order)
        {
            Sorts.Add(new Sort(name, order));
            return this;
        }

        public Query AddFilter(string name, object value)
        {
            Filters.Add(new Filter(name, value));
            return this;
        }

        public void AddFields(params string[] optionalFields)
        {
            _optionalFields.AddRange(optionalFields);
        }

        public Query(int? start, int? limit)
        {
            Start = start;
            Limit = limit;

            _filters = new List<Filter>();
            _sorts = new List<Sort>();
            _optionalFields = new List<string>();
        }

        public Query()
            : this(null, null)
        { 
        }
    }
}
