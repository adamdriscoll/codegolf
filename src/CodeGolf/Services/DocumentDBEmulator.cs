using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGolf.Services
{
    public class DocumentDbEmulator
    {
        private string GetInstallPath()
        {
            return @"C:\Program Files\DocumentDB Emulator\DocumentDB.Emulator.exe";
        }

        public bool IsInstalled()
        {
            return File.Exists(GetInstallPath());
        }

        public bool IsRunning()
        {
            return Process.GetProcessesByName("DocumentDB.Emulator").Any();
        }

        public void Start()
        {
            if (!IsInstalled())
            {
                throw new Exception("DocumentDB Emulator is not install! See: https://docs.microsoft.com/en-us/azure/documentdb/documentdb-nosql-local-emulator");
            }

            if (IsRunning())
            {
                return;
            }

            Process.Start(GetInstallPath());
        }
    }
}
