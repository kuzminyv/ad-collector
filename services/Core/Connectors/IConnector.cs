 using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Core.Connectors
{
	public interface IConnector
	{
        ConnectorOptions GetOptions();
		string Id { get; }
        IEnumerable<Ad> GetAds();
        bool FillDetails(Ad ad);
	}
}
