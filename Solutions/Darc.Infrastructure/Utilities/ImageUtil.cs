namespace Darc.Infrastructure.Utilities
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using ComLib.MediaSupport;

    public class ImageUtil
    {
        private Image _image;

        public ImageUtil(byte[] data)
        {
            _image = ImageHelper.ConvertToImage(data);
        }

        public ImageUtil ResizeImage(int maxWidth)
        {
            if (_image.Width > maxWidth)
            {
                var newHeight = (maxWidth*_image.Height)/_image.Width;
                ResizeImage(maxWidth, newHeight);
            }

            return this;
        }

        public ImageUtil ResizeImage(int width, int height)
        {
            var newSize = new Size(width, height);

            var thumb = new Bitmap(_image, newSize);
            using (var g = Graphics.FromImage(_image)) // Create Graphics object from original Image
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                var codec = ImageCodecInfo.GetImageEncoders()[1];

                //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                var eParams = new EncoderParameters(1);
                eParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                //Now draw the image on the instance of thumbnail Bitmap object
                g.DrawImage(thumb, new Rectangle(0, 0, thumb.Width, thumb.Height));

                _image = thumb;
            }

            return this;
        }

        public ImageUtil CutImage(int x, int y, int width, int height)
        {
            var cropRect = new Rectangle(x, y, width, height);
            var src = new Bitmap(_image);

            var target = src.Clone(cropRect, src.PixelFormat);

            var thumb = new Bitmap(target);
            using (var g = Graphics.FromImage(thumb)) // Create Graphics object from original Image
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                //Set Image codec of JPEG type, the index of JPEG codec is "1"
                var codec = ImageCodecInfo.GetImageEncoders()[1];

                //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                var eParams = new EncoderParameters(1);
                eParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                //Now draw the image on the instance of thumbnail Bitmap object
                g.DrawImage(target, new Rectangle(0, 0, thumb.Width, thumb.Height));

                _image = thumb;
            }

            return this;
        }

        public ImageUtil RoundCorners(int cornerRadius, Color backgroundColor)
        {
            cornerRadius *= 2;

            var roundedImage = new Bitmap(_image.Width, _image.Height);
            var g = Graphics.FromImage(roundedImage);

            g.Clear(backgroundColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias; //文字抗锯齿

            Brush brush = new TextureBrush(_image);
            var gp = new GraphicsPath();
            gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
            gp.AddArc(0 + _image.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
            gp.AddArc(0 + _image.Width - cornerRadius, 0 + _image.Height - cornerRadius, cornerRadius, cornerRadius, 0,
                90);
            gp.AddArc(0, 0 + _image.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            g.FillPath(brush, gp);

            _image = roundedImage;

            return this;
        }

        public ImageUtil TextWaterMark(string text, int position)
        {
            //create a image object containing the photograph to watermark
            var imgPhoto = _image;
            var phWidth = imgPhoto.Width;
            var phHeight = imgPhoto.Height;

            //create a Bitmap the Size of the original photograph
            var bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //load the Bitmap into a Graphics object 
            var grPhoto = Graphics.FromImage(bmPhoto);

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
            for (var i = 0; i < 7; i++)
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
                case (int) ImageWatermarkPosition.LeftTop:
                    xCoordinate = (xPixlesFromLeftOrRight + (crSize.Width/2));
                    yCoordinate = (yPixlesFromTopOrBottom - (crSize.Height/2));
                    break;
                case (int) ImageWatermarkPosition.LeftBottom:
                    xCoordinate = xPixlesFromLeftOrRight + (crSize.Width/2);
                    yCoordinate = (phHeight - yPixlesFromTopOrBottom) - (crSize.Height/2);
                    break;
                case (int) ImageWatermarkPosition.RightTop:
                    xCoordinate = (phWidth - xPixlesFromLeftOrRight) - (crSize.Width/2);
                    yCoordinate = (yPixlesFromTopOrBottom - (crSize.Height/2));
                    break;
                case (int) ImageWatermarkPosition.RightBottom:
                    xCoordinate = (phWidth - xPixlesFromLeftOrRight) - (crSize.Width/2);
                    yCoordinate = (phHeight - yPixlesFromTopOrBottom) - (crSize.Height/2);
                    break;
                case (int) ImageWatermarkPosition.Center:
                    xCoordinate = (phWidth/2);
                    yCoordinate = (phHeight/2);
                    break;
                case (int) ImageWatermarkPosition.BottomCenter:
                    xCoordinate = (phWidth/2);
                    yCoordinate = ((phHeight - yPixlesFromTopOrBottom) - (crSize.Height/2));
                    break;
                default:
                    break;
            }

            //Define the text layout by setting the text alignment to centered
            var strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;

            //define a Brush which is semi trasparent black (Alpha set to 153)
            var semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));

            //Draw the Copyright string
            grPhoto.DrawString(text, //string of text
                crFont, //font
                semiTransBrush2, //Brush
                new PointF(xCoordinate + 1, yCoordinate + 1), //Position
                strFormat);

            //define a Brush which is semi trasparent white (Alpha set to 153)
            var semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));

            //Draw the Copyright string a second time to create a shadow effect
            //Make sure to move this text 1 pixel to the right and down 1 pixel
            grPhoto.DrawString(text, //string of text
                crFont, //font
                semiTransBrush, //Brush
                new PointF(xCoordinate, yCoordinate), //Position
                strFormat); //Text alignment


            _image = bmPhoto;

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return this;
        }

        public ImageUtil ImageWaterMark(string maskImagePath, ImageWatermarkPosition position)
        {
            var imgPhoto = _image;
            var phWidth = imgPhoto.Width;
            var phHeight = imgPhoto.Height;

            var mark = Image.FromFile(maskImagePath);

            //Resize the wartermark image

            var maskWidth = (phWidth/10);
            var maskHeight = (mark.Height/(mark.Width/maskWidth));

            if (maskWidth > mark.Width)
            {
                maskWidth = mark.Width;
                maskHeight = mark.Height;
            }

            var newSize = new Size(maskWidth, maskHeight);

            var thumb = new Bitmap(mark, newSize);
            using (var g = Graphics.FromImage(mark)) // Create Graphics object from original Image
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                var codec = ImageCodecInfo.GetImageEncoders()[1];

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
            var grPhoto = Graphics.FromImage(bmPhoto);

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

            var xPixlesOfMask = 0;
            var yPixlesOfMask = 0;

            switch (position)
            {
                case ImageWatermarkPosition.LeftTop:
                    xPixlesOfMask = xPixlesFromLeftOrRight;
                    yPixlesOfMask = yPixlesFromTopOrBottom;
                    break;
                case ImageWatermarkPosition.LeftBottom:
                    xPixlesOfMask = xPixlesFromLeftOrRight;
                    yPixlesOfMask = (phHeight - mark.Height - yPixlesFromTopOrBottom);
                    break;
                case ImageWatermarkPosition.RightTop:
                    xPixlesOfMask = (phWidth - mark.Width - xPixlesFromLeftOrRight);
                    yPixlesOfMask = yPixlesFromTopOrBottom;
                    break;
                case ImageWatermarkPosition.RightBottom:
                    xPixlesOfMask = (phWidth - mark.Width - xPixlesFromLeftOrRight);
                    yPixlesOfMask = (phHeight - mark.Height - yPixlesFromTopOrBottom);
                    break;
                case ImageWatermarkPosition.Center:
                    xPixlesOfMask = (phWidth/2) - (mark.Width/2);
                    yPixlesOfMask = (phHeight/2) - (mark.Height/2);
                    break;
                case ImageWatermarkPosition.BottomCenter:
                    break;
                default:
                    break;
            }

            grPhoto.DrawImage(mark, new Rectangle(xPixlesOfMask, yPixlesOfMask, mark.Width, mark.Height), 0, 0,
                mark.Width,
                mark.Height, GraphicsUnit.Pixel);

            _image = bmPhoto;

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return this;
        }

        public void SaveImage(string path)
        {
            var codecParams = new EncoderParameters(1);
            codecParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

            var encoders = ImageCodecInfo.GetImageEncoders();

            var extension = Path.GetExtension(path);
            if (extension != null)
                switch (extension.ToUpper())
                {
                    case ".PNG":
                        _image.Save(path, encoders[4], codecParams);
                        break;
                    case ".BMP":
                        _image.Save(path, encoders[0], codecParams);
                        break;
                    case ".JPG":
                        _image.Save(path, encoders[1], codecParams);
                        break;
                    case ".GIF":
                        _image.Save(path, encoders[2], codecParams);
                        break;
                }
        }
    }


    public enum FileType
    {
        [Description("图片")] Image = 1,
        [Description("文件")] File,
        [Description("视频")] Video,
        [Description("音频")] Audio,
        [Description("Flash")] Flash,
        [Description("封面图")] CoverImage
    }

    public enum ImageWatermarkPosition
    {
        [Description("左上角")] LeftTop = 1,
        [Description("左下角")] LeftBottom,
        [Description("右上角")] RightTop,
        [Description("右下角")] RightBottom,
        [Description("正中心")] Center,
        [Description("底部居中")] BottomCenter
    }
}