using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MfVerificationLib;

namespace MFTestImageService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MfCtmVerify : System.Web.Services.WebService
    {
        [WebMethod]
        public string VerifyFocus(string base64ImgStr,string runId,string deviceId,string verificationCategory, string verificationValue, string serviceName)
        {
            var verification = new MfVerification();
            return verification.VerifyFocus(base64ImgStr, runId, deviceId, verificationCategory, verificationValue,serviceName);
        }

        [WebMethod]
        public bool VerifyFreeze(string runId, string boxId)
        {
            var verification = new MfVerification();
            return verification.VerifyFreeze(runId, boxId);
        }
    }
}