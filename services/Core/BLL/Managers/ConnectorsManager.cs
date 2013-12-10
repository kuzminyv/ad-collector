using Core.Connectors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Core.BLL
{
	public class ConnectorsManager
	{
		public List<IConnector> GetConnectors()
		{ 
			return new List<IConnector>(new IConnector[]
				{
                    new CnIrr(),
                    new CnKvadrat64NewBuildings(),
                    new CnRealtySarbcNewBuildings(),
                    new CnRealtySarbc(),
                    new CnKvadrat64()
				});
		}

        public IConnector GetById(string id)
        {
            return GetConnectors().Where(c => c.Id == id).First();
        }
    }
}
