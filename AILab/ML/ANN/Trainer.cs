using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN
{
    public abstract class Trainer
    {
        private double[][] dataSet = null;
        private int? epochs = null;
        private Model model = null;

        protected string log = "";
        protected Neuron[][] neurons;

        protected int nofSet;

        public Trainer()
        {

        }

        public virtual Trainer Configure(Model model, int? epochs, double[][] dataSet)
        {
            this.dataSet = dataSet;
            this.model = model;

            // 0. assert input and output size for all of data set
            nofSet = DataSet.Length % 2;
            if (nofSet != 0)
                throw new Exception();

            nofSet = DataSet.Length / 2;
            for (int i = 0; i < nofSet; i++)
                if ((DataSet[i * 2].Length != Model.Input.Length) || (DataSet[(i * 2) + 1].Length != Model.Output.Length))
                    throw new Exception();

            neurons = Model.Neurons;

            // 1. assert epochs
            if (epochs == null)
                throw new Exception();
            this.epochs = epochs;

            return this;
        }

        public double[][] DataSet
        {
            get { return dataSet; }
        }

        public int? Epochs
        {
            get { return epochs; }
        }

        public Model Model
        {
            get { return model; }
        }

        public abstract string Next();
    }
}
