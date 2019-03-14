using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;
using Aspose.Pdf;
using Aspose.Pdf.Text;

namespace PDFDocument
{
    /// <summary>
    /// Interaction logic for WFind.xaml
    /// </summary>
    public partial class WFind : Window
    {
        PdfDocument Doc;
        List<int> Pages;
        string ActiveFileName;

        public WFind(PdfDocument doc)
        {
            InitializeComponent();
            Doc = doc;
            Pages = new List<int>();
        }

        public WFind(string fileName)
        {
            InitializeComponent();
            ActiveFileName = fileName;
            Pages = new List<int>();
        }

        //public List<int> ReadPdfFile(string fileName, String searthText)
        //{
        //    List<int> pages = new List<int>();
        //    if (File.Exists(fileName))
        //    {
        //        //PdfReader pdfReader = new PdfReader(fileName);
        //        for (int page = 1; page <= Doc.PageCount; page++)
        //        {
        //            //ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();


        //            string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
        //            if (currentPageText.Contains(searthText))
        //            {
        //                pages.Add(page);
        //            }
        //        }
        //        pdfReader.Close();
        //    }
        //    return pages;
        //}

        private  void ExtractData1()
        {
            var pdfDocument = new Document(ActiveFileName);
            TextAbsorber textAbsorber = new TextAbsorber();
            pdfDocument.Pages.Accept(textAbsorber);
            String extractedText = textAbsorber.Text;
            textAbsorber.Visit(pdfDocument);
            File.WriteAllText(@"demodata.txt", extractedText);
        }

        private void Read(string text)
        {
            for (int i = 0; i < Doc.PageCount; i++)
            {
                var page = Doc.Pages[i];

                try
                {
                    CObject content = ContentReader.ReadContent(page);
                    var extractedText = MyExtractText(content);

                    foreach (string item in extractedText)
                    {

                        if (item.ToLower().Contains(text.ToLower()))
                        {
                            Pages.Add(i);
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);


                    //MessageBox.Show($"Error on page {i}");
                }


            }

            //lbPages.DataContext = Pages;

        }

        private static IEnumerable<string> MyExtractText(CObject cObject)
        {
            var textList = new List<string>();
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                    {
                        textList.AddRange(MyExtractText(cOperand));
                    }
                }
            }

            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                {
                    textList.AddRange(MyExtractText(element));
                }
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                textList.Add(cString.Value);
            }
            return textList;
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            //Read(txtFind.Text);
            ExtractData1();

            Close();
        }
    }
}
