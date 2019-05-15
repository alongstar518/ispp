using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Util;

namespace ImageManager
{
    public static class ImageMgr
    {
        /// <summary>
        /// Save Images and modify the file with frame counts for management.
        /// Create used Directory stucture and files needed if it doesn`t exists.
        /// File name will change like: RunID_BoxID_Packagename_VerifyCategory_Value.png  --> ImgCount_RunID_BoxID_VerifyCategory_Value.png
        /// </summary>
        /// <param name="base64ImgStr">base64 string of the image</param>
        /// <param name="runId"></param>
        /// <param name="boxId"></param>
        /// <param name="verificationCategory"></param>
        /// <param name="verificationValue"></param>
        /// <returns>String array, array[0] == Image Store directory, array[1] == file Name</returns>
        public static string[] SavePicture(string base64ImgStr, string runId, string boxId, string verificationCategory, string verificationValue)
        {
            try
            {
                string path1 = Path.Combine(SystemConfig.SysConfig.ImageStoragePath, runId);
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(path1);
                }
                string path2 = Path.Combine(path1, boxId);
                string imgCounterFile = Path.Combine(path2, SystemConfig.SysConfig.ImgCounterFile);
                if (!Directory.Exists(path2))
                {
                    Directory.CreateDirectory(path2);
                    File.WriteAllText(imgCounterFile,"0");
                    File.WriteAllText(Path.Combine(path2,SystemConfig.SysConfig.FreezeLastFrameChecked),"0");
                    File.WriteAllText(Path.Combine(path2, SystemConfig.SysConfig.FreezeStatusFileName), "false");
                    File.WriteAllText(Path.Combine(path2,SystemConfig.SysConfig.FailureCountFileName),"0");
                }

                int count = int.Parse(File.ReadAllText(imgCounterFile));
                string newImgName = string.Format("{0}_{1}_{2}_{3}_{4}.png", ++count, runId, boxId, verificationCategory,
                    verificationValue);
                string imgPath = Path.Combine(path2, newImgName);
                byte[] data = Convert.FromBase64String(base64ImgStr);
                using (var stream = new MemoryStream(data, 0, data.Length))
                {
                    var img= Image.FromStream(stream);
                    img.Save(imgPath);
                }
                File.WriteAllText(imgCounterFile,count.ToString(CultureInfo.InvariantCulture));
                return new []{path2,newImgName};
            }
            catch (Exception e)
            {
                //Logger.Log("Error when save Picture for " + runId + " box:" +boxId + " Exception:" +e.Message);
            }
            return null;
        }
    }
}
