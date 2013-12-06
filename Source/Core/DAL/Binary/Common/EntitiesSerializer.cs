using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Core.DAL.Binary.Common
{
    public class EntitiesSerializer<TKey, TEntity>
    {
        public Dictionary<TKey, TEntity> Load(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            if (File.Exists(fileName) && (new FileInfo(fileName)).Length > 0)
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    return (Dictionary<TKey, TEntity>)formatter.Deserialize(stream);
                }
            }
            return null;
        }

        public void Save(string fileName, Dictionary<TKey, TEntity> entities)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, entities);
            }
        }
    }
}
