using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.ComponentModel.Design.Serialization;
using System.Collections;

namespace MyNewService
{
    internal static class Program
    {

        static void Main(string[] args)
        {


            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                 new MyNewService(args)
             };
            ServiceBase.Run(ServicesToRun);

        }
       

        }

    }

