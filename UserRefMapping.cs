using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNewService
{
    internal class UserRefMapping
    {

        protected string mapKey;
        protected string mapValue;
        protected string description;


        public UserRefMapping(string mapKey, string mapValue, string description)
        {
            MapKey = mapKey;
            MapValue = mapValue;
            Description = description;
          
        }

        public string MapKey {  get { return mapKey; } set { mapKey = value; } }
        public string MapValue { get { return mapValue; } set { description = value; } }
        public string Description { get { return description; } set { description = value; } }

        public static string GetPath(string connectionString, string UserReference)
        {
            string selectQuery = " SELECT MapValue FROM UserRefMapping Where MapKey = @MapKey ";
            string path = "";

            EventLogs eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), " Before Retrieving " + UserReference + " path from UserrefMapping", "Retrieving " + UserReference + " path from UserrefMapping");
            EventLogs.InsertEventLog(connectionString, eventLog);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@MapKey", UserReference);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            path = reader.GetString(0);
                            eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), " Retrieved " + UserReference + " path from UserrefMapping Succesfully!!!", "Retrieving " + UserReference + " path from UserrefMapping");
                            EventLogs.InsertEventLog(connectionString, eventLog);
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Error while Retrieving " + UserReference + " path from UserrefMapping:\n"+ex.Message, "Retrieving " + UserReference + " path from UserrefMapping");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                    }
                }
            }

            return path;
        }
    }
    
}
