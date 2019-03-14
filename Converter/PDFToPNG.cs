using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MoonPdfLib;
using MoonPdfLib.MuPdf;
using System.IO;
using System.Windows.Forms;

namespace Converter
{
    public static class PDFToPNG
    {
        private static float Precision = 1;
        public static int PagesCount = 0;

        private static IPdfSource pdfSource;
        public static string outputDir;
        public static string inputFile;

        public static int  CurPage = 0;

        private static void convertPdfToImg()
        {
            for (CurPage = 1; CurPage <= PagesCount; CurPage++)
            {
                System.Drawing.Bitmap pageImage = MuPdfWrapper.ExtractPage(pdfSource, CurPage, Precision);
                pageImage.Save(outputDir + "\\" + CurPage + ".png");
            }
        }

        public static void Convert()
        {
            if (!File.Exists(inputFile))
            {
                MessageBox.Show("File not found!");
                return;
            }

            pdfSource = new FileSource(inputFile);
            PagesCount = MuPdfWrapper.CountPages(pdfSource);

            Thread thread = new Thread(convertPdfToImg);
            thread.Start();
        }
    }
}
