
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XenMaster.Helpers;
using XenMaster.IntrospectorAPI.XMLMessages;
using XenMaster.IntrospectorAPI.XMLObjects;
using XenMaster.LibvirtAPI;
using XenMaster.LibvirtAPI.XMLMessages;
using XenMaster.LibvirtAPI.XMLObjects;
using XenMaster.Models;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using static XenMaster.XenMasterEventArgs;
using XenMaster.MachineListener;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;

namespace XenMaster
{
    /// <summary>
    /// Encapsulates all the functionality necessary to connect to and control a host and its virtual machines;
    /// Version 1.3.7
    /// Last Update 2016 October 30
    /// </summary>
    public class XenConnect
    {
        //Update after every minor, major, patch revision
        public static readonly string VERSION = "Xen Master 1.3.7";

        #region ERROR CODES
        public static readonly int ERROR_NONE = 101;
        public static readonly int ERROR_VM_RUNNING = 202;
        public static readonly int ERROR_VM_STOPPED = 303;
        public static readonly int ERROR_VM_PAUSED = 404;
        public static readonly int ERROR_VM_DOES_NOT_EXIST = 505;
        public static readonly int ERROR_VM_SHUTDOWN = 606;
        #endregion

        #region RESPONSE CODES
        public static readonly int RESPONSE_CODE_ACKNOWLEDGE = 707;

        #endregion

        public event EventHandler<XenMasterEventArgs> Notifications;

        protected virtual void Notify(XenMasterEventArgs e)
        {
            Notifications?.Invoke(this, e);
        }

        private Host _host;
        private NetworkStream _stream;
        private bool _isConnected = false;
        private TcpClient _client;
        private string _requestorId;
        private string _requestorIp;
        private const int STRING_MAX_CAPACITY = 1000;
        private string SHARED_DRIVE_PATH = "\\\\172.16.10.180\\public\\malware";

        public bool isConnected { get { return _isConnected; } }

        public XenConnect(Host host, string requestorId, string requestorIp)
        {
            _host = host;
            _requestorId = requestorId;
            _requestorIp = requestorIp;
        }

        /// <summary>
        /// Returns the details of the host specified by the XenConnect connection object. This function will return
        /// a default host (ip, port) instantation if the connectToHost function was never called
        /// </summary>
        /// <returns></returns>
        public Host getHost()
        {
            return _host;
        }

        #region Virtual Machine Management Functions


        /// <summary>
        /// Starts the virtual machine specified by the uuid string. Callers should check the return type of XenError to determine
        /// whether or not the command was executed successfully.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError startVm(string uuid)
       {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient libvirtClient = null;
            NetworkStream networkStream;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper startVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.START, _requestorId, _requestorIp);
                
                //set the VM uuid
                LibvirtCommand startDomain= (LibvirtCommand)startVMMessage.Message;
                startDomain.Vms.Add(new LibvirtVM(uuid));
                string msg = startVMMessage.toXMLString();
                          
                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response =  LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_START_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs(response.ResponseMessage, MasterEventType.VM_START_FAILURE, Serverity.Error));
                }else
                {
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: "+uuid+" has been successfully started", MasterEventType.VM_START, Serverity.Notification));
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: "+_host.Id, MasterEventType.VM_START_FAILURE, Serverity.Error));
            }
            finally
            {
                if(libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close(); 
            }

            return status;
        }

        /// <summary>
        /// Starts the virtual machine specified by the uuid string. Callers should check the return type of XenError to determine
        /// whether or not the command was executed successfully.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError cloneVm(string uuid, string cloneName)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient libvirtClient = null;
            NetworkStream networkStream;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper cloneVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.CLONE_VM, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand cloneDomain = (LibvirtCommand)cloneVMMessage.Message;
                LibvirtVM vmInfo = new LibvirtVM(uuid); //name of parent
                vmInfo.Name = cloneName; //name of clone
                cloneDomain.Vms.Add(vmInfo);
                string msg = cloneVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_CLONE_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs(response.ResponseMessage, MasterEventType.VM_CLONE_FAILURE, Serverity.Notification));

                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.VM_CLONE_FAILURE, Serverity.Error));
            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close();
            }

            return status;
        }

        public XenError listVmStats(string uuid, out List<VcpuInfo> infoList)
        {
            XenError status = new XenError();
            infoList = null;


            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion


            TcpClient libvirtClient = null;
            NetworkStream networkStream;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper listVmStatsMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.LIST_VM_STATISTICS, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand shutDomain = (LibvirtCommand)listVmStatsMessage.Message;
                shutDomain.Vms.Add(new LibvirtVM(uuid));
                string msg = listVmStatsMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_STATS_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs(response.ResponseMessage, MasterEventType.VM_LIST_FAILURE, Serverity.Error));

                }else
                {
                    LibvirtCommand comm = (LibvirtCommand)response.Message;
                    LibvirtVM vm = comm.Vms[0];
                    LibvirtVcpuStats stats = vm.VcpuStats;
                    List<VcpuInfo> cpuInfo = new List<VcpuInfo>();

                    foreach (LibvirtVCpu cpu in vm.VcpuStats.cpus)
                    {
                        cpuInfo.Add(new VcpuInfo(cpu.VcpuNumber, cpu.State, cpu.CpuTime, cpu.Cpu));
                    }
                    infoList = cpuInfo;
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + uuid + "virtual machine list retrieved", MasterEventType.VM_LIST, Serverity.Notification));

                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.CONNECTION_FAILURE, Serverity.Error));
            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close();
            }

            return status;

        }



        public XenError deleteVm(string uuid)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion


            TcpClient libvirtClient = null;
            NetworkStream networkStream;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);
                
                //request virtual machine list
                LibvirtMessageWrapper deleteVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.DELETE_VM, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand shutDomain = (LibvirtCommand)deleteVMMessage.Message;
                shutDomain.Vms.Add(new LibvirtVM(uuid));
                string msg = deleteVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_DELETE_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs(response.ResponseMessage, MasterEventType.VM_DELETE_FAILURE, Serverity.Notification));

                }else
                {
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + uuid + " has been successfully deleted", MasterEventType.VM_DELETED, Serverity.Notification));

                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.CONNECTION_FAILURE, Serverity.Error));

            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close();
            }

            return status;

        }

        /// <summary>
        /// Forces shutdown of the virtual machine specified by the uuid string. The force shutdown call is equivalent to
        /// removing power to the machine. Callers should check the return type of XenError to determine
        /// whether or not the command was executed successfully.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError ForceShutdownVm(string uuid)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient libvirtClient = null;
            NetworkStream networkStream;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper forceShutdownVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.FORCE_SHUTDOWN, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand shutDomain = (LibvirtCommand)forceShutdownVMMessage.Message;
                shutDomain.Vms.Add(new LibvirtVM(uuid));
                string msg = forceShutdownVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_FORCE_SHUTDOWN_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + uuid + " has been successfully started", MasterEventType.VM_FORCE_SHUTDOWN_FAILURE, Serverity.Error));

                }else
                {
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + uuid + " has been successfully started", MasterEventType.VM_FORCE_SHUTDOWN, Serverity.Notification));

                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.VM_START_FAILURE, Serverity.Error));
            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close();
            }

            return status;
        }

        public async Task<XenError> SendProcessListScan(string uuid, string name, int duration, ScanTypes type)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient introspectorClient = null;
            NetworkStream networkStream;
            try
            {
                introspectorClient = new TcpClient(_host.Ip, _host.Port);

                //request virtual machine list
                IntrospectorMessageWrapper introspectorMessage =
                    IntrospectorMessageBase.BuildMessage(IntrospectorMessageBase.MessageTypes.PROCESS_LIST_SCAN, _requestorId, _requestorIp);



                Message message = new Message();

                VmInfo vmInfo = new VmInfo();
                vmInfo.Name = name;
                vmInfo.UUID = uuid;

                FeatureScan featureScan = new FeatureScan();
                if (type == ScanTypes.LINUX_PROCESS_LIST_SCAN)
                {
                    featureScan.Type = "LinuxProcessListScans";
                }
                else if (type == ScanTypes.WINDOWS_PROCESS_LIST_SCAN)
                {
                    featureScan.Type = "WindowsProcessListScans";
                }

                featureScan.Duration = duration;
                featureScan.VmInfo = vmInfo;
                message.Data = featureScan;

                introspectorMessage.Message = message;
                //string msg = "<Introspector><Command>PROCESS LIST SCAN</Command><Requestor></Requestor><RequestorId></RequestorId><Response></Response><ResponseMessage></ResponseMessage><Message><FeatureScan><Type>LinuxProcessListScans</Type><Duration>1</Duration><VmInfo><Name>Buntu5-1</Name><UUID>xxx-yyy-zzz</UUID></VmInfo></FeatureScan></Message></Introspector>";//introspectorMessage.toXMLString();

                string msg = introspectorMessage.toXMLString();
                networkStream = introspectorClient.GetStream();

                IntrospectorMessageWrapper response = await Task.Run(() => IntrospectorXMLMarshall(msg, networkStream));

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_START_FAILURE;
                    status.Message = response.ResponseMessage;
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.VM_START_FAILURE, Serverity.Error));
            }
            finally
            {
                if (introspectorClient != null)
                    introspectorClient.Close();
            }

            return status;
        }

        public void startVMProcessListScan(FeatureScanArguments args)
        {
            switch (args.Type)
            {
                case ScanTypes.LINUX_PROCESS_LIST_SCAN:
                    startVMProcessListScanExclusive(args.Uuid, args.Vm, args.Duration, args.getExclusionList());
                    break;
                case ScanTypes.WINDOWS_PROCESS_LIST_SCAN:
                    break;
            }
        }

        public string getScanType(ScanTypes type)
        {
            string res = "UNKNOWN";
            if (ScanTypes.LINUX_PROCESS_LIST_SCAN == type)
            {
                res = "LinuxProcessListScans";
            }
            else if (ScanTypes.WINDOWS_PROCESS_LIST_SCAN == type)
            {
                res = "WindowsProcessListScans";
            }
            return res;
        }
        private XenError startVMProcessListScanExclusive(string uuid, string name, int duration, ProcessList exclusionList)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient introspectorClient = null;
            NetworkStream networkStream;
            try
            {
                introspectorClient = new TcpClient(_host.Ip, _host.Port);

                //request virtual machine list
                IntrospectorMessageWrapper introspectorMessage = 
                    IntrospectorMessageBase.BuildMessage(IntrospectorMessageBase.MessageTypes.PROCESS_LIST_SCAN, _requestorId, _requestorIp);
                
                Message message = new Message();

                VmInfo vmInfo = new VmInfo();
                vmInfo.Name = name;
                vmInfo.UUID = uuid;

                FeatureScan featureScan = new FeatureScan();

                featureScan.Type = getScanType(ScanTypes.LINUX_PROCESS_LIST_SCAN);
                featureScan.Duration = duration;
                featureScan.VmInfo = vmInfo;
                featureScan.ProcessList = exclusionList;
                
                message.Data = featureScan;

                introspectorMessage.Message = message;
                
                string msg = introspectorMessage.toXMLString();
                networkStream = introspectorClient.GetStream();

                IntrospectorMessageWrapper response = IntrospectorXMLMarshall(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_START_FAILURE;
                    status.Message = response.ResponseMessage;
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.VM_START_FAILURE, Serverity.Error));
            }
            finally
            {
                if (introspectorClient != null)
                    introspectorClient.Close();
            }

            return status;
        }

        public XenError SendHeartBeat()
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient introspectorClient = null;
            NetworkStream networkStream;
            try
            {
                introspectorClient = new TcpClient(_host.Ip, _host.Port);

                //request virtual machine list
                IntrospectorMessageWrapper introspectorMessage =
                    IntrospectorMessageBase.BuildMessage(IntrospectorMessageBase.MessageTypes.HELLO, _requestorId, _requestorIp);

                //string msg = "<Introspector><Command>PROCESS LIST SCAN</Command><Requestor></Requestor><RequestorId></RequestorId><Response></Response><ResponseMessage></ResponseMessage><Message><FeatureScan><Type>LinuxProcessListScans</Type><Duration>1</Duration><VmInfo><Name>Buntu5-1</Name><UUID>xxx-yyy-zzz</UUID></VmInfo></FeatureScan></Message></Introspector>";//introspectorMessage.toXMLString();

                string msg = introspectorMessage.toXMLString();
                networkStream = introspectorClient.GetStream();

                IntrospectorMessageWrapper response = IntrospectorXMLMarshall(msg, networkStream);

                if (!(response.ResponseCode == 1))
                {
                    status.Type = XenError.XenErrorType.VM_START_FAILURE;
                    status.Message = response.ResponseMessage;
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.VM_START_FAILURE, Serverity.Error));
            }
            finally
            {
                if (introspectorClient != null)
                    introspectorClient.Close();
            }

            return status;
        }

        /// <summary>
        /// Stops the virtual machine specified by the uuid string. Callers should check the return type of XenError to determine
        /// whether or not the command was executed successful.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError stopVm(string uuid)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient libvirtClient = null;
            NetworkStream networkStream;

            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper startVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.STOP, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand stopDomain = (LibvirtCommand)startVMMessage.Message;
                stopDomain.Vms.Add(new LibvirtVM(uuid));
                string msg = startVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_STOP_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs(response.ResponseMessage, MasterEventType.VM_STOP_FAILURE, Serverity.Error));

                }else
                {
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + uuid + " has been successfully stopped", MasterEventType.VM_STOP, Serverity.Notification));

                }


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }
            finally
            {
                if(libvirtClient!= null && libvirtClient.Connected)
                    libvirtClient.Close();
            }
            return status;
        }

        /// <summary>
        /// Pauses the virtual machine specified by the uuid string. Callers should check the return type of XenError to determine
        /// whether or not the command was executed successful.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError pauseVm(string uuid)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient libvirtClient = null;
            NetworkStream networkStream;

            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper startVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.PAUSE, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand stopDomain = (LibvirtCommand)startVMMessage.Message;
                stopDomain.Vms.Add(new LibvirtVM(uuid));
                string msg = startVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_PAUSE_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs(response.ResponseMessage, MasterEventType.VM_PAUSE_FAILURE, Serverity.Error));

                }else
                {
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + uuid + " has been successfully paused", MasterEventType.VM_PAUSE, Serverity.Notification));

                }


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));

            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close();
            }
            return status;
        }

        /// <summary>
        /// Resumes the virtual machine specified by the uuid string. Callers should check the return type of XenError to determine
        /// whether or not the command was executed successful.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError resumeVm(string uuid)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient libvirtClient = null;
            NetworkStream networkStream;

            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper startVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.RESUME, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand stopDomain = (LibvirtCommand)startVMMessage.Message;
                stopDomain.Vms.Add(new LibvirtVM(uuid));
                string msg = startVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_RESUME_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + uuid + " has been successfully resume", MasterEventType.VM_RESUME_FAILURE, Serverity.Notification));

                }else
                {
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + uuid + " has been successfully resume", MasterEventType.VM_RESUME, Serverity.Notification));
                }


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close();
            }
            return status;
        }

        /*
        public XenError deleteVm(string uuid, bool keepDisk)
        {

        }*/
        #endregion

        public XenError createVm(VirtualMachineBuilder builder)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient libvirtClient = null;
            NetworkStream networkStream;

            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper createVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.CREATE_VM, builder, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand stopDomain = (LibvirtCommand)createVMMessage.Message;
                string msg = createVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.CREATE_VM_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs(response.ResponseMessage, MasterEventType.CREATE_VM_FAILURE, Serverity.Error));

                }else
                {
                    Notify(new XenMasterEventArgs("Virtual Machine with UUID: " + builder.VmName + " has been successfully created", MasterEventType.CREATE_VM, Serverity.Notification));

                }


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
            }
            finally
            {
                if (libvirtClient != null)
                    libvirtClient.Close();
            }


            return status;
        }


        /// <summary>
        /// Retrieves the VNC port number for the virtual machine with the specified UUID. The VNC port number can be used to connect to the virtual machine's 
        /// VNC port and receive Remote Frame Buffer information
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError getVmProcessList(string vmName, out List<Processes> vmProcessList)
        {
            XenError status = new XenError();

            vmProcessList = null;

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion


            TcpClient introspectorClient = null;
            NetworkStream networkStream;

            try
            {
                introspectorClient = new TcpClient(_host.Ip, _host.Port);

                //request virtual machine list
                IntrospectorMessageWrapper introspectorMessage =
                    IntrospectorMessageBase.BuildMessage(IntrospectorMessageBase.MessageTypes.LIST_LINUX_VM_PROCESSES, _requestorId, _requestorIp);

                    Message message = new Message();

                    ProcessList list = new ProcessList();
                    list.VM = vmName;
                    message.Data = list;
                introspectorMessage.Message = message;

                string msg = introspectorMessage.toXMLString();
                networkStream = introspectorClient.GetStream();

                IntrospectorMessageWrapper response = IntrospectorXMLMarshall(msg, networkStream);
                Message respMessage = (Message)response.Message;
                ProcessList procs = (ProcessList)respMessage.Data;

                vmProcessList = procs.ProcessBlock;

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.VM_PROCESS_LIST_RETRIEVAL_FAILURE;
                    status.Message = response.ResponseMessage;
                }


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }
            finally
            {
                if (introspectorClient != null && introspectorClient.Connected)
                    introspectorClient.Close();
            }

            return status;
        }

        public static string getDSN(string DbIpAddress,int DbPort, string NetLibrary, string InitialCatalog, string UserId, string Password)
        {
            //"Data Source=172.16.10.181,1433;Network Library=DBMSSOCN;Initial Catalog=ASHIDS;User ID=ASHIDSAdmin;Password=ASHIDS@2015";
            StringBuilder str = new StringBuilder(STRING_MAX_CAPACITY);
            str.AppendFormat("Data Source={0},{1};Network Library={2};Initial Catalog={3};User ID={4};Password={5}", DbIpAddress, DbPort, NetLibrary,
                                                                                                                     InitialCatalog, UserId, Password);

            return str.ToString();
        }

        public ApplicationType getApplicationType(string type)
        {
            if (type == "Benign")
            {
                return ApplicationType.BENIGN;
            }
            else if (type == "Malware")
            {
                return ApplicationType.MALWARE;
            }
            return ApplicationType.UNKOWN;
        }

        public XenError listMalwareSharedDrive(out List<Application> list)
        {

            list = new List<Application>();
            DirectoryInfo di = null;
            XenError status = new XenError();
            try
            {
                di = new DirectoryInfo(SHARED_DRIVE_PATH); //connect to shared drive
            }
            catch (Exception e) when (e is UnauthorizedAccessException || e is ArgumentException
                   || e is ArgumentNullException || e is PathTooLongException || e is IOException || e is DirectoryNotFoundException)
            {
                Console.WriteLine(e.Message);
                status.Type = XenError.XenErrorType.SHARED_DRIVE_LIST_RETRIEVAL_FAILURE;
                status.Message = e.StackTrace + "\n" + e.Message;
                return status;
            }
            
            if(di != null)
            {
                //Create list of files
                foreach (var fi in di.EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    Application malware = new Application();
                    malware.name = fi.Name;
                    malware.execution_name = fi.Name;
                    malware.extension = fi.Extension;
                    malware.size = fi.Length;
                    malware.type = ApplicationType.MALWARE;
                    malware.os = "-";
                    list.Add(malware);
                }
            }

            status.Type = XenError.XenErrorType.SUCCESS;
            return status;
        }
        
       
        //need to be modified
        public XenError getCommonApplication(string dns, Dictionary<string,Application> appList)
        {
            return null;
        }


        
        public XenError sendMalwareTo(Application malware,string host_ip, int port)
        {
            XenError status = new XenError();
            MachineListenerResponse resp = null;
            
            //check if malware is in Shared Drive
            if (!checkMalFromDBInSharedDrive(malware))
            {
                status.Type = XenError.XenErrorType.MALWARE_NOT_FOUND;
                status.Message = "Malware " + malware.name + " could not be located in the shared drive with path " + SHARED_DRIVE_PATH;
                return status;
            }

            //Retrieve byte stream for the specific malware
            injectMalware(host_ip, port, out resp, malware);

            Console.WriteLine(resp.message);

            status.Type = XenError.XenErrorType.SUCCESS;
            status.Message = "Malware " + malware.name + " is in shared drive with path " + SHARED_DRIVE_PATH;

            return status;
        }

        private bool checkMalFromDBInSharedDrive(Application malware)
        {

            string malware_name = malware.name;
            List<Application> malware_list = null;
            listMalwareSharedDrive(out malware_list);

            List<string> tmp_mal_list = new List<string>();

            foreach (Application mal in malware_list)
            {
                tmp_mal_list.Add(mal.name);
            }

            foreach (string mal in tmp_mal_list)
            {
                if (malware.name.Equals(mal))
                {
                    return true;
                }
            }
            return false;
        }

        //done
        public XenError listMalwareDB(out List<Application> malwareList)
        {
            XenError status = new XenError();
            SqlConnection sqlConnection = null;
            malwareList = new List<Application>();

            string connectionString = getDSN("172.16.10.201", 1433, "DBMSSOCN", "CTAM", "CTAMAdmin", "CTAM@2016");
            Console.WriteLine(connectionString);
             using (sqlConnection = new SqlConnection(connectionString)){

                    try
                    {
                        sqlConnection.Open();
                         using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlDataReader reader;

                            cmd.CommandText = "ProcessMaster_GetProcessByType";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = sqlConnection;
                            cmd.CommandTimeout = 0;

                            cmd.Parameters.Add("@ProcessType", SqlDbType.VarChar, 30).Value = "Malware";
                            cmd.ExecuteNonQuery();

                            // Data is accessible through the DataReader object here.
                            reader = cmd.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read()) {
                                    //Console.WriteLine(reader.GetString(3));
                                    Application malware = new Application();
                                    malware.name = reader.GetString(1);
                                    malware.execution_name = reader.GetString(2);
                                    malware.extension = reader.GetString(3);
                                    malware.description = reader.GetString(4);
                                    malware.type = getApplicationType(reader.GetString(5));
                                    malware.size = reader.GetInt32(6);
                                    malware.os = reader.GetString(7);
                                    malware.comments = reader.GetString(8);
                                    malwareList.Add(malware);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No rows found.");
                                malwareList = null;
                            }
                         }
                    }
                    catch (SqlException ex)
                    {
                        status.Type = XenError.XenErrorType.CONNECTION_FAILURE;
                        status.Message = "State of the connection: " + sqlConnection.State + "\nFailure: \n" + ex.Message + "\n";
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
                sqlConnection.Close();
            return status;
        }

        /// <summary>
        /// Retrieves the VNC port number for the virtual machine with the specified UUID. The VNC port number can be used to connect to the virtual machine's 
        /// VNC port and receive Remote Frame Buffer information
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError getVmVNCPort(string uuid, out int port)
        {
            XenError status = new XenError();

            port = 0; //denotes a failure as the port will never be 0

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion


            TcpClient libvirtClient = null;
            NetworkStream networkStream;

            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper vncVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.GET_VNC_PORT, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand getPortDomain = (LibvirtCommand)vncVMMessage.Message;
                getPortDomain.Vms.Add(new LibvirtVM(uuid));
                string msg = vncVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.HOST_DETAILS_FAILURE;
                    status.Message = response.ResponseMessage;
                }else
                {
                    LibvirtCommand resp = (LibvirtCommand)response.Message;
                   port = int.Parse(resp.Vms[0].VNC);
                }


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close();
            }

            return status;
        }

        /// <summary>
        /// Transfers a file to the host
        /// </summary>
        /// <returns></returns>
        public XenError transferFile(string path, string remoteDirectory, long size)
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient libvirtClient = null;
            NetworkStream networkStream;

            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request file transfer
                LibvirtMessageWrapper transferMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.TRANSFER_FILE, _requestorId, _requestorIp);

                string filename = null;

                string[] parts = path.Split('\\');
                filename = parts[parts.Length - 1];

                //create file transfer XML
                LibvirtFileTransfer xmltransfer = new LibvirtFileTransfer(filename, remoteDirectory, size);

                //build Data object to send raw string parameters instead of XML
                DataWrapper dataWrapper = new DataWrapper();
                dataWrapper.Data = xmltransfer;

                transferMessage.Message = dataWrapper;
                

                string msg = transferMessage.toXMLString();


                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallAsync(msg, networkStream);

                if (!(response.ResponseCode == RESPONSE_CODE_ACKNOWLEDGE))
                {
                    status.Type = XenError.XenErrorType.FILE_TRANSFER_FAILURE;
                    status.Message = response.ResponseMessage;
                    return status;
                }

                //the host has acknowledged the file transfer request, send the file
                FileStream fs = File.Open(SHARED_DRIVE_PATH, FileMode.Open, FileAccess.Read);
                fs.CopyTo(networkStream);
                fs.Flush();
                fs.Close();

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                {
                    libvirtClient.GetStream().Close();
                    libvirtClient.Close();
                }
            }

            return status;
        }

        //string ip, int port, string app_name, out MachineListenerResponse resp
        private XenError injectMalware(string ip, int port, out MachineListenerResponse resp, Application malware) //malware contains name and size
        {

            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    resp = null;
                    return status;
                }
            }
            #endregion

            resp = null;
            TcpClient tcpClient = null;
            NetworkStream networkStream = null;
            StreamWriter streamWriter = null;
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            try
            {
                tcpClient = new TcpClient(ip, port);
                
                Command command = new Command();
                command.identifier = Command.getEnumeratedType(commandType.TRANSFER_FILE);
                command.name = malware.name;
                command.size = malware.size;

                string msg = serializer.Serialize(command);
                Console.WriteLine(msg);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);

                networkStream = tcpClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(msg);
                streamWriter.Flush();
                
                //check if worked?
                FileStream fs = File.Open(SHARED_DRIVE_PATH + "\\" + malware.name, FileMode.Open, FileAccess.Read);
                fs.CopyTo(networkStream);
                fs.Flush();
                fs.Close();

                //Read response
                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }

                resp = serializer.Deserialize<MachineListenerResponse>(sData);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                return new XenError(ex.StackTrace, XenError.XenErrorType.CONNECTION_FAILURE);
            }
            finally
            {
                if (networkStream != null)
                    networkStream.Close();
                if (tcpClient != null && tcpClient.Connected)
                    tcpClient.Close();
            }
            return status;
        }

        //string ip, int port, string app_name, out MachineListenerResponse resp
        private XenError echoMachineListener(string ip, int port) //malware contains name and size
        {

            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion

            TcpClient tcpClient = null;
            NetworkStream networkStream = null;
            StreamWriter streamWriter = null;
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            try
            {
                tcpClient = new TcpClient(ip, port);

                Command command = new Command();
                command.identifier = Command.getEnumeratedType(commandType.ECHO);

                string msg = serializer.Serialize(command);
                Console.WriteLine(msg);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);

                networkStream = tcpClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(msg);
                streamWriter.Flush();

                //Read response
                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }

                MachineListenerResponse resp = serializer.Deserialize<MachineListenerResponse>(sData);
                if(resp.message != null)
                {

                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                return new XenError(ex.StackTrace, XenError.XenErrorType.CONNECTION_FAILURE);
            }
            finally
            {
                if (networkStream != null)
                    networkStream.Close();
                if (tcpClient != null && tcpClient.Connected)
                    tcpClient.Close();
            }
            return status;
        }

        /// <summary>
        /// Retrieves the host details information, this includes CPU, Cores, Memory KB, Mhz, Model, Nodes, Sockets, Threads
        /// and should not be called in lieu of the resyncHost function
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public XenError getHostDetails()
        {
            XenError status = new XenError();

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                {
                    status = sync;
                    return status;
                }
            }
            #endregion


            TcpClient libvirtClient = null;
            NetworkStream networkStream;

            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper startVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.HOST_DETAILS, _requestorId, _requestorIp);

                //set the VM uuid
                LibvirtCommand stopDomain = (LibvirtCommand)startVMMessage.Message;
                string msg = startVMMessage.toXMLString();

                networkStream = libvirtClient.GetStream();

                LibvirtMessageWrapper response = LibvirtMarshallSync(msg, networkStream);

                if (!(response.ResponseCode == ERROR_NONE))
                {
                    status.Type = XenError.XenErrorType.HOST_DETAILS_FAILURE;
                    status.Message = response.ResponseMessage;
                    Notify(new XenMasterEventArgs(response.ResponseMessage, MasterEventType.VM_START, Serverity.Error));

                }


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }
            finally
            {
                if (libvirtClient != null && libvirtClient.Connected)
                    libvirtClient.Close();
            }

            return status;
        }

        public XenError getHostDiskUsage(out List<DiskStatistic> stats)
        {
            XenError status = new XenError();
            stats = null;

            #region Resync Host
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                    return sync;
            }
            #endregion


            List<DiskStatistic> hostDiskStats = new List<DiskStatistic>();

            TcpClient libvirtClient;
            NetworkStream networkStream;
            StreamWriter streamWriter;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper listDisksStatsMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.LIST_DISK_USAGE, _requestorId, _requestorIp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(listDisksStatsMessage.toXMLString());

                networkStream = libvirtClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(listDisksStatsMessage.toXMLString());
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                LibvirtMessageWrapper syncMessage = LibvirtXMLUnmarshall(sData);
                List<LibvirtDiskStats> diskStats = ((LibvirtDiskUsage)syncMessage.Message).Stats;
                Notify(new XenMasterEventArgs("Retrieved Host VHD disk list", MasterEventType.VHD_RETRIEVAL, Serverity.Notification));

                foreach (LibvirtDiskStats dstat in diskStats)
                {
                    hostDiskStats.Add(new DiskStatistic(dstat));
                }

                stats = hostDiskStats;

                networkStream.Close();
                libvirtClient.Close();


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }

            return status;

        }

        /// <summary>
        /// Returns a list of the current virtual disk (virtual machine hard drives) on the host;
        /// </summary>
        /// <returns></returns>
        public List<VirtualDisk> getVirtualDiskList()
        {
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                    return new List<VirtualDisk>(); //returns empty vm object in the case of an error
            }


            List<VirtualDisk> hostDisks = new List<VirtualDisk>();

            TcpClient libvirtClient;
            NetworkStream networkStream;
            StreamWriter streamWriter;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper listDisksMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.LIST_DISKS, _requestorId, _requestorIp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(listDisksMessage.toXMLString());

                networkStream = libvirtClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(listDisksMessage.toXMLString());
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                LibvirtMessageWrapper syncMessage = LibvirtXMLUnmarshall(sData);
                List<LibvirtDisks> VirtualDisks = syncMessage.Host.VirtualDisks.VirtualDisks;
                Notify(new XenMasterEventArgs("Retrieved Host VHD disk list", MasterEventType.VHD_RETRIEVAL, Serverity.Notification));


                foreach (LibvirtDisks disk in VirtualDisks)
                {

                    VirtualDisk d = new VirtualDisk();
                    d.Name = disk.Name;
                    d.Size = disk.Size;
                    hostDisks.Add(d);
                }

                networkStream.Close();
                libvirtClient.Close();


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }

            return hostDisks;
        }

        
        /// <summary>
        /// Returns a list of the current iso images stored on the host
        /// </summary>
        /// <returns></returns>
        public List<VirtualDisk> getIStorageList()
        {
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                    return new List<VirtualDisk>(); //returns empty vm object in the case of an error
            }

            List<VirtualDisk> hostDisks = new List<VirtualDisk>();

            TcpClient libvirtClient;
            NetworkStream networkStream;
            StreamWriter streamWriter;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper listIsoMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.LIST_IMAGES, _requestorId, _requestorIp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(listIsoMessage.toXMLString());

                networkStream = libvirtClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(listIsoMessage.toXMLString());
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                LibvirtMessageWrapper syncMessage = LibvirtXMLUnmarshall(sData);
                List<LibvirtDisks> VirtualDisks = syncMessage.Host.VirtualDisks.VirtualDisks;


                foreach (LibvirtDisks disk in VirtualDisks)
                {

                    VirtualDisk d = new VirtualDisk();
                    d.Name = disk.Name;
                    d.Size = disk.Size;
                    hostDisks.Add(d);
                }

                networkStream.Close();
                libvirtClient.Close();


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }

            return hostDisks;
        }

        /// <summary>
        /// Returns a list of the current iso images stored on the host
        /// </summary>
        /// <returns></returns>
        public List<StorageDisk> getVHDStorageList()
        {
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                    return new List<StorageDisk>(); //returns empty vm object in the case of an error
            }

            List<StorageDisk> hostVHDDisks = new List<StorageDisk>();

            TcpClient libvirtClient;
            NetworkStream networkStream;
            StreamWriter streamWriter;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper listIsoMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.LIST_VHD_STORAGE, _requestorId, _requestorIp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(listIsoMessage.toXMLString());

                networkStream = libvirtClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(listIsoMessage.toXMLString());
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                LibvirtMessageWrapper syncMessage = LibvirtXMLUnmarshall(sData);
                List<LibvirtVHDDisk> StorageDisks = syncMessage.Host.StorageList.StorageList;


                foreach (LibvirtVHDDisk disk in StorageDisks)
                {

                    StorageDisk d = new StorageDisk();
                    d.Name = disk.Name;
                    d.SizeGB = disk.SizeGB;
                    d.Os = disk.Os;
                    d.Kernel = disk.Kernel;
                    d.Rekall = disk.Rekall;
                    d.Description = disk.Description;
                    d.Distribution = disk.Distribution;
                    hostVHDDisks.Add(d);
                }

                networkStream.Close();
                libvirtClient.Close();


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }

            return hostVHDDisks;
        }


        /// <summary>
        /// Returns a list of the current iso images stored on the host
        /// </summary>
        /// <returns></returns>
        public List<VirtualDisk> getISOStorageList()
        {
            if (_host == null || !_host.IsSynced)
            {
                XenError sync = reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                    return new List<VirtualDisk>(); //returns empty vm object in the case of an error
            }

            List<VirtualDisk> hostDisks = new List<VirtualDisk>();

            TcpClient libvirtClient;
            NetworkStream networkStream;
            StreamWriter streamWriter;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper listIsoMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.LIST_IMAGES, _requestorId, _requestorIp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(listIsoMessage.toXMLString());

                networkStream = libvirtClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(listIsoMessage.toXMLString());
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                LibvirtMessageWrapper syncMessage = LibvirtXMLUnmarshall(sData);
                List<LibvirtDisks> VirtualDisks = syncMessage.Host.VirtualDisks.VirtualDisks;
                Notify(new XenMasterEventArgs("Retrieved Host ISO storage list", MasterEventType.ISO_RETRIEVAL, Serverity.Notification));


                foreach (LibvirtDisks disk in VirtualDisks)
                {

                    VirtualDisk d = new VirtualDisk();
                    d.Name = disk.Name;
                    d.Size = disk.Size;
                    hostDisks.Add(d);
                }

                networkStream.Close();
                libvirtClient.Close();


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }

            return hostDisks;
        }



        /// <summary>
        /// Returns a Observable Concurrent Dictionary (Collection) containing all the VM's on the host;
        /// </summary>
        /// <returns></returns>
        public VirtualMachineList getHostVirtualMachineList()
        {
            if (_host == null || !_host.IsSynced)
            {
               XenError sync =  reSyncHost();
                if (sync.Type == XenError.XenErrorType.CONNECTION_FAILURE)
                    return new VirtualMachineList(); //returns empty vm object in the case of an error
            }

            VirtualMachineList vms = new VirtualMachineList();

            TcpClient libvirtClient;
            NetworkStream networkStream;
            StreamWriter streamWriter;
            try
            {
                libvirtClient = new TcpClient(_host.LibvirtIp, _host.LibvirtPort);

                //request virtual machine list
                LibvirtMessageWrapper listVMMessage =
                    LibvirtMessageBase.BuildMessage(LibvirtMessageBase.LIBVIRT_MESSAGE_TYPE.LIST, _requestorId, _requestorIp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(listVMMessage.toXMLString());

                networkStream = libvirtClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(listVMMessage.toXMLString());
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                LibvirtMessageWrapper syncMessage = LibvirtXMLUnmarshall(sData);
                LibvirtCommand domains = (LibvirtCommand)syncMessage.Message;

                //add to list
                foreach (LibvirtVM xmlVM in domains.Vms)
                {
                    VM vm = getVMFromXML(xmlVM);
                    vms.Add(vm.UUID, vm);
                }

                networkStream.Close();
                libvirtClient.Close();


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                Notify(new XenMasterEventArgs("Unable to connect to Host: " + _host.Id, MasterEventType.HOST_OFFLINE, Serverity.Error));
            }

            return vms;
        }

        #region Connectivity Functions

        /// <summary>
        /// Synchronize the host's details with that of the remote server. This function will retrieve the ip address and port settings etc. of 
        /// the host as well as current cpu and memory state.
        /// </summary>
        /// <returns></returns>
        public XenError reSyncHost()
        {

            XenError status = new XenError();

            try
            {
                clientConnect();
                //request synchronization with the host
                IntrospectorMessageWrapper syncRequestMessage =
                    IntrospectorMessageBase.BuildMessage(IntrospectorMessageBase.MessageTypes.SYNC_REQUEST, _requestorId, _requestorIp);
                byte[] data = System.Text.Encoding.ASCII.GetBytes(syncRequestMessage.toXMLString() + "\n");

                _stream = _client.GetStream();
                _stream.Write(data, 0, data.Length);

                StreamReader sReader = new StreamReader(_stream, Encoding.ASCII);
                string sData;
                sData = sReader.ReadLine();

                IntrospectorMessageWrapper syncMessage = IntrospectorXMLUnmarshall(sData);

                //hosts is online, get host details
                if (syncMessage._enumeratedType == IntrospectorMessageBase.MessageTypes.SYNC_REQUEST)
                {
                    HostSettings settings = (HostSettings)((Message)syncMessage.Message).Data;
                    _host.setHostSettings(settings);
                    _host.PowerState = Host.HOST_POWER_STATE.Online;

                    _stream.Close();
                    clientDisconnect();
                }


            }
            catch (SocketException ex)

            {

                Console.WriteLine(ex);
                _host.PowerState = Host.HOST_POWER_STATE.Offline;
                return new XenError(ex.StackTrace, XenError.XenErrorType.CONNECTION_FAILURE);
            }

            return status;
        }

        public XenError connect()
        {
            return connectToHost();

        }

        public async Task<XenError> connectAsync()
        {
            return await Task.Run(() => connectToHost());

        }

        /// <summary>
        /// Connects to the host server, creates a populates a C# host object and requests host details
        /// </summary>
        /// <returns></returns>
        private XenError connectToHost()
        {
            XenError status = new XenError();

            try
            {
                clientConnect();
                //request synchronization with the host
                IntrospectorMessageWrapper syncRequestMessage =
                    IntrospectorMessageBase.BuildMessage(IntrospectorMessageBase.MessageTypes.SYNC_REQUEST, _requestorId, _requestorIp);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(syncRequestMessage.toXMLString() + "\n");

                _stream = _client.GetStream();
                _stream.Write(data, 0, data.Length);

                StreamReader sReader = new StreamReader(_stream, Encoding.ASCII);
                string sData;
                sData = sReader.ReadLine();

                IntrospectorMessageWrapper syncMessage = IntrospectorXMLUnmarshall(sData);

                //hosts is online, get host details
                if (syncMessage._enumeratedType == IntrospectorMessageBase.MessageTypes.SYNC_REQUEST)
                {
                    HostSettings settings = (HostSettings)((Message)syncMessage.Message).Data;
                    _host.setHostSettings(settings);

                    _stream.Close();
                    clientDisconnect();
                    //Get the hosts vm list

                }


            }
            catch (SocketException ex)
            {

                Console.WriteLine(ex);
                return new XenError(ex.StackTrace, XenError.XenErrorType.CONNECTION_FAILURE);
            }

            return status;
        }

        /// <summary>
        /// Disconnect the Tcp client
        /// </summary>
        private void clientDisconnect()
        {
            if (_client.Connected)
                _client.Close();
            _isConnected = false;

        }

        /// <summary>
        /// Open a Tcp client connection to the ip address and port
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        private void clientConnect()
        {
            _client = new TcpClient(_host.Ip, _host.Port);
            _isConnected = true;
        }
        #endregion
        #region XML API Functions

        /// <summary>
        /// Parse a XML string and convert it to the IntrospectorMessageWrapper type
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static IntrospectorMessageWrapper IntrospectorXMLUnmarshall(string msg)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(IntrospectorMessageWrapper));
            IntrospectorMessageWrapper wrappedMessage = null;

            //deserialize
            using (TextReader reader = new StringReader(msg))
            {
                wrappedMessage = (IntrospectorMessageWrapper)deserializer.Deserialize(reader);
            }
            
            return wrappedMessage;

        }
        

        public static IntrospectorMessageWrapper IntrospectorXMLMarshall(string msg, NetworkStream networkStream)
        {
            XenError status = new XenError();

            StreamWriter streamWriter;
            IntrospectorMessageWrapper response = null;

            try
            {
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(msg);
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                response = IntrospectorXMLUnmarshall(sData);


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (networkStream != null)
                    networkStream.Close();
            }
            
            return response;

        }


        /// <summary>
        /// Marshalls a LibvirtMessageWrapper class object to it's equivalent XML string and sends it out the network stream then closes the stream
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        public LibvirtMessageWrapper LibvirtMarshallSync(string msg, NetworkStream networkStream)
        {
            XenError status = new XenError();

            StreamWriter streamWriter;
            LibvirtMessageWrapper response = null;

            try
            {
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(msg);
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                response = LibvirtXMLUnmarshall(sData);


            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (networkStream != null)
                    networkStream.Close();
            }


            return response;
        }


        /// <summary>
        /// Marshalls a LibvirtMessageWrapper class object to it's equivalent XML string and sends it out the network stream then closes the stream
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        public LibvirtMessageWrapper LibvirtMarshallAsync(string msg, NetworkStream networkStream)
        {
            XenError status = new XenError();

            StreamWriter streamWriter;
            LibvirtMessageWrapper response = null;

            try
            {
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(msg);
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;
                //sData = sReader.ReadLine();

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }


                response = LibvirtXMLUnmarshall(sData);


            }catch(SocketException ex){
                Console.WriteLine(ex);
            }


            return response;
        }

        /// <summary>
        /// Parse a XML string and convert it to the LibvirtMessageWrapper type
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static LibvirtMessageWrapper LibvirtXMLUnmarshall(string msg) 
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(LibvirtMessageWrapper));
            LibvirtMessageWrapper wrappedMessage = null;

            //deserialize
            using (TextReader reader = new StringReader(msg))
            {
                wrappedMessage = (LibvirtMessageWrapper)deserializer.Deserialize(reader);
            }


            return wrappedMessage;

        }

        /// <summary>
        /// Convert XML virtual machine info into it's C# class representation
        /// </summary>
        /// <param name="xmlVm"></param>
        /// <returns></returns>
        public static VM getVMFromXML(LibvirtVM xmlVm)
        {
            VM vm = new VM();
            if (xmlVm.Id != null && uint.Parse(xmlVm.State)!=5)
                vm.Id = uint.Parse(xmlVm.Id);
            if (xmlVm.MaxMemory != null)
                vm.MaxMem = ulong.Parse(xmlVm.MaxMemory);
            if (xmlVm.Memory != null)
                vm.Memory = ulong.Parse(xmlVm.Memory);
            if (xmlVm.Name != null)
                vm.Name = xmlVm.Name;
            if (xmlVm.Os != null)
                vm.Os = xmlVm.Os;
            if (xmlVm.VCpus != null)
                vm.VCpus = int.Parse(xmlVm.VCpus);
            if (xmlVm.CpuTime != null)
                vm.CpuTime = ulong.Parse(xmlVm.CpuTime);
            if (xmlVm.UUID != null)
                vm.UUID = xmlVm.UUID;
            if (xmlVm.VNC != null)
                vm.VNCPort = int.Parse(xmlVm.VNC);
            if (xmlVm.State != null)
            {
                vm.State = int.Parse(xmlVm.State);
                vm.PowerState = VMPowerState.getEnumeratePowerState(vm.State);
            }
            if(xmlVm.VcpuStats != null)
            {
                List<VcpuInfo> cpuInfo = new List<VcpuInfo>();

                foreach(LibvirtVCpu cpu in xmlVm.VcpuStats.cpus)
                {
                    cpuInfo.Add(new VcpuInfo(cpu.VcpuNumber, cpu.State, cpu.CpuTime, cpu.Cpu));
                }
                vm.updateVcpuStats(cpuInfo);
            }
            return vm;
        }

        //MachineListenerResponse
        public XenError getMachineApplicationList(string ip, int port, out MachineListenerResponse resp)
        {
            XenError status = new XenError();
            resp = null;
            TcpClient tcpClient = null;
            NetworkStream networkStream = null;
            StreamWriter streamWriter = null;

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            try
            {
                tcpClient = new TcpClient(ip, port);

                //request virtual machine application list
                Command command = new Command();
                command.identifier = Command.getEnumeratedType(commandType.LIST_APPLICATION);
                command.name = "";
                command.size = 0;

                string msg = serializer.Serialize(command);
                Console.WriteLine(msg);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);

                networkStream = tcpClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(msg);
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }

                resp = serializer.Deserialize<MachineListenerResponse>(sData);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                return new XenError(ex.StackTrace, XenError.XenErrorType.CONNECTION_FAILURE);
            }
            finally
            {
                if (networkStream != null)
                    networkStream.Close();
                if (tcpClient != null && tcpClient.Connected)
                    tcpClient.Close();
            }
            return status;
        }

        public XenError startMachineApplication(string ip, int port, string app_name, out MachineListenerResponse resp)
        {
            XenError status = new XenError();
            resp = null;
            TcpClient tcpClient = null;
            NetworkStream networkStream = null;
            StreamWriter streamWriter = null;

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            try
            {
                tcpClient = new TcpClient(ip, port);

                //request virtual machine application list
                Command command = new Command();
                command.identifier = Command.getEnumeratedType(commandType.START_APPLICATION);
                command.name = app_name;
                command.size = 0;

                string msg = serializer.Serialize(command);
                Console.WriteLine(msg);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);

                networkStream = tcpClient.GetStream();
                streamWriter = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };
                streamWriter.WriteLine(msg);
                streamWriter.Flush();

                StreamReader sReader = new StreamReader(networkStream, Encoding.ASCII);
                string sData = null;

                while ((sData = sReader.ReadLine()) == null)
                {
                    sData = sReader.ReadLine();
                }

                resp = serializer.Deserialize<MachineListenerResponse>(sData);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                return new XenError(ex.StackTrace, XenError.XenErrorType.CONNECTION_FAILURE);
            }
            finally
            {
                if (networkStream != null)
                    networkStream.Close();
                if (tcpClient != null && tcpClient.Connected)
                    tcpClient.Close();
            }
            return status;
        }

    }

    #endregion
}
