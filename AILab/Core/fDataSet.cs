using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI.Core
{
    [Serializable]
	public class fDataSet
    {
        private IList<fData> images = new List<fData>();
        private IList<int> idx;

        public fDataSet()
        {
            //
        }
        
        public int Count
        {
            get { return images.Count; }
        }

        public IList<fData> fData
        {
            get { return images; }
            set
            {
                for (int i = 0; i < value.Count; i++)
                    images.Add(value[i]);
            }
        }

        public fData Next()
        {
            // 0. assert images
            if (images.Count == 0)
                return null;
            // 1. get next fData
            int i = Math.Daemon.Random.Next(0, idx.Count);
            fData f = images[idx[i]];
            idx.RemoveAt(i);
            return f;
        }

        public void Refresh()
        {
            if (images.Count == 0)
            {
                idx = null;
                return;
            }

            idx = new List<int>();
            for (int i = 0; i < images.Count; i++)
                idx.Add(i);
        }
        
        public string Serilizer(string filepath)
        {
            IList<fData.Serialize> fDataSerialize = new List<fData.Serialize>();
            for (int i = 0; i < images.Count; i++)
                fDataSerialize.Add(images[i].Serializer());

            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(filepath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, fDataSerialize);
            }
            return Path.GetFullPath(filepath);
        }

        public void Deserializer(string filepath)
        {
            byte[] data;
            byte[] label;
            fData fData;
            IList<fData.Serialize> fDataSerialize = JsonConvert.DeserializeObject<IList<fData.Serialize>>(File.ReadAllText(filepath));
            for(int i=0; i<fDataSerialize.Count; i++)
            {
                data = new byte[1];
                label = new byte[1];
                fData = new fData(data, 0, 0, label, 0, 0);
                fData.Deserializer(fDataSerialize[i]);
                images.Add(fData);
            }            
        }

        public void Resize(int[] input_size, int channel)
        {
            for (int i = 0; i< fData.Count; i++)
            {
                fData[i].Resize(input_size, channel);
            }
        }
    }
}
