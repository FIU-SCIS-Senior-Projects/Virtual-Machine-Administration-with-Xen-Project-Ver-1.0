using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using XenMaster;
using XenMaster.Models;
using System.Data.SqlClient;
using System.Web.Configuration;


public partial class About : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Panel_slow.Visible = false;
    }



    protected void LinkButton_Connect_Click(object sender, EventArgs e)
    {
        Label_IPAddress.Text = Textbox_IPAddress.Text;

        //populate information


        Textbox_Host.Text = "VM Hypervisor";

        Textbox_CPU.Text = "Cores";

        Textbox_MHZ.Text = "GHZ";

        Textbox_Memory.Text = "GB";

        Panel_slow.Visible = true;

        Label_IPAddress.Text = "";
        Label_VMList.Text = "";

        //unique identifier or name for the server that is sending the request
        string testServerId = "Senior Project Xen Master Test";
        //ip address of the sending server
        string testip = "172.16.10.76";
        //info for the remote host we want to connect to
        //Host host = new Host("172.16.10.115", 29171);
        Host host = new Host(Textbox_IPAddress.Text, 29171);
        host.Name = "TCC9";

        //Console.WriteLine(">>>>D'Mita Levy User Story 1: Senior Project<<<");

        //Console.Write("Please Enter the Xen Host Ip: ");
        testip = Textbox_IPAddress.Text;

        Label_IPAddress.Text = ("Connecting to Xen Host: " + host.Ip + ":" + host.Port + "...<br>");

        XenConnect master = new XenConnect(host, testServerId, testip);

        VirtualMachineList vms = master.getHostVirtualMachineList();

        




        Label_IPAddress.Text = (Label_IPAddress.Text + "Retrieved host details " + master.getHost().ToString()) + "<br>";
        Label_IPAddress.Text = (Label_IPAddress.Text + "Retrieving Virtual Machine List...<br>");

        DataTable dt = new DataTable();
        
        dt.Columns.Add("VMName", Type.GetType("System.String"));
        dt.Columns.Add("State", Type.GetType("System.String"));
        dt.Columns.Add("Vcpus", Type.GetType("System.String"));
        dt.Columns.Add("Max memory", Type.GetType("System.String"));

        using (SqlConnection con = new SqlConnection("CTAMConnectionString"))
        {
            using (SqlCommand cmd = new SqlCommand("VMStatus_AddStatus", con))
        {
            foreach (VM vm in vms.Values)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@HostID", vm.Id);
                cmd.Parameters.AddWithValue("@HostName",vm.);
                cmd.Parameters.AddWithValue("@VMUUID", vm.UUID);
                cmd.Parameters.AddWithValue("@VMName", vm.Name);
                cmd.Parameters.AddWithValue("@OperatingSystem", vm.Os);
                cmd.Parameters.AddWithValue("@CPUTime", "123456");
                cmd.Parameters.AddWithValue("@VMState", vm.State);
                cmd.Parameters.AddWithValue("@Memory", "TEst");
                cmd.Parameters.AddWithValue("@MaxMemory", vm.MaxMem);
                cmd.Parameters.AddWithValue("@VirtualCPUS", vm.VCpus);
                cmd.Parameters.AddWithValue("@StatusCode", vm.Id);
                cmd.Parameters.AddWithValue("@Comments", vm.Id);
                con.Open();

                cmd.ExecuteNonQuery();

            }
          }
            
            
        }

        foreach (VM vm in vms.Values)
        {
            //Console.WriteLine("\t" + vm.ToString());
            dt.Rows.Add();
          
            dt.Rows[dt.Rows.Count - 1]["VMName"] = vm.Name;
            dt.Rows[dt.Rows.Count - 1]["State"] = vm.PowerState;
            dt.Rows[dt.Rows.Count - 1]["Vcpus"] = vm.VCpus;
            dt.Rows[dt.Rows.Count - 1]["Max memory"] = vm.MaxMem;
            Label_VMList.Text = Label_VMList.Text + vm.ToString() + "<br>";

            
        }

        GridView1.DataSource = dt;
        GridView1.DataBind();
        //Console.WriteLine("Complete...");

    }
}