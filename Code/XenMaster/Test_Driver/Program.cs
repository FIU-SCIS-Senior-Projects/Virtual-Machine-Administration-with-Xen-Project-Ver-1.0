using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenMaster;
using XenMaster.Helpers;
using XenMaster.IntrospectorAPI.XMLObjects;
using XenMaster.LibvirtAPI;
using XenMaster.MachineListener;
using XenMaster.Models;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using XenMaster.LibvirtAPI.XMLMessages;

namespace Test_Driver
{
    class Program
    {
        //unique identifier or name for the server that is sending the request
        public static string testServerId = "Senior Project Xen Master Test";
        //ip address of the sending server
        public static string testip = "172.16.10.76";

        static void Main(string[] args)
        {
            //info for the remote host we want to connect to
            Host host = new Host("172.16.10.115", 29171);
            host.Name = "TCC9";

            Console.WriteLine("Connecting to Xen Host: "+host.Ip + ":" + host.Port+"...");

                XenConnect master = new XenConnect(host, testServerId, testip);
 
            VirtualMachineList vms = master.getHostVirtualMachineList();

            Console.WriteLine("Retrieved host details\n\t"+master.getHost().ToString());
            Console.WriteLine("Retrieving Virtual Machine List...");

            foreach(VM vm in vms.Values)
            {
                Console.WriteLine("\t" + vm.ToString());
            }

            Console.WriteLine("Test Complete...");
            //Test_ListIso(host);
            //Test_CreateVmExisting(host);
            //Test_GetVNCPort(host, "4cb12bd0-b1c6-a6bb-d1c4-46246585d351");
            //Test_FileTransfer(host, "C:\\Users\\DODTech\\Desktop\\sample.txt", "\\home\\guest_images\\");
            //Test_CloneVM(host, "4cb12bd0-b1c6-a6bb-d1c4-46246585d351", "CopyBuntu");
            //Test_GetProcessList(host, "Buntu5-1");
            //Test_SendProcessListScan(host, "Buntu5-1");
            //Test_ListVMStatistics(host, "4cb12bd0-b1c6-a6bb-d1c4-46246585d351");

            //Test_ListVmApplications(host);
            //Test_StartVmApplication(host);
            //Test_ListMalware(host);
            //TestMalwareDirectory(host, "\\\\172.16.10.180\\public\\malware");
            //Test_SendMalwareTo(host, "172.16.10.89", 3645);
            //Test_getVHDStorageList(host);
            Test_CreateVmExisting2(host);

            //Console.ReadLine();
        }

        static void Test_CreateVmExisting2(Host host)
        {
            //Get vm iso
            XenConnect conn = new XenConnect(host, testServerId, testip);
            

            string vmName = "IntrospectionTestBuntu";
            int memory = 2000000;
            int vcpus = 1;
            VirtualMachineBuilder.DomainType domType = VirtualMachineBuilder.DomainType.TRANSIENT;
            VirtualMachineBuilder builder = new VirtualMachineBuilder(vmName, "Ubuntu14-04", memory, vcpus, domType);
            //builder.setHardDrive(20000, true); //set hard drive size to 20 GB (20k MB)
            builder.setHardDriveExisting(true, false, "");
            string xml = builder.toXMLString();

            conn.createVm(builder);
            //string xml = builder.toXMLString();
        }

        private static void Test_getVHDStorageList(Host host)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            List<StorageDisk> disks = conn.getVHDStorageList();
        }

        private static void Test_GetHostDiskusage(Host host)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            List<DiskStatistic> stats = null;
            conn.getHostDiskUsage(out stats);
        }
        private static void Test_StartVmApplication(Host host)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            MachineListenerResponse response = null;
            conn.startMachineApplication("172.16.10.89", 8080, "sol", out response);

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            string serializedResponse = serializer.Serialize(response);
            Console.WriteLine(serializedResponse);
        }

        /*
        private static void Test_ListMalware(Host host)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            
            List<Application> malwareList = null;
            Console.WriteLine(conn.listMalwares(out malwareList).Message);
            
        }
        */
        private static void Test_ListVmApplications(Host host)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            MachineListenerResponse response = null;
            conn.getMachineApplicationList("172.16.10.89", 8080, out response);

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            string serializedResponse = serializer.Serialize(response);
            Console.WriteLine(serializedResponse);
        }

        static void Test_ListVMStatistics(Host host, string uuid)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            List<VcpuInfo> info = null;
            conn.listVmStats(uuid, out info);
        }

        static void Test_DeleteVM(Host host, string uuid)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            conn.deleteVm(uuid);
        }

        static void Test_GetProcessList(Host host, string vmName)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            List<Processes> vmProcesses = null;
            conn.getVmProcessList(vmName, out vmProcesses);
        }

        static void Test_CloneVM(Host host, string uuid, string name)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            conn.cloneVm(uuid,name);
        }

        static void Test_CreateVmExisting(Host host)
        {
                //Get vm iso
                XenConnect conn = new XenConnect(host, testServerId, testip);
                List<VirtualDisk> disks = conn.getISOStorageList();

                string filename = null;

                //check if the disk exists
                foreach (VirtualDisk disk in disks)
                {
                    if (disk.NameCanonical.Contains("Ubuntu14"))
                    {
                        filename = disk.Name;
                        break;
                    }
                }

                string vmName = "SeniorBuntu";
                int memory = 2000000;
                int vcpus = 1;
                VirtualMachineBuilder.DomainType domType = VirtualMachineBuilder.DomainType.TRANSIENT;
                VirtualMachineBuilder builder = new VirtualMachineBuilder(vmName, memory, vcpus, VirtualMachineBuilder.XEN_PATH_DEFAULT_ISO + filename, domType);
                //builder.setHardDrive(20000, true); //set hard drive size to 20 GB (20k MB)
                builder.setHardDriveExisting(true,false,VirtualMachineBuilder.XEN_PATH_DEFAULT_DISK + "BuntuMaster.img");
                conn.createVm(builder);
                //string xml = builder.toXMLString();
        }

        #region Old Tests
        static void Test_FileTransfer(Host host, string file, string remoteDirectory)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            FileStream fs = File.Open(file, FileMode.Open,FileAccess.Read);
            long size = fs.Length;
            fs.Close();
            conn.transferFile(file,remoteDirectory,size);
        }

        static void Test_GetVNCPort(Host host, string uuid)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            int portNum;
            conn.getVmVNCPort(uuid, out portNum);
        }

        static void Test_CreateVm(Host host)
        {
            //Get vm iso
            XenConnect conn = new XenConnect(host, testServerId, testip);
            conn.SendHeartBeat();

        }

        static async void Test_SendProcessListScan(Host host, string VmName)
        {
            XenConnect conn = new XenConnect(host, testServerId, testip);
            await conn.SendProcessListScan("UUID", VmName,1, ScanTypes.LINUX_PROCESS_LIST_SCAN);
        }


        /// <summary>
        /// Tests List Disks
        /// </summary>
        /// <param name="host"></param>
        static void Test_ListIso(Host host)
        {
            Console.WriteLine("Retrieving host virtual machine disks...");
            XenConnect con = new XenConnect(host, testServerId, testip);
            List<VirtualDisk> disks = con.getISOStorageList();
        }



        /// <summary>
        /// Tests List Disks
        /// </summary>
        /// <param name="host"></param>
        static void Test_ListDisks(Host host)
        {
            Console.WriteLine("Retrieving host virtual machine disks...");
            XenConnect con = new XenConnect(host, testServerId, testip);
            List<VirtualDisk> disks = con.getVirtualDiskList();
        }


        /// <summary>
        /// Tests starting a VM
        /// </summary>
        static void Test_StartVM(Host host, string uuid)
        {
            Console.WriteLine("Starting virtual machine with uuid "+uuid);
            XenConnect con = new XenConnect(host, testServerId, testip);
            XenError status = con.startVm(uuid);
            if (status.Type == XenError.XenErrorType.SUCCESS)
                Console.WriteLine("VM with uuid " + uuid + " started successfully");
            else
                Console.WriteLine("Unable to start vm with uuid " + uuid + " " + status.Message);
        }

        /// <summary>
        /// Tests stopping a VM
        /// </summary>
        static void Test_StopVM(Host host, string uuid)
        {
            Console.WriteLine("Stopping virtual machine with uuid " + uuid);
            XenConnect con = new XenConnect(host, testServerId, testip);
            XenError status = con.stopVm(uuid);
            if (status.Type == XenError.XenErrorType.SUCCESS)
                Console.WriteLine("VM with uuid " + uuid + " stopped successfully");
            else
                Console.WriteLine("Unable to stopped vm with uuid " + uuid + " " + status.Message);
        }

        /// <summary>
        /// Tests stopping a VM
        /// </summary>
        static void Test_PauseVM(Host host, string uuid)
        {
            Console.WriteLine("Pauseing virtual machine with uuid " + uuid);
            XenConnect con = new XenConnect(host, testServerId, testip);
            XenError status = con.pauseVm(uuid);
            if (status.Type == XenError.XenErrorType.SUCCESS)
                Console.WriteLine("VM with uuid " + uuid + " pause successfully");
            else
                Console.WriteLine("Unable to pause vm with uuid " + uuid + " " + status.Message);
        }

        static void Test_ResumeVM(Host host, string uuid)
        {
            Console.WriteLine("Resuming virtual machine with uuid " + uuid);
            XenConnect con = new XenConnect(host, testServerId, testip);
            XenError status = con.resumeVm(uuid);
            if (status.Type == XenError.XenErrorType.SUCCESS)
                Console.WriteLine("VM with uuid " + uuid + " resumed successfully");
            else
                Console.WriteLine("Unable to resume vm with uuid " + uuid + " " + status.Message);
        }

        static void Test_GetHostDetails(Host host)
        {
            Console.WriteLine("Retrieving host details...");
            XenConnect con = new XenConnect(host, testServerId, testip);
            XenError status = con.getHostDetails();
            if (status.Type == XenError.XenErrorType.SUCCESS)
                Console.WriteLine("Host details retrieved successfully");
            else
                Console.WriteLine("Unable to retrieve the hosts details "+status.Message);
        }
    }
    #endregion
}
