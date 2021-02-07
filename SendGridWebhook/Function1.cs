using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            //bool sent = SendBody(requestBody);
            //if (sent)
            //{
            //    log.LogInformation("Email Sent");
            //}
            //else
            //{
            //    log.LogInformation("Email not sent");
            //}


            var data = JsonConvert.DeserializeObject<Rootobject>(requestBody);

            log.LogInformation(data.GetType().ToString());


            //log.LogInformation($"Custom Guid of this email is {data.Property1[0].Guid}");
            //log.LogInformation($"Email with Guid of {data.Property1[0].Guid} has been {data.Property1[0]._event}");
            //switch (data.Property1[0]._event)
            //{
            //    case "processed":

            //        break;
            //    case "dropped":

            //        break;
            //    case "delivered":

            //        break;
            //    case "deferred":

            //        break;
            //    case "bounce":

            //        break;
            //    case "open":

            //        break;
            //    case "click":

            //        break;
            //    case "spam report":

            //        break;
            //    case "unsubscribe":

            //        break;
            //    case "group unsubscribe":

            //        break;
            //    case "group resubscribe":

            //        break;
            //}
            // group resubscribe

            //string name = "Adam";
            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";
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
            public string _event { get; set; }
            public int send_at { get; set; }
            public string sg_event_id { get; set; }
            public string sg_message_id { get; set; }
            public string smtpid { get; set; }
            public int timestamp { get; set; }
        }


    }
}
