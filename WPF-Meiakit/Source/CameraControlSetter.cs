using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;
using WPFMediaKit.Model;

namespace WPFMediaKit
{
    public class CameraControlSetter
    {
        public CameraControlSetter(DsDevice device)
        {   //这里初始化iAMCameraControl
            iAMCameraControl = GetIAMCameraControl(device);
        }
        private IAMCameraControl iAMCameraControl;
        #region GetIAMCameraControl(string deviceName)
#if DEBUG
        private DsROTEntry m_rotEntry;
#endif
        /// <summary>
        /// The capture device filter
        /// </summary>
        private IBaseFilter m_captureDevice;
        private IAMCameraControl GetIAMCameraControl(DsDevice device)
        {
            IGraphBuilder m_graph;
            /* Create a new graph */
            m_graph = (IGraphBuilder)new FilterGraphNoThread();

#if DEBUG
            m_rotEntry = new DsROTEntry(m_graph);
#endif

            /* Create a capture graph builder to help 
             * with rendering a capture graph */
            var graphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

            /* Set our filter graph to the capture graph */
            int hr = graphBuilder.SetFiltergraph(m_graph);
            DsError.ThrowExceptionForHR(hr);
            m_captureDevice = AddFilterByDevice(m_graph,
                                              device);
            object ampControl;
            int hr11 = graphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, m_captureDevice
                , typeof(IAMCameraControl).GUID, out ampControl);
            DsError.ThrowExceptionForHR(hr11);
            iAMCameraControl = ampControl as IAMCameraControl;
            return iAMCameraControl;
        }

        private IBaseFilter AddFilterByName(IGraphBuilder graphBuilder, Guid deviceCategory, string friendlyName)
        {
            var devices = DsDevice.GetDevicesOfCat(deviceCategory);

            var deviceList = (from d in devices
                              where d.Name == friendlyName
                              select d).ToList();
            DsDevice device = deviceList.FirstOrDefault();

            foreach (var item in deviceList)
            {
                if (item != device)
                    item.Dispose();
            }

            return AddFilterByDevice(graphBuilder, device);
        }
        private IBaseFilter AddFilterByDevice(IGraphBuilder graphBuilder, DsDevice device)
        {
            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");
            if (device == null)
                return null;

            var filterGraph = graphBuilder as IFilterGraph2;

            if (filterGraph == null)
                return null;

            IBaseFilter filter = null;
            int hr = filterGraph.AddSourceFilterForMoniker(device.Mon, null, device.Name, out filter);
            DsError.ThrowExceptionForHR(hr);
            return filter;
        }
        #endregion

        public int GetParameterValue(CameraControlProperty property)
        {
            int iRet;
            CameraControlFlags cameraControlFlags;
            iAMCameraControl.Get(property, out iRet, out cameraControlFlags);
            return iRet;
        }
        public CameraControlRangeParameter GetRangeParameterValue(CameraControlProperty property)
        {
            CameraControlRangeParameter _model = new CameraControlRangeParameter();
            int Min, Max, Step, Default;
            CameraControlFlags _flgs;
            int hr2 = iAMCameraControl.GetRange(property,
                out Min, out Max, out Step, out Default, out _flgs);
            DsError.ThrowExceptionForHR(hr2);
            _model.MinValue = Min;
            _model.MaxValue = Max;
            _model.SetpValue = Step;
            _model.DefaultValue = Default;
            _model.CameraControlFlags = _flgs;
            return _model;
        }
        public void SetAutoCameraControlParameter(CameraControlProperty property, int iVal)
        {
            iAMCameraControl.Set(property, iVal, CameraControlFlags.Auto);
        }
        /// <summary>
        /// 以CameraControlFlags.Manual设置iAMCameraControl的property属性值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="iVal"></param>
        public void SetCameraControlParameter(CameraControlProperty property, int iVal)
        {
            iAMCameraControl.Set(property, iVal, CameraControlFlags.Manual);
        }
    }
}
