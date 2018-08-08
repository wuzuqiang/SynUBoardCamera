using Microsoft.Win32;
using System;
using System.Windows;
using WPFMediaKit.DirectShow.Controls;
using System.Linq;
using DirectShowLib;
using WPFMediaKit;
using System.Collections.Generic;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace Test_Application
{
    public partial class MainWindow : Window
    {
        private bool sliderDrag;
        private bool sliderMediaChange;

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;
            this.mediaUriElement.MediaFailed += MediaUriElement_MediaFailed;
            this.mediaUriElement.MediaUriPlayer.MediaPositionChanged += MediaUriPlayer_MediaPositionChanged;
            this.cameraCaptureElement.MediaFailed += CameraCaptureElement_MediaFailed;
            this.cameraCaptureElement.MediaPlayError += CameraCaptureElement_MediaPlayError;
            //this.cameraCaptureElement.Window
            //this.cameraCaptureElement.lOS

            if (MultimediaUtil.VideoInputDevices.Any())
            {
                string[] strArray = MultimediaUtil.VideoInputNames;
                string[] strArray1 = new string[strArray.Length + 1];
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray1[i] = strArray[i];
                }
                strArray1[strArray.Length] = "";
                cobVideoSource.ItemsSource = strArray1;
            }
            SetCameraCaptureElementVisible(false);
        }

        private void CameraCaptureElement_MediaPlayError()
        {
            MessageBox.Show("CameraCaptureElement_MediaPlayError");
        }

        private void CameraCaptureElement_MediaFailed(object sender, WPFMediaKit.DirectShow.MediaPlayers.MediaFailedEventArgs e)
        {
            //MessageBox.Show("CameraFailed");
        }

        private void SetCameraCaptureElementVisible(bool visible)
        {
            cameraCaptureElement.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            mediaUriElement.Visibility = !visible ? Visibility.Visible : Visibility.Collapsed;
            btnStop.IsEnabled = !visible;
            btnPause.IsEnabled = !visible;
            slider.IsEnabled = !visible;
            if (visible)
            {
                btnStop_Click(null, null);
            }
            else
            {
                cobVideoSource.SelectedIndex = -1;
            }
        }

        private void SetPlayButtons(bool playing)
        {
            if (playing)
            {
                btnPause.Content = "Pause";
            }
            else
            {
                btnPause.Content = "Play";
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result != true)
                return;
            errorText.Text = null;
            SetCameraCaptureElementVisible(false);
            mediaUriElement.Source = new Uri(dlg.FileName);
            SetPlayButtons(true);
        }

        private void MediaUriElement_MediaFailed(object sender, WPFMediaKit.DirectShow.MediaPlayers.MediaFailedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => errorText.Text = e.Message));
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mediaUriElement.Close();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaUriElement.Stop();
            SetPlayButtons(false);
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            bool playing = mediaUriElement.IsPlaying;
            if (playing)
                mediaUriElement.Pause();
            else
                mediaUriElement.Play();
            SetPlayButtons(!playing);
        }

        private void MediaUriPlayer_MediaPositionChanged(object sender, EventArgs e)
        {
            if (sliderDrag)
                return;
            this.Dispatcher.BeginInvoke(new Action(ChangeSlideValue), null);
        }

        private void ChangeSlideValue()
        {
            if (sliderDrag)
                return;

            sliderMediaChange = true;
            double perc = (double)mediaUriElement.MediaPosition / mediaUriElement.MediaDuration;
            slider.Value = slider.Maximum * perc;
            sliderMediaChange = false;
        }

        private void ChangeMediaPosition()
        {
            if (sliderMediaChange)
                return;

            sliderDrag = true;
            double perc = slider.Value / slider.Maximum;
            mediaUriElement.MediaPosition = (long)(mediaUriElement.MediaDuration * perc);
            sliderDrag = false;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderMediaChange)
                return;

            this.Dispatcher.BeginInvoke(new Action(ChangeMediaPosition), null);
        }

        private void cobVideoSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (cobVideoSource.SelectedIndex < 0)
                    return;
                if (string.IsNullOrEmpty(cobVideoSource.SelectedItem.ToString()))
                {
                    //cameraCaptureElement.Stop();
                    SetCameraCaptureElementVisible2(true);
                    //cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[0];
                    cameraCaptureElement.VideoCaptureSource = MultimediaUtil.VideoInputDevices[0].Name;
                    //cameraCaptureElement.RefreshVideoCapture();
                    return;
                }
                SetCameraCaptureElementVisible(true);
                //cameraCaptureElement.VideoCaptureSource = MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex].Name;
                cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex];
            }
            catch(Exception ex)
            {
                MessageBox.Show("Message:ERROR：" + ex.Message.ToString());
            }
        }

        private void SetCameraCaptureElementVisible2(bool visible)
        {
            cameraCaptureElement.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            mediaUriElement.Visibility = !visible ? Visibility.Visible : Visibility.Collapsed;
            btnStop.IsEnabled = !visible;
            btnPause.IsEnabled = !visible;
            slider.IsEnabled = !visible;
            if (visible)
            {
                btnStop_Click(null, null);
            }
            else
            {
                //cobVideoSource.SelectedIndex = -1;
            }
        }

        private void btnChangeParam_Click(object sender, RoutedEventArgs e)
        {
            //cameraCaptureElement.SetBrightness(int.Parse(txtBrightness.Text));
            //cameraCaptureElement.SetContrast(int.Parse(txtContrast.Text));
            cameraCaptureElement.SetVideoProcAmpProperty(VideoProcAmpProperty.Brightness, int.Parse(txtBrightness.Text));
            Get();
            //SetCameraCaptureElementVisible2(true);
            //DeviceParaInfo _ParaInfo = new DeviceParaInfo(VideoProcAmpProperty.Brightness, txtBrightness.Text
            //    , MultimediaUtil.VideoInputDevices[0].Name);
            //List<DeviceParaInfo> listParamInfo = new List<DeviceParaInfo>();
            //listParamInfo.Add(_ParaInfo);
            //(new MyVideoCapturePropertySet()).SetVideoCaptureParametersOthers(MultimediaUtil.VideoInputDevices[0].Name, listParamInfo);
            //cameraCaptureElement.VideoCaptureParaChanged = listParamInfo;   
            //上行变动了之后会触发OnVideoCaptureParaIsChangedChanged，不会进入下行，导致VideoCaptrueSource为空字符
            //cameraCaptureElement.VideoCaptureSource = MultimediaUtil.VideoInputDevices[0].Name;
            //CameraControlProperty property = CameraControlProperty.Pan;
            //Action<int> callBackAction = CallBackAction<int>;
            //int iRet = cameraCaptureElement.GetCameraControlParameterAsnyc( property, callBackAction);
            //VideoProcAmpProperty propertyVideo = VideoProcAmpProperty.Brightness;
            //Action<int> callBackActionVideo = CallBackAction<int>;
            //int iRetVideo = cameraCaptureElement.GetVideoProcAmpParameterAsnyc(propertyVideo, callBackActionVideo);
        }
        private void CallBackAction<T>(T b)
        {
        }
        char a = '\u000B';
        int a1 = 'a' + 'c';

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cameraCaptureElement.SetVideoProcAmpPropertyAuto(VideoProcAmpProperty.Brightness, int.Parse(txtBrightness.Text));
            Get();
        }
        void Get()
        {
            List<Object> listObj = cameraCaptureElement.GetVideoProcAmpPropertyAuto(VideoProcAmpProperty.Brightness);
        }
    }
}