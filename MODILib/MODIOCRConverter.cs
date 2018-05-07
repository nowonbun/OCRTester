using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODI;

namespace MODILib
{
    public class MODIOCRConverter
    {
        //https://github.com/tesseract-ocr/tesseract
        //https://github.com/tesseract-ocr/tessdata
        /*public enum LanguageType
        {
            JPN,
            ENG
        }*/
        public static String Run(String img)
        {
            Document ocr = new Document();
            ocr.Create(img);
            ocr.OCR(MiLANGUAGES.miLANG_ENGLISH, true, true);
            //The error will be occurred.
            //md.OCR(MODI.MiLANGUAGES.miLANG_JAPANESE, true, true);
            String ret = ocr.Images[0].Layout.Text;
            ocr.Close();
            return ret;
        }
    }
}
