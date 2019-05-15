using System.Drawing;
using Tesseract;

namespace ocrlib
{
    public static class Ocr
    {
        public static string GetText(string imgpath, string tessdataPath, string lang)
        {
            var ocrtext = string.Empty;
            var imgsource = new Bitmap(imgpath);
            using (var engine = new TesseractEngine(tessdataPath, lang, EngineMode.Default))
            {
                using (var img = PixConverter.ToPix(imgsource))
                {
                    using (var page = engine.Process(img))
                    {
                        ocrtext = page.GetText();
                    }
                }
            }

            return ocrtext;
        }
    }
}
