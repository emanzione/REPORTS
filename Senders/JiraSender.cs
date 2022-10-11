using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MHLab.Reports.Reports.Senders
{

    public class JiraSender : SenderBase
    {
        public string IssueUrl;
        public string ImageURL;
        public string JiraEmail;
        public string JiraApiToken;
        public string JiraProjectKey;
        public List<JiraIssue> IssueTypes;

        [Multiline] public string JiraJson;

        public override string SenderName => "JiraSender";
        public override async Task SendReport(Report report)
        {
            IsSuccess   = false;
            IsCompleted = false;

            using var httpClient = new HttpClient();

            var byteArray   = Encoding.ASCII.GetBytes($"{JiraEmail}:{JiraApiToken}");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            
            int issueType    = TryParseJiraIssueType(report.Type);
            JToken json      = JsonConvert.DeserializeObject<JToken>(JiraJson);
            json["fields"]["summary"] = report.Email;
            json["fields"]["issuetype"]["id"] = issueType;
            json["fields"]["project"]["key"] = JiraProjectKey;
            json["fields"]["description"]["content"][0]["content"][0]["text"] = report.Message;
            string data      = JsonConvert.SerializeObject(json);

            var issueContent = new StringContent(data, Encoding.Default, "application/json");

            try
            {
                var response = await httpClient.PostAsync(IssueUrl, issueContent);

                IsSuccess    = response.IsSuccessStatusCode;

                if (IsSuccess == true && report.Attachments.Count > 0)
                {
                    JToken issueResponse = JsonConvert.DeserializeObject<JToken>(response.Content.ReadAsStringAsync().Result);
                    string issueID       = issueResponse["id"].ToString();
                    ImageURL             = string.Format(ImageURL, issueID);
                        
                    response.Dispose();

                    using var imageHttpClient  = new HttpClient();  
                    using var multipartContent = new MultipartFormDataContent();
                    
                    imageHttpClient.DefaultRequestHeaders.Add("X-Atlassian-Token", "no-check");
                    imageHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    foreach (var attachment in report.Attachments)
                    {
                        var streamContent = new StreamContent(new MemoryStream(attachment.Content));
                        streamContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("image/png");
                        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name="\"file\"",
                            FileName = attachment.Name
                        };
                        
                        multipartContent.Add(streamContent, attachment.Name, attachment.Name);
                    }
                    
                    response = await imageHttpClient.PostAsync(ImageURL, multipartContent);

                    IsSuccess = response.IsSuccessStatusCode;
                }
                
                if (IsSuccess == false)
                {
                    Error = GetDecoratedErrorMessage(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {
                IsSuccess   = false;
                Error       = GetDecoratedErrorMessage(e.Message);
            }
            
            IsCompleted = true;

        }

        private int TryParseJiraIssueType(string reportType)
        {
            int ID = IssueTypes.Find(ji => ji.Name == reportType).ID;
            
            if (ID == 0)
                ID = IssueTypes[0].ID;
            
            return ID;
        }
        
        [System.Serializable]
        public struct JiraIssue
        {
            public string Name;
            public int ID;
        }
    }
}
