using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Connectors
{
    public interface IVerificator
    {
        /// <summary>
        /// Returns null if verification was successful otherwise returns error message
        /// </summary>
        string Verify(Ad ad);
    }
}
