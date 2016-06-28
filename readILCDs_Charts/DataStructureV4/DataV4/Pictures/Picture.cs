using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4
{
    public class Picture : IPicture
    {
        #region Fields

        private Image _image;
        private string _name = "";

        #endregion

        #region Constructors

        public Picture() { }

        public Picture(GData data, XmlNode xmlNode, string optionalParamPrefix)
        {
            this.FromXmlNode(data, xmlNode, "");
        }

        public Picture(string name, Image image)
        {
            _name = name;
            _image = image;
        }

        #endregion

        #region Methods

        private Image ImageFromByteArray(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                return Image.FromStream(stream);
            }
        }

        private byte[] ByteArrayFromImage(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        //deprecated method used to create bitmap from uncompressed byte array loaded from database
        private Bitmap BitmapFromByteArrayOld(byte[] byteArray)
        {
            int n = 0;
            // Get the width
            uint x = (((uint)byteArray[n] * 256 + (uint)byteArray[n + 1]) * 256 + (uint)byteArray[n + 2]) * 256 + (uint)byteArray[n + 3];
            int width = (int)x;
            n += 4;
            // Get the height
            x = (((uint)byteArray[n] * 256 + (uint)byteArray[n + 1]) * 256 + (uint)byteArray[n + 2]) * 256 + (uint)byteArray[n + 3];
            int height = (int)x;
            n += 4;
            // Create the Bitmmap object
            Bitmap bmp = new Bitmap(width, height);
            // The pixels are stored in order by rows
            for (int j = 0; j < height; j++)
            {
                // Read the pixels for each row
                for (int i = 0; i < width; i++)
                {
                    //this could be optimized using the unsafe picture processing which uses pointers to access a pixel instead of managed object
                    //the SetPixel line takes 24 % of the database loading time
                    x = (((uint)byteArray[n] * 256 + (uint)byteArray[n + 1]) * 256 + (uint)byteArray[n + 2]) * 256 + (uint)byteArray[n + 3];
                    bmp.SetPixel(i, j, Color.FromArgb((int)x));
                    n += 4;
                }
            }
            return bmp;
        }

        #endregion

        #region IPicture

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateNode("picture", xmlDoc.CreateAttr("name", _name));

            byte[] rawBitmap = ByteArrayFromImage(_image);
            string compressedString = Convert.ToBase64String(rawBitmap);
            XmlAttribute picValue = xmlDoc.CreateAttr("base64", compressedString);

            node.Attributes.Append(picValue);

            return node;
        }


        private void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            _name = node.Attributes["name"].Value;
            string value = node.Attributes["base64"].Value;
            byte[] imageData = Convert.FromBase64String(value);

            //check to make sure database supports new image format
            byte[] pngHeaderSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

            if (pngHeaderSignature.SequenceEqual(imageData.Take(pngHeaderSignature.Length)))
            {
                _image = ImageFromByteArray(imageData);
            }
            else
            {
                _image = BitmapFromByteArrayOld(imageData);
            }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }

        #endregion

        #region Accessors

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Image Image
        {
            get { return _image; }
        }

        #endregion

    }
}