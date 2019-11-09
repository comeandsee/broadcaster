﻿using Accord.Video.FFMPEG;
using AForge.Video;
using Indes2;
using Microsoft.Win32;
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
        Image imgWeb1;
        Image imgWeb2;
        Image imgLive;
        Double volume = 0;

        int currentRowLocal = 0;
        int currentColumnLocal = 0;
        int numberOfColumnsLocal = 1;


        int currentRow = 0;
        int currentColumn = 0;
        int numberOfVideos = 0;

        private WebCamManager webCam;
        private WebCamManager webCam1;
        private WebCamManager webCam2;
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
            webCam1 = new WebCamManager();
            webCam2 = new WebCamManager();


            //local

            imgLocal = new Image();
            Grid.SetRow(imgLocal, currentRowLocal);
            Grid.SetColumn(imgLocal, currentColumnLocal);

            webCamGrid.Children.Add(imgLocal);
            locaCam.StartCamera(LiveCamStatus.webCamLocal1, imgLocal);
            imgLocal.MouseDown += new MouseButtonEventHandler((sender, e) =>
            {
                meLocalCam_Clicked(sender, e, LiveCamStatus.webCamLocal1);
            });

            NewPlaceInGrid(numberOfColumnsLocal, ref currentRowLocal, ref currentColumnLocal);



            //web
            imgWeb = new Image();
            imgWeb1 = new Image();
            imgWeb2 = new Image();

          //  AddNewWebCam("http://192.168.1.192:8089/video", imgWeb, webCam);

        }

        private void AddNewWebCam(String ipWebcam, Image img, WebCamManager camManager)
        {
            Grid.SetRow(img, currentRowLocal);
            Grid.SetColumn(img, currentColumnLocal);
            webCamGrid.Children.Add(img);

            camManager.StartCamera(LiveCamStatus.webCamLocal2, img, ipWebcam);
            img.MouseDown += new MouseButtonEventHandler((sender, e) =>
            {
                meWebCam_Clicked(sender, e, LiveCamStatus.webCamLocal2, ipWebcam);
            });

            NewPlaceInGrid(numberOfColumnsLocal, ref currentRowLocal, ref currentColumnLocal);
        }

        public void loadMovies()
        {
            for (int i = 0; i < videoPath.Length; i++)
            {
                movies.Add(new Uri(videoPath[i]));
            }
            numberOfVideos = videoPath.Length;
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
          
            for (int i = 0; i < movies.Count; i++)
            {
                MediaElement meVideo = characterizeVideo( currentRow, currentColumn, uris.ElementAt(i));
                grid.Children.Add(meVideo);

                NewPlaceInGridByColumn(numberOfColumns, ref currentRow, ref currentColumn);
            }
           
        }
        public void loadNewVideoToGallery( String path)
        {
            Uri uri = new Uri(path);
            int numberOfColumns = moviesGrid.ColumnDefinitions.Count;
            int numberOfRows = moviesGrid.RowDefinitions.Count;

           MediaElement meVideo = characterizeVideo(currentRow, currentColumn, uri);
            moviesGrid.Children.Add(meVideo);

            NewPlaceInGridByColumn(numberOfColumns, ref currentRow, ref currentColumn);
            
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

        private MediaElement characterizeVideo(int currentRow, int currentColumn, Uri uri)
        {
            MediaElement meVideo = new MediaElement();
            meVideo.HorizontalAlignment = HorizontalAlignment.Stretch;
            meVideo.VerticalAlignment = VerticalAlignment.Stretch;
            meVideo.Stretch = Stretch.Fill;
            meVideo.ScrubbingEnabled = true;
            meVideo.Source = uri;

            Grid.SetRow(meVideo, currentRow);
            Grid.SetColumn(meVideo, currentColumn);
            meVideo.LoadedBehavior = MediaState.Manual;
            meVideo.Volume = volume;
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


        private void meWebCam_Clicked(object sender, MouseButtonEventArgs e, LiveCamStatus status, String ipWebcam)
        {
            liveGrid.Children.Clear();
            imgLive = new Image();
            liveGrid.Children.Add(imgLive);
            webCam.StartCamera(status, imgLive, ipWebcam);
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
                    meVideo.Volume = volume;

                    meVideo.LoadedBehavior = MediaState.Play;
                    meVideo.Visibility = Visibility.Visible;
                
                liveGrid.Children.Add(meVideo);

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

              //  int me = playlist.GetIndex(name);
            }

        }

        private void AddNewIpCam_Click(object sender, RoutedEventArgs e)
        {
            string ipWebcam = webCameraIp.Text;
            AddNewWebCam(ipWebcam,imgWeb1,webCam1);
    }

        private void PlaylistPlay_Click(object sender, RoutedEventArgs e)
        {

            PlayVideoList(playlist, sender, e);
        }

        private void PlayVideoList(Playlist playlist, object sender, RoutedEventArgs e)
        {
            //LIVE
            liveGrid.Children.Clear();
            if (!playlist.CheckIfPlaylistLiveDone() )
            {

                MediaElement movie = playlist.GetVideoList()[playlist.NextIndexLive];
                MediaElement movieNew = new MediaElement();
                movieNew.Source = movie.Source;
                movieNew.LoadedBehavior = MediaState.Play;
                movieNew.MediaEnded += new RoutedEventHandler(PlaylistPlay_Click);
                movieNew.Volume = volume;

                liveGrid.Children.Add(movieNew);
                playlist.NextIndexLive += 1;
            }
            else if (playlist.CheckIfPlaylistNotNull())
            {
                playlist.NextIndexLive = 0;
                PlayVideoList(playlist, sender, e);
            }

        }

        private void TurnOnVolume_Click(object sender, RoutedEventArgs e)
        {
            volume = Convert.ToDouble(TurnOnVolumeValue.Value) / 100;
            ChangeVolume(volume);
        }

        private void ChangeVolume(double volume)
        {
            var media = this.liveGrid.Children.OfType<MediaElement>().FirstOrDefault();
            if (media != null) media.Volume = volume;
        }

        private void TurnOffVolume_Click(object sender, RoutedEventArgs e)
        {
            ChangeVolume(0);
        }

        private void AddMovie_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video (*.mp4)|*.mp4";
            if (openFileDialog.ShowDialog() == true)
            {
                string me = openFileDialog.FileName;
                loadNewVideoToGallery(me);

            }
        }
    }
}

