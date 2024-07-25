using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MyNewService
{
    internal class MesgFiles
    {
        public static int lastID =0;
        protected int IDkey;
        protected string fileName;
        protected string dateTime;
        protected string fileContent;
        protected string insertionDateTime;


        public MesgFiles(string fileName,string dateTime,string fileContent,string insertionDateTime){
            FileName = fileName;
            DateTimes = dateTime;
            FileContent = fileContent;
            InsertionDateTime = insertionDateTime;
               }
        public int IDKey { get { return IDkey; } set { IDkey = value; } }
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public string DateTimes
        { get { return dateTime; }
          set { dateTime = value; } 
        }
        public string FileContent
        {
           get { return fileContent; }
           set { fileContent = value; }
        }
        public string InsertionDateTime
        { get { return insertionDateTime; } 
          set { insertionDateTime = value; } 
        }    

        public static int InsertMsgFiles( string connectionString,MesgFiles mesgfile)
        {
            string insertQuery = "INSERT INTO MesgFiles (FileName, DateTime, FileContent, InsertionDateTime) " +
                      "VALUES (@FileName, @DateTime, @FileContent, @InsertionDateTime);" +
                      " SELECT SCOPE_IDENTITY();";

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            EventLogs eventLog = new EventLogs( date, "Before adding File " + mesgfile.FileName, "Inserting File ");
            EventLogs.InsertEventLog(connectionString, eventLog);
            int fileId = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {

                    command.Parameters.AddWithValue("@FileName", mesgfile.FileName);
                    command.Parameters.AddWithValue("@DateTime", mesgfile.DateTimes);
                    command.Parameters.AddWithValue("@FileContent", mesgfile.FileContent);
                    command.Parameters.AddWithValue("@InsertionDateTime", mesgfile.InsertionDateTime);
                    try
                    {

                        connection.Open();


                       
                            fileId = Convert.ToInt32(command.ExecuteScalar());


                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "File " +fileId + " " + mesgfile.FileName + " Inserted Succesfully!!!!!", "Inserting File ");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                        Console.WriteLine("Insertion into MesgFiles successful.");
                        
                       
                    }
                    catch (Exception ex)
                    {
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "File" + mesgfile.FileName + " suffered an error while being inserted.Folder is being Moved into error Folder:\n" + ex.Message, "Inserting File ");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                        
                    }
                }
            }
            return fileId;


        }
        
    }
}
