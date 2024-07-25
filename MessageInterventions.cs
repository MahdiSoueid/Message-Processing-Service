using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNewService
{
    internal class MessageInterventions
    {
        public static int lastID = 0;
        protected int IDkey;
        protected int messageID;
        protected string intervText;
        protected string dateTime;

       
        public MessageInterventions (string intervText, string dateTime)
        {   
        this.
           IntervText = intervText;
            DatesTime = dateTime;
        }
        public int IDKey { get { return IDkey; } set { IDkey = value; } }
        public int MessageID { get { return messageID; } set { messageID = value; } }
        public string IntervText { get { return intervText; } set { intervText = value; } }
        public string DatesTime { get { return dateTime; } set { dateTime = value; } }

        public static int InsertMsgInterv(string connectionString, Messages message , MessageInterventions interv , int MessageID)
        {
            int intervID = -1;
            int messageID = MessageID;

            string insertQuery = "INSERT INTO MessagesInterventions (MessageID, IntervText ,DateTime) " +
                     "VALUES (@MessageID, @IntervText ,@DateTime);" +
                     "SELECT SCOPE_IDENTITY();";


            EventLogs eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Before adding MessageIntervention " + interv.DatesTime, "Inserting messageIntervention ");
            EventLogs.InsertEventLog(connectionString, eventLog);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {

                    command.Parameters.AddWithValue("@MessageID", messageID);
                    command.Parameters.AddWithValue("@IntervText ", interv.IntervText);
                    command.Parameters.AddWithValue("@DateTime", interv.DatesTime);
                    
              
                    try
                    {

                        connection.Open();


                            intervID =Convert.ToInt32(command.ExecuteScalar());
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "MessageIntervention " + interv.DatesTime + " Inserted successfully", "Inserting messageIntervention ");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                        Console.WriteLine("Insertion into mesgintervention successful.");
                      
                    }
                    catch (Exception ex)
                    {
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "MessageIntervention " + interv.DatesTime + "  suffered an error while being inserted.Folder is being Moved into error Folder:\n" + ex.Message, "Inserting messageIntervention ");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                        
                    }
                }
            }
            return intervID;
            

        }
    }
}
