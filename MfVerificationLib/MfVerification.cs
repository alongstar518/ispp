using System.IO;
using ImageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MfVerificationLib
{
    public class MfVerification
    {
        //Verification category: UI.Box.eng or UI.70.Line.eng
        public string VerifyFocus(string base64ImgStr, string runId, string deviceId, string verificationCategory, string verificationValue, string packageName)
        {
            string[] imagepaths = ImageMgr.SavePicture(base64ImgStr, runId,deviceId,verificationCategory,verificationValue);

            return MfVerificationMethods.VerifyFocus(Path.Combine(imagepaths[0], imagepaths[1]), verificationCategory, verificationValue,packageName);
        }

        public bool VerifyFreeze(string runId, string boxId)
        {
            return MfVerificationMethods.VerifyFreeze(runId, boxId);
        }
    }
}
