using System.Text;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using PowerCloud.ViewModels;

namespace PowerCloud.NasHttp
{
    public class NE201FileManager
    {
        public static Dictionary<AccountViewModel, NE201FileManager> UsersList = new Dictionary<AccountViewModel, NE201FileManager>();
        public HttpClient SharedHttp;
        public HttpClientHandler SharedHttpHandle;
        AccountViewModel user;

        public static NE201FileManager FileManagerFactory(AccountViewModel user)
        {
            if (UsersList != null)
            {
                foreach (KeyValuePair<AccountViewModel, NE201FileManager> item in UsersList)
                {
                    if (user.UserName == item.Key.UserName || user.UserNasLink == item.Key.UserNasLink)
                        return item.Value;
                }
            }

            NE201FileManager result = new NE201FileManager(user);

            result.ResetHttpClient();

            UsersList.Add(user, result);
            return result;
        }

        public void ResetHttpClient()
        {
            if (SharedHttp != null)
            {
                SharedHttp = null;
                SharedHttpHandle = null;
            }

            SharedHttpHandle = new HttpClientHandler();
            SharedHttpHandle.ServerCertificateCustomValidationCallback = NE201Login.ServerCertificateCustomValidation;
            SharedHttp = new HttpClient(SharedHttpHandle);
        }

        public NE201FileManager(AccountViewModel u)
        {
            user = u;
        }


        public string ResultMessage;
        private byte[] ByteResultMessage;
        public async Task<bool> NE201CopyFile(string fullFile, string fullTarget)
        {
            if (fullFile == null)
            {
                ResultMessage = "No source file available";
                return false;
            }
            if (string.IsNullOrEmpty(fullTarget))
            {
                ResultMessage = "No target fullpath name available";
                return false;
            }

            if (string.IsNullOrEmpty(user.AccessToken) || user.AccessToken == "unknown")
            {
                if (!await NE201Login.Login(user.UserNasLink, user.UserName, user.UserSecret))
                {
                    ResultMessage = "Login Failed";
                    return false;
                }
            }


            MultipartFormDataContent content = new MultipartFormDataContent();

            string targetPath = Path.GetDirectoryName(fullTarget);
            if (!targetPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                targetPath += Path.DirectorySeparatorChar;

            string postBody = $"{targetPath}"; //serachstr=*&

            content.Add(new StringContent(postBody), "DestPath");
            Stream stream = await NE201DownloadStream(fullFile);
            StreamContent streamContent = new StreamContent(stream);
            content.Add(streamContent, "file", Path.GetFileName(fullTarget));

            string result = await SendToNE201(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/UploadFile");
            stream.Close();

            return string.IsNullOrEmpty(result);
        }

        public async Task<bool> NE201FileUpload(FileResult file, string targetPath, string newName = null)
        {
            if (file == null)
            {
                ResultMessage = "No source file available";
                return false;
            }
            if (string.IsNullOrEmpty(targetPath))
            {
                ResultMessage = "No target fullpath name available";
                return false;
            }

            if (string.IsNullOrEmpty(user.AccessToken) || user.AccessToken == "unknown")
            {
                if (!await NE201Login.Login(user.UserNasLink, user.UserName, user.UserSecret))
                {
                    ResultMessage = "Login Failed";
                    return false;
                }
            }

            if (!targetPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                targetPath += Path.DirectorySeparatorChar;
            if (string.IsNullOrEmpty(newName))
                newName = file.FileName;
            ResultMessage = await doNE201Upload(file, targetPath, newName);
            if (string.IsNullOrEmpty(ResultMessage))
                return true;

            return false;
        }

        public async Task<string> NE201FileUpload(Stream file, string targetPath, string newName)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();

            string postBody = $"{targetPath}"; //serachstr=*&
            content.Add(new StringContent(postBody), "DestPath");
            //Stream fstream = await file.OpenReadAsync();
            //byte[] allBytes = new byte[0];
            //byte[] buffer = new byte[4096];
            //int n = 4096;
            //while (n > 0)
            //{
            //    n = await file.ReadAsync(buffer, 0, 4096);
            //    if (n > 0)
            //    {
            //        int oldSize = allBytes.Length;
            //        Array.Resize(ref allBytes, oldSize + n);
            //        Array.Copy(buffer, 0, allBytes, oldSize, n);
            //    }
            //}
            StreamContent streamContent = new StreamContent(file);  // fstream);
            content.Add(streamContent, "file", newName);

            string result = await SendToNE201(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/UploadFile");

            //fstream.Close();
            file.Close();

            return result;
        }

        private async Task<string> doNE201Upload(FileResult file, string targetPath, string newName)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();

            string postBody = $"{targetPath}"; //serachstr=*&
            content.Add(new StringContent(postBody), "DestPath");
            Stream fstream = await file.OpenReadAsync();
            StreamContent streamContent = new StreamContent(fstream);
            content.Add(streamContent, "file", newName);

            string result = await SendToNE201(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/UploadFile");

            fstream.Close();

            return result;
        }

        internal async Task<List<NASFileViewModel>> NE201ReadList(string sourcepath, int pageNo, int pageSize)
        {
            StringContent content = new StringContent($"SourcePath={sourcepath}&page={pageNo}&pagesize={pageSize}", Encoding.UTF8, NE201Login.mediaType); //serachstr=*&
            HttpResponseMessage rsp = await SendOut(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/FileList");
            if (rsp == null)
                return new List<NASFileViewModel>();
            string jstr = await rsp.Content.ReadAsStringAsync();
            NASFileResponse nasF = JsonConvert.DeserializeObject<NASFileResponse>(jstr);

            return nasF.Result.Files;
        }

        internal async Task NE201Download(NASFileViewModel sourcFile, string targetFile)
        {
            Stream source = await NE201DownloadStream(sourcFile);
            if (/*Device.RuntimePlatform*/DeviceInfo.Platform == DevicePlatform.iOS /*Device.iOS*/)
            {
                FileStream target = new FileStream(targetFile, FileMode.Create);
                await source.CopyToAsync(target);

                target.Close();
                source.Close();
            }
            else
            {
                //Console.WriteLine($"{Path.Combine(file.FileOnNas.PathName, file.FileOnNas.Name)} copyto {localFile}");
                FileStream target = new FileStream(targetFile, FileMode.Create);
                await source.CopyToAsync(target);

                target.Close();
                source.Close();
            }
        }

        internal async Task NE201Move(string targetPath, List<NASFileViewModel> files)
        {
            if (files == null || files.Count < 1)
                return;

            string postBody = $"SourcePath={files[0].PathName}&DestPath={targetPath}";
            foreach (NASFileViewModel item in files)
            {
                postBody += $"&Items[]={item.Name}";
            }
            StringContent content = new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType);

            HttpResponseMessage result = await SendOut(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/FileMove");
        }

        //public async Task<string> doNE201Upload(string file, string targetPath)
        //{
        //    MultipartFormDataContent content = new MultipartFormDataContent();
        //    //byte[] allFileContent = new byte[0];
        //    //int st = 0;
        //    //byte[] buffer = new byte[4096];
        //    //Stream instream = await file.OpenReadAsync();
        //    //while (true)
        //    //{
        //    //    int read = await instream.ReadAsync(buffer, 0, 4096);
        //    //    if (read == 0)
        //    //        break;
        //    //    Array.Resize(ref allFileContent, st + read);
        //    //    Array.Copy(buffer, 0, allFileContent, st, read);
        //    //}
        //    //string s = Convert.ToBase64String(allFileContent);
        //    //content.Add(new StringContent(s), "file", file.FileName);

        //    FileResult fr = new FileResult(file);
        //    string postBody = $"{targetPath}"; //serachstr=*&
        //    content.Add(new StringContent(postBody), "DestPath");
        //    content.Add(new StreamContent(await fr.OpenReadAsync()), "file", Path.GetFileName(file));

        //    return await SendToNE201(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/UploadFile");
        //}

        async public Task<string> NE201AddFolder(string source, string fileName = "NewName")
        {
            string postBody = $"SourcePath={source}&NewName={fileName}"; //serachstr=*&
            StringContent content = new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType);
            return await SendToNE201(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/FileAddFolder");
        }

        internal async Task NE201Copy(string target, List<NASFileViewModel> files)
        {
            if (files == null || files.Count < 1)
                return;

            string postBody = $"SourcePath={files[0].PathName}&DestPath={target}";
            foreach (NASFileViewModel item in files)
            {
                postBody += $"&Items[]={item.Name}";
            }
            StringContent content = new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType);

            HttpResponseMessage result = await SendOut(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/FileCopy");
        }

        public async Task<bool> NE201DeleteFile(string sourcePath)
        {
            return await NE201DeleteFile(Path.GetDirectoryName(sourcePath), Path.GetFileName(sourcePath));
        }
        public async Task<bool> NE201DeleteFile(string sourcePath, string file)
        {
            if (file == null)
            {
                ResultMessage = "No file";
                return false;
            }
            if (string.IsNullOrEmpty(sourcePath))
            {
                ResultMessage = "No path";
                return false;
            }

            if (string.IsNullOrEmpty(user.AccessToken) || user.AccessToken == "unknown")
            {
                if (!await NE201Login.Login(user.UserNasLink, user.UserName, user.UserSecret))
                {
                    ResultMessage = "Login Failed";
                    return false;
                }
            }

            ResultMessage = await doNE201Delete(sourcePath, file);
            if (string.IsNullOrEmpty(ResultMessage))
                return true;

            return false;
        }

        private async Task<string> doNE201Delete(string sourcePath, string file)
        {
            string[] items = file.Split(new char[] { ',' });

            if (!sourcePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                sourcePath += Path.DirectorySeparatorChar.ToString();

            string postBody = $"SourcePath={sourcePath}";
            foreach (string item in items)
            {
                postBody += $"&Items[]={item}";
            }
            StringContent content = new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType);

            return await SendToNE201(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/FileDelete");
        }

        async public Task<string> NE201RenameFile(string source, string fileName)
        {
            string postBody = $"SourcePath={source}&NewName={fileName}"; //serachstr=*&
            StringContent content = new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType);

            return await SendToNE201(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/FileRename");
        }

        public async Task<byte[]> NE201ImageThumbnail(string source, int thumbSize)
        {
            string postBody = $"SourcePath={source}&thumbSize={thumbSize}"; //serachstr=*&
            StringContent content = new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType);

            return await SendToNE201Byte(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/ImageThumbnail");
        }


        public async Task<Stream> NE201StreamFile(NASFileViewModel file, [CallerMemberName] string caller = null)
        {
            string source = Path.Combine(file.PathName, file.Name);

            string postBody = $"SourcePath={source}"; //serachstr=*&
            StringContent content = new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType);

            HttpResponseMessage response = await SendOut(content, App.PC2ViewModel.UserSelected.UserNasLink, "/FileManage/StreamFile");

            if (response != null)
            {
                //ByteResultMessage = await response.Content.ReadAsByteArrayAsync();
                //ResultMessage = string.Empty;
                return await response.Content.ReadAsStreamAsync();
            }

            ResultMessage = $"[{caller}]: FileUpload worker overflow; {ResultMessage}";
            return null;
        }

        async public Task<string> NE201SystemInfo()
        {
            StringContent content = null;
            string s = await SendToNE201JsonStr(content, App.PC2ViewModel.UserSelected.UserNasLink, "/PowerNAS/Index/powernas.php", false); // "/ITE2/ProductInfo", false); ;
            user.SystemInfoStr = s;
            App.PC2ViewModel.SaveAccounts();
            return s;
        }

        public async Task<bool> NE201IsFileExist(string fullfilename)
        {
            string postBody = $"SourcePath={fullfilename}";
            HttpResponseMessage response = await SendOut(
                new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType),
                App.PC2ViewModel.UserSelected.UserNasLink,
                "/FileManage/FileCheckExists");

            string s = await response.Content.ReadAsStringAsync();
            FileManageResponse3 o = JsonConvert.DeserializeObject<FileManageResponse3>(s);
            return o.Result;

            //string[] s2 = s.Split(new string[] { "Result\":" }, StringSplitOptions.None);

            //if (s2.Length > 1)
            //{
            //    if (s2[1].StartsWith("F", StringComparison.InvariantCultureIgnoreCase))
            //    {

            //        return false;
            //    }
            //    return true;
            //}
            //else
            //{
            //    if (s.Contains("Result\":false"))
            //        return false;
            //}
            //return true;
        }

        public async Task<Stream> NE201DownloadStream(NASFileViewModel file, [CallerMemberName] string caller = null)
        {
            string postBody = $"SourcePath={Path.Combine(file.PathName, file.Name)}";
            HttpResponseMessage response = await SendOut(
                new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType),
                App.PC2ViewModel.UserSelected.UserNasLink,
                "/FileManage/DownloadFile");

            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //ByteResultMessage = await response.Content.ReadAsByteArrayAsync();
                //ResultMessage = string.Empty;
                return await response.Content.ReadAsStreamAsync();
            }

            ResultMessage = $"[{caller}]: File Download worker overflow; {ResultMessage}";
            return null;
        }

        public async Task<Stream> NE201DownloadStream(string fileFullName, [CallerMemberName] string caller = null)
        {
            string postBody = $"SourcePath={fileFullName}";
            HttpResponseMessage response = await SendOut(
                new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType),
                App.PC2ViewModel.UserSelected.UserNasLink,
                "/FileManage/DownloadFile");

            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //ByteResultMessage = await response.Content.ReadAsByteArrayAsync();
                //ResultMessage = string.Empty;
                return await response.Content.ReadAsStreamAsync();
            }

            ResultMessage = $"[{caller}]: File Download worker overflow; {ResultMessage}";
            return null;
        }






        #region send to ne201
        async Task<string> SendToNE201JsonStr(HttpContent content, string host, string path, bool authz = true, [CallerMemberName] string caller = null)
        {
            HttpResponseMessage response = await SendOut(content, host, path, authz);

            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }

            return $"[{caller}] -- {ResultMessage}";
        }

        async Task<byte[]> SendToNE201Byte(HttpContent content, string host, string path, [CallerMemberName] string caller = null)
        {
            HttpResponseMessage response = await SendOut(content, host, path);

            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ByteResultMessage = await response.Content.ReadAsByteArrayAsync();
                //ByteResultMessage = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                ResultMessage = string.Empty;

                return ByteResultMessage;
            }

            ResultMessage = $"[{caller}] -- {ResultMessage}";

            return new byte[0];
        }

        public async Task<string> SendToNE201(HttpContent content, string host, string path, [CallerMemberName] string caller = null)
        {
            HttpResponseMessage response = await SendOut(content, host, path);

            if (response != null)
            {
                ResultMessage = await response.Content.ReadAsStringAsync();
                int err = 1;

                if (ResultMessage.Contains("Version"))
                {
                    if (ResultMessage.Contains("Result\":{"))
                    {
                        FileManageResponse outcome = JsonConvert.DeserializeObject<FileManageResponse>(ResultMessage);
                        err = outcome.Result.error;
                    }
                    else
                    {
                        FileManageResponse2 outcome = JsonConvert.DeserializeObject<FileManageResponse2>(ResultMessage);
                        if (outcome.Result)
                            err = 0;
                    }
                }
                else
                {
                    if (ResultMessage.Contains("ucce"))
                        err = 0;
                }

                if (err == 0)
                    return string.Empty;
                else
                    return $"[{caller}] -- {err} eror{(err > 1 ? "s" : "")}.";
            }

            return $"[{caller}] -- {ResultMessage}";
        }


        async Task<HttpResponseMessage> SendOut(HttpContent content, string host, string path, bool auz = true, [CallerMemberName] string caller = null)
        {

            int redoCount = 0;
            Uri ne201uri;
            try
            {
                ne201uri = new Uri(string.Format("{1}://{0}", host, NE201Login.httpPrtl) + path);
            }
            catch
            {
                ResultMessage = $"[{caller}] {user.UserNasLink} is not an available computer address.";
                Console.WriteLine(ResultMessage);
                return null;
            }

        redo:
            SharedHttp.DefaultRequestHeaders.Clear();
            if (auz)
                SharedHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.PC2ViewModel.UserSelected.AccessToken);

            HttpResponseMessage response;
            try
            {
                response = await SharedHttp.PostAsync(ne201uri, content);
            }
            catch (Exception allErr)
            {
                ResultMessage = $"[{caller}] {allErr.Message}";
                response = null;
            }

            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response;
            }
            else if (response != null && response.StatusCode == System.Net.HttpStatusCode.PartialContent)
            {
                return response;
            }
            else
            {
                if (response == null)
                {
                    ResultMessage = $"[{caller}] Http Post Failed.";
                    ResetHttpClient();
                }
                else
                {
                    ResultMessage = await response.Content.ReadAsStringAsync();
                    bool loginSuccess = false;
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        loginSuccess = await NE201Login.RefreshLogin(App.PC2ViewModel.UserSelected);
                        if (loginSuccess)
                        {
                            ResultMessage = $"[{caller}]: Token refreshed. Upload retry: {redoCount}";
                        }
                        else
                        {
                            ResultMessage = $"[{caller}]: Token refresh faied.";
                        }
                    }
                    else
                    {
                        ResultMessage = $"[{caller}]: Upload failed, Http StatusCode: {response.StatusCode}";
                        ResetHttpClient();
                    }
                }

                redoCount++;

                if (redoCount > 5)
                {
                    ResultMessage = $"[{caller}]: File Upload worker overflow; {ResultMessage}";
                    return null;
                }
                goto redo;
            }
        }
        #endregion
    }
}