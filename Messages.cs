using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MyNewService
{
    internal class Messages
    {
        public static int lastID = 0;
        protected int IDkey;
        protected int fileID;
        protected string messageContent;
        protected string insertionDateTime;
        protected string ownBic;
        protected string corrBic ;
        protected string messageType;
        protected string subFormat;
        protected string userReference;
        protected string sLA;
        protected string uETR;
        protected string reference;
        protected string orderingCust;
        protected string beneficiaryCust;
        protected string detailsOfCharges;
        protected string block1;
        protected string block2;
        protected string block3;
        protected string block4;
        protected string mesgStatus;


        public Messages(string messageContent, string insertionDateTime)
        {   

            MessageContent = messageContent;
            InsertionDateTime = insertionDateTime;

            string[] parts = MessageContent.Split('}');
            string[] blocks = new string[4];

            int count = 1;
            foreach (string part in parts)
            {
                if (count != 3)
                    blocks[count - 1] = part + '}';
                else if (!part.Contains("{4:") && count == 3)
                {

                    blocks[2] = blocks[2] + part + '}';
                    continue;

                }
                count++;

            }
            blocks[3] = parts[parts.Length - 2] + '}';
            MesgStatus = "MesgInserted";

            Block1 = blocks[0];
            Block2 = blocks[1];
            Block3 = blocks[2];
            Block4 = blocks[3];
            int endindex = 0;

            //OwnBic
            int startindex = Block1.IndexOf("1:F01") + 5;
            OwnBic = Block1.Substring(startindex, 12);
           

            //SubFormat
            if (Block2[3] == 'I')
                SubFormat = "INPUT";
            else
                SubFormat = "OUTPUT";

            //CorrBic
            if (SubFormat == "INPUT")
            {
                startindex = 3 + 4;
                CorrBic = Block2.Substring(startindex, 12);
                
            }
            else
            {
                startindex = 3 + 4 + 10;
                CorrBic = Block2.Substring(startindex, 12);
                


            }
            //mesgtype
            MessageType = Block2.Substring(4, 3);
            

            string[] block4divided = Block4.Split(new string[] { "\r\n" }, StringSplitOptions.None);


            //reference
            startindex = block4divided[1].IndexOf(":20:") + 4;
            Reference = block4divided[1].Substring(startindex, block4divided[1].Length - startindex);
           

            //OrderingCUST
            if (SubFormat == "INPUT")
            {
                startindex = Block4.IndexOf(":50K:") + 5;
                endindex = Block4.IndexOf(":57A:");
                OrderingCust = Block4.Substring(startindex, endindex - startindex);
                

            }
            else
            {
                startindex = Block4.IndexOf(":50K:") + 5;
                endindex = Block4.IndexOf(":59:");
                OrderingCust = Block4.Substring(startindex, endindex - startindex);
               

            }

            //BeneficiaryCust

            startindex = Block4.IndexOf(":59:") + 4;
            endindex = Block4.IndexOf(":70:");
            BeneficiaryCust = Block4.Substring(startindex, endindex - startindex);
           




            //DetailsOfCharges
            startindex = Block4.IndexOf(":71A:") + 5;
            DetailsOfCharges = Block4.Substring(startindex, 3);
            

            //userreference
            startindex = Block3.IndexOf("{108:") + 5;
            endindex = Block3.IndexOf('}');
            if (startindex != 4)
                UserReference = Block3.Substring(startindex, endindex - startindex);
            else
                UserReference = "DEFAULT";
           


            //SLA
            startindex = Block3.IndexOf("{111:") + 5;

            if (startindex != 4)
                SLA = Block3.Substring(startindex, 3);
            else
                SLA = "000";
           

            //UETR
            startindex = Block3.IndexOf("{121:") + 5;
            endindex = Block3.IndexOf("}}");

            UETR = Block3.Substring(startindex, endindex - startindex);

            


        }
        public int IDKey { get { return IDkey; } set { IDkey = value; } }
        public int FileID { get { return fileID; }set { fileID = value; } }
        public string MessageContent {  get { return messageContent; } set { messageContent = value; } }
        public string InsertionDateTime {  get { return insertionDateTime; } set { insertionDateTime = value; } }
        public string OwnBic
        {
            get { return ownBic; }
            set { ownBic = value; }
        } 
        public string CorrBic { get { return corrBic; } set { corrBic = value; } }
        public string MessageType {  get { return messageType; } set {messageType = value; } }
        public string SubFormat { get { return subFormat; } set { subFormat = value; } }
        public string UserReference { get { return userReference; } set { userReference = value; } }
        public string SLA {  get { return sLA; } set { sLA = value; } }
        public string UETR { get { return uETR; } set { uETR = value; } }
        public string BeneficiaryCust { get { return beneficiaryCust; } set { beneficiaryCust = value; } }

        public string OrderingCust { get { return orderingCust; } set { orderingCust = value; } }
        public string Reference {  get { return reference; } set { reference = value; } }

        public string DetailsOfCharges { get { return detailsOfCharges; } set { detailsOfCharges = value; } }
        public string Block1 { get { return block1; } set { block1 = value; } }
        public string Block2 { get { return block2; } set { block2 = value; } }
        public string Block3 { get { return block3; } set { block3 = value; } }
        public string Block4 { get { return block4; } set { block4 = value; } }
        public string MesgStatus { get { return mesgStatus; } set { mesgStatus = value; } }


        public static int InsertMessage(string connectionString, Messages message , MesgFiles file , int FileID)

        {
            int fileID = FileID;
            int messageID = -1; 
            string insertQuery = @"
        INSERT INTO Messages (
            FileID, 
            MessageContent, 
            InsertionDateTime, 
            OwnBic, 
            CorrBic, 
            MessageType, 
            SubFormat, 
            UserReference, 
            SLA, 
            UETR, 
            Reference, 
            OrderingCust, 
            BeneficiaryCust, 
            DetailsOfCharges, 
            Block1, 
            Block2, 
            Block3, 
            Block4, 
            MesgStatus
        ) 
        VALUES (
            @FileID, 
            @MessageContent, 
            @InsertionDateTime, 
            @OwnBic, 
            @CorrBic, 
            @MessageType, 
            @SubFormat, 
            @UserReference, 
            @SLA, 
            @UETR, 
            @Reference, 
            @OrderingCust, 
            @BeneficiaryCust, 
            @DetailsOfCharges, 
            @Block1, 
            @Block2, 
            @Block3, 
            @Block4, 
            @MsgStatus
        );
 SELECT SCOPE_IDENTITY();"
            ;

            EventLogs eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Before adding Message " + message.InsertionDateTime, "Inserting message ");
            EventLogs.InsertEventLog(connectionString, eventLog);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@FileID", fileID); 
                    command.Parameters.AddWithValue("@MessageContent", message.MessageContent);
                    command.Parameters.AddWithValue("@InsertionDateTime", message.InsertionDateTime);
                    command.Parameters.AddWithValue("@OwnBic", message.OwnBic);
                    command.Parameters.AddWithValue("@CorrBic", message.CorrBic);
                    command.Parameters.AddWithValue("@MessageType", message.MessageType);
                    command.Parameters.AddWithValue("@SubFormat",message.SubFormat);
                    command.Parameters.AddWithValue("@UserReference", message.UserReference);
                    command.Parameters.AddWithValue("@SLA", message.SLA);
                    command.Parameters.AddWithValue("@UETR", message.UETR);
                    command.Parameters.AddWithValue("@Reference", message.Reference);
                    command.Parameters.AddWithValue("@OrderingCust", message.OrderingCust);
                    command.Parameters.AddWithValue("@BeneficiaryCust", message.BeneficiaryCust);
                    command.Parameters.AddWithValue("@DetailsOfCharges", message.DetailsOfCharges);
                    command.Parameters.AddWithValue("@Block1", message.Block1);
                    command.Parameters.AddWithValue("@Block2", message.Block2);
                    command.Parameters.AddWithValue("@Block3", message.Block3);
                    command.Parameters.AddWithValue("@Block4", message.Block4);
                    command.Parameters.AddWithValue("@MsgStatus", message.MesgStatus);
                    try
                    {

                        connection.Open();


                        messageID = Convert.ToInt32(command.ExecuteScalar());
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message " + message.IDKey + " " + message.InsertionDateTime + " in " + file.FileName + " Inserted Succesfully!!!!!", "Inserting Message ");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                        Console.WriteLine("Insertion into Messages successful.");
                      
                    }
                    catch (Exception ex)
                    {
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message " + message.IDKey + " " + message.InsertionDateTime + " " + file.FileName + " suffered an error while being inserted.Folder is being Moved into error Folder:\n" + ex.Message, "Inserting Message ");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                        
                    }
                }
            }
            return messageID;


        }
       

        public static void UpdateMesgStatus(string connectionString, int MessageID)
        {


            string selectQuery = @"
                 UPDATE Messages
                    SET MesgStatus = @NewMesgStatus
                    WHERE IDKey = @IDKey;";
            EventLogs eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Before Updating message status for Message " + MessageID, "Updating and processing message ");
            EventLogs.InsertEventLog(connectionString, eventLog);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@NewMesgStatus", "MesgProcessed");
                    command.Parameters.AddWithValue("@IDKey", MessageID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), " Updated message status for Message " + MessageID +"Successfully !!!!", "Updating and processing message ");
                            EventLogs.InsertEventLog(connectionString, eventLog);
                            Console.WriteLine("Message updated.");
                        }
                        else
                        {
                            eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message not found", "Updating and processing message ");
                            EventLogs.InsertEventLog(connectionString, eventLog);
                            Console.WriteLine("Message not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), " Error while Updating message status for Message " + MessageID +" :\n" + ex.Message, "Updating and processing message ");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                        
                    }
                }
            }
         
        }
        public static void processMessages(string connectionString )
        {
            string selectQuery = @"
            SELECT IDKey, MessageContent,UserReference,InsertionDateTime
            FROM Messages
            WHERE MesgStatus = 'MesgInserted';";
            EventLogs eventLog;


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
                                int idKey = reader.GetInt32(0);
                                string messageContent = reader.GetString(1);
                                string userReference = reader.GetString(2);
                                string time = reader.GetString(3);

                                eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Before Proccessing Message " + idKey, "Processing message ");
                                EventLogs.InsertEventLog(connectionString, eventLog);

                                string path = UserRefMapping.GetPath(connectionString , userReference);

                                Console.WriteLine(time);
                                string fileName = time+".txt";
                                string filePath = path +"\\"+ fileName;
                                Console.WriteLine(filePath);
                                string content = messageContent;


                                eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Moving messsage " + idKey + " to following userreference " + userReference, "Processing message ");
                                EventLogs.InsertEventLog(connectionString, eventLog);
                                using (StreamWriter writer = new StreamWriter(filePath))
                                {
                                    
                                    writer.Write(content);
                                }
                                eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message " + idKey + " moved successfulyy to the following userreference " + userReference, "Processing message ");
                                EventLogs.InsertEventLog(connectionString, eventLog);
                                UpdateMesgStatus(connectionString, idKey);
                                eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Proccessed Message Successfully " + idKey, "Processing message ");
                                EventLogs.InsertEventLog(connectionString, eventLog);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Encountered an error while proccessing Message " +ex.Message , "Processing message ");
                        EventLogs.InsertEventLog(connectionString, eventLog);
                       
                    }
                }
            }

            
        }


    }


}

