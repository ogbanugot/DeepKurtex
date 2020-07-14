using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN
{
    public abstract class Trainer
    {
        private Core.fDataSet dataSet = null;
        private int? epochs = null;
        private Model model = null;
        protected string log = "";
        protected int nofSet;

        public Trainer()
        {

        }

        public virtual Trainer Configure(Model model, int? epochs, Core.fDataSet dataSet)
        {
            this.dataSet = dataSet;
            this.model = model;


            // 0. assert input and output size for all of data set
            if (nofSet != 0)
                throw new Exception();

            nofSet = dataSet.fData.Count;
            //for (int i = 0; i < nofSet; i++)
            //    if ((System.Math.Sqrt(dataSet.fData[i].Data.Length) != model.Input.Output[0].Rows) || (dataSet.fData[i].Label.Length != model.Output.Output[0].Rows))
            //        throw new Exception();

            // 1. assert epochs
            if (epochs == null)
                throw new Exception();
            this.epochs = epochs;

            return this;
        }

        public Core.fDataSet DataSet
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
