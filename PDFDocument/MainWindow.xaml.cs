using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PdfSharp;
using PdfSharp.Pdf;
using Converter;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace PDFDocument
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string DirName = "Pages";
        private int ActivePage;
        private string ActiveFileName = "";
        private DispatcherTimer timer;

        public PdfDocument Doc;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500)
            };

            timer.Tick += Timer_Tick;
            Settings.Instance.State = ProgramState.Start;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lbPage.Content = PDFToPNG.CurPage + "/" + PDFToPNG.PagesCount;
            prBar.Value = PDFToPNG.CurPage;
            Settings.Instance.State = ProgramState.Read;            

            if (PDFToPNG.CurPage >= PDFToPNG.PagesCount)
            {
                timer.Stop();
                prBar.Visibility = Visibility.Hidden;
                Settings.Instance.State = ProgramState.Complete;

                Doc = PdfSharp.Pdf.IO.PdfReader.Open(ActiveFileName, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                ShowPage(ActivePage);
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".pdf";
            openFile.Filter = "File PDF|*.pdf";
            openFile.FilterIndex = 0;


            Cursor = Cursors.Wait;

            if (openFile.ShowDialog().Value)
            {
                ActivePage = 1;
                ActiveFileName = openFile.FileName;             

                OpenActiveFile();
            }

            Cursor = Cursors.Arrow;
        }        

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            //WFind find = new WFind(Doc);
            WFind find = new WFind(ActiveFileName);
            find.ShowDialog();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (ActivePage < PDFToPNG.PagesCount)
            {
                ActivePage++;
                ShowPage(ActivePage);
            }
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (ActivePage > 1)
            {
                ActivePage--;
                ShowPage(ActivePage);
            }
        }

        private void ShowPage(int pageNum)
        {
            lbPage.Content = pageNum;
            ImageBrush image = new ImageBrush
            {
                Stretch = Stretch.Fill
            };

            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

            FileStream fileStream = File.OpenRead("Pages\\" + pageNum + ".png");
            bitmapImage.StreamSource = fileStream;
            bitmapImage.EndInit();

            fileStream.Close();
            fileStream.Dispose();

            image.ImageSource = bitmapImage;           

            brPage.Background = image;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Settings.Instance.State != ProgramState.Read)
            {
                Settings.Instance.ActivePage = ActivePage;
                Settings.Instance.ActiveFileName = ActiveFileName;
                Settings.Instance.Save();
            }            

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            prBar.Visibility = Visibility.Hidden;
            
            if (Settings.Instance.CheckFile)
            {
                ActiveFileName = Settings.Instance.ActiveFileName;
                ActivePage = Settings.Instance.ActivePage;

                if (!String.IsNullOrEmpty(ActiveFileName))
                {
                    OpenActiveFile();
                }                    
            }            

        }

        private void OpenActiveFile()
        {
            if (Directory.Exists(DirName))
            {
                Directory.Delete(DirName, true);
            }

            Directory.CreateDirectory(DirName);

            PDFToPNG.inputFile = ActiveFileName;
            PDFToPNG.outputDir = DirName;

            prBar.Value = 0;
          
            timer.Start();
            PDFToPNG.Convert();

            prBar.Maximum = PDFToPNG.PagesCount;
            prBar.Visibility = Visibility.Visible;
        }
    }
}
