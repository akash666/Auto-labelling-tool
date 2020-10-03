using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using Microsoft.WindowsAPICodePack.Dialogs;
//using Path = System.Windows.Shapes.Path;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using Path = System.IO.Path;
using Microsoft.Win32;

namespace IvisLabellingAutomationtoolVersion1._2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List of images 
        List<string> ImageList = new List<string>();
        //List of xml files
        List<String> XmlFileList = new List<string>();
        //Dictionary for Image and File if file is existed...
        Dictionary<string, string> Image_XmlFile_Dictionary = new Dictionary<string, string>();

        bool IsRectangleSelected = false;

        ObservableCollection<LabelClass1> LabelsList1 = null;
        ObservableCollection<Rectangle> RectanglesList = null;
        Dictionary<string, ObservableCollection<LabelClass1>> ImageListObjectDictionary = new Dictionary<string, ObservableCollection<LabelClass1>>();
        Dictionary<string, ObservableCollection<Rectangle>> RectangleListObjectDictionary = new Dictionary<string, ObservableCollection<Rectangle>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        string folder;
        int fileCount;
        string[] pathOfSets;
        public void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            //List to keep only name of image files...
            List<string> ImageList2 = new List<string>();
            var Dialog = new CommonOpenFileDialog();
            
            Dialog.IsFolderPicker = true;
            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                folder = Dialog.FileName;
                // Do something with selected folder string...
                string[] Images = Directory.GetFiles(folder);

                foreach (string img in Images)
                {
                    // fileName = Path.GetFileNameWithoutExtension(img);
                    // s = Path.GetFileName(img);
                    string FileExtension = Path.GetExtension(Path.GetFileName(img));
                    if (FileExtension == ".xml")
                    {
                        XmlFileList.Add(img);
                    }
                    else
                    {
                        string imageName = Path.GetFileNameWithoutExtension(img);
                        ImageList.Add(img);
                        ImageList2.Add(imageName);
                    }
                }
                fileCount = (from file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
                                 select file).Count();
                MessageBox.Show("No. of files : " + fileCount.ToString());

            }
          
            //!!!--- Spliting the hole images in sets ---!!!
            
            int n = fileCount / 5;

            string DestinationDir = @Path.GetDirectoryName(ImageList[0]);
            int x = 0;
            for (int i = 1; i <= 5; i++)
            {
                //int fn = i + 1;
                for (int j = 0; j < n; j++)
                {
                    try
                    {
                        string Foldername = "Set- " + i;
                        string FolderPath = System.IO.Path.Combine(DestinationDir, Foldername);
                       // pathOfSets[i] = FolderPath;
                        System.IO.Directory.CreateDirectory(FolderPath);
                        //string str = Path.GetFileName(ImageSets[i][j]);
                        string str = Path.GetFileName(ImageList[x]);

                        // Bitmap b = new Bitmap(@ImageSets[i][j]);
                        System.Drawing.Bitmap b = new System.Drawing.Bitmap(@ImageList[x]);
                        x++;
                        MessageBox.Show("Image name : " + str);
                        string PathForSaveImage = FolderPath + "/" + str;
                        ////MessageBox.Show("Image name :" + Fname);
                        b.Save(@PathForSaveImage);
                    }
                    catch (Exception ex)
                     {
                        MessageBox.Show(ex.Message);
                    }
                }
            }


            //Load all images and xml files into Image_File_Dictionary...
            if (ImageList.Count() > 0)
            {
                
                for(int i = 0; i < ImageList.Count(); i++)
                {
                    string str = Path.GetFileName( ImageList[i] ) + ".xml";
                    if (XmlFileList.Contains(str))
                    {
                        MessageBox.Show("Image Name is : "+ ImageList[i] + " Xml File name is : " + str);
                        Image_XmlFile_Dictionary.Add(ImageList[i] , str);
                    }
                }
            }

            string fileName1 = Path.GetFileNameWithoutExtension(ImageList[0]);
            imageName1.Text = fileName1;
            Image1.Source = new BitmapImage(new Uri(ImageList[0]));

            ImageListView1.ItemsSource = ImageList2;

            
        }

        private void CreateRectBox_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRectangleSelected)
                IsRectangleSelected = true;
            else
                IsRectangleSelected = false;
        }

        private Point startPoint;
        public Rectangle rectSelectArea;

        public Double xmin, ymin, xmax, ymax, w, h;

        private void Canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsRectangleSelected)
            {
                startPoint = e.GetPosition(Canvas1);
                //if (rectSelectArea != null)
                //{
                //    Canvas1.Children.Remove(rectSelectArea);
                //}

                rectSelectArea = new Rectangle
                {
                    Stroke = Brushes.Aqua,
                    StrokeThickness = 2,
                    //Fill = Brushes.Transparent          

                };

                Canvas.SetLeft(rectSelectArea, startPoint.X);
                Canvas.SetTop(rectSelectArea, startPoint.X);
                Canvas1.Children.Add(rectSelectArea);
            }
            else
            {
                MessageBox.Show("Please select rectangle to create rectangle");
            }
        }
        private void Canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                var pos = e.GetPosition(Canvas1);

                // Set the position of rectangle
                xmin = Math.Min(pos.X, startPoint.X);
                ymin = Math.Min(pos.Y, startPoint.Y);
                xmax = Math.Max(pos.X, startPoint.X);
                ymax = Math.Max(pos.Y, startPoint.Y);
                // Set the dimenssion of the rectangle
                w = xmax - xmin;
                h = ymax - ymin;

                rectSelectArea.Width = w;
                rectSelectArea.Height = h;

                Canvas.SetLeft(rectSelectArea, xmin);
                Canvas.SetTop(rectSelectArea, ymin);
            }
        }

        private void Canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Canvas1.Children.Add(rectSelectArea);
            Popup1.IsOpen = true;
            IsRectangleSelected = false;
            PopUp_TextBox1.Focusable = true;
            Keyboard.Focus(PopUp_TextBox1);

            //if (e.Key == Key.Enter)
            //{
            //    PopUp_Ok_Button1.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //    // PopUp_Ok_Button1.PerformClick();
            //}
        }

        private void PopUp_Ok_Button1_Click(object sender, RoutedEventArgs e)
        {

            LabelClass1 l = new LabelClass1();
            l.labelName = PopUp_TextBox1.Text;
            l.posXMin = xmin;
            l.posYMin = ymin;
            l.posXMax = xmax;
            l.posYMax = ymax;
            l.width = w;
            l.height = h;

            if (LabelsList1 == null)
                LabelsList1 = new ObservableCollection<LabelClass1>();
            LabelsList1.Add(l);
            //MessageBox.Show("label" + l.labelName +" is added and No. of labels is: " + LabelsList1.Count().ToString());

            if (RectanglesList == null)
                RectanglesList = new ObservableCollection<Rectangle>();
            RectanglesList.Add(rectSelectArea);
            // MessageBox.Show("No. of Rectangle is added: " + RectanglesList.Count().ToString());

            LabelListView1.ItemsSource = LabelsList1;

            Popup1.IsOpen = false;
            PopUp_TextBox1.Text = null;

            IsRectangleSelected = true;

            //if (e.KeyCode == Keys.Enter)
            //{
            //    PopUp_Ok_Button1.PerformClick();
            //}
        }

        private void PopUp_Cancel_Button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Canvas1.Children.Remove(rectSelectArea);
                Popup1.IsOpen = false;
                IsRectangleSelected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void PopUp_Reset_Button1_Click(object sender, RoutedEventArgs e)
        {
            PopUp_TextBox1.Text = null;
            IsRectangleSelected = true;
        }
        public int imageIndex = 0, pIndx, cIndx, nIndx;
        private void Submit_button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("No. of labels is: " + LabelsList1.Count().ToString());
            //LabelListView1.ItemsSource = LabelsList1;
            //ImageListObjectDictionary.Add(ImageList[imageIndex], LabelsList1);
            //RectangleListObjectDictionary.Add(ImageList[imageIndex], RectanglesList);
        }

        bool isPreviousFileSave = false;
        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (LabelsList1 != null)
            {
                if (isPreviousFileSave)
                {
                    IsRectangleSelected = false;
                    if (LabelsList1 != null)
                        LabelsList1 = null;

                    LabelListView1.ItemsSource = null;

                    if (RectanglesList != null)
                    {
                        RectanglesList = null;
                        //Canvas1.Children.Clear();
                    }

                    Canvas1.Children.Clear();
                    imageIndex++;

                    //MessageBox.Show("Next image is " + ImageList[imageIndex]);


                    Image1.Source = new BitmapImage(new Uri(ImageList[imageIndex]));
                    Canvas1.Children.Add(Image1);
                    string ImageName = Path.GetFileNameWithoutExtension(ImageList[imageIndex]);
                    imageName1.Text = ImageName;

                    cIndx = imageIndex;
                    pIndx = imageIndex - 1;

                    isPreviousFileSave = false;
                }
                else
                {
                    FileSavePopup.IsOpen = true;
                    //SaveFile.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    //SaveFile.Click(SaveFile_Click) ;
                }
            }

            else
            {
                MessageBox.Show("There is No labels regarding to this image to save");
                //FileSavePopup.IsOpen = true;
            }

        }

        private void YesSaveFile_Click(object sender, RoutedEventArgs e)
        {
            //isPreviousFileSave = true;
            FileSavePopup.IsOpen = false;
            SaveFile.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            //MessageBox.Show("File is saved.");
            NextImage.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void NoSaveFile_Click(object sender, RoutedEventArgs e)
        {
            FileSavePopup.IsOpen = false;
            isPreviousFileSave = true;
            NextImage.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            isPreviousFileSave = true;
            //LabelListView1.ItemsSource = LabelsList1;
            string Image = ImageList[imageIndex];
            double width, height;
            using (var stream = new FileStream(Image, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bitmapFrame = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                width = bitmapFrame.PixelWidth;
                height = bitmapFrame.PixelHeight;
            }
            MessageBox.Show("Image width :"+width + "Image height :"+height);
            if (!ImageListObjectDictionary.ContainsKey(Image))
            {

                ImageListObjectDictionary.Add(ImageList[imageIndex], LabelsList1);

                RectangleListObjectDictionary.Add(ImageList[imageIndex], RectanglesList);

                string pathOfImage = Path.GetFullPath(Image);
                string imgName1 = Path.GetFileNameWithoutExtension(Image);
                string folder = "text_files";
                //string filepath = Path.GetFileNameWithoutExtension(Image) ;
                string folderName = Path.GetDirectoryName(Image);
                MessageBox.Show(folderName + "/" + imgName1 + ".xml");

                //!!!---without checking that file exist or don't exist---!!! 

                using (XmlWriter writer = XmlWriter.Create(folderName + "/" + imgName1 + ".xml"))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(folder);

                    // writer.WriteStartElement(foldername);
                    writer.WriteElementString("Folder", folderName);
                    writer.WriteElementString("Image", imgName1);
                    writer.WriteElementString("Path", pathOfImage);


                    writer.WriteStartElement("Source");
                    writer.WriteElementString("Database", "UNKNOWN");
                    writer.WriteEndElement();
                    writer.WriteStartElement("Image_Size");
                    writer.WriteElementString("Width", width.ToString());
                    writer.WriteElementString("Height", height.ToString());
                    writer.WriteElementString("Depth", "3");
                    writer.WriteEndElement();
                    int count = 0;
                    foreach (LabelClass1 label in LabelsList1)
                    {
                        count++;
                        writer.WriteStartElement("Object_"+ count);
                        writer.WriteElementString("Label", "" + label.labelName);
                        writer.WriteElementString("Segment", "0");

                        writer.WriteElementString("Xmin", "" + label.posXMin);
                        writer.WriteElementString("Ymin", "" + label.posYMin);
                        writer.WriteElementString("Xmax", "" + label.posXMax);
                        writer.WriteElementString("Ymax", "" + label.posYMax);                       

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                //for (int i = 0; i < LabelsList1.Count(); i++)
                //{
                //    LabelClass1 lx = LabelsList1[i];

                //    if (!File.Exists("C:/Users/A/Desktop/text_files/label10.xml"))
                //    {
                //        XmlWriter xmlWriter = XmlWriter.Create("C:/Users/A/Desktop/text_files/label10.xml");

                //        xmlWriter.WriteStartDocument();
                //        xmlWriter.WriteStartElement("LabelInfo");
                //        xmlWriter.WriteStartElement("Label");

                //        xmlWriter.WriteStartElement("Label");
                //        xmlWriter.WriteString(lx.labelName.ToString());
                //        xmlWriter.WriteEndElement();

                //        xmlWriter.WriteStartElement("PositionX");
                //        xmlWriter.WriteString(lx.posX.ToString());
                //        xmlWriter.WriteEndElement();

                //        xmlWriter.WriteStartElement("PositionY");
                //        xmlWriter.WriteString(lx.posY.ToString());
                //        xmlWriter.WriteEndElement();

                //        xmlWriter.WriteStartElement("Width");
                //        xmlWriter.WriteString(lx.width.ToString());
                //        xmlWriter.WriteEndElement();

                //        xmlWriter.WriteStartElement("Height");
                //        xmlWriter.WriteString(lx.height.ToString());
                //        xmlWriter.WriteEndElement();

                //        xmlWriter.WriteEndElement();
                //        xmlWriter.WriteEndElement();
                //        xmlWriter.WriteEndDocument();
                //        xmlWriter.Flush();
                //        xmlWriter.Close();
                //        //i++;
                //    }
                //    else
                //    {
                //        XDocument xDocument = XDocument.Load("C:/Users/A/Desktop/text_files/label10.xml");
                //        XElement root = xDocument.Element("LabelInfo");
                //        IEnumerable<XElement> rows = root.Descendants("Label");
                //        XElement firstRow = rows.First();
                //        firstRow.AddBeforeSelf(
                //           new XElement("Label",
                //           new XElement("LabelName", LabelsList1[i].labelName.ToString()),
                //           new XElement("PositionX", LabelsList1[i].posX.ToString()),
                //           new XElement("PositionY", LabelsList1[i].posY.ToString()),
                //           new XElement("Width", LabelsList1[i].width.ToString()),
                //           new XElement("Height", LabelsList1[i].height.ToString())
                //           ));
                //        xDocument.Save("C:/Users/A/Desktop/text_files/label10.xml");

                //        //i++;
                //    }
                //}
                MessageBox.Show("File is saved.");
                //LabelsList1.Clear();
                //LabelsList1 = null;
                //LabelListView1.ItemsSource = null;
                //RectanglesList = null;
            }
            else
            {
                ImageListObjectDictionary.Remove(ImageList[imageIndex]);
                RectangleListObjectDictionary.Remove(ImageList[imageIndex]);
                SaveFile.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }


        }

        private void Popup1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PopUp_Ok_Button1.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                // PopUp_Ok_Button1.PerformClick();
            }
        }

        public void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {


            if (pIndx >= 0)
            {
                Canvas1.Children.Clear();
                LabelListView1.ItemsSource = null;
                string img = ImageList[pIndx];
                //MessageBox.Show("Prev image is : " + img);
                Canvas1.Children.Add(Image1);
                Image1.Source = new BitmapImage(new Uri(img));
                string ImageName = Path.GetFileNameWithoutExtension(img);
                imageName1.Text = ImageName;

                if (ImageListObjectDictionary.ContainsKey(img))
                {
                    ObservableCollection<LabelClass1> labelsList2;
                    labelsList2 = ImageListObjectDictionary[img];
                    if (labelsList2 != null && labelsList2.Count() > 0)
                    {
                        LabelListView1.ItemsSource = labelsList2;
                        ObservableCollection<Rectangle> rectanglesList2;
                        rectanglesList2 = RectangleListObjectDictionary[img];
                        //MessageBox.Show("No. of rectangles are : " + rectanglesList2.Count().ToString());
                        for (int i = 0; i < rectanglesList2.Count(); i++)
                        {
                            Canvas1.Children.Add(rectanglesList2[i]);
                            rectanglesList2[i].PreviewMouseLeftButtonDown += FinalProjectMainWindow_PreviewMouseLeftButtonDown;
                        }
                        //Rectangle rectangle = new Rectangle
                        //{
                        //    Stroke = Brushes.Red,
                        //    StrokeThickness = 2,
                        //    Fill = Brushes.Yellow

                        //};

                        //List<Rectangle> RectList = new List<Rectangle>();
                        //for (int i = 0; i < labelsList2.Count(); i++)
                        //{
                        //    Rectangle rectangle = new Rectangle
                        //    {
                        //        Stroke = Brushes.Aqua,
                        //        StrokeThickness = 2,
                        //        Fill = Brushes.SkyBlue
                        //    };
                        //    Canvas.SetLeft(rectangle, labelsList2[i].posXMin);
                        //    Canvas.SetTop(rectangle, labelsList2[i].posYMin);
                        //    rectangle.Width = labelsList2[i].width;
                        //    rectangle.Height = labelsList2[i].height;
                        //    Canvas1.Children.Add(rectangle);
                        //    RectList.Add(rectangle);
                        //    //    rectangle.MouseDown += Rectangle_MouseDown;
                        //    //    rectangle = null;
                        //    //}

                        //}
                    }
                    else
                    {
                        MessageBox.Show("This image has no label.");
                    }
                }
                else
                {
                    MessageBox.Show("There is no such type of image.");
                }

                pIndx--;
            }
            else
            {
                pIndx = 0;
                MessageBox.Show("No previous image!!!");
            }

        }

        public Rectangle rect1 = null;
        private void FinalProjectMainWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle _Rectangle = sender as Rectangle;
            if (_Rectangle != null)
            {
                _Rectangle.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                _Rectangle.StrokeThickness = 3;
                _Rectangle.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f69321"));
            }

            if (rect1 == null)
            {
                rect1 = _Rectangle;
            }
            else
            {
                rect1.Stroke = Brushes.Aqua;
                rect1.StrokeThickness = 2;

                rect1 = _Rectangle;
            }
            //throw new NotImplementedException();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle _Rectangle = sender as Rectangle;

            //double x= Canvas.GetLeft(_Rectangle);
            //double y = Canvas.GetTop(_Rectangle);
            //double w = _Rectangle.Width;
            //double h = _Rectangle.Height;
            if (_Rectangle != null)
            {
                _Rectangle.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                _Rectangle.StrokeThickness = 3;
                _Rectangle.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f69321"));
            }

            if (rect1 == null)
            {
                rect1 = _Rectangle;
            }
            else
            {
                rect1.Stroke = Brushes.Aqua;
                rect1.StrokeThickness = 2;

                rect1 = _Rectangle;
            }
            //throw new NotImplementedException();
        }


        //private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Rectangle _Rectangle = sender as Rectangle;

        //    //double x= Canvas.GetLeft(_Rectangle);
        //    //double y = Canvas.GetTop(_Rectangle);
        //    //double w = _Rectangle.Width;
        //    //double h = _Rectangle.Height;
        //    if (_Rectangle != null)
        //    {
        //        _Rectangle.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        //        _Rectangle.StrokeThickness = 3;
        //        _Rectangle.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f69321"));
        //    }

        //    if(rect1 == null)
        //    {
        //        rect1 = _Rectangle;
        //    }
        //    else
        //    {
        //        rect1.Stroke = Brushes.Aqua;
        //        rect1.StrokeThickness = 2;

        //        rect1 = _Rectangle;
        //    }
        //    //    //throw new NotImplementedException();
        //}

        //public void Remainig_Rectangle_Unchanged(List<Rectangle> RectList, ObservableCollection<LabelClass1> labelsList2)
        //{
        //    if (RectList != null)
        //    {
        //        for (int i = 0; i < RectList.Count(); i++)
        //        {
        //            Rectangle rectangle = new Rectangle
        //            {
        //                Stroke = Brushes.Red,
        //                StrokeThickness = 2,
        //                Fill = Brushes.Yellow
        //            };


        //            Canvas.SetLeft(rectangle, labelsList2[i].posXMin);
        //            Canvas.SetTop(rectangle, labelsList2[i].posYMin);
        //            rectangle.Width = labelsList2[i].width;
        //            rectangle.Height = labelsList2[i].height;
        //            Canvas1.Children.Add(rectangle);
        //        }
        //    }
        //}
        private void ImageNameBtn_Click(object sender, RoutedEventArgs e)
        {
            Canvas1.Children.Clear();
            string img = ImageList[imageIndex];
            //MessageBox.Show("Curent image is : " + img);
            Canvas1.Children.Add(Image1);
            Image1.Source = new BitmapImage(new Uri(img));
            string ImageName = Path.GetFileNameWithoutExtension(img);
            imageName1.Text = ImageName;

            if (ImageListObjectDictionary.ContainsKey(img))
            {
                ObservableCollection<LabelClass1> labelsList2;
                labelsList2 = ImageListObjectDictionary[img];
                LabelListView1.ItemsSource = null;
                LabelListView1.ItemsSource = labelsList2;

                ObservableCollection<Rectangle> rectanglesList2;
                rectanglesList2 = RectangleListObjectDictionary[img];
                MessageBox.Show("No. of rectangles are : " + rectanglesList2.Count().ToString());
                for (int i = 0; i < rectanglesList2.Count(); i++)
                {
                    Canvas1.Children.Add(rectanglesList2[i]);
                }
            }
            else
            {
                LabelListView1.ItemsSource = null;
                MessageBox.Show("This is the current image and it has not contain any label");

            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            //Canvas1.Children.Clear();

            if (pIndx < cIndx)
            {
                Canvas1.Children.Clear();
                pIndx++;

                string img = ImageList[pIndx];
                //MessageBox.Show("Next image is : " + img);
                Canvas1.Children.Add(Image1);
                Image1.Source = new BitmapImage(new Uri(img));
                string fileName = Path.GetFileNameWithoutExtension(img);
                imageName1.Text = fileName;

                if (ImageListObjectDictionary.ContainsKey(img))
                {
                    ObservableCollection<LabelClass1> labelsList2;
                    labelsList2 = ImageListObjectDictionary[img];
                    LabelListView1.ItemsSource = null;
                    LabelListView1.ItemsSource = labelsList2;

                    ObservableCollection<Rectangle> rectanglesList2;
                    rectanglesList2 = RectangleListObjectDictionary[img];
                    //MessageBox.Show("No. of rectangles are : " + rectanglesList2.Count().ToString());
                    for (int i = 0; i < rectanglesList2.Count(); i++)
                    {
                        Canvas1.Children.Add(rectanglesList2[i]);
                    }
                }
                else
                {
                    LabelListView1.ItemsSource = null;
                    MessageBox.Show("This image has not contained any label.");
                }
            }
            else
            {
                MessageBox.Show("This is last image.");
            }
        }


        private void OpenDirectory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {

        }

        //public void Save_Image_And_Labels_Into_File(ObservableCollection<LabelClass1> rectangles, string imagename1, string foldername, string path)
        //{
        //    //  MainWindow m1 = new MainWindow();

        //    // AnotationTool[] employees = new AnotationTool[0];
        //    // XamlWriter xw = XamlWriter. 


        //}
    }
}
