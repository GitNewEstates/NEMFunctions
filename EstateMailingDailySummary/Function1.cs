using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace EstateMailingDailySummary
{

    public static class EstateCaseDailySummary
    {
        private static string Constring { get { return "Server=tcp:nemserver2017.database.windows.net,1433;Initial Catalog=NEMDb2;Persist Security Info=False;User ID=adam.new;Password=N3westates1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; } }
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 0 8 * * *")]TimerInfo myTimer, ILogger log)
        {
            //function to run every hour - aim to execute once per day at 8 am
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //set datetime for test
            DateTime test = DateTime.Now;

            TimeSpan time = new TimeSpan(7, 59, 0);
            test = test.Date + time;

            log.LogInformation($"C# Test time is: {test}");

            //if time is over 8am
            if (DateTime.Now >= test)
            {
                //test if already ran today
                DateTime LastSentTest = DateTime.Now;

                DateTime Lastsent =  EstateCaseReportDLL.EstateActionEmailReports.GetLastSendDate(Constring);
                log.LogInformation($"Last sent date is {Lastsent}");

                if (Lastsent.Date < LastSentTest.Date)
                {
                    DayOfWeek day = LastSentTest.DayOfWeek;
                    if (day != DayOfWeek.Saturday && day != DayOfWeek.Sunday)
                    {
                        EstateCaseReportDLL.EstateActionEmailReports.UsersDailyTasks(Constring);
                        log.LogInformation($"Daily User Emails Sent");
                        EstateCaseReportDLL.EstateActionEmailReports.ManagerDailySummary(Constring);
                        log.LogInformation($"Daily manager summary sent");
                    }
                }
            }

            
        }

     

    }
}
