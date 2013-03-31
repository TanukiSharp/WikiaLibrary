using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WikiaLibrary.Queries;

namespace WikiaLibrary
{
    public static class Utility
    {
        public static async Task<bool> LoginAsync(string username, string password, CookieContainer cookieContainer, HttpClient client)
        {
            var login = new Login(client, new Param("lgname", username), new Param("lgpassword", password));

            while (true)
            {
                try
                {
                    await login.Run();
                    if (login.IsError)
                        return false;
                }
                catch (HttpRequestException)
                {
                    return false;
                }

                if (login.Result == "Throttled")
                    await Task.Delay(login.WaitTime * 1000);
                else
                    break;
            }

            var token = login.Token;
            if (login.SetCookie != null)
            {
                foreach (var c in login.SetCookie)
                    cookieContainer.Add(c);
            }

            if (login.Result == "NeedToken")
            {
                login = new Login(client,
                    new Param("lgname", username),
                    new Param("lgpassword", password),
                    new Param("lgtoken", login.Token));

                while (true)
                {
                    try
                    {
                        await login.Run();
                        if (login.IsError)
                            return false;
                    }
                    catch (HttpRequestException)
                    {
                        return false;
                    }

                    if (login.Result == "Throttled")
                        await Task.Delay(login.WaitTime * 1000);
                    else
                        break;
                }

                if (login.Result != "Success")
                    return false;

                if (login.SetCookie != null)
                {
                    foreach (var c in login.SetCookie)
                        cookieContainer.Add(c);
                }
            }

            return true;
        }

        public static async Task<string> RequestEditToken(HttpClient client)
        {
            var editToken = new EditToken(client);
            await editToken.Run();
            if (editToken.IsError)
                return null;
            return editToken.Token;
        }

        public static Task DownloadFilesAsync(string localDirectory, IEnumerable<ImageInfo> images)
        {
            return DownloadFilesAsync(localDirectory, images, null);
        }

        public static Task DownloadFilesAsync(string localDirectory, IEnumerable<ImageInfo> images, IProgress<int> progress)
        {
            return DownloadFilesAsync(4, localDirectory, images, progress);
        }

        public static Task DownloadFilesAsync(int concurrentActions, string localDirectory, IEnumerable<ImageInfo> images)
        {
            return DownloadFilesAsync(concurrentActions, localDirectory, images, null);
        }

        public static async Task DownloadFilesAsync(int concurrentActions, string localDirectory, IEnumerable<ImageInfo> images, IProgress<int> progress)
        {
            var tasks = new List<Task>(concurrentActions);

            int count = 0;
            foreach (var imageInfo in images)
            {
                if (tasks.Count == concurrentActions)
                {
                    tasks.Remove(await Task.WhenAny(tasks));
                    count++;
                    if (progress != null)
                        progress.Report(count);
                }

                var normalizedName = NormalizeFilename(imageInfo.Name);

                var localFilename = string.Format("{0}\\{1}", localDirectory, normalizedName);
                var webClient = new WebClient();
                tasks.Add(webClient.DownloadFileTaskAsync(imageInfo.Url, localFilename));
            }

            while (tasks.Count > 0)
            {
                tasks.Remove(await Task.WhenAny(tasks));
                count++;
                if (progress != null)
                    progress.Report(count);
            }
        }

        public static Task UploadFileAsync(string filename, string editToken, Upload upload)
        {
            return UploadFileAsync(filename, editToken, true, upload);
        }

        public static async Task UploadFileAsync(string filename, string editToken, bool ignoreWarnings, Upload upload)
        {
            var data = File.ReadAllBytes(filename);

            filename = Path.GetFileName(filename);

            var httpContent = new MultipartFormDataContent();
            httpContent.Add(new StringContent("upload"), "action");
            httpContent.Add(new StringContent(filename), "filename");
            httpContent.Add(new StringContent(editToken), "token");
            if (ignoreWarnings)
                httpContent.Add(new StringContent("true"), "ignorewarnings");
            httpContent.Add(new StringContent("xml"), "format");
            httpContent.Add(new ByteArrayContent(data), "file", filename);

            await upload.Run(httpContent);
        }

        public static async Task<ImageInfo[]> GetAllImagesAsync(HttpClient client)
        {
            var list = new List<ImageInfo>();

            try
            {
                var query = new GetAllImages(client, list);
                await query.Run();
                if (query.IsError == false)
                {
                    while (query.ContinueFrom != null)
                    {
                        query = new GetAllImages(client, list, query.ContinueFrom);
                        await query.Run();
                        if (query.IsError)
                            break;
                    }
                }

                return list.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static string NormalizeFilename(string filename)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            var needNormalize = false;
            foreach (var c in filename)
            {
                if (invalidChars.Contains(c))
                {
                    needNormalize = true;
                    break;
                }
            }

            if (needNormalize == false)
                return filename;

            var sb = new StringBuilder();

            foreach (var c in filename)
            {
                if (invalidChars.Contains(c))
                    sb.Append('_');
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
