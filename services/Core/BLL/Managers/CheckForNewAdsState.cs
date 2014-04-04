using Core.BLL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.BLL
{
    public class CheckForNewAdsState : OperationState
    {
        public string SourceUrl
        {
            get;
            set;
        }

        public int FrameTotalAds
        {
            get;
            set;
        }

        public int FrameNewAds
        {
            get;
            set;
        }

        public double FrameProgress
        {
            get;
            set;
        }
    }
}
