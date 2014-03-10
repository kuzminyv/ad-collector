using Core.DAL.Common;
using Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AdsQuery
    {
        public string SearchExpression
        {
            get;
            set;
        }

        public double? PriceMin
        {
            get;
            set;
        }

        public double? PriceMax
        {
            get;
            set;
        }

        public double? PricePerMeterMax
        {
            get;
            set;
        }

        public double? PricePerMeterMin
        {
            get;
            set;
        }

        public float? LivingSpaceMin
        {
            get;
            set;
        }

        public float? LivingSpaceMax
        {
            get;
            set;
        }

        public int? FloorMin
        {
            get;
            set;
        }

        public int? FloorMax
        {
            get;
            set;
        }

        public DateTime? PublishDateMin
        {
            get;
            set;
        }

        public DateTime? PublishDateMax
        {
            get;
            set;
        }

        public AdSort Sort
        {
            get;
            set;
        }

        public SortOrder SortOrder
        {
            get;
            set;
        }
    }
}
