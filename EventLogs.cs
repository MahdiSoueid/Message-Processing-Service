using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization.Configuration;

namespace MyNewService
{
    internal class EventLogs
    {
        protected string datetime;
        protected string description;
        protected string serviceName;
        
        public EventLogs(string datetime, string description , string serviceName)
        {
            DateTimes = datetime;
            Description = description;
            ServiceName = serviceName;

        }

        public string DateTimes { get;  set; }
        public string Description { get; set; }
        public string ServiceName { get; set; }

        public static void InsertEventLog(string connectionString, EventLogs eventLog)
        {
            string insertQuery = "INSERT INTO EventLogs (DateTime, Description, ServiceName) " +
                      "VALUES (@DateTime, @Description, @ServiceName);" ;


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                      
                        command.Parameters.AddWithValue("@DateTime", eventLog.DateTimes);
                        command.Parameters.AddWithValue("@Description", eventLog.Description);
                        command.Parameters.AddWithValue("@ServiceName", eventLog.ServiceName);

                        
                        int rowsAffected = command.ExecuteNonQuery();

                       
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error inserting into EventLogs: {ex.Message}");
            }
        }
    }
 }








   
        
    

