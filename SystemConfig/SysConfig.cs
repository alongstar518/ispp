using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SystemConfig
{
    public static class SysConfig
    {
        public static string TessrectDataPath;
        public static string ImageStoragePath;
        public static string ReferenceImgPath;

        public static string FreezeStatusFileName = "Freeze.txt";
        public static string ImgCounterFile = "counter.txt";
        public static string FreezeLastFrameChecked = "LastFrameChked.txt";
        public static string FailureCountFileName = "FailureCount.txt";
        public static string FailureOCRFileName = "OCRFailure.txt";
        public static string SuccessOCRFileName = "OCRSuccess.txt";
        public static string AVVerifyOCRFileName = "OCRAV.txt";
        public static string NoFreezeCheckFileName = "nofreezecheck.txt";
        

        public static List<string> AvErrorMessage = new List<string>();
        public static List<string> CrashInfoList = new List<string>(); 
        public static int FreezeCheckingDepth;
        public static int FreezeCheckingDelay;
        public static int FreezeFailureTestThreadsHold;

        public static Dictionary<string,string> VerifyImageMapping = new Dictionary<string, string>(); //Some focused item doesn`t have words. needs bitmap compare.
        public static Dictionary<string,int[]> ServiceFocusColorMapping = new Dictionary<string, int[]>(); //Focus color mapping, "com.ericsson.mediaroom.tvx : {b,g,r}"

        public static void InitConfig(string tessrectDataPath, string imageStoragePath, string referenceImgPath,
            string verifyImgMappingPath, string colorConfigPath, string AvErrorListFilePath, int freezeCheckingDepth, int freezeCheckingDelay, int freezeFailureTestThreadsHold,string crashInfoPath)
        {
            TessrectDataPath = tessrectDataPath;
            ImageStoragePath = imageStoragePath;
            ReferenceImgPath = referenceImgPath;
            FreezeCheckingDepth = freezeCheckingDepth;
            FreezeCheckingDelay = freezeCheckingDelay;
            FreezeFailureTestThreadsHold = freezeFailureTestThreadsHold;
            
            var content = File.ReadAllLines(verifyImgMappingPath);
            foreach (var l in content)
            {
                string[] c = l.Split('=');
                string path = Path.Combine(referenceImgPath, c[1]);
                VerifyImageMapping.Add(c[0],path);
            }

            content = File.ReadAllLines(colorConfigPath);
            foreach (var l in content)
            {
                string[] c = l.Split('=');
                string[] v = c[1].Split(',');
                int[] bgr = new[] {int.Parse(v[0]), int.Parse(v[1]), int.Parse(v[2])};
                ServiceFocusColorMapping.Add(c[0], bgr);
            }

            content = File.ReadAllLines(AvErrorListFilePath);
            foreach (var l in content)
            {
                AvErrorMessage.Add(l);
            }

            content = File.ReadAllLines(crashInfoPath);
            foreach (var l in content)
            {
                CrashInfoList.Add(l);
            }
        }
    }
}
