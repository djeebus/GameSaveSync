using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSync
{
    public class Folder
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public Folder(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        public bool IsEnabled
        {
            get { return Directory.Exists(this.Path); }
        }
    }
}
