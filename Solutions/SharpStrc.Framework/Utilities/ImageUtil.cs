namespace SharpStrc.Framework.Utilities
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using ComLib.MediaSupport;
    using Domain;

    public class ImageUtil
    {
        private Image image;

        public ImageUtil(byte[] data)
        {
            image = ImageHelper.ConvertToImage(data);
        }

        public ImageUtil ResizeImage(int maxWidth)
        {
            if (image.Width > maxWidth)
            {
                int newHeight = (maxWidth*image.Height)/image.Width;
                ResizeImage(maxWidth, newHeight);
            }

            return this;
        }

        public ImageUtil ResizeImage(int width, int height)
        {
            var newSize = new Size(width, height);

            var thumb = new Bitmap(image, newSize);
            using (Graphics g = Graphics.FromImage(image)) // Create Graphics object from original Image
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders()[1];

                //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                var eParams = new EncoderParameters(1);
                eParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                //Now draw the image on the instance of thumbnail Bitmap object
                g.DrawImage(thumb, new Rectangle(0, 0, thumb.Width, thumb.Height));

                image = thumb;
            }

            return this;
        }

        public ImageUtil CutImage(int x, int y, int width, int height)
        {
            var cropRect = new Rectangle(x, y, width, height);
            var src = new Bitmap(image);

            Bitmap target = src.Clone(cropRect, src.PixelFormat);

            var thumb = new Bitmap(target);
            using (Graphics g = Graphics.FromImage(thumb)) // Create Graphics object from original Image
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                //Set Image codec of JPEG type, the index of JPEG codec is "1"
                ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders()[1];

                //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                var eParams = new EncoderParameters(1);
                eParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                //Now draw the image on the instance of thumbnail Bitmap object
                g.DrawImage(target, new Rectangle(0, 0, thumb.Width, thumb.Height));

                image = thumb;
            }

            return this;
        }

        public ImageUtil RoundCorners(int CornerRadius, Color backgroundColor)
        {
            CornerRadius *= 2;

            var roundedImage = new Bitmap(image.Width, image.Height);
            Graphics g = Graphics.FromImage(roundedImage);

            g.Clear(backgroundColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias; //文字抗锯齿

            Brush brush = new TextureBrush(image);
            var gp = new GraphicsPath();
            gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
            gp.AddArc(0 + image.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
            gp.AddArc(0 + image.Width - CornerRadius, 0 + image.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
            gp.AddArc(0, 0 + image.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
            g.FillPath(brush, gp);

            image = roundedImage;

            return this;
        }

        public ImageUtil TextWaterMark(string text, int position)
        {
            //create a image object containing the photograph to watermark
            Image imgPhoto = image;
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            //create a Bitmap the Size of the original photograph
            var bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //load the Bitmap into a Graphics object 
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //------------------------------------------------------------
            //Step #1 - Insert Copyright message
            //------------------------------------------------------------

            //Set the rendering quality for this Graphics object
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //Draws the photo Image object at original size to the graphics object.
            grPhoto.DrawImage(
                imgPhoto, // Photo Image object
                new Rectangle(0, 0, phWidth, phHeight), // Rectangle structure
                0, // x-coordinate of the portion of the source image to draw. 
                0, // y-coordinate of the portion of the source image to draw. 
                phWidth, // Width of the portion of the source image to draw. 
                phHeight, // Height of the portion of the source image to draw. 
                GraphicsUnit.Pixel); // Units of measure 

            //-------------------------------------------------------
            //to maximize the size of the Copyright message we will 
            //test multiple Font sizes to determine the largest posible 
            //font we can use for the width of the Photograph
            //define an array of point sizes you would like to consider as possiblities
            //-------------------------------------------------------
            var sizes = new[] {16, 14, 12, 10, 8, 6, 4};

            Font crFont = null;
            var crSize = new SizeF();

            //Loop through the defined sizes checking the length of the Copyright string
            //If its length in pixles is less then the image width choose this Font size.
            for (int i = 0; i < 7; i++)
            {
                //set a Font object to Arial (i)pt, Bold
                crFont = new Font("arial", sizes[i], FontStyle.Bold);
                //Measure the Copyright string in this Font
                crSize = grPhoto.MeasureString(text, crFont);

                if ((ushort) crSize.Width < (ushort) phWidth/3)
                    break;
            }

            //Since all photographs will have varying heights, determine a 
            //position 5% from the bottom of the image
            var yPixlesFromTopOrBottom = (int) (phHeight*.05);
            var xPixlesFromLeftOrRight = (int) (phWidth*.05);

            //Now that we have a point size use the Copyrights string height 
            //to determine a y-coordinate to draw the string of the photograph
            float yCoordinate = 0;
            //Determine its x-coordinate by calculating the center of the width of the image
            float xCoordinate = 0;

            switch (position)
            {
                case (int) GVar.ImageWatermarkPosition.LeftTop:
                    xCoordinate = (xPixlesFromLeftOrRight + (crSize.Width/2));
                    yCoordinate = (yPixlesFromTopOrBottom - (crSize.Height/2));
                    break;
                case (int) GVar.ImageWatermarkPosition.LeftBottom:
                    xCoordinate = xPixlesFromLeftOrRight + (crSize.Width/2);
                    yCoordinate = (phHeight - yPixlesFromTopOrBottom) - (crSize.Height/2);
                    break;
                case (int) GVar.ImageWatermarkPosition.RightTop:
                    xCoordinate = (phWidth - xPixlesFromLeftOrRight) - (crSize.Width/2);
                    yCoordinate = (yPixlesFromTopOrBottom - (crSize.Height/2));
                    break;
                case (int) GVar.ImageWatermarkPosition.RightBottom:
                    xCoordinate = (phWidth - xPixlesFromLeftOrRight) - (crSize.Width/2);
                    yCoordinate = (phHeight - yPixlesFromTopOrBottom) - (crSize.Height/2);
                    break;
                case (int) GVar.ImageWatermarkPosition.Center:
                    xCoordinate = (phWidth/2);
                    yCoordinate = (phHeight/2);
                    break;
                case (int) GVar.ImageWatermarkPosition.BottomCenter:
                    xCoordinate = (phWidth/2);
                    yCoordinate = ((phHeight - yPixlesFromTopOrBottom) - (crSize.Height/2));
                    break;
                default:
                    break;
            }

            //Define the text layout by setting the text alignment to centered
            var StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;

            //define a Brush which is semi trasparent black (Alpha set to 153)
            var semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));

            //Draw the Copyright string
            grPhoto.DrawString(text, //string of text
                               crFont, //font
                               semiTransBrush2, //Brush
                               new PointF(xCoordinate + 1, yCoordinate + 1), //Position
                               StrFormat);

            //define a Brush which is semi trasparent white (Alpha set to 153)
            var semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));

            //Draw the Copyright string a second time to create a shadow effect
            //Make sure to move this text 1 pixel to the right and down 1 pixel
            grPhoto.DrawString(text, //string of text
                               crFont, //font
                               semiTransBrush, //Brush
                               new PointF(xCoordinate, yCoordinate), //Position
                               StrFormat); //Text alignment


            image = bmPhoto;

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return this;
        }

        public ImageUtil ImageWaterMark(string maskImagePath, GVar.ImageWatermarkPosition position)
        {
            Image imgPhoto = image;
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            Image mark = Image.FromFile(maskImagePath);

            //Resize the wartermark image

            int maskWidth = (phWidth/10);
            int maskHeight = (mark.Height/(mark.Width/maskWidth));

            if (maskWidth > mark.Width)
            {
                maskWidth = mark.Width;
                maskHeight = mark.Height;
            }

            var newSize = new Size(maskWidth, maskHeight);

            var thumb = new Bitmap(mark, newSize);
            using (Graphics g = Graphics.FromImage(mark)) // Create Graphics object from original Image
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders()[1];

                //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                var eParams = new EncoderParameters(1);
                eParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                //Now draw the image on the instance of thumbnail Bitmap object
                g.DrawImage(thumb, new Rectangle(0, 0, thumb.Width, thumb.Height));

                mark = thumb;
            }

            //Add wartermark

            var bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //load the Bitmap into a Graphics object 
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //Draws the photo Image object at original size to the graphics object.
            grPhoto.DrawImage(
                imgPhoto, // Photo Image object
                new Rectangle(0, 0, phWidth, phHeight), // Rectangle structure
                0, // x-coordinate of the portion of the source image to draw. 
                0, // y-coordinate of the portion of the source image to draw. 
                phWidth, // Width of the portion of the source image to draw. 
                phHeight, // Height of the portion of the source image to draw. 
                GraphicsUnit.Pixel); // Units of measure 

            var xPixlesFromLeftOrRight = (int) (phWidth*.05);
            var yPixlesFromTopOrBottom = (int) (phHeight*.05);

            int xPixlesOfMask = 0;
            int yPixlesOfMask = 0;

            switch (position)
            {
                case GVar.ImageWatermarkPosition.LeftTop:
                    xPixlesOfMask = xPixlesFromLeftOrRight;
                    yPixlesOfMask = yPixlesFromTopOrBottom;
                    break;
                case GVar.ImageWatermarkPosition.LeftBottom:
                    xPixlesOfMask = xPixlesFromLeftOrRight;
                    yPixlesOfMask = (phHeight - mark.Height - yPixlesFromTopOrBottom);
                    break;
                case GVar.ImageWatermarkPosition.RightTop:
                    xPixlesOfMask = (phWidth - mark.Width - xPixlesFromLeftOrRight);
                    yPixlesOfMask = yPixlesFromTopOrBottom;
                    break;
                case GVar.ImageWatermarkPosition.RightBottom:
                    xPixlesOfMask = (phWidth - mark.Width - xPixlesFromLeftOrRight);
                    yPixlesOfMask = (phHeight - mark.Height - yPixlesFromTopOrBottom);
                    break;
                case GVar.ImageWatermarkPosition.Center:
                    xPixlesOfMask = (phWidth/2) - (mark.Width/2);
                    yPixlesOfMask = (phHeight/2) - (mark.Height/2);
                    break;
                case GVar.ImageWatermarkPosition.BottomCenter:
                    break;
                default:
                    break;
            }

            grPhoto.DrawImage(mark, new Rectangle(xPixlesOfMask, yPixlesOfMask, mark.Width, mark.Height), 0, 0,
                              mark.Width,
                              mark.Height, GraphicsUnit.Pixel);

            image = bmPhoto;

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return this;
        }

        public void SaveImage(string path)
        {
            var codecParams = new EncoderParameters(1);
            codecParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

            ImageCodecInfo[] encoders;

            encoders = ImageCodecInfo.GetImageEncoders();

            switch (Path.GetExtension(path).ToUpper())
            {
                case ".PNG":
                    image.Save(path, encoders[4], codecParams);
                    break;
                case ".BMP":
                    image.Save(path, encoders[0], codecParams);
                    break;
                case ".JPG":
                    image.Save(path, encoders[1], codecParams);
                    break;
                case ".GIF":
                    image.Save(path, encoders[2], codecParams);
                    break;
            }
        }
    }
}