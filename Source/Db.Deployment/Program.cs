using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsSql = Core.DAL.MsSql;
using Bin = Core.DAL.Binary;
using System.IO;
using Core.Entities;
using Core.Utils;

namespace Db.Deployment
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] streetsData = File.ReadAllLines("Data\\SaratovStreets.txt");

            Bin.StreetsRepository binStreets = new Bin.StreetsRepository();
            if (!binStreets.GetList(8452).Any())
            {
                Console.WriteLine("Writing streets to Binary Repository");
                binStreets.AddList(streetsData.Select(s => new Street() { LocationId = 8452, Name = s }).ToList());
                Console.WriteLine("done.\n");
            }

            MsSql.StreetsRepository sqlStreets = new MsSql.StreetsRepository();
            if (!sqlStreets.GetList(8452).Any())
            {
                Console.WriteLine("Writing streets to SQL Repository");
                sqlStreets.AddList(streetsData.Select(s => new Street() { LocationId = 8452, Name = s }).ToList());
                Console.WriteLine("done.\n");
            }

            BufferedAction.WaitAll();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
