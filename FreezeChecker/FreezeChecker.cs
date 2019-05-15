using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

namespace FreezeChecker
{
    public class FreezeChecker
    {
        private static FreezeChecker freezeChecker;
        private string CheckPath;
        private int CheckingDepths;

        public static void Init()
        {
            if(freezeChecker == null)
                freezeChecker = new FreezeChecker(SystemConfig.SysConfig.ImageStoragePath,SystemConfig.SysConfig.FreezeCheckingDepth);
        }

        public FreezeChecker(string checkPath,int freezeCheckDepth)
        {
            CheckPath = checkPath;
            CheckingDepths = freezeCheckDepth;
            Start();
        }

        public void CheckFreeze()
        {
            try
            {
                foreach (var dir1 in Directory.GetDirectories(CheckPath))
                {

                    foreach (var dir2 in Directory.GetDirectories(dir1))
                    {
                        if(File.Exists(Path.Combine(dir2,SystemConfig.SysConfig.NoFreezeCheckFileName)))
                            continue;
                        
                        string freezeStatusFilePath = Path.Combine(dir2, SystemConfig.SysConfig.FreezeStatusFileName);
                        bool freezed = bool.Parse(File.ReadAllText(freezeStatusFilePath));

                        if(freezed)
                            continue;

                        string freezeFailureCountPath = Path.Combine(dir2, SystemConfig.SysConfig.FailureCountFileName);
                        int freezeFailureCount = int.Parse(File.ReadAllText(freezeFailureCountPath));

                        if(freezeFailureCount < SystemConfig.SysConfig.FreezeFailureTestThreadsHold)
                            continue;

                        string lastFrameCheckedPath = Path.Combine(dir2, SystemConfig.SysConfig.FreezeLastFrameChecked);
                        int lastFrameChecked =
                            int.Parse(
                                File.ReadAllText(lastFrameCheckedPath));

                        int frameCounter =
                            int.Parse(File.ReadAllText(Path.Combine(dir2, SystemConfig.SysConfig.ImgCounterFile)));

                        if(lastFrameChecked == frameCounter || frameCounter < CheckingDepths)
                            continue;

                        var pngsToCheck = new List<string>();
                        foreach (var png in Directory.GetFiles(dir2,"*.png"))
                        {
                            for (int i = frameCounter; i > frameCounter - CheckingDepths; i--)
                            {
                                string beginString = i + "_";
                                if (png.Length > 0)
                                {
                                    string pngName = Path.GetFileNameWithoutExtension(png);
                                    if (pngName.StartsWith(beginString) && !pngName.Contains("_outfocus"))
                                    {
                                        pngsToCheck.Add(png);
                                    }
                                }
                            }
                        }
                        
                        
                        for (int i = 1; i < pngsToCheck.Count; i++)
                        {
                            if (!MfVerificationLib.NativeMethods.CompareImage(Encoding.Default.GetBytes(pngsToCheck[i]),
                                Encoding.Default.GetBytes(pngsToCheck[i - 1])))
                            {
                                File.WriteAllText(lastFrameCheckedPath, frameCounter.ToString(CultureInfo.InvariantCulture));
                                return;
                            }
                        }
                        File.WriteAllText(freezeStatusFilePath, "true");
                    }

                }
            }
            catch (Exception e)
            {
                   
            }
        }

        public void FreezeCheckWorker(object o)
        {
            while (true)
            {
                CheckFreeze();
                Thread.Sleep(SystemConfig.SysConfig.FreezeCheckingDelay);
            }
        }

        public void Start()
        {
            Thread t = new Thread(new ParameterizedThreadStart(FreezeCheckWorker));
            t.Start();
        }
    }
}
