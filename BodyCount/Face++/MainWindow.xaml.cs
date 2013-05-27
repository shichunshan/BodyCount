using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.IO;
using System.Windows.Threading;
using Face__.Properties;
using FaceppSDK;
using System.ComponentModel;
using System.Timers;
using Raven.Client.Document;

namespace Face__
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<BitmapSource> saveImages = new ObservableCollection<BitmapSource>();
        public ObservableCollection<DetectFaceInfo> DetectFaceInfos = new ObservableCollection<DetectFaceInfo>();
        private FaceService faceService;
        private Settings settings = Settings.Default;
        private string savePath;
        private string detectPath;
        private BackgroundWorker backgroundWorker;
        private List<string> fileNames = new List<string>();
        private DocumentStore DocumentStore;

        public MainWindow()
        {
            InitializeComponent();
            faceService = new FaceService("2affcadaeddd18f422375adc869f3991", "EsU9hmgweuz8U-nwv6s4JP-9AJt64vhz");
            savePath = settings.SaveLocation;
            detectPath = settings.DetectLocation;

            Loaded += MainWindow_Loaded;
            this.saveImagesElement.ItemsSource = saveImages;
            this.detectResultElement.ItemsSource = DetectFaceInfos;
            DocumentStore = new DocumentStore {Url = "http://localhost:8080/"};
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 2);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Start();
            DocumentStore.Initialize();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            fileNames = ReadFilePath(savePath);
            LoadImages(fileNames);
            DetectImages(fileNames);
        }


        private void DetectImages(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                DetectResult detectResult = new DetectResult();
                detectResult = faceService.Detection_DetectImg(savePath + "\\" + fileName);

                DetectFaceInfo detectFaceInfo = new DetectFaceInfo(detectResult, fileName);
                DetectFaceInfos.Add(detectFaceInfo);
                MoveFile(savePath + "\\" + fileName);
                SaveDataToDB(detectResult, fileName);
            }
        }

        private void SaveDataToDB(DetectResult detectResult, string fileName)
        {
            using (var session = DocumentStore.OpenSession())
            {
                string[] st = fileName.Split('_');
                DetectFaceInfo detectFaceInfo = session.Load<DetectFaceInfo>(st[0]);
                if (detectFaceInfo != null)
                {
                    if (detectFaceInfo.Race == null)
                    {
                        if (detectResult.face.Count > 0)
                        {
                            Face face = detectResult.face.SingleOrDefault();
                            detectFaceInfo.Age = face.attribute.age.value;
                            detectFaceInfo.AgeRange = face.attribute.age.range;

                            detectFaceInfo.Gender = face.attribute.gender.value;
                            detectFaceInfo.GenderConfidence = face.attribute.gender.confidence;

                            detectFaceInfo.Race = face.attribute.race.value;
                            detectFaceInfo.RaceConfidence = face.attribute.race.confidence;
                        }
                    }
                    session.Store(detectFaceInfo);
                    session.SaveChanges();
                }
            }
        }

        private void MoveFile(string filepath)
        {
            try
            {
                FileInfo file = new FileInfo(filepath);
                file.MoveTo(detectPath + "//" + file.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // MessageBox.Show(ex.ToString());
            }
        }

        private void LoadImages(List<string> fileNames)
        {
            foreach (var filePath in fileNames)
            {
                BinaryReader binReader = new BinaryReader(File.Open(savePath + "\\" + filePath, FileMode.Open));

                FileInfo fileInfo = new FileInfo(savePath + "\\" + filePath);

                byte[] bytes = binReader.ReadBytes((int) fileInfo.Length);

                binReader.Close();

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();

                bitmap.StreamSource = new MemoryStream(bytes);
                bitmap.EndInit();

                saveImages.Add(bitmap);
            }
        }

        private static List<string> ReadFilePath(string path)
        {
            List<string> filePaths = new List<string>();
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
                foreach (var fileInfo in fileInfos)
                {
                    filePaths.Add(fileInfo.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            return filePaths;
        }
    }
}
