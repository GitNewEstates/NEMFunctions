using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using dbConn;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SendGridWebhook
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);
       

            var data = JsonConvert.DeserializeObject<List<Rootobject>>(requestBody);

             foreach(Rootobject root in data)
            {
                RootObjectMethods.Insert(root, log);
            }

            string responseMessage = "done";
            return new OkObjectResult(responseMessage);
        }

        private static bool SendBody(string body)
        {
            MailServiceDLL.MailService mail = new MailServiceDLL.MailService("mail", body, MailServiceDLL.MailService.TextPartType.html, MailServiceDLL.MailService.MailFromType.Info,
                3, 2, "", new System.Collections.Generic.List<string> { "adam.new@newestates.co.uk" });

            mail.Send();

            bool r = false; ;
            if (mail.sentStatus == MailServiceDLL.MailService.SentStatus.Sent)
            {
                r = true;
            }
            return r;
        }




        public class Rootobject
        {
            public string Guid { get; set; }
            public string email { get; set; }

            [JsonProperty("event")]
            public string @event { get; set; }
            public int send_at { get; set; }
            public string sg_event_id { get; set; }
            public string sg_message_id { get; set; }
            public string smtpid { get; set; }
            public int timestamp { get; set; }

            //public string html { get; set; }

            //public string SentUserID { get; set; }
            //public string RecipientUnitID { get; set; }
            //public string RecipientCustomerID { get; set; }
        }

        public static class RootObjectMethods
        {
            private static string constring { get { return "Server=tcp:nemserver2017.database.windows.net,1433;Initial Catalog=NEMDb2;Persist Security Info=False;User ID=adam.new;Password=N3westates1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; } }
            public static void Insert(Rootobject rootobject, ILogger log)
            {
                int eventid = GetEventID(rootobject.@event);
                if(eventid == 0)
                {
                    log.LogError($"Error retrieving event for email {rootobject.Guid}.");
                }

                List<string> c = new List<string>();
                List<string> p = new List<string>();
                List<object> o = new List<object>();

                c.Add("_Guid");
                
                //c.Add("_event");
                c.Add("_timestamp");
                c.Add("eventID");
                

                p.Add("@guid");
              
               // p.Add("@_event");
                p.Add("@timestamp");
                p.Add("@eventID");
            

                o.Add(rootobject.Guid);
              
               // o.Add(rootobject.@event);
               // log.LogInformation($"{rootobject.@event}");
                o.Add(DateTime.Now);
                o.Add(eventid);
              

                dbConnection db = new dbConnection();
                DataTable dt = db.InsertCommandWithReturnID(constring, "core.SendGridEmailLog", c, p, o);

                if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
                {

                    log.LogInformation($"email with Guid {rootobject.Guid} succesfully inserted into db with id of {dt.Rows[0][0].ToString()}");
                } else
                {
                    //error
                    log.LogInformation($"email with Guid {rootobject.Guid} Unsuccesfully inserted into db. {dt.Rows[0][0].ToString()}");
                }

            }

            private static int GetEventID(string _event)
            {
                string q = $"Select id from core.emailevents where eventname ='{_event}'";

                dbConnection db = new dbConnection();
                DataTable dt = db.GetDataTable(constring, q);
                int id = 0;
                if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
                {
                    int.TryParse(dt.Rows[0][0].ToString(), out id);
                    
                } else
                {
                    
                }

                return id;
            }
        }

    }
}
