using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TopVisor.Core.Model.DTO;
using TopVisor.Core.Utils;

namespace TopVisor.Core.Services
{
    public class APIService
    {
        public static async Task<List<ProjectDTO>> GetProjects()
        {
            var result = await APICall<List<ProjectDTO>> (APIOperations.Get, APIModules.Projects);
            return result;
        }

        public static async Task<EditResultDTO> AddProject(String name, String site)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Add, APIModules.Projects,null,null,new Dictionary<string, string>{ {"site",site}, {"name", name} });
            return result;
        }
        public static async Task<EditResultDTO> EditProject(UInt32 id, String name, String site)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Edit, APIModules.Projects,null,null,new Dictionary<string, string>{ {"id", id.ToString()}, {"site", site}, {"name", name} });
            return result;
        }
        public static async Task<EditResultDTO> DeleteProject(UInt32 id)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Delete, APIModules.Projects,null,null,new Dictionary<string, string>{ {"id", id.ToString()}});
            return result;
        }

        public static async Task<List<PhraseDTO>> GetPhrases(UInt32 projectId)
        {
            var result = await APICall<List<PhraseDTO>>(APIOperations.Get, APIModules.Keywords, null, null, new Dictionary<string, string> { { "project_id", projectId.ToString() } });
            return result;
        }
        public static async Task<EditResultDTO> AddPhrase(UInt32 projectId, String phrase)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Add, APIModules.Keywords, null, null, new Dictionary<string, string> { { "phrase", phrase }, { "project_id", projectId.ToString() } });
            return result;
        }

        public static async Task<EditResultDTO> DeletePhrase(UInt32 projectId, UInt32 phraseId)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Delete, APIModules.Keywords, null, null, new Dictionary<string, string> { { "id", phraseId.ToString()}, { "project_id", projectId.ToString() } });
            return result;
        }
        public static async Task<EditResultDTO> EditPhrase(UInt32 projectId, UInt32 phraseId, String phrase)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Edit, APIModules.Keywords, null, null, new Dictionary<string, string> { { "phrase", phrase }, { "id", phraseId.ToString()}, { "project_id", projectId.ToString() } });
            return result;
        }

        public static async Task<T> APICall<T>(APIOperations operation, APIModules module, String function = null, String filter = null, Dictionary<String, String> post = null)
            where T : new()
        {
            var url = BuildUrl(operation, module, function, filter, post);
            var responce = await GetServerResponce(url);

            String errorMessage;
            if (CheckResponceForError(responce, out errorMessage))
                throw new Exception(errorMessage);
            else
                return JsonHelper.Deserialize<T>(responce);
        }

        private const String APIKey = "bd5253f3d350ec53a629";
        private const String MainUrlTemplate = @"https://api.topvisor.ru/?api_key={0}&oper={1}&module={2}";
        private const String FuncUrlTemplate = @"&func={0}";
        private const String FilterUrlTemplate = @"&filter={0}";
        private const String PostUrlTemplate = @"&post[{0}]={1}";
        private static String BuildUrl(APIOperations operation, APIModules module, String function, String filter, Dictionary<String,String> post)
        {
            var oper = String.Empty;
            switch (operation)
            {
                case APIOperations.Get:
                    oper = "get";
                    break;
                case APIOperations.Add:
                    oper = "add";
                    break;
                case APIOperations.Edit:
                    oper = "edit";
                    break;
                case APIOperations.Delete:
                    oper = "del";
                    break;
            }

            var mod = String.Empty;
            switch (module)
            {
                case APIModules.Projects:
                    mod = "mod_projects";
                    break;
                case APIModules.Keywords:
                    mod = "mod_keywords";
                    break;
            }

            var url = String.Format(MainUrlTemplate, APIKey, oper, mod);
            if (!String.IsNullOrEmpty(function))
                url += String.Format(FuncUrlTemplate, WebUtility.UrlEncode(function));
            if (!String.IsNullOrEmpty(filter))
                url += String.Format(FilterUrlTemplate, WebUtility.UrlEncode(filter));
            if (post!=null && post.Count>0)
                url = post.Keys.Aggregate(url, (current, key) => current + String.Format(PostUrlTemplate, key, WebUtility.UrlEncode(post[key])));

            return url;
        }

        private static async Task<String> GetServerResponce(String url)
        {
            var httpRequest = WebRequest.CreateHttp(new Uri(url, UriKind.Absolute));
            httpRequest.Method = "Get";

            var webResponceTask = new TaskFactory<WebResponse>().FromAsync(httpRequest.BeginGetResponse, httpRequest.EndGetResponse, httpRequest);
            await webResponceTask;

            using (var webResponce = webResponceTask.Result)
            {
                using (var responceStream = webResponce.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responceStream))
                    {
                        var readToEndTask = streamReader.ReadToEndAsync();
                        await readToEndTask;

                        var responce = readToEndTask.Result;
                        return responce;
                    }
                }
            }
        }

        private static Boolean CheckResponceForError(String responce, out String errorMessage)
        {
            EditResultDTO responceObject;
            if (JsonHelper.TryDeserialize(responce, out responceObject) && responceObject!=null)
            {
                errorMessage = responceObject.message;
                return responceObject.error;
            }
            else
            {
                errorMessage = null;
                return false;
            }
        }
    }
}