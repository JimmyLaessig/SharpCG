using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace SharpCG
{
    public class Image
    {
        private uint width;
        private uint height;
        private byte[] data;


        public byte[] Data
        {
            get{return data;}
        }


        public uint Height
        {
            get{return height; }

        }


        public uint Width
        {
            get{return width;}
        }


        public Image(uint width, uint height, byte[] data)
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


        public Image(uint width, uint height)
        {
            this.width = width;
            this.height = height;
            this.data = new byte[width * height * 4];
        }


        public static Image FromBitmap(Bitmap bmp)
        {
            var img = new Image((uint)bmp.Width, (uint)bmp.Height);
           
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] data = new byte[Math.Abs(bitmapData.Stride * bmp.Height)];
            Marshal.Copy(bitmapData.Scan0, img.data, 0, data.Length);
            bmp.UnlockBits(bitmapData);            

            return img;
        }


        public static Image FromFile(string path)
        {
            return FromBitmap( Bitmap.FromFile(path) as Bitmap);           
        }
    }
}
