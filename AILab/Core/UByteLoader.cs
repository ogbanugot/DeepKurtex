using System;
using System.Collections.Generic;
using System.IO;

namespace AI.Core
{
    [Serializable]
	public class UByteLoader
    {

        public static byte[] EncodeLabel(byte label)
        {
            byte[] code = new byte[10];
            for (int i = 0; i < 10; i++)
                code[i] = (byte)(i == label ? 255 : 0);
            return code;
        }

        public static IList<fData> ReadColorImage(string ubyteImageFileName, int? number,
            double dataMin, double dataMax, double labelMin, double labelMax)
        {
            const int imageSize = 32;
            const int imageLength = imageSize * imageSize * 3;

            byte[] pixels;
            IList<fData> images = new List<fData>();
            fData image;
            FileStream fileStreamImage;
            BinaryReader imageReader;
            byte[] byteImage;
            Random random = new Random();
            try
            {
                //Stream byte data from file
                fileStreamImage = new FileStream(ubyteImageFileName, FileMode.Open, FileAccess.Read, FileShare.Read); // train images
                imageReader = new BinaryReader(fileStreamImage);
                byteImage = new byte[fileStreamImage.Length];      
                for (int i = 0; i < byteImage.Length; i++)
                    byteImage[i] = imageReader.ReadByte();                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            int nofStreamImages = byteImage.Length / (imageLength + 1);

            bool[] retr = new bool[nofStreamImages];
            long[] cntr = new long[10];

            int j, n = 0, r;
            byte l;

            switch (number == null)
            {
                case false:
                    int nofImages = number.Value * 10;
                    while (n < nofImages)
                    {
                        r = random.Next(0, nofStreamImages);                  
                        j = r * (imageLength + 1);
                        pixels = new byte[imageLength];
                        l = byteImage[j];

                        if ((retr[r] == true) || (cntr[l] >= number))
                            continue;

                        for (int i = 1; i < imageLength; i++)
                            pixels[i] = byteImage[j + i];

                        // create image
                        image = new fData(pixels, dataMin, dataMax, EncodeLabel(l), labelMin, labelMax);

                        // add to list of images
                        images.Add(image);

                        // update records
                        retr[r] = true;
                        cntr[l] += 1;
                        ++n;
                    }

                    break;

                case true:

                    while (n < nofStreamImages)
                    {
                        j = n * (imageLength + 1);
                        pixels = new byte[imageLength];
                        l = byteImage[j];
                        for (int i = 1; i < imageLength; i++)
                            pixels[i] = byteImage[j + i];

                        // create image
                        image = new fData(pixels, dataMin, dataMax, EncodeLabel(l), labelMin, labelMax);

                        // add to list of images
                        images.Add(image);
                        ++n;
                    }

                    break;
            }

            fileStreamImage.Close();
            imageReader.Close();
            return images;

        }
        public static IList<fData> ReadGrayImage(string ubyteImageFileName, int? number,
            double dataMin, double dataMax, string ubyteLabelFileName, double labelMin, double labelMax)
        {
            const int imageSize = 28;
            const int imageLength = imageSize * imageSize;

            byte[] pixels;
            IList<fData> images = new List<fData>();
            fData image;
            FileStream fileStreamLabel, fileStreamImage;
            BinaryReader labelReader, imageReader;
            byte[] byteImage, byteLabel;
            Random random = new Random();

            try
            {
                //Stream byte data from file
                fileStreamImage = new FileStream(ubyteImageFileName, FileMode.Open, FileAccess.Read, FileShare.Read); // train images
                imageReader = new BinaryReader(fileStreamImage);
                byteImage = new byte[fileStreamImage.Length - 16];
                for (int i = 0; i < 16; i++)
                    imageReader.ReadByte();
                for (int i = 0; i < byteImage.Length; i++)
                    byteImage[i] = imageReader.ReadByte();

                fileStreamLabel = new FileStream(ubyteLabelFileName, FileMode.Open, FileAccess.Read, FileShare.Read); // train labels
                labelReader = new BinaryReader(fileStreamLabel);
                byteLabel = new byte[fileStreamLabel.Length - 8];
                for (int i = 0; i < 8; i++)
                    labelReader.ReadByte();
                for (int i = 0; i < byteLabel.Length; i++)
                    byteLabel[i] = labelReader.ReadByte();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            int nofStreamImages = byteImage.Length / imageLength;
            int rem = byteImage.Length % imageLength;

            bool[] retr = new bool[nofStreamImages];
            long[] cntr = new long[10];

            int j, n = 0, r;
            byte l;

            switch (number == null)
            {
                case false:
                    int nofImages = number.Value * 10;
                    while (n < nofImages)
                    {
                        r = random.Next(0, nofStreamImages);
                        l = byteLabel[r];
                        if ((retr[r] == true) || (cntr[l] >= number))
                            continue;

                        // valid selection
                        j = r * imageLength;
                        pixels = new byte[imageLength];
                        for (int i = 0; i < imageLength; i++)
                            pixels[i] = byteImage[j + i];

                        // create image
                        image = new fData(pixels, dataMin, dataMax, EncodeLabel(l), labelMin, labelMax);

                        // add to list of images
                        images.Add(image);

                        // update records
                        retr[r] = true;
                        cntr[l] += 1;
                        ++n;
                    }

                    break;

                case true:

                    while (n < nofStreamImages)
                    {
                        j = n * imageLength;
                        pixels = new byte[imageLength];
                        for (int i = 0; i < imageLength; i++)
                            pixels[i] = byteImage[j + i];

                        // create image
                        image = new fData(pixels, dataMin, dataMax, EncodeLabel(byteLabel[n]), labelMin, labelMax);

                        // add to list of images
                        images.Add(image);
                        ++n;
                    }

                    break;
            }

            fileStreamImage.Close();
            imageReader.Close();

            fileStreamLabel.Close();
            labelReader.Close();

            return images;
        }      

    }
}