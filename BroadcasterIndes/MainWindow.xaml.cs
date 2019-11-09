using Accord.Video.FFMPEG;
using AForge.Video;
using Indes2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BroadcasterIndes
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] videoPath = Directory.GetFiles(@"C:\Users\Martusia\source\repos\BroadcasterIndes\Resources");
        List<Uri> movies = new List<Uri>();
        
       
        Grid liveGrid;
        Grid webCamGrid;
        Border myborder;
        Image imgLocal;
        Image imgWeb;
        Image imgLive;
        private WebCamManager webCam;
        private WebCamManager locaCam;
        private Playlist playlist = new Playlist();

        public enum LiveCamStatus
        {
            webCamLocal1 = 1,
            webCamLocal2 = 2,
            live1 = 3,
            live2 = 4,
            playlist = 5
        }
        public MainWindow()
        {
            InitializeComponent();
            loadMovies();
            loadGrid();
            loadVideoGallery(moviesGrid, movies);

            InitializeWebCams();

          

         

        }

        private void InitializeWebCams()
        {
            webCam = new WebCamManager();
            locaCam = new WebCamManager();


            int currentRow = 0;
            int currentColumn = 0;
            int numberOfColumns = 1;
            //local

            imgLocal = new Image();
            Grid.SetRow(imgLocal, currentRow);
            Grid.SetColumn(imgLocal, currentColumn);

            webCamGrid.Children.Add(imgLocal);
            locaCam.StartCamera(LiveCamStatus.webCamLocal1, imgLocal);
            imgLocal.MouseDown += new MouseButtonEventHandler((sender, e) => {
                meLocalCam_Clicked(sender, e, LiveCamStatus.webCamLocal1);
            });

            NewPlaceInGrid(numberOfColumns,ref currentRow,ref currentColumn);



            //web
            imgWeb = new Image();
         //   imgWeb.Source = new BitmapImage(new Uri (@"C:\Users\Martusia\source\repos\Indes2\Indes2\back.jpg"));
            Grid.SetRow(imgWeb, currentRow);
            Grid.SetColumn(imgWeb, currentColumn);
            webCamGrid.Children.Add(imgWeb);
            
            webCam.StartCamera(LiveCamStatus.webCamLocal2, imgWeb);
            imgWeb.MouseDown += new MouseButtonEventHandler((sender, e) => {
                meWebCam_Clicked(sender, e, LiveCamStatus.webCamLocal2);
            });

            NewPlaceInGrid(numberOfColumns, ref currentRow, ref currentColumn);



        }
        public void loadMovies()
        {
            for (int i = 0; i < videoPath.Length; i++)
            {
                movies.Add(new Uri(videoPath[i]));
            }
        }

        public void loadGrid()
        {
        //live
            liveGrid = liveGridXaml;


            //movies 
            moviesGrid.ShowGridLines = true;
            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            ColumnDefinition colDef3 = new ColumnDefinition();
            moviesGrid.ColumnDefinitions.Add(colDef1);
            moviesGrid.ColumnDefinitions.Add(colDef2);
            moviesGrid.ColumnDefinitions.Add(colDef3);

            RowDefinition rowDef1 = new RowDefinition();
            RowDefinition rowDef2 = new RowDefinition();
            RowDefinition rowDef3 = new RowDefinition();
            RowDefinition rowDef4 = new RowDefinition();
            RowDefinition rowDef5 = new RowDefinition();
            RowDefinition rowDef6 = new RowDefinition();
            RowDefinition rowDef7 = new RowDefinition();
            moviesGrid.RowDefinitions.Add(rowDef1);
            moviesGrid.RowDefinitions.Add(rowDef2);
            moviesGrid.RowDefinitions.Add(rowDef3);
            moviesGrid.RowDefinitions.Add(rowDef4);
            moviesGrid.RowDefinitions.Add(rowDef5);
            moviesGrid.RowDefinitions.Add(rowDef6);
            moviesGrid.RowDefinitions.Add(rowDef7);


            // web grid
            webCamGrid = webCamGridXaml;
            webCamGrid.ShowGridLines = true;

            ColumnDefinition colDefWC1 = new ColumnDefinition();
            ColumnDefinition colDefWC2 = new ColumnDefinition();
            webCamGrid.ColumnDefinitions.Add(colDefWC1);
            webCamGrid.ColumnDefinitions.Add(colDefWC2);

            RowDefinition rowDefWC1 = new RowDefinition();
            RowDefinition rowDefWC2 = new RowDefinition();
            webCamGrid.RowDefinitions.Add(rowDefWC1);
            webCamGrid.RowDefinitions.Add(rowDefWC2);


 
    }
      
        public void loadVideoGallery(Grid grid, List<Uri> uris)
        {
            int numberOfColumns = grid.ColumnDefinitions.Count;
            int numberOfRows = grid.RowDefinitions.Count;
            int currentRow = 0;
            int currentColumn = 0;
            for (int i = 0; i < movies.Count; i++)
            {
                MediaElement meVideo = characterizeVideo(uris, currentRow, currentColumn, i);
                grid.Children.Add(meVideo);

                NewPlaceInGridByColumn(numberOfColumns, ref currentRow, ref currentColumn);
            }
        }
        private static void NewPlaceInGridByColumn(int numberOfColumns, ref int currentRow, ref int currentColumn)
        {
            currentColumn++;
            if (currentColumn == numberOfColumns)
            {
                currentColumn = 0;
                currentRow++;
            }
        }


        private static void NewPlaceInGrid(int numberOfRows, ref int currentRow, ref int currentColumn)
        {
            currentRow++;
            if (currentRow == numberOfRows)
            {
                currentRow = 0;
                currentColumn++;
            }
        }

        private MediaElement characterizeVideo(List<Uri> uris, int currentRow, int currentColumn, int i)
        {
            MediaElement meVideo = new MediaElement();
            meVideo.HorizontalAlignment = HorizontalAlignment.Stretch;
            meVideo.VerticalAlignment = VerticalAlignment.Stretch;
            meVideo.Stretch = Stretch.Fill;
            meVideo.ScrubbingEnabled = true;
            meVideo.Source = uris.ElementAt(i);

            Grid.SetRow(meVideo, currentRow);
            Grid.SetColumn(meVideo, currentColumn);
            meVideo.LoadedBehavior = MediaState.Manual;
            meVideo.Volume = 0;
            meVideo.Loaded += (sender, args) =>
            {
                MediaElement me = sender as MediaElement;
                me.Play();
                // me.Pause();
            };
            meVideo.MouseLeftButtonDown += new MouseButtonEventHandler(meVideo_Clicked);
            meVideo.MouseRightButtonDown += new MouseButtonEventHandler(AddToPlaylist);
            return meVideo;
        }


        private void meWebCam_Clicked(object sender, MouseButtonEventArgs e, LiveCamStatus status)
        {
            liveGrid.Children.Clear();
            imgLive = new Image();
            liveGrid.Children.Add(imgLive);
            webCam.StartCamera(status, imgLive);
        }
        private void meLocalCam_Clicked(object sender, MouseButtonEventArgs e, LiveCamStatus status)
        {
            liveGrid.Children.Clear();
            imgLive = new Image();
            liveGrid.Children.Add(imgLive);
            locaCam.StartCamera(status, imgLive);
        }
        private void meVideo_Clicked(object sender, MouseButtonEventArgs e)
            {
                liveGrid.Children.Clear();
                MediaElement senderVideo = sender as MediaElement;
                MediaElement meVideo = new MediaElement();
                meVideo.Source = senderVideo.Source;
                if (meVideo != null)
                {
                    meVideo.Stretch = Stretch.Fill;
                    meVideo.Height = liveCanvas.ActualHeight;
                    meVideo.Width = liveCanvas.ActualWidth;
                    meVideo.Volume = 0;

                    meVideo.LoadedBehavior = MediaState.Play;
                    meVideo.Visibility = Visibility.Visible;

                    myborder = new Border();
                    myborder.BorderBrush = Brushes.Red;
                    myborder.BorderThickness = new Thickness(1, 1, 1, 1);
                    myborder.Child = meVideo;
                liveGrid.Children.Add(myborder);

                }

            }

        private void AddToPlaylist(object sender, RoutedEventArgs e)
        {
            MediaElement senderVideo = sender as MediaElement;
            if (senderVideo != null)
            {
                playlist.AddVideo(senderVideo);
                var nameIndex = senderVideo.Source.Segments.Count() -1 ;
                String name = senderVideo.Source.Segments.GetValue(nameIndex).ToString();
                playlistBox.Items.Add(name);

                int me = playlist.GetIndex(name);
            }

        }
       
    }
}

