using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.SessionState;

namespace MFTestImageService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            SystemConfig.SysConfig.InitConfig(WebConfigurationManager.AppSettings["TessrectDataPath"], WebConfigurationManager.AppSettings["ImageStoreagePath"], WebConfigurationManager.AppSettings["ReferenceImgPath"], WebConfigurationManager.AppSettings["VerifyImgMappingPath"], WebConfigurationManager.AppSettings["ColorConfigPath"], WebConfigurationManager.AppSettings["AvErrorListFilePath"], int.Parse(WebConfigurationManager.AppSettings["FreezingCheckingDepth"]), int.Parse(WebConfigurationManager.AppSettings["FreezingCheckingDelay"]), int.Parse(WebConfigurationManager.AppSettings["FreezeFailureTestThreadsHold"]), WebConfigurationManager.AppSettings["CrashInfoPath"]);
            FreezeChecker.FreezeChecker.Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}