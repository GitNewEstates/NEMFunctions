using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace CancelAttendanceNotificationFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string ID = req.Query["id"];
            string Value = req.Query["value"];
            string responseMessage = "";
            if (!string.IsNullOrWhiteSpace(Value)) {
                int.TryParse(ID, out int attendanceID);
                bool.TryParse(Value, out bool _value);

                log.LogInformation($"the value of the name property is {attendanceID}");


                if (attendanceID > 0)
                {
                    if (_value)
                    {
                        responseMessage = PreventSendingUpdate(attendanceID);
                    } else
                    {
                        responseMessage = AllowSendingUpdate(attendanceID);
                    }

                } else
                {
                    responseMessage = "No valid Attendance ID was provided";
                }
            } else
            {
                responseMessage = "No valid value was passed.";
            }

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";



            return new OkObjectResult(responseMessage);
        }

        private static string PreventSendingUpdate(int id)
        {
            AttendanceVisits.AttendanceNotificationFunctionData attendanceNotificationFunctionData
                = new AttendanceVisits.AttendanceNotificationFunctionData(id);

            attendanceNotificationFunctionData.HoldCustomerNotification();
            string r = "";
            if (attendanceNotificationFunctionData.HasError)
            {
                r = $"Error Occurred - {attendanceNotificationFunctionData.ErrorMessage}";
            } else
            {
                r = "Attendance Notification successfully held";
            }

            return r;
        }

        private static string AllowSendingUpdate(int id)
        {
            AttendanceVisits.AttendanceNotificationFunctionData attendanceNotificationFunctionData
                = new AttendanceVisits.AttendanceNotificationFunctionData(id);

            attendanceNotificationFunctionData.UnHoldCustomerNotification();
            string r = "";
            if (attendanceNotificationFunctionData.HasError)
            {
                r = $"Error Occurred - {attendanceNotificationFunctionData.ErrorMessage}";
            }
            else
            {
                r = "Attendance Notification successfully unheld";
            }

            return r;
        }
    }
}
