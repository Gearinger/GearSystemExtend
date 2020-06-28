using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace GearImageTools
{
    public static class Class1
    {
        /// <summary>
        /// 压缩图片(调整压缩系数、分辨率)
        /// </summary>
        /// <param name="sFile">原图片文件路径</param>
        /// <param name="dFile">压缩后图片文件路径</param>
        /// <param name="compressRate">压缩系数（越小压缩程度越高</param>
        /// <param name="dWidth">压缩后重新定义宽度（像素）</param>
        /// <param name="dHeight">压缩后重新定义高度（像素）</param>
        /// <returns></returns>
        public static bool GetPicThumbnail(string sFile, string dFile, int compressRate, int dWidth, int dHeight)
        {
            Image iSource = System.Drawing.Image.FromFile(sFile);
            var sourceWidth = iSource.Width;
            var sourceHeight = iSource.Height;

            #region 判断输出压缩系数/高度/宽度不合理时，赋默认值
            if (compressRate <= 0 || compressRate >= 100)
            {
                compressRate = 75;
            }
            if (dWidth <= 0 && dHeight <= 0)
            {
                dWidth = sourceWidth;
                dHeight = sourceHeight;
            }
            else if (dWidth <= 0)
            {
                dWidth = Convert.ToInt32(dHeight / (double)sourceHeight * sourceWidth);
            }
            else if (dHeight <= 0)
            {
                dHeight = Convert.ToInt32(dWidth / (double)sourceWidth * sourceHeight);
            }
            #endregion

            ImageFormat tFormat = iSource.RawFormat;
            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle(0, 0, dWidth, dHeight), 0, 0, sourceWidth, sourceHeight, GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量  
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = compressRate;//设置压缩的比例1-100  
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="sFile">原图片文件路径</param>
        /// <param name="dFile">压缩后图片文件路径</param>
        /// <param name="flag">压缩系数（越小压缩程度越高</param>
        /// <param name="scale">缩放比例</param>
        /// <returns></returns>
        public static bool GetPicThumbnail(string sFile, string dFile, int flag, double scale = 1)
        {
            Image iSource = System.Drawing.Image.FromFile(sFile);
            var width = iSource.Width * scale;
            var height = iSource.Height * scale;
            iSource.Dispose();
            return GetPicThumbnail(sFile, dFile, flag, Convert.ToInt32(width), Convert.ToInt32(height));
        }
    }
}
