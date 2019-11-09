using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebEye.Controls.Wpf;
using AForge.Video;
using AForge.Video.DirectShow;

using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Accord.Video.FFMPEG;
using Indes2;
using static BroadcasterIndes.MainWindow;
using System.Threading;

namespace Indes2
{
    class WebCamManager : IDisposable
    {
       // private string ipWebcam = "http://192.168.1.192:8089//video";

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }
        private FilterInfo _currentDevice;


      //  public M ImageLocal { get; private set; }
     //   public bool IsWebCamLive { get;  set; }
      ///  public System.Windows.Controls.Image ImageLive { get; private set; }
      //  public bool IsWebCamLocal1Play { get;  set; }
      //  public bool IsWebCamLocal2Play { get;  set; }

        private IVideoSource _videoSource;
        private IVideoSource _videoSourceLive;
        private VideoFileWriter _writer;
        private VideoFileWriter _writerLive;


        public WebCamManager()
        {
            GetVideoDevices();
        }

            private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in devices)
            {
                VideoDevices.Add(device);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No webcam found");
            }
        }


        public void StopCamera(System.Windows.Controls.Image image)
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler((sender, args) => {
                    video_NewFrame(sender, args, image);
                });
            }
            image.Dispatcher.Invoke(() => image.Source = null);
        }
    

        public void Dispose()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
               
            }
            _writer?.Dispose();
        }

     //   private void video_NewFrame(object sender, NewFrameEventArgs eventArgs) => video_NewFrame(sender, eventArgs, null);
        
            private void video_NewFrame(object sender, NewFrameEventArgs eventArgs, System.Windows.Controls.Image img)
        {
            try
            {
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    var bi = bitmap.ToBitmapImage();
                    bi.Freeze();
                    img.Dispatcher.Invoke(() => img.Source = bi);

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StopCamera(img);
            }
        }
        private FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; }
        }

    

        public void StartCamera(LiveCamStatus cameraId, System.Windows.Controls.Image image, String ipWebcam = null)
        {

            switch (cameraId)
            {
                case LiveCamStatus.webCamLocal2:
                    _videoSource = new MJPEGStream(ipWebcam);
                    _videoSource.NewFrame += new NewFrameEventHandler((sender, args) => {
                        video_NewFrame(sender, args, image);
                    });
                    _videoSource.Start();
                    break;
             
                case LiveCamStatus.webCamLocal1:
                    if (CurrentDevice != null)
                    {
                        StopCamera(image);
                        Thread.Sleep(1000);
                        _videoSource = new AForge.Video.DirectShow.VideoCaptureDevice(CurrentDevice.MonikerString);
                        _videoSource.NewFrame += new NewFrameEventHandler((sender, args) => {
                            video_NewFrame(sender, args, image);
                        });
                        _videoSource.Start();
                    }
                    else
                    {
                        MessageBox.Show("Current device can't be null");
                    }
                    break;


                default:
                    break;
            }
        }

    }
}
