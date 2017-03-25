﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace SharpCG.Core
{
    public class Image
    {

        private int width;
        private int height;
        private byte[] data;

        private string path;

        public byte[] Data
        {
            get => data;
        }


        public int Height
        {
            get => height; 

        }


        public int Width
        {
            get => width;
        }

        public string Path
        {
           get => path;
           set => path = value;
        }

        public Image(int width, int height, byte[] data)
        {
            this.width = width;
            this.width = height;

            
            if(width * height != data.Length)
            {
                Console.WriteLine("Image dimensions do not match!");
                this.data = new byte[width * height];
            }
            else
            {
                this.data = data;
            }                           
        }


        public Image(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new byte[width * height * 4];
        }


        public static Image FromBitmap(Bitmap bmp, bool flipped = true)
        {
            
            var img = new Image(bmp.Width, bmp.Height);
            if(flipped)
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            byte[] data = new byte[Math.Abs(bitmapData.Stride * bmp.Height)];
            Marshal.Copy(bitmapData.Scan0, img.data, 0, data.Length);
            bmp.UnlockBits(bitmapData);            

            return img;
        }


        public static Image FromFile(string path, bool flipped = true)
        {
            var i = FromBitmap(System.Drawing.Image.FromFile(path) as Bitmap, flipped);
            i.Path = path;
            
            return i;
        }
    }
}
