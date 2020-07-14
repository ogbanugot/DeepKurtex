using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public class ModelSerializer
    {
        
        public string Serialize (string config, object model, string saveas)
        {
            string modeltype = model.GetType().ToString();
            switch (modeltype)
            {
                case "AI.ML.CNN.Model":
                    AI.ML.CNN.Model cnnmodel = (AI.ML.CNN.Model)model;
                    return SerializeCNN(config, cnnmodel, saveas);

                case "AI.ML.ANN.Model":
                    throw new NotImplementedException();

                default:
                    throw new Exception("Model Serilizer not found");
            }
            throw new Exception();
        }

        public object Deserialize (string filename)
        {
            throw new NotImplementedException();
        }

        public string SerializeCNN (string config, ML.CNN.Model Model, string saveas)
        {            
            CNNModel cnnmodel = new CNNModel();
            cnnmodel.config = config;
            for (int i = 1; i < Model.Layers.Length; i++)
            {
                ML.CNN.Model.Unit lyr;
                lyr = Model.Layers[i];
                bool convtype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Convolution" ? true : false;
                bool conctype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Connected" ? true : false;

                switch (convtype)
                {
                    case true:
                        ML.CNN.Layers.Convolution convLyr = (ML.CNN.Layers.Convolution)lyr;                        
                        AI.ML.CNN.Layers.Convolution.Kernel krn;
                        CNNModel.CNNWeights cnnweight;
                        for (int j = 0; j < convLyr.Filters.Count; j++)
                        {                          
                            // j: indexing filters
                            for (int k = 0; k < ((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels.Length; k++)
                            {
                                //k: indexing kernels
                                krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[k];
                                cnnweight = new CNNModel.CNNWeights
                                {
                                    Weights = krn.Weights
                                };
                                cnnmodel.cnnWeights.Add(cnnweight);
                            }
                        }
                        break;

                    case false:
                        switch (conctype)
                        {
                            case true:
                                ML.CNN.Layers.Connected connLyr = (ML.CNN.Layers.Connected)lyr;
                                ML.ANN.Neuron n; ML.ANN.Synapse syn;
                                CNNModel.ANNWeights annweight;

                                for (int j = 0; j < connLyr.Neurons.Length; j++)
                                {
                                    n = connLyr.Neurons[j];
                                    annweight = new CNNModel.ANNWeights();
                                    annweight.Weights = new double?[n.Synapse.Count];
                                    for (int k = 0; k < n.Synapse.Count; k++)
                                    {
                                        syn = n.Synapse[k];
                                        annweight.Weights[k] = syn.W;
                                    }
                                    cnnmodel.annWeights.Add(annweight);
                                }
                                break;
                                
                            case false:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            saveas +=  ".txt";
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(saveas))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, cnnmodel);
            }

            return Path.GetFullPath(saveas);
        }

        public ML.CNN.Model DeserializeCNN(string filename)
        {
            CNNModel cnnmodel = JsonConvert.DeserializeObject<CNNModel>(File.ReadAllText(filename));
            ML.CNN.Model Model = new AI.ML.CNN.Model();
            Model.Configure(cnnmodel.config);
            int count = 0;
            int anncount = 0;
            for (int i = 1; i < Model.Layers.Length; i++)
            {
                ML.CNN.Model.Unit lyr;
                lyr = Model.Layers[i];
                bool convtype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Convolution" ? true : false;
                bool conctype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Connected" ? true : false;

                switch (convtype)
                {
                    case true:
                        ML.CNN.Layers.Convolution convLyr = (ML.CNN.Layers.Convolution)lyr;
                        AI.ML.CNN.Layers.Convolution.Kernel krn;
                        for (int j = 0; j < convLyr.Filters.Count; j++)
                        {
                            // j: indexing filters
                            for (int k = 0; k < ((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels.Length; k++)
                            {
                                //k: indexing kernels
                                krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[k];
                                krn.Weights = cnnmodel.cnnWeights[count++].Weights;
                            }
                        }
                        break;

                    case false:
                        switch (conctype)
                        {
                            case true:
                                ML.CNN.Layers.Connected connLyr = (ML.CNN.Layers.Connected)lyr;
                                ML.ANN.Neuron n; ML.ANN.Synapse syn;
                                double?[] weights;
                                for (int j = 0; j < connLyr.Neurons.Length; j++)
                                {
                                    n = connLyr.Neurons[j];
                                    weights = cnnmodel.annWeights[anncount++].Weights;
                                    for (int k = 0; k < n.Synapse.Count; k++)
                                    {
                                        syn = n.Synapse[k];
                                        syn.W = weights[k];
                                    }
                                }
                                break;

                            case false:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return Model;
        }
    }

    public class CNNModel
    {
        public string config;
        public IList<CNNWeights> cnnWeights = new List<CNNWeights>();
        public IList<ANNWeights> annWeights = new List<ANNWeights>();

        public class CNNWeights
        {
            public double?[][][] Weights { get; set; }
        }
        public class ANNWeights
        {
            public double?[] Weights { get; set; }
        }
    }
}
