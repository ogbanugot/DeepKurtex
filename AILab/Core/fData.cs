using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace AI.Core
{
    [Serializable]
	public class fData
    {
        private int[] intPixel;
        private byte[] bytePixel;
        public double del, dataMax, dataMin, labelMax, labelMin;

        public fData(byte[] data, double dataMin, double dataMax, byte[] label, double labelMin, double labelMax)
        {
            bytePixel = data;
            this.del = (dataMax - dataMin) / byte.MaxValue;

            this.Data = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
                this.Data[i] = dataMin + (data[i] * del);

            this.Label = new double[label.Length];
            for (int i = 0; i < label.Length; i++)
                this.Label[i] = label[i] == 0 ? labelMin : labelMax;

            this.dataMax = dataMax;
            this.dataMin = dataMin;

            this.labelMax = labelMax;
            this.labelMin = labelMin;
        }

        public double[] Data { get; set; }

        public double[] Label { get; set; }

        public int DecodeLabel
        {
            get
            {
                int label = 0;
                for (int i = 0; i < Label.Length; i++)
                {
                    if (Label[i] == labelMax)
                    {
                        label = i;
                    }
                }
                return label;
            } 
        }
        

        
        public int[] Pixel
        {
            get {
                intPixel = new int[bytePixel.Length];
                for(int i=0; i<bytePixel.Length; i++)
                {
                    intPixel[i] = bytePixel[i];
                }
                return intPixel; 
            }            
        }


        public virtual string ToImageString()
        {
            string s = "";

            double t = dataMin + ((dataMax - dataMin) / 2.0);
            int size = (int)System.Math.Sqrt(Data.Length);
            for (int c = 0, i = 0; i < size; i++)
            {
                s += "\n";
                for (int j = 0; j < size; j++)
                    s += Data[c++] > t ? " " : "1";
            }
            s += "\n\n[label: ";
            for (int i = 0; i < Label.Length; i++)
                s += Label[i].ToString("e4") + " ";
            s += "]";
            return s;
        }

        public  override string ToString()
        {
            string s = ""; byte b;
            
            s += "\n\n[byte: ";
            for (int i = 0; i < Data.Length; i++)
            {
                b = (byte)System.Math.Round(((Data[i] - dataMin) / del), 0);
                s += b.ToString() + " ";
            }
            s += "]";

            s += "\n\n[data: ";
            for (int i = 0; i < Data.Length; i++)
                s += Data[i].ToString("e4") + " ";
            s += "]";

            s += "\n\n[label: ";
            for (int i = 0; i < Label.Length; i++)
                s += Label[i].ToString("e4") + " ";
            s += "]";

            return s;
        }

        public byte[] ToByte(int[] data)
        {
            byte[] byteData = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                byteData[i] = (byte)data[i];
            }
            return byteData;
        }

        public struct Serialize
        {
            public double dataMax, dataMin, labelMax, labelMin;
            public int[] intPixel;
            public byte[] bytePixel;
            public double[] data;
            public double[] Label;
        }
        public Serialize Serializer()
        {
            Serialize serialize;
            serialize.intPixel = intPixel;
            serialize.bytePixel = bytePixel;
            serialize.data = Data;
            serialize.Label = Label;
            serialize.dataMax = dataMax;
            serialize.dataMin = dataMin;
            serialize.labelMax = labelMax;
            serialize.labelMin = labelMin;
            return serialize;
        }
        public void Deserializer(Serialize serialize)
        {
            intPixel = serialize.intPixel;
            bytePixel = serialize.bytePixel;
            Data = serialize.data;
            Label = serialize.Label;
            dataMax = serialize.dataMax;
            dataMin = serialize.dataMin;
            labelMax = serialize.labelMax;
            labelMin = serialize.labelMin;
        }

        public void Resize(int[] input_size, int channel)
        {
            int size;
            Mat matImg;
            Image<Gray, Byte> Gimg;
            Image<Gray, Byte> Gimage;
            Image<Bgr, Byte> Cimg;
            Image<Bgr, Byte> Cimage;

            if (channel == 1)
            {
                size = (int)System.Math.Sqrt(Pixel.Length / 3);
                matImg = new Mat(size, size, DepthType.Cv8U, 3);
                Gimg = matImg.ToImage<Gray, Byte>();
                Gimage = setGrayPixels(Gimg, size, Pixel);
                intPixel = Resize_Gray(Gimage, input_size);
                Scale(ToByte(intPixel));
            }
            if (channel == 3)
            {
                size = (int)System.Math.Sqrt(Pixel.Length / 3);
                matImg = new Mat(size, size, DepthType.Cv8U, 3);
                Cimg = matImg.ToImage<Bgr, Byte>();
                Cimage = setColorPixels(Cimg, size, Pixel);
                intPixel = Resize_Color(Cimage, input_size);
                Scale(ToByte(intPixel));
            }
            else
            {
                throw new Exception("Channel must be 1 or 3");
            }       
           
        }

        public void Scale(byte[] data)
        {
            bytePixel = data;
            this.del = (dataMax - dataMin) / byte.MaxValue;

            this.Data = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
                this.Data[i] = dataMin + (data[i] * del);        
        }

        public Image<Gray, Byte> setGrayPixels(Image<Gray, Byte> img, int size, int[] pixel)
        {
            byte[,,] pix = img.Data;
            int c = 0;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    pix[i, j, 0] = (byte)pixel[c++];
                }
            img.Data = pix;
            return img;
        }
        public int[] Resize_Gray(Image<Gray, Byte> img, int[] input_size)
        {
            System.Drawing.Size size = new System.Drawing.Size();
            size.Width = input_size[0];
            size.Height = input_size[1];
            Image<Gray, Byte> img2 = new Image<Gray, Byte>(size.Width, size.Height);
            CvInvoke.Resize(img, img2, size, 0.0, 0.0, Inter.Linear);

            int[] image = new int[size.Height * size.Width];

            byte[,,] pix = new byte[size.Height, size.Width, 1];

            pix = img2.Data;
            int c = 0;
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                {
                    image[c++] = pix[i, j, 0];
                }

            return image;
        }

        public Image<Bgr, Byte> setColorPixels(Image<Bgr, Byte> img, int size, int[] pixel)
        {
            byte[,,] pix = img.Data;
            int c = 0;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    pix[i, j, 0] = (byte)pixel[c++];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    pix[i, j, 1] = (byte)pixel[c++];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    pix[i, j, 2] = (byte)pixel[c++];
                }

            img.Data = pix;
            return img;
        }
        public int[] Resize_Color(Image<Bgr, Byte> img, int[] input_size)
        {
            System.Drawing.Size size = new System.Drawing.Size();
            size.Width = input_size[0];
            size.Height = input_size[1];
            Image<Bgr, Byte> img2 = new Image<Bgr, Byte>(size.Width, size.Height);
            CvInvoke.Resize(img, img2, size, 0.0, 0.0, Inter.Linear);

            int[] image = new int[size.Height * size.Width * 3];

            byte[,,] pix = new byte[size.Height, size.Width, 3];

            pix = img2.Data;
            int c = 0;
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                {
                    image[c++] = pix[i, j, 0];
                }
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                {
                    image[c++] = pix[i, j, 1];
                }
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                {
                    image[c++] = pix[i, j, 2];
                }

            return image;
        }
    }
}