using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMAX.ApplicationSettings
{
    [Serializable]
    public class Node
    {
        private string _ip;
        private string _id;
        private string _name;
        private string _description;
        private int _port;
        private bool _autoSync = true;

        public string Ip
        {
            get
            {
                return _ip;
            }

            set
            {
                _ip = value;
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
            }
        }

        public bool AutoSync
        {
            get
            {
                return _autoSync;
            }

            set
            {
                _autoSync = value;
            }
        }

        public Node()
        {

        }

        public Node(string _ip, int port, string _id, string _name, string _description)
        {
            this._ip = _ip;
            this._id = _id;
            this._name = _name;
            this._description = _description;
            this._port = port;
        }
    }
}
