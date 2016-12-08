using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.MachineListener
{
    public class Command
    {
        public string identifier { get; set; }
        public string name { get; set; }
        public long size { get; set; }
        public static string getEnumeratedType(commandType type)
        {
            string command;
            if (type == commandType.LIST_APPLICATION)
                command = "LIST_APPLICATION";
            else if (type == commandType.START_APPLICATION)
                command = "START_APPLICATION";
            else if (type == commandType.TRANSFER_FILE)
                command = "TRANSFER_FILE";
            else if (type == commandType.TRANSFER_FILE)
                command = "ECHO";
            else command = "UNKNOWN";

            return command;
        }
        public Command() { }
    }

    public enum commandType
    {
        LIST_APPLICATION,
        START_APPLICATION,
        TRANSFER_FILE,
        ECHO
    }
}
