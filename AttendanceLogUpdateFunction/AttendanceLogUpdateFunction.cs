using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Collections.Generic;

namespace AttendanceLogUpdateFunction
{
    public static class AttendanceLogUpdateFunction
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("00:01:00")] TimerInfo myTimer, ILogger log)
        {

            string connectionString = "Server=tcp:nemserver2017.database.windows.net,1433;Initial Catalog=NEMDb2;Persist Security Info=False;User ID=adam.new;Password=N3westates1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            List<int> AttendanceLogs =  AttendanceVisits.CustomerNotificationMethods.GetAttendanceLogs(conString);

            foreach(int AttendanceID in AttendanceLogs)
            {
                log.LogInformation($"send attempt for attendance id {AttendanceID}");
                

                //delete the notification - only if this succeeds send the email - 
                if (AttendanceVisits.CustomerNotificationMethods.Delete(AttendanceID))
                {
                    log.LogInformation($"send attempt");
                    AttendanceVisits.AttendanceNotifications notification
                    = new AttendanceVisits.AttendanceNotifications(AttendanceID, connectionString);

                    notification.SendCUstomerotifications();
                } else
                {
                    log.LogInformation($"Attendance ID no deleted so send attempts aborted - Attendance ID = {AttendanceID}");
                }
              
              
            }

         
        }

        //private static string conString { get 
        //    { return "Server=tcp:nemfunctionserver.database.windows.net,1433;Initial Catalog=FunctionDB;Persist Security Info=False;User ID=adam.new;Password=Function567654;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; } }

        private static string conString
        {
            get
            { return "Server=tcp:nemserver2017.database.windows.net,1433;Initial Catalog=NEMDb2;Persist Security Info=False;User ID=adam.new;Password=N3westates1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; }
        }





     
    }
}
