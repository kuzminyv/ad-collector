using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Connectors
{
    public interface ICorrector
    {
        void Correct(Ad ad);
    }
}
