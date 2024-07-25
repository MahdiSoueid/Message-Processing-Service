using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNewService
{
    internal class Configuration
    {
        protected string path;
        protected string description;

        public Configuration(string path , string description)
        {
            Path = path;
            Description = description;
        }
        public string Path { get { return path; } set { path = value; } }
        public string Description { get { return description; } set { description = value; } }


        public static ArrayList getFolders(string connectionString)
        {
            string selectQuery = @"
            SELECT Path
            FROM Configuration
            Where Description != 'Success Folder Path '  AND  Description != 'Error Folder Path ' ;";

            EventLogs eventLog;
           

            ArrayList Folders = new ArrayList();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                               
                                string FolderPath = reader.GetString(0);
                                eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Retrieving the FOLDER Paths from Configuration" + FolderPath, "Retrieving Folders from Configuration");
                                Folders.Add(FolderPath);
                               

                            }
                            eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Folder Paths retrieved successfully from Configuration" , "Retrieving Folders from Configuration");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving messages: " + ex.Message);
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Error while retrieving Folder Paths from Configuration", "Retrieving Folders from Configuration");
                    }
                }
            }
            return Folders;


        }
        public static string getErrorFolder(string connectionString)
        {
            string selectQuery = @"
            SELECT Path
            FROM Configuration
            Where Description = 'Error Folder Path ';";
            string Folders = @"";
            EventLogs eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Retrieving the FOLDER Error Path from Configuration" , "Retrieving error Folder from Configuration");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                               Folders= reader.GetString(0);
                                


                            }
                            eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Folder error Path retrieved successfully from Configuration", "Retrieving error Folder from Configuration");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving messages: " + ex.Message);
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Error while retrieving Folder error Path from Configuration", "Retrieving error Folder from Configuration");
                    }
                }
            }
            return Folders;


        }
        public static string getSuccessFolder(string connectionString)
        {
            string selectQuery = @"
            SELECT Path
            FROM Configuration
            Where Description = 'Success Folder Path ';";
            string Folders = @"";
            EventLogs eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Retrieving the FOLDER Success Path from Configuration", "Retrieving Success Folder from Configuration");
            EventLogs.InsertEventLog(connectionString, eventLog);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                Folders = reader.GetString(0);


                            }
                            eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Folder Success Path retrieved successfully from Configuration", "Retrieving Success Folder from Configuration");
                            EventLogs.InsertEventLog(connectionString, eventLog);
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Error while retrieving Folder Success Path from Configuration:\n" + ex.Message, "Retrieving Success Folder from Configuration");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                    }
                }
            }
            return Folders;


        }


    }


}
