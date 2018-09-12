using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Unti
{
    public class Base64Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basestr"></param>
        /// <returns></returns>
        public static Bitmap Base64StringToImage(string basestr)
        {
            Bitmap bitmap = null;
            try
            {
                String inputStr = basestr;
                byte[] arr = Convert.FromBase64String(inputStr);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                bitmap = bmp;
            }
            catch (Exception ex)
            {
                throw new Exception("转换失败!错误信息:" + ex.Message);
            }

            return bitmap;

        }

        public static String ImgToBase64String(Image bmp)
        {
            String strbaser64 = String.Empty;
            var btarr = convertByte(bmp);
            strbaser64 = Convert.ToBase64String(btarr);

            return strbaser64;
        }

        /// <summary>
        /// Image转byte[]
        /// </summary>
        /// <param name="img">Img格式数据</param>
        /// <returns>byte[]格式数据</returns>
        public static byte[] convertByte(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, img.RawFormat);
            byte[] bytes = ms.ToArray();
            ms.Close();
            return bytes;
        }
    }
}