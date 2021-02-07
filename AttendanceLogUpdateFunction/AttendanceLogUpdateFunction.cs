using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Data;
using MailServiceDLL;

namespace AttendanceLogUpdateFunction
{
    public static class AttendanceLogUpdateFunction
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("00:01:00")] TimerInfo myTimer, ILogger log)
        {

            string connectionString = "Server=tcp:nemserver2017.database.windows.net,1433;Initial Catalog=NEMDb2;Persist Security Info=False;User ID=adam.new;Password=N3westates1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now} by {GetData()}");
            string data = "";
            string content = "";
            try
            {
                data = "Adam";
                 content = $"<div>C# Timer trigger function executed at: {DateTime.Now} by {data}</div>";
            } catch (Exception ex)
            {
                log.LogInformation($"Error occurred getting info from db {ex.Message}");
            }
            try
            {
                MailServiceDLL.MailService mail = new MailService("Test", content,
                    MailService.TextPartType.html, MailService.MailFromType.Info, 3, 2,
                    connectionString, new System.Collections.Generic.List<string> { "adam.new@newestates.co.uk" });

                mail.Send();

                if (mail.sentStatus == MailService.SentStatus.Sent)
                {

                }
                else
                {
                    log.LogInformation($"Email not sent");
                }

            } catch (Exception ex1)
            {
                log.LogInformation($"Error occurred sending email {ex1.Message}");
            }
        }

        private static string conString { get { return "Server=tcp:nemfunctionserver.database.windows.net,1433;Initial Catalog=FunctionDB;Persist Security Info=False;User ID=adam.new;Password=Function567654;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; } }
                    
                    
                    
                    

        private static string GetData()
        {
            string q = "select * from test";
            dbConn.dbConnection db = new dbConn.dbConnection();

            DataTable dt = db.GetDataTable(q, conString);

            return dt.Rows[0][0].ToString();
            

        }
    }
}
