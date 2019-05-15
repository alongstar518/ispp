using System;
using System.Globalization;
using System.IO;
using System.Text;
using ocrlib;
using Util;

namespace MfVerificationLib
{
    public static class MfVerificationMethods
    {
        public static string VerifyFocus(string imagePath, string verificationCategory, string expectedFocus ,string serviceName)
        {
            string ret;
            try
            {
                int[] focusrgba = SystemConfig.SysConfig.ServiceFocusColorMapping[serviceName];
                string imageDirectory = Path.GetDirectoryName(imagePath);
                string outFile = imageDirectory + "\\" + Path.GetFileNameWithoutExtension(imagePath) +  "_outfocus.png";
                string outFileCrash = imageDirectory + "\\" + Path.GetFileNameWithoutExtension(imagePath) + "_outfocuscrash.png";
                byte[] outFileBytes = Encoding.Default.GetBytes(outFile);
                byte[] outFileBytesCrash = Encoding.Default.GetBytes(outFileCrash);
                string[] _verificationCategory = verificationCategory.ToLower().Split('.');
                string ocrLang = _verificationCategory[_verificationCategory.Length - 1];
                bool linedItem = _verificationCategory[_verificationCategory.Length - 2] == "lined";
                int itemSize = 70;
                bool avVerify = false;
                string failureCounterFilePath = Path.Combine(imageDirectory, SystemConfig.SysConfig.FailureCountFileName);
                int failureCount = int.Parse(File.ReadAllText(failureCounterFilePath));
                string failureOCRLogFilePath = Path.Combine(imageDirectory, SystemConfig.SysConfig.FailureOCRFileName);
                string successOCRLogFilePath = Path.Combine(imageDirectory, SystemConfig.SysConfig.SuccessOCRFileName);
                string avOCRLogFilePath = Path.Combine(imageDirectory, SystemConfig.SysConfig.AVVerifyOCRFileName);

                try
                {
                    itemSize = int.Parse(_verificationCategory[_verificationCategory.Length - 3]);
                }
                catch (Exception)
                {                   
                }
                string ocrLog = string.Empty;
                NativeMethods.GetFocusImg(Encoding.Default.GetBytes(imagePath), outFileBytesCrash, false, itemSize, new[] { 72, 66, 50 },false);
                if (File.Exists(outFileCrash))
                {
                    try
                    {
                        string crashInfo = Ocr.GetText(outFileCrash, SystemConfig.SysConfig.TessrectDataPath, ocrLang);
                        ocrLog = string.Format("{0}-->{1}-->{2}, Expedcted: {3}", Path.GetFileName(imagePath),
                            Path.GetFileName(outFileCrash), crashInfo, expectedFocus);
                        if (VerifyCrash(crashInfo.ToLower()))
                            return "Crashed|";
                    }
                    catch (Exception e)
                    {
                        File.Delete(outFileCrash);
                    }
                }

                if (_verificationCategory[0].ToLower() == "ui")
                {
                    NativeMethods.GetFocusImg(Encoding.Default.GetBytes(imagePath), outFileBytes, linedItem, itemSize,focusrgba,true);
                    if (SystemConfig.SysConfig.VerifyImageMapping.ContainsKey(verificationCategory))
                    {
                        return NativeMethods.CompareImage(outFileBytes, Encoding.Default.GetBytes(SystemConfig.SysConfig.VerifyImageMapping[expectedFocus])) ? "Success|" : "Failed|";
                    }
                }
                else if (_verificationCategory[0].ToLower() == "page")
                {
                    outFile = imagePath;
                }
                else if (_verificationCategory[0].ToLower() == "av")
                {
                    outFile = imagePath;
                    avVerify = true;
                }

                string textFromPic;
                if (File.Exists(outFile))
                    textFromPic = Ocr.GetText(outFile, SystemConfig.SysConfig.TessrectDataPath, ocrLang);
                else
                    throw new Exception("Can not Find Focus");

                ocrLog += string.Format("{0}-->{1}-->{2}, Expedcted: {3}", Path.GetFileName(imagePath), Path.GetFileName(outFile), textFromPic,
                    expectedFocus);

                if (avVerify)
                {
                    ret = "Success|";
                    foreach (var error in SystemConfig.SysConfig.AvErrorMessage)
                    {
                        if (textFromPic.ToLower().Contains(error.ToLower()))
                        {
                            ret = "Failed|AV Playback Error";
                            failureCount++;
                            File.WriteAllText(failureCounterFilePath, failureCount.ToString(CultureInfo.InvariantCulture));
                            break;
                        }
                    }
                    File.WriteAllText(failureCounterFilePath, "0");
                    Logger.Log(ocrLog, avOCRLogFilePath);
                }
                else
                {
                    if (textFromPic.ToLower().Contains(expectedFocus.ToLower()))
                    {
                        ret = "Success|";
                        File.WriteAllText(failureCounterFilePath, "0");
                        Logger.Log(ocrLog,successOCRLogFilePath);
                    }
                    else
                    {
                        ret = "Failed|Expected Focus: " + expectedFocus + " Actual Focus:" + textFromPic;
                        failureCount++;
                        File.WriteAllText(failureCounterFilePath, failureCount.ToString(CultureInfo.InvariantCulture));
                        Logger.Log(ocrLog,failureOCRLogFilePath);
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error when verify focus, exception: " + e.Message);
                ret = "Failed|" + "Error When verify Focus, Exception: " + e.Message;
            }
            return ret;
        }

        private static bool VerifyCrash(string ocrText)
        {
            foreach (string t in SystemConfig.SysConfig.CrashInfoList)
            {
                string[] ta = t.Split('/');
                int exp = ta.Length;
                int act = 0;
                foreach (string l in ta)
                {
                    if (ocrText.Contains(l))
                        act++;
                }
                if (exp == act)
                    return true;
                
            }
            return false;
        }

        public static bool VerifyFreeze(string runId, string boxId)
        {
            try
            {
                string freezeFile = Path.Combine(SystemConfig.SysConfig.ImageStoragePath, runId, boxId,SystemConfig.SysConfig.FreezeStatusFileName);
                if (File.ReadAllText(freezeFile).ToLower().Contains("true"))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                //Logger.Log("Error when verifiy Freeze for run: " + runId +" box: " +boxId +" Exception:" + e.Message);
            }
            return false;
        }
    }
}
