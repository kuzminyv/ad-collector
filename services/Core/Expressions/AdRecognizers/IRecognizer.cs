﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Expressions.AdRecognizers
{
    public interface IRecognizer
    {
        Ad Recognize(string adText);
    }
}
