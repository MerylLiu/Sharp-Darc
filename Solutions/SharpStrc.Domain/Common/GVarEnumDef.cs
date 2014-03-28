namespace SharpStrc.Domain
{
    using System.ComponentModel;

    public partial class GVar
    {
        public enum AvatarSize
        {
            Size30 = 1,
            Size50,
            Size100,
            Size180
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
}