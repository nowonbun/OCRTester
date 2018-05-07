using System;
using System.Drawing;
using Tesseract;

namespace TessecratLib
{
    public class TessecratOCRConverter
    {
        //https://github.com/tesseract-ocr/tesseract
        //https://github.com/tesseract-ocr/tessdata
        public enum LanguageType
        {
            JPN,
            ENG
        }
        public static String Run(String config, LanguageType lang, String img)
        {
            Bitmap bit = (Bitmap)Bitmap.FromFile(img);
            var ocr = new TesseractEngine(config, lang == LanguageType.ENG ? "eng" : "jpn", EngineMode.TesseractOnly);
            var texts = ocr.Process(bit);
            return texts.GetText();
        }
    }
}
