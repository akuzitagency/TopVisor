using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TopVisor.Core.Model.DTO;
using TopVisor.Core.Utils;

namespace TopVisor.Core.Services
{
    public static class APIService
    {
        private const UInt32 DefaultPageSize = 100;
        public static async Task<List<ProjectDTO>> GetProjects(Dictionary<String,Object> filter =null)
        {
            var result = new List<ProjectDTO>();

            var pageNumber = 1;
            List<ProjectDTO> page;
            do
            {
                page = await APICall<List<ProjectDTO>>(APIOperations.Get,
                    APIModules.Projects,
                    null,
                    filter,
                    new Dictionary<string, string>
                    {
                        {"rows", DefaultPageSize.ToString(CultureInfo.InvariantCulture)},
                        {"page", pageNumber++.ToString(CultureInfo.InvariantCulture)}
                    });
                result.AddRange(page);
            } while (page.Count >= DefaultPageSize);

            return result;
        }

        public static async Task<ProjectDTO> GetProject(UInt32 projectId)
        {
            var result = await APICall<List<ProjectDTO>>(APIOperations.Get, APIModules.Projects, null, new Dictionary<String, Object> { {"id", projectId.ToString(CultureInfo.InvariantCulture) }});
            return result[0];
        }

        public static async Task<EditResultDTO> AddProject(String name, String site)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Add, APIModules.Projects,null,null,new Dictionary<string, string>{ {"site",site}, {"name", name} });
            if (result?.result == null)
            {
                var errorMessage = "Не удалось добавить проект.";
                LogService.Log("API error: " + errorMessage, LogCategories.APIErrorCategories);
                throw new Exception(errorMessage);
            }
            return result;
        }
        public static async Task<EditResultDTO> EditProject(UInt32 id, String name, String site)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Edit, APIModules.Projects,null,null,new Dictionary<string, string>{ {"id", id.ToString()}, {"site", site}, {"name", name} });
            if((result?.result??0)==0)
            {
                var errorMessage = "Не удалось отредактировать проект.";
                LogService.Log("API error: " + errorMessage, LogCategories.APIErrorCategories);
                throw new Exception(errorMessage);
            }
            return result;
        }
        public static async Task<EditResultDTO> DeleteProject(UInt32 id)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Delete, APIModules.Projects,null,null,new Dictionary<string, string>{ {"id", id.ToString()}});
            if ((result?.result ?? 0) == 0)
            {
                var errorMessage = "Не удалось удалить проект.";
                LogService.Log("API error: " + errorMessage, LogCategories.APIErrorCategories);
                throw new Exception(errorMessage);
            }
            return result;
        }

        public static async Task<List<PhraseDTO>> GetPhrases(UInt32 projectId)
        {
            var result = new List<PhraseDTO>();

            var pageNumber = 1;
            List<PhraseDTO> page;
            do
            {
                page = await APICall<List<PhraseDTO>>(APIOperations.Get,
                    APIModules.Keywords,
                    null,
                    null,
                    new Dictionary<string, string>
                    {
                        {"project_id", projectId.ToString(CultureInfo.InvariantCulture)},
                        {"rows", DefaultPageSize.ToString(CultureInfo.InvariantCulture)},
                        {"page", pageNumber++.ToString(CultureInfo.InvariantCulture)}
                    });
                result.AddRange(page);
            } while (page.Count >= DefaultPageSize);

            return result;
        }
        public static async Task<EditResultDTO> AddPhrase(UInt32 projectId, String phrase)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Add, APIModules.Keywords, null, null, new Dictionary<string, string> { { "phrase", phrase }, { "project_id", projectId.ToString() } });
            if (result?.result == null)
            {
                var errorMessage = "Не удалось добавить фразу.";
                LogService.Log("API error: " + errorMessage, LogCategories.APIErrorCategories);
                throw new Exception(errorMessage);
            }
            return result;
        }

        public static async Task<EditResultDTO> DeletePhrase(UInt32 projectId, UInt32 phraseId)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Delete, APIModules.Keywords, null, null, new Dictionary<string, string> { { "id", phraseId.ToString()}, { "project_id", projectId.ToString() } });
            if ((result?.result ?? 0) == 0)
            {
                var errorMessage = "Не удалось удалить фразу.";
                LogService.Log("API error: " + errorMessage, LogCategories.APIErrorCategories);
                throw new Exception(errorMessage);
            }
            return result;
        }
        public static async Task<EditResultDTO> EditPhrase(UInt32 projectId, UInt32 phraseId, String phrase)
        {
            var result = await APICall<EditResultDTO>(APIOperations.Edit, APIModules.Keywords, null, null, new Dictionary<string, string> { { "phrase", phrase }, { "id", phraseId.ToString()}, { "project_id", projectId.ToString() } });
            if ((result?.result ?? 0) == 0)
            {
                var errorMessage = "Не удалось отредактировать фразу.";
                LogService.Log("API error: " + errorMessage, LogCategories.APIErrorCategories);
                throw new Exception(errorMessage);
            }
            return result;
        }

        public static async Task<T> APICall<T>(APIOperations operation, APIModules module, String function = null, Dictionary<String, Object> filter = null, Dictionary<String, String> post = null)
            where T : new()
        {
            var url = BuildUrl(operation, module, function, filter, post);

            LogService.Log("API call:     " + url, LogCategories.APICategories);
            var responce = await GetServerResponce(url);
            LogService.Log("API responce: " + responce, LogCategories.APICategories);

            String errorMessage;
            if (CheckResponceForError(responce, out errorMessage))
            {
                LogService.Log("API error: " + errorMessage, LogCategories.APIErrorCategories);
                throw new Exception(errorMessage);
            }
            else
                return JsonHelper.Deserialize<T>(responce);
        }

        private const String APIKey = "bd5253f3d350ec53a629";
        private const String MainUrlTemplate = @"https://api.topvisor.ru/?api_key={0}&oper={1}&module={2}";
        private const String FuncUrlTemplate = @"&func={0}";
        private const String FilterUrlTemplate = @"&filter={0}";
        private const String PostUrlTemplate = @"&post[{0}]={1}";
        private static String BuildUrl(APIOperations operation, APIModules module, String function, Dictionary<String, Object> filter, Dictionary<String,String> post)
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

            if (filter != null && filter.Count > 0)
                url += String.Format(FilterUrlTemplate,BuildFilterString(filter));

            if (post!=null && post.Count>0)
                url = post.Keys.Aggregate(url, (current, key) => current + String.Format(PostUrlTemplate, key, WebUtility.UrlEncode(post[key])));

            return url;
        }

        private static string BuildFilterString(Dictionary<string, object> filter)
        {
            var filterString = String.Empty;
            if (filter != null && filter.Count > 0)
            {
                var pairFilterStrings = new List<String>();
                foreach (var pair in filter)
                {
                    var pairFilterString = "\"" + pair.Key + "\":";
                    var val = pair.Value;
                    if (pair.Value == null)
                        pairFilterString += "null,";
                    else
                    {
                        var stringVal = val as String;
                        if (stringVal != null)
                            pairFilterString += "\"" + WebUtility.UrlEncode(stringVal) + "\"";
                        else
                            pairFilterString += WebUtility.UrlEncode(val.ToString());
                    }
                    pairFilterStrings.Add(pairFilterString);
                }
                filterString = "{" + pairFilterStrings.Aggregate((a,b)=>a+", " + b) +"}";
            }
            return filterString;
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
            Object validJson;
            if (JsonHelper.TryDeserialize(responce, out responceObject) && responceObject!=null)
            {
                errorMessage = responceObject.message;
                return responceObject.error;
            }
            else if (JsonHelper.TryDeserialize(responce, out validJson))
            {
                errorMessage = null;
                return false;
            }
            else
            {
                // вообще не json пришёл, ошибку отплёвываем как есть
                errorMessage = responce;
                return true;
            }
        }
    }
}