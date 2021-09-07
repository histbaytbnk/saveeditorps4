
// Type: PS3SaveEditor.USB


// Hacked by SystemAce

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PS3SaveEditor
{
  public class USB
  {
    private const int IOCTL_STORAGE_GET_DEVICE_NUMBER = 2953344;
    private const string GUID_DEVINTERFACE_DISK = "53f56307-b6bf-11d0-94f2-00a0c91efb8b";
    private const int GENERIC_WRITE = 1073741824;
    private const int FILE_SHARE_READ = 1;
    private const int FILE_SHARE_WRITE = 2;
    private const int OPEN_EXISTING = 3;
    private const int INVALID_HANDLE_VALUE = -1;
    private const int IOCTL_GET_HCD_DRIVERKEY_NAME = 2229284;
    private const int IOCTL_USB_GET_ROOT_HUB_NAME = 2229256;
    private const int IOCTL_USB_GET_NODE_INFORMATION = 2229256;
    private const int IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX = 2229320;
    private const int IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION = 2229264;
    private const int IOCTL_USB_GET_NODE_CONNECTION_NAME = 2229268;
    private const int IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME = 2229280;
    private const int USB_DEVICE_DESCRIPTOR_TYPE = 1;
    private const int USB_STRING_DESCRIPTOR_TYPE = 3;
    private const int BUFFER_SIZE = 2048;
    private const int MAXIMUM_USB_STRING_LENGTH = 255;
    private const string GUID_DEVINTERFACE_HUBCONTROLLER = "3abf6f2d-71c4-462a-8a92-1e6861e6af27";
    private const string REGSTR_KEY_USB = "USB";
    private const int DIGCF_PRESENT = 2;
    private const int DIGCF_ALLCLASSES = 4;
    private const int DIGCF_DEVICEINTERFACE = 16;
    private const int SPDRP_DRIVER = 9;
    private const int SPDRP_DEVICEDESC = 0;
    private const int REG_SZ = 1;

    public static List<USB.USBDevice> GetConnectedDevices()
    {
      List<USB.USBDevice> DevList = new List<USB.USBDevice>();
      foreach (USB.USBController usbController in USB.GetHostControllers())
        USB.ListHub(usbController.GetRootHub(), DevList);
      return DevList;
    }

    private static void ListHub(USB.USBHub Hub, List<USB.USBDevice> DevList)
    {
      foreach (USB.USBPort usbPort in Hub.GetPorts())
      {
        if (usbPort.IsHub)
          USB.ListHub(usbPort.GetHub(), DevList);
        else if (usbPort.IsDeviceConnected)
          DevList.Add(usbPort.GetDevice());
      }
    }

    public static USB.USBDevice FindDeviceByDriverKeyName(string DriverKeyName)
    {
      USB.USBDevice FoundDevice = (USB.USBDevice) null;
      foreach (USB.USBController usbController in USB.GetHostControllers())
      {
        USB.SearchHubDriverKeyName(usbController.GetRootHub(), ref FoundDevice, DriverKeyName);
        if (FoundDevice != null)
          break;
      }
      return FoundDevice;
    }

    private static void SearchHubDriverKeyName(USB.USBHub Hub, ref USB.USBDevice FoundDevice, string DriverKeyName)
    {
      foreach (USB.USBPort usbPort in Hub.GetPorts())
      {
        if (usbPort.IsHub)
          USB.SearchHubDriverKeyName(usbPort.GetHub(), ref FoundDevice, DriverKeyName);
        else if (usbPort.IsDeviceConnected)
        {
          USB.USBDevice device = usbPort.GetDevice();
          if (device.DeviceDriverKey == DriverKeyName)
          {
            FoundDevice = device;
            break;
          }
        }
      }
    }

    public static USB.USBDevice FindDeviceByInstanceID(string InstanceID)
    {
      USB.USBDevice FoundDevice = (USB.USBDevice) null;
      foreach (USB.USBController usbController in USB.GetHostControllers())
      {
        USB.SearchHubInstanceID(usbController.GetRootHub(), ref FoundDevice, InstanceID);
        if (FoundDevice != null)
          break;
      }
      return FoundDevice;
    }

    private static void SearchHubInstanceID(USB.USBHub Hub, ref USB.USBDevice FoundDevice, string InstanceID)
    {
      foreach (USB.USBPort usbPort in Hub.GetPorts())
      {
        if (usbPort.IsHub)
          USB.SearchHubInstanceID(usbPort.GetHub(), ref FoundDevice, InstanceID);
        else if (usbPort.IsDeviceConnected)
        {
          USB.USBDevice device = usbPort.GetDevice();
          if (device.InstanceID == InstanceID)
          {
            FoundDevice = device;
            break;
          }
        }
      }
    }

    [DllImport("setupapi.dll")]
    private static extern int CM_Get_Parent(out IntPtr pdnDevInst, int dnDevInst, int ulFlags);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
    private static extern int CM_Get_Device_ID(IntPtr dnDevInst, IntPtr Buffer, int BufferLen, int ulFlags);

    public static USB.USBDevice FindDriveLetter(string DriveLetter)
    {
      USB.USBDevice usbDevice = (USB.USBDevice) null;
      string InstanceID = "";
      int deviceNumber = USB.GetDeviceNumber("\\\\.\\" + DriveLetter.TrimEnd('\\'));
      if (deviceNumber < 0)
        return usbDevice;
      Guid guid = new Guid("53f56307-b6bf-11d0-94f2-00a0c91efb8b");
      IntPtr classDevs = USB.SetupDiGetClassDevs(ref guid, 0, IntPtr.Zero, 18);
      if (classDevs.ToInt32() != -1)
      {
        int MemberIndex = 0;
        bool flag;
        do
        {
          USB.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new USB.SP_DEVICE_INTERFACE_DATA();
          DeviceInterfaceData.cbSize = Marshal.SizeOf((object) DeviceInterfaceData);
          flag = USB.SetupDiEnumDeviceInterfaces(classDevs, IntPtr.Zero, ref guid, MemberIndex, ref DeviceInterfaceData);
          if (flag)
          {
            USB.SP_DEVINFO_DATA DeviceInfoData = new USB.SP_DEVINFO_DATA();
            DeviceInfoData.cbSize = Marshal.SizeOf((object) DeviceInfoData);
            USB.SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData = new USB.SP_DEVICE_INTERFACE_DETAIL_DATA();
            DeviceInterfaceDetailData.cbSize = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8;
            int RequiredSize = 0;
            int num1 = 2048;
            if (USB.SetupDiGetDeviceInterfaceDetail(classDevs, ref DeviceInterfaceData, ref DeviceInterfaceDetailData, num1, ref RequiredSize, ref DeviceInfoData) && USB.GetDeviceNumber(DeviceInterfaceDetailData.DevicePath) == deviceNumber)
            {
              IntPtr pdnDevInst;
              USB.CM_Get_Parent(out pdnDevInst, DeviceInfoData.DevInst, 0);
              IntPtr num2 = Marshal.AllocHGlobal(num1);
              USB.CM_Get_Device_ID(pdnDevInst, num2, num1, 0);
              InstanceID = Marshal.PtrToStringAuto(num2);
              Marshal.FreeHGlobal(num2);
              break;
            }
          }
          ++MemberIndex;
        }
        while (flag);
        USB.SetupDiDestroyDeviceInfoList(classDevs);
      }
      if (InstanceID.StartsWith("USB\\"))
        usbDevice = USB.FindDeviceByInstanceID(InstanceID);
      return usbDevice;
    }

    private static int GetDeviceNumber(string DevicePath)
    {
      int num1 = -1;
      IntPtr file = USB.CreateFile(DevicePath.TrimEnd('\\'), 0, 0, IntPtr.Zero, 3, 0, IntPtr.Zero);
      if (file.ToInt32() != -1)
      {
        int num2 = Marshal.SizeOf((object) new USB.STORAGE_DEVICE_NUMBER());
        IntPtr num3 = Marshal.AllocHGlobal(num2);
        int lpBytesReturned;
        if (USB.DeviceIoControl(file, 2953344, IntPtr.Zero, 0, num3, num2, out lpBytesReturned, IntPtr.Zero))
        {
          USB.STORAGE_DEVICE_NUMBER storageDeviceNumber = (USB.STORAGE_DEVICE_NUMBER) Marshal.PtrToStructure(num3, typeof (USB.STORAGE_DEVICE_NUMBER));
          num1 = (storageDeviceNumber.DeviceType << 8) + storageDeviceNumber.DeviceNumber;
        }
        Marshal.FreeHGlobal(num3);
        USB.CloseHandle(file);
      }
      return num1;
    }

    [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, int Enumerator, IntPtr hwndParent, int Flags);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SetupDiGetClassDevs(int ClassGuid, string Enumerator, IntPtr hwndParent, int Flags);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref USB.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref USB.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref USB.SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, ref USB.SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref USB.SP_DEVINFO_DATA DeviceInfoData, int iProperty, ref int PropertyRegDataType, IntPtr PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref USB.SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiGetDeviceInstanceId(IntPtr DeviceInfoSet, ref USB.SP_DEVINFO_DATA DeviceInfoData, StringBuilder DeviceInstanceId, int DeviceInstanceIdSize, out int RequiredSize);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DeviceIoControl(IntPtr hDevice, int dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    public static ReadOnlyCollection<USB.USBController> GetHostControllers()
    {
      List<USB.USBController> list = new List<USB.USBController>();
      Guid guid = new Guid("3abf6f2d-71c4-462a-8a92-1e6861e6af27");
      IntPtr classDevs = USB.SetupDiGetClassDevs(ref guid, 0, IntPtr.Zero, 18);
      if (classDevs.ToInt32() != -1)
      {
        IntPtr num = Marshal.AllocHGlobal(2048);
        int MemberIndex = 0;
        bool flag;
        do
        {
          USB.USBController usbController = new USB.USBController();
          usbController.ControllerIndex = MemberIndex;
          USB.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new USB.SP_DEVICE_INTERFACE_DATA();
          DeviceInterfaceData.cbSize = Marshal.SizeOf((object) DeviceInterfaceData);
          flag = USB.SetupDiEnumDeviceInterfaces(classDevs, IntPtr.Zero, ref guid, MemberIndex, ref DeviceInterfaceData);
          if (flag)
          {
            USB.SP_DEVINFO_DATA DeviceInfoData = new USB.SP_DEVINFO_DATA();
            DeviceInfoData.cbSize = Marshal.SizeOf((object) DeviceInfoData);
            USB.SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData = new USB.SP_DEVICE_INTERFACE_DETAIL_DATA();
            DeviceInterfaceDetailData.cbSize = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8;
            int RequiredSize1 = 0;
            int DeviceInterfaceDetailDataSize = 2048;
            if (USB.SetupDiGetDeviceInterfaceDetail(classDevs, ref DeviceInterfaceData, ref DeviceInterfaceDetailData, DeviceInterfaceDetailDataSize, ref RequiredSize1, ref DeviceInfoData))
            {
              usbController.ControllerDevicePath = DeviceInterfaceDetailData.DevicePath;
              int RequiredSize2 = 0;
              int PropertyRegDataType = 1;
              if (USB.SetupDiGetDeviceRegistryProperty(classDevs, ref DeviceInfoData, 0, ref PropertyRegDataType, num, 2048, ref RequiredSize2))
                usbController.ControllerDeviceDesc = Marshal.PtrToStringAuto(num);
              if (USB.SetupDiGetDeviceRegistryProperty(classDevs, ref DeviceInfoData, 9, ref PropertyRegDataType, num, 2048, ref RequiredSize2))
                usbController.ControllerDriverKeyName = Marshal.PtrToStringAuto(num);
            }
            list.Add(usbController);
          }
          ++MemberIndex;
        }
        while (flag);
        Marshal.FreeHGlobal(num);
        USB.SetupDiDestroyDeviceInfoList(classDevs);
      }
      return new ReadOnlyCollection<USB.USBController>((IList<USB.USBController>) list);
    }

    private static string GetDescriptionByKeyName(string DriverKeyName)
    {
      string str1 = "";
      IntPtr classDevs = USB.SetupDiGetClassDevs(0, "USB", IntPtr.Zero, 6);
      if (classDevs.ToInt32() != -1)
      {
        IntPtr num = Marshal.AllocHGlobal(2048);
        int MemberIndex = 0;
        bool flag;
        do
        {
          USB.SP_DEVINFO_DATA DeviceInfoData = new USB.SP_DEVINFO_DATA();
          DeviceInfoData.cbSize = Marshal.SizeOf((object) DeviceInfoData);
          flag = USB.SetupDiEnumDeviceInfo(classDevs, MemberIndex, ref DeviceInfoData);
          if (flag)
          {
            int RequiredSize = 0;
            int PropertyRegDataType = 1;
            string str2 = "";
            if (USB.SetupDiGetDeviceRegistryProperty(classDevs, ref DeviceInfoData, 9, ref PropertyRegDataType, num, 2048, ref RequiredSize))
              str2 = Marshal.PtrToStringAuto(num);
            if (str2 == DriverKeyName)
            {
              if (USB.SetupDiGetDeviceRegistryProperty(classDevs, ref DeviceInfoData, 0, ref PropertyRegDataType, num, 2048, ref RequiredSize))
              {
                str1 = Marshal.PtrToStringAuto(num);
                break;
              }
              break;
            }
          }
          ++MemberIndex;
        }
        while (flag);
        Marshal.FreeHGlobal(num);
        USB.SetupDiDestroyDeviceInfoList(classDevs);
      }
      return str1;
    }

    private static string GetInstanceIDByKeyName(string DriverKeyName)
    {
      string str1 = "";
      IntPtr classDevs = USB.SetupDiGetClassDevs(0, "USB", IntPtr.Zero, 6);
      if (classDevs.ToInt32() != -1)
      {
        IntPtr num1 = Marshal.AllocHGlobal(2048);
        int MemberIndex = 0;
        bool flag;
        do
        {
          USB.SP_DEVINFO_DATA DeviceInfoData = new USB.SP_DEVINFO_DATA();
          DeviceInfoData.cbSize = Marshal.SizeOf((object) DeviceInfoData);
          flag = USB.SetupDiEnumDeviceInfo(classDevs, MemberIndex, ref DeviceInfoData);
          if (flag)
          {
            int RequiredSize = 0;
            int PropertyRegDataType = 1;
            string str2 = "";
            if (USB.SetupDiGetDeviceRegistryProperty(classDevs, ref DeviceInfoData, 9, ref PropertyRegDataType, num1, 2048, ref RequiredSize))
              str2 = Marshal.PtrToStringAuto(num1);
            if (str2 == DriverKeyName)
            {
              int num2 = 2048;
              StringBuilder DeviceInstanceId = new StringBuilder(num2);
              USB.SetupDiGetDeviceInstanceId(classDevs, ref DeviceInfoData, DeviceInstanceId, num2, out RequiredSize);
              str1 = DeviceInstanceId.ToString();
              break;
            }
          }
          ++MemberIndex;
        }
        while (flag);
        Marshal.FreeHGlobal(num1);
        USB.SetupDiDestroyDeviceInfoList(classDevs);
      }
      return str1;
    }

    private struct STORAGE_DEVICE_NUMBER
    {
      public int DeviceType;
      public int DeviceNumber;
      public int PartitionNumber;
    }

    private enum USB_HUB_NODE
    {
      UsbHub,
      UsbMIParent,
    }

    private enum USB_CONNECTION_STATUS
    {
      NoDeviceConnected,
      DeviceConnected,
      DeviceFailedEnumeration,
      DeviceGeneralFailure,
      DeviceCausedOvercurrent,
      DeviceNotEnoughPower,
      DeviceNotEnoughBandwidth,
      DeviceHubNestedTooDeeply,
      DeviceInLegacyHub,
    }

    private enum USB_DEVICE_SPEED : byte
    {
      UsbLowSpeed,
      UsbFullSpeed,
      UsbHighSpeed,
    }

    private struct SP_DEVINFO_DATA
    {
      public int cbSize;
      public Guid ClassGuid;
      public int DevInst;
      public IntPtr Reserved;
    }

    private struct SP_DEVICE_INTERFACE_DATA
    {
      public int cbSize;
      public Guid InterfaceClassGuid;
      public int Flags;
      public IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
      public int cbSize;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
      public string DevicePath;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct USB_HCD_DRIVERKEY_NAME
    {
      public int ActualLength;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
      public string DriverKeyName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct USB_ROOT_HUB_NAME
    {
      public int ActualLength;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
      public string RootHubName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct USB_HUB_DESCRIPTOR
    {
      public byte bDescriptorLength;
      public byte bDescriptorType;
      public byte bNumberOfPorts;
      public short wHubCharacteristics;
      public byte bPowerOnToPowerGood;
      public byte bHubControlCurrent;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
      public byte[] bRemoveAndPowerMask;
    }

    private struct USB_HUB_INFORMATION
    {
      public USB.USB_HUB_DESCRIPTOR HubDescriptor;
      public byte HubIsBusPowered;
    }

    private struct USB_NODE_INFORMATION
    {
      public int NodeType;
      public USB.USB_HUB_INFORMATION HubInformation;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct USB_NODE_CONNECTION_INFORMATION_EX
    {
      public int ConnectionIndex;
      public USB.USB_DEVICE_DESCRIPTOR DeviceDescriptor;
      public byte CurrentConfigurationValue;
      public byte Speed;
      public byte DeviceIsHub;
      public short DeviceAddress;
      public int NumberOfOpenPipes;
      public int ConnectionStatus;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct USB_DEVICE_DESCRIPTOR
    {
      public byte bLength;
      public byte bDescriptorType;
      public short bcdUSB;
      public byte bDeviceClass;
      public byte bDeviceSubClass;
      public byte bDeviceProtocol;
      public byte bMaxPacketSize0;
      public short idVendor;
      public short idProduct;
      public short bcdDevice;
      public byte iManufacturer;
      public byte iProduct;
      public byte iSerialNumber;
      public byte bNumConfigurations;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct USB_STRING_DESCRIPTOR
    {
      public byte bLength;
      public byte bDescriptorType;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
      public string bString;
    }

    private struct USB_SETUP_PACKET
    {
      public byte bmRequest;
      public byte bRequest;
      public short wValue;
      public short wIndex;
      public short wLength;
    }

    private struct USB_DESCRIPTOR_REQUEST
    {
      public int ConnectionIndex;
      public USB.USB_SETUP_PACKET SetupPacket;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct USB_NODE_CONNECTION_NAME
    {
      public int ConnectionIndex;
      public int ActualLength;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
      public string NodeName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct USB_NODE_CONNECTION_DRIVERKEY_NAME
    {
      public int ConnectionIndex;
      public int ActualLength;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
      public string DriverKeyName;
    }

    public class USBController
    {
      internal int ControllerIndex;
      internal string ControllerDriverKeyName;
      internal string ControllerDevicePath;
      internal string ControllerDeviceDesc;

      public int Index
      {
        get
        {
          return this.ControllerIndex;
        }
      }

      public string DevicePath
      {
        get
        {
          return this.ControllerDevicePath;
        }
      }

      public string DriverKeyName
      {
        get
        {
          return this.ControllerDriverKeyName;
        }
      }

      public string Name
      {
        get
        {
          return this.ControllerDeviceDesc;
        }
      }

      public USBController()
      {
        this.ControllerIndex = 0;
        this.ControllerDevicePath = "";
        this.ControllerDeviceDesc = "";
        this.ControllerDriverKeyName = "";
      }

      public USB.USBHub GetRootHub()
      {
        USB.USBHub usbHub = new USB.USBHub();
        usbHub.HubIsRootHub = true;
        usbHub.HubDeviceDesc = "Root Hub";
        IntPtr file1 = USB.CreateFile(this.ControllerDevicePath, 1073741824, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
        if (file1.ToInt32() != -1)
        {
          int num1 = Marshal.SizeOf((object) new USB.USB_ROOT_HUB_NAME());
          IntPtr num2 = Marshal.AllocHGlobal(num1);
          int lpBytesReturned;
          if (USB.DeviceIoControl(file1, 2229256, num2, num1, num2, num1, out lpBytesReturned, IntPtr.Zero))
          {
            USB.USB_ROOT_HUB_NAME usbRootHubName = (USB.USB_ROOT_HUB_NAME) Marshal.PtrToStructure(num2, typeof (USB.USB_ROOT_HUB_NAME));
            usbHub.HubDevicePath = "\\\\.\\" + usbRootHubName.RootHubName;
          }
          IntPtr file2 = USB.CreateFile(usbHub.HubDevicePath, 1073741824, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
          if (file2.ToInt32() != -1)
          {
            USB.USB_NODE_INFORMATION usbNodeInformation1 = new USB.USB_NODE_INFORMATION();
            usbNodeInformation1.NodeType = 0;
            int num3 = Marshal.SizeOf((object) usbNodeInformation1);
            IntPtr num4 = Marshal.AllocHGlobal(num3);
            Marshal.StructureToPtr((object) usbNodeInformation1, num4, true);
            if (USB.DeviceIoControl(file2, 2229256, num4, num3, num4, num3, out lpBytesReturned, IntPtr.Zero))
            {
              USB.USB_NODE_INFORMATION usbNodeInformation2 = (USB.USB_NODE_INFORMATION) Marshal.PtrToStructure(num4, typeof (USB.USB_NODE_INFORMATION));
              usbHub.HubIsBusPowered = Convert.ToBoolean(usbNodeInformation2.HubInformation.HubIsBusPowered);
              usbHub.HubPortCount = (int) usbNodeInformation2.HubInformation.HubDescriptor.bNumberOfPorts;
            }
            Marshal.FreeHGlobal(num4);
            USB.CloseHandle(file2);
          }
          Marshal.FreeHGlobal(num2);
          USB.CloseHandle(file1);
        }
        return usbHub;
      }
    }

    public class USBHub
    {
      internal int HubPortCount;
      internal string HubDriverKey;
      internal string HubDevicePath;
      internal string HubDeviceDesc;
      internal string HubManufacturer;
      internal string HubProduct;
      internal string HubSerialNumber;
      internal string HubInstanceID;
      internal bool HubIsBusPowered;
      internal bool HubIsRootHub;

      public int PortCount
      {
        get
        {
          return this.HubPortCount;
        }
      }

      public string DevicePath
      {
        get
        {
          return this.HubDevicePath;
        }
      }

      public string DriverKey
      {
        get
        {
          return this.HubDriverKey;
        }
      }

      public string Name
      {
        get
        {
          return this.HubDeviceDesc;
        }
      }

      public string InstanceID
      {
        get
        {
          return this.HubInstanceID;
        }
      }

      public bool IsBusPowered
      {
        get
        {
          return this.HubIsBusPowered;
        }
      }

      public bool IsRootHub
      {
        get
        {
          return this.HubIsRootHub;
        }
      }

      public string Manufacturer
      {
        get
        {
          return this.HubManufacturer;
        }
      }

      public string Product
      {
        get
        {
          return this.HubProduct;
        }
      }

      public string SerialNumber
      {
        get
        {
          return this.HubSerialNumber;
        }
      }

      public USBHub()
      {
        this.HubPortCount = 0;
        this.HubDevicePath = "";
        this.HubDeviceDesc = "";
        this.HubDriverKey = "";
        this.HubIsBusPowered = false;
        this.HubIsRootHub = false;
        this.HubManufacturer = "";
        this.HubProduct = "";
        this.HubSerialNumber = "";
        this.HubInstanceID = "";
      }

      public ReadOnlyCollection<USB.USBPort> GetPorts()
      {
        List<USB.USBPort> list = new List<USB.USBPort>();
        IntPtr file = USB.CreateFile(this.HubDevicePath, 1073741824, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
        if (file.ToInt32() != -1)
        {
          int num1 = Marshal.SizeOf(typeof (USB.USB_NODE_CONNECTION_INFORMATION_EX));
          IntPtr num2 = Marshal.AllocHGlobal(num1);
          for (int index = 1; index <= this.HubPortCount; ++index)
          {
            USB.USB_NODE_CONNECTION_INFORMATION_EX connectionInformationEx = new USB.USB_NODE_CONNECTION_INFORMATION_EX();
            connectionInformationEx.ConnectionIndex = index;
            Marshal.StructureToPtr((object) connectionInformationEx, num2, true);
            int lpBytesReturned;
            if (USB.DeviceIoControl(file, 2229320, num2, num1, num2, num1, out lpBytesReturned, IntPtr.Zero))
            {
              connectionInformationEx = (USB.USB_NODE_CONNECTION_INFORMATION_EX) Marshal.PtrToStructure(num2, typeof (USB.USB_NODE_CONNECTION_INFORMATION_EX));
              USB.USBPort usbPort = new USB.USBPort();
              usbPort.PortPortNumber = index;
              usbPort.PortHubDevicePath = this.HubDevicePath;
              USB.USB_CONNECTION_STATUS connectionStatus = (USB.USB_CONNECTION_STATUS) connectionInformationEx.ConnectionStatus;
              usbPort.PortStatus = connectionStatus.ToString();
              USB.USB_DEVICE_SPEED usbDeviceSpeed = (USB.USB_DEVICE_SPEED) connectionInformationEx.Speed;
              usbPort.PortSpeed = usbDeviceSpeed.ToString();
              usbPort.PortIsDeviceConnected = connectionInformationEx.ConnectionStatus == 1;
              usbPort.PortIsHub = Convert.ToBoolean(connectionInformationEx.DeviceIsHub);
              usbPort.PortDeviceDescriptor = connectionInformationEx.DeviceDescriptor;
              list.Add(usbPort);
            }
          }
          Marshal.FreeHGlobal(num2);
          USB.CloseHandle(file);
        }
        return new ReadOnlyCollection<USB.USBPort>((IList<USB.USBPort>) list);
      }
    }

    public class USBPort
    {
      internal int PortPortNumber;
      internal string PortStatus;
      internal string PortHubDevicePath;
      internal string PortSpeed;
      internal bool PortIsHub;
      internal bool PortIsDeviceConnected;
      internal USB.USB_DEVICE_DESCRIPTOR PortDeviceDescriptor;

      public int PortNumber
      {
        get
        {
          return this.PortPortNumber;
        }
      }

      public string HubDevicePath
      {
        get
        {
          return this.PortHubDevicePath;
        }
      }

      public string Status
      {
        get
        {
          return this.PortStatus;
        }
      }

      public string Speed
      {
        get
        {
          return this.PortSpeed;
        }
      }

      public bool IsHub
      {
        get
        {
          return this.PortIsHub;
        }
      }

      public bool IsDeviceConnected
      {
        get
        {
          return this.PortIsDeviceConnected;
        }
      }

      public USBPort()
      {
        this.PortPortNumber = 0;
        this.PortStatus = "";
        this.PortHubDevicePath = "";
        this.PortSpeed = "";
        this.PortIsHub = false;
        this.PortIsDeviceConnected = false;
      }

      public USB.USBDevice GetDevice()
      {
        if (!this.PortIsDeviceConnected)
          return (USB.USBDevice) null;
        USB.USBDevice usbDevice = new USB.USBDevice();
        usbDevice.DevicePortNumber = this.PortPortNumber;
        usbDevice.DeviceHubDevicePath = this.PortHubDevicePath;
        usbDevice.DeviceDescriptor = this.PortDeviceDescriptor;
        IntPtr file = USB.CreateFile(this.PortHubDevicePath, 1073741824, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
        if (file.ToInt32() != -1)
        {
          int num1 = 2048;
          string s = new string(char.MinValue, 2048 / Marshal.SystemDefaultCharSize);
          int lpBytesReturned;
          if ((int) this.PortDeviceDescriptor.iManufacturer > 0)
          {
            USB.USB_DESCRIPTOR_REQUEST descriptorRequest = new USB.USB_DESCRIPTOR_REQUEST();
            descriptorRequest.ConnectionIndex = this.PortPortNumber;
            descriptorRequest.SetupPacket.wValue = (short) (768 + (int) this.PortDeviceDescriptor.iManufacturer);
            descriptorRequest.SetupPacket.wLength = (short) (num1 - Marshal.SizeOf((object) descriptorRequest));
            descriptorRequest.SetupPacket.wIndex = (short) 1033;
            IntPtr num2 = Marshal.StringToHGlobalAuto(s);
            Marshal.StructureToPtr((object) descriptorRequest, num2, true);
            if (USB.DeviceIoControl(file, 2229264, num2, num1, num2, num1, out lpBytesReturned, IntPtr.Zero))
            {
              USB.USB_STRING_DESCRIPTOR stringDescriptor = (USB.USB_STRING_DESCRIPTOR) Marshal.PtrToStructure(new IntPtr(num2.ToInt32() + Marshal.SizeOf((object) descriptorRequest)), typeof (USB.USB_STRING_DESCRIPTOR));
              usbDevice.DeviceManufacturer = stringDescriptor.bString;
            }
            Marshal.FreeHGlobal(num2);
          }
          if ((int) this.PortDeviceDescriptor.iProduct > 0)
          {
            USB.USB_DESCRIPTOR_REQUEST descriptorRequest = new USB.USB_DESCRIPTOR_REQUEST();
            descriptorRequest.ConnectionIndex = this.PortPortNumber;
            descriptorRequest.SetupPacket.wValue = (short) (768 + (int) this.PortDeviceDescriptor.iProduct);
            descriptorRequest.SetupPacket.wLength = (short) (num1 - Marshal.SizeOf((object) descriptorRequest));
            descriptorRequest.SetupPacket.wIndex = (short) 1033;
            IntPtr num2 = Marshal.StringToHGlobalAuto(s);
            Marshal.StructureToPtr((object) descriptorRequest, num2, true);
            if (USB.DeviceIoControl(file, 2229264, num2, num1, num2, num1, out lpBytesReturned, IntPtr.Zero))
            {
              USB.USB_STRING_DESCRIPTOR stringDescriptor = (USB.USB_STRING_DESCRIPTOR) Marshal.PtrToStructure(new IntPtr(num2.ToInt32() + Marshal.SizeOf((object) descriptorRequest)), typeof (USB.USB_STRING_DESCRIPTOR));
              usbDevice.DeviceProduct = stringDescriptor.bString;
            }
            Marshal.FreeHGlobal(num2);
          }
          if ((int) this.PortDeviceDescriptor.iSerialNumber > 0)
          {
            USB.USB_DESCRIPTOR_REQUEST descriptorRequest = new USB.USB_DESCRIPTOR_REQUEST();
            descriptorRequest.ConnectionIndex = this.PortPortNumber;
            descriptorRequest.SetupPacket.wValue = (short) (768 + (int) this.PortDeviceDescriptor.iSerialNumber);
            descriptorRequest.SetupPacket.wLength = (short) (num1 - Marshal.SizeOf((object) descriptorRequest));
            descriptorRequest.SetupPacket.wIndex = (short) 1033;
            IntPtr num2 = Marshal.StringToHGlobalAuto(s);
            Marshal.StructureToPtr((object) descriptorRequest, num2, true);
            if (USB.DeviceIoControl(file, 2229264, num2, num1, num2, num1, out lpBytesReturned, IntPtr.Zero))
            {
              USB.USB_STRING_DESCRIPTOR stringDescriptor = (USB.USB_STRING_DESCRIPTOR) Marshal.PtrToStructure(new IntPtr(num2.ToInt32() + Marshal.SizeOf((object) descriptorRequest)), typeof (USB.USB_STRING_DESCRIPTOR));
              usbDevice.DeviceSerialNumber = stringDescriptor.bString;
            }
            Marshal.FreeHGlobal(num2);
          }
          USB.USB_NODE_CONNECTION_DRIVERKEY_NAME connectionDriverkeyName = new USB.USB_NODE_CONNECTION_DRIVERKEY_NAME();
          connectionDriverkeyName.ConnectionIndex = this.PortPortNumber;
          int num3 = Marshal.SizeOf((object) connectionDriverkeyName);
          IntPtr num4 = Marshal.AllocHGlobal(num3);
          Marshal.StructureToPtr((object) connectionDriverkeyName, num4, true);
          if (USB.DeviceIoControl(file, 2229280, num4, num3, num4, num3, out lpBytesReturned, IntPtr.Zero))
          {
            connectionDriverkeyName = (USB.USB_NODE_CONNECTION_DRIVERKEY_NAME) Marshal.PtrToStructure(num4, typeof (USB.USB_NODE_CONNECTION_DRIVERKEY_NAME));
            usbDevice.DeviceDriverKey = connectionDriverkeyName.DriverKeyName;
            usbDevice.DeviceName = USB.GetDescriptionByKeyName(usbDevice.DeviceDriverKey);
            usbDevice.DeviceInstanceID = USB.GetInstanceIDByKeyName(usbDevice.DeviceDriverKey);
          }
          Marshal.FreeHGlobal(num4);
          USB.CloseHandle(file);
        }
        return usbDevice;
      }

      public USB.USBHub GetHub()
      {
        if (!this.PortIsHub)
          return (USB.USBHub) null;
        USB.USBHub usbHub = new USB.USBHub();
        usbHub.HubIsRootHub = false;
        usbHub.HubDeviceDesc = "External Hub";
        IntPtr file1 = USB.CreateFile(this.PortHubDevicePath, 1073741824, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
        if (file1.ToInt32() != -1)
        {
          USB.USB_NODE_CONNECTION_NAME nodeConnectionName = new USB.USB_NODE_CONNECTION_NAME();
          nodeConnectionName.ConnectionIndex = this.PortPortNumber;
          int num1 = Marshal.SizeOf((object) nodeConnectionName);
          IntPtr num2 = Marshal.AllocHGlobal(num1);
          Marshal.StructureToPtr((object) nodeConnectionName, num2, true);
          int lpBytesReturned;
          if (USB.DeviceIoControl(file1, 2229268, num2, num1, num2, num1, out lpBytesReturned, IntPtr.Zero))
          {
            nodeConnectionName = (USB.USB_NODE_CONNECTION_NAME) Marshal.PtrToStructure(num2, typeof (USB.USB_NODE_CONNECTION_NAME));
            usbHub.HubDevicePath = "\\\\.\\" + nodeConnectionName.NodeName;
          }
          IntPtr file2 = USB.CreateFile(usbHub.HubDevicePath, 1073741824, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
          if (file2.ToInt32() != -1)
          {
            USB.USB_NODE_INFORMATION usbNodeInformation = new USB.USB_NODE_INFORMATION();
            usbNodeInformation.NodeType = 0;
            int num3 = Marshal.SizeOf((object) usbNodeInformation);
            IntPtr num4 = Marshal.AllocHGlobal(num3);
            Marshal.StructureToPtr((object) usbNodeInformation, num4, true);
            if (USB.DeviceIoControl(file2, 2229256, num4, num3, num4, num3, out lpBytesReturned, IntPtr.Zero))
            {
              usbNodeInformation = (USB.USB_NODE_INFORMATION) Marshal.PtrToStructure(num4, typeof (USB.USB_NODE_INFORMATION));
              usbHub.HubIsBusPowered = Convert.ToBoolean(usbNodeInformation.HubInformation.HubIsBusPowered);
              usbHub.HubPortCount = (int) usbNodeInformation.HubInformation.HubDescriptor.bNumberOfPorts;
            }
            Marshal.FreeHGlobal(num4);
            USB.CloseHandle(file2);
          }
          USB.USBDevice device = this.GetDevice();
          usbHub.HubInstanceID = device.DeviceInstanceID;
          usbHub.HubManufacturer = device.Manufacturer;
          usbHub.HubProduct = device.Product;
          usbHub.HubSerialNumber = device.SerialNumber;
          usbHub.HubDriverKey = device.DriverKey;
          Marshal.FreeHGlobal(num2);
          USB.CloseHandle(file1);
        }
        return usbHub;
      }
    }

    public class USBDevice
    {
      internal int DevicePortNumber;
      internal string DeviceDriverKey;
      internal string DeviceHubDevicePath;
      internal string DeviceInstanceID;
      internal string DeviceName;
      internal string DeviceManufacturer;
      internal string DeviceProduct;
      internal string DeviceSerialNumber;
      internal USB.USB_DEVICE_DESCRIPTOR DeviceDescriptor;

      public int PortNumber
      {
        get
        {
          return this.DevicePortNumber;
        }
      }

      public string HubDevicePath
      {
        get
        {
          return this.DeviceHubDevicePath;
        }
      }

      public string DriverKey
      {
        get
        {
          return this.DeviceDriverKey;
        }
      }

      public string InstanceID
      {
        get
        {
          return this.DeviceInstanceID;
        }
      }

      public string Name
      {
        get
        {
          return this.DeviceName;
        }
      }

      public string Manufacturer
      {
        get
        {
          return this.DeviceManufacturer;
        }
      }

      public string Product
      {
        get
        {
          return this.DeviceProduct;
        }
      }

      public string SerialNumber
      {
        get
        {
          return this.DeviceSerialNumber;
        }
      }

      public USBDevice()
      {
        this.DevicePortNumber = 0;
        this.DeviceHubDevicePath = "";
        this.DeviceDriverKey = "";
        this.DeviceManufacturer = "";
        this.DeviceProduct = "Unknown USB Device";
        this.DeviceSerialNumber = "";
        this.DeviceName = "";
        this.DeviceInstanceID = "";
      }
    }
  }
}
