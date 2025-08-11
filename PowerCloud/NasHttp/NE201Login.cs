using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using PowerCloud.ViewModels;
using PowerCloud.Models;
using System.Collections.ObjectModel;

namespace PowerCloud.NasHttp
{
    public class NE201Login
    {
        public static string httpPrtl = "http";
        public static readonly string mediaType = "application/x-www-form-urlencoded";
        public static readonly string jsonMediaType = "application/json";
        static string secret = string.Empty;
        public static async Task<bool> Login(string ddns, string username, string password)
        {
            try
            {
                secret = password;
                string postUri = string.Format("{1}://{0}", ddns, httpPrtl);

                string postBody = $"username={username}&password={password}&grant_type=password&LoginType=1&OAuth=True";



                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation;
                HttpClient client = new HttpClient(handler);



                client.DefaultRequestHeaders.Clear();


                Uri ne201uri = new Uri(postUri + "/Account/Login");
                //postBody = $"username=admin&password=test&grant_type=password&LoginType=1&OAuth=True";
                HttpResponseMessage response = await client.PostAsync(ne201uri, new StringContent(postBody, Encoding.UTF8, mediaType));

                //string result = await client.GetStringAsync(ddns + "/Account/Login");
                //NE201LogonMsg response = JsonConvert.DeserializeObject<IEnumerable<NE201LogonMsg>>(result);
                string result = string.Empty;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = await response.Content.ReadAsStringAsync();
                    NE201LogonResp? ne201Token = JsonConvert.DeserializeObject<NE201LogonResp>(result);
                    if (ne201Token != null)
                    {
                        ne201Token.secret = password;
                        ne201Token.url = ddns;
                        ne201Token.username = username;
                    }
                    SetItemSelected(ne201Token);
                    NE201FileManager mgr = NE201FileManager.FileManagerFactory(App.PC2ViewModel.UserSelected);
                    App.PC2ViewModel.UserSelected.SystemInfoStr = await mgr.NE201SystemInfo();
                    App.PC2ViewModel.UserSelected.SystemInfo.LoginAt = DateTime.Now;
                    CheckRecentAccounts(ne201Token);

                    App.PC2ViewModel.UserSelected.AccessToken = ne201Token?.access_token ?? string.Empty;
                    App.PC2ViewModel.UserSelected.RefreshToken = ne201Token?.refresh_token ?? string.Empty;
                    try
                    {
                        App.PC2ViewModel.SaveAccounts();
                    }
                    catch (Exception a)
                    {
                        ErrorMessage = a.ToString();
                    }

                    string s = ne201Token?.url ?? string.Empty;
                    AccountManager ngr = App.PC2ViewModel.accountManager;

                    if (ne201Token != null)
                    {
                        App.PC2ViewModel.accountManager.SelectedNasAddress = ne201Token.url;
                        App.PC2ViewModel.accountManager.SelectedAccessToeken = ne201Token.access_token;
                        App.PC2ViewModel.accountManager.SelectedRefreshToken = ne201Token.refresh_token;
                    }
                }
                else
                {
                    result = await response.Content.ReadAsStringAsync();
                    return false;
                }
                return true;
            }
            catch (Exception allErr)
            {
                ErrorMessage = allErr.ToString();
                return false;
            }
        }

        //public static bool LoginSync(string userName, string ddns, string secret)
        //{
        //    if (string.IsNullOrEmpty(secret) || secret == "unknown")
        //    {
        //        //do a login on UI
        //        return Login(userName, ddns, secret).GetAwaiter().GetResult();
        //    }

        //    return Login(userName, ddns, secret).GetAwaiter().GetResult();
        //}



        public static async Task<bool> RefreshLogin(AccountViewModel item)
        {
            ErrorMessage = string.Empty;
            try
            {
                string postUri = string.Format("{1}://{0}", item.UserNasLink, httpPrtl);

                string postBody = $"grant_type=refresh_token&refresh_token={App.PC2ViewModel.UserSelected.RefreshToken}";

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation;
                HttpClient client = new HttpClient(handler);



                client.DefaultRequestHeaders.Clear();


                Uri ne201uri = new Uri(postUri + "/Account/RefreshToken");
                HttpResponseMessage response = await client.PostAsync(ne201uri, new StringContent(postBody, Encoding.UTF8, mediaType));
                //HttpResponseMessage response = client.PostAsync(ne201uri, new StringContent(postBody, Encoding.UTF8, mediaType)).GetAwaiter().GetResult();

                string result = string.Empty;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = await response.Content.ReadAsStringAsync();
                    //result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    NE201LogonResp? ne201Token = JsonConvert.DeserializeObject<NE201LogonResp>(result);
                    App.PC2ViewModel.UserSelected.AccessToken = ne201Token?.access_token ?? string.Empty;
                    App.PC2ViewModel.SaveAccounts();

                    App.PC2ViewModel.accountManager.SelectedAccessToeken = ne201Token?.access_token ?? string.Empty;

                    return true;
                    //return Task.Run(() => true);
                }
                else
                {
                    ErrorMessage = await response.Content.ReadAsStringAsync();

                    //ErrorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    //item.RefreshToken = "unknown";
                    //item.AccessToken = "unknown";

                    //// TO DO.
                    ////
                    ////return await Login(item.UserNasLink, item.UserName, item.UserSecret);
                    ////
                    //return Task.Run(() => false);
                    return false;
                }

            }
            catch (Exception err)
            {
                ErrorMessage = err.ToString();
            }

            return false;
            //return Task.Run(() => false);
        }

        public static async Task<bool> Logout(AccountViewModel acc)
        {
            string postUri = string.Format("{1}://{0}", acc.UserNasLink, httpPrtl);

            string postBody = string.Empty;



            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation;
            HttpClient client = new HttpClient(handler);



            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.PC2ViewModel.UserSelected.AccessToken);


            Uri ne201uri = new Uri(postUri + "/Account/Logout");
            StringContent? emptyContent = null;
            HttpResponseMessage response = await client.PostAsync(ne201uri, emptyContent);

            //string result = await client.GetStringAsync(ddns + "/Account/Login");
            //NE201LogonMsg response = JsonConvert.DeserializeObject<IEnumerable<NE201LogonMsg>>(result);
            string result = string.Empty;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
                return false;
            }
            return true;
        }


        public static string ErrorMessage = string.Empty;

        private static bool SetItemSelected(NE201LogonResp? ne201Token)
        {
            if (ne201Token == null) 
                return false;

            Account acc = new Account(ne201Token.username, ne201Token.access_token, ne201Token.refresh_token, ne201Token.url, "1", string.Empty);
            App.PC2ViewModel.UserSelected = new AccountViewModel(acc);
            //App.PC2ViewModel.UserSelected.UserSecret = ne201Token.secret;
            //App.PC2ViewModel.UserSelected.UserName = ne201Token.username;
            //App.PC2ViewModel.UserSelected.UserNasLink = ne201Token.url;
            bool found = false;
            if (App.PC2ViewModel.RecentAccounts != null)
            {
                foreach (AccountViewModel item in App.PC2ViewModel.RecentAccounts)
                {
                    if (item.UserName == ne201Token.username && item.UserNasLink == ne201Token.url)
                    {
                        App.PC2ViewModel.RecentAccounts.Remove(item);
                        break;
                    }
                }
                App.PC2ViewModel.RecentAccounts.Insert(0, App.PC2ViewModel.UserSelected);
            }
            else
            {
                App.PC2ViewModel.RecentAccounts = new ObservableCollection<AccountViewModel>() { App.PC2ViewModel.UserSelected };
            }
            AccountViewModel? userRemoved = null;
            if (App.PC2ViewModel.RecentAccounts?.Count > 3)
            {
                userRemoved = App.PC2ViewModel.RecentAccounts[3];
                App.PC2ViewModel.RecentAccounts.RemoveAt(3);
            }

            if (App.PC2ViewModel.Accounts != null)
            {
                foreach (AccountViewModel item in App.PC2ViewModel.Accounts)
                {
                    if (item.UserName == ne201Token.username && item.UserNasLink == ne201Token.url)
                    {
                        found = true;
                        App.PC2ViewModel.Accounts.Remove(item);
                        break;
                    }
                }
            }

            if (userRemoved != null)
            {
                if (App.PC2ViewModel.Accounts != null)
                {
                    App.PC2ViewModel.Accounts.Insert(0, userRemoved);
                }
                else
                {
                    App.PC2ViewModel.Accounts = new ObservableCollection<AccountViewModel> { userRemoved };
                }
            }

            return found;
        }

        private static void CheckRecentAccounts(NE201LogonResp? ne201Token)
        {
            MainViewModel viewModel = App.PC2ViewModel;
            AccountViewModel acc = App.PC2ViewModel.UserSelected;
            if (ne201Token != null)
            {

                int i = viewModel.RecentAccounts.Count;
                //bool findOne = false;
                for (; i > 0; i--)
                {
                    if (viewModel.RecentAccounts[i - 1].UserName == ne201Token.username &&
                        viewModel.RecentAccounts[i - 1].UserNasLink == ne201Token.url)
                    {
                        viewModel.RecentAccounts.RemoveAt(i - 1);
                        //findOne = true;
                        break;
                    }
                }

                if (i == 0)
                    viewModel.RecentAccounts.Add(acc);
                else
                    viewModel.RecentAccounts.Insert(0, acc);

                if (viewModel.RecentAccounts.Count > 3)
                {
                    acc = viewModel.RecentAccounts[3];
                    viewModel.RecentAccounts.RemoveAt(3);
                    if (viewModel.Accounts.Count == 0)
                        viewModel.Accounts.Add(acc);
                    else
                        viewModel.Accounts.Insert(0, acc);
                }
            }

            viewModel.AllLoginAccounts.Clear();
            viewModel.AllLogoutAccounts.Clear();
            viewModel.EveryAccount.Clear();
        }

        public static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors sslErrors)
        {
            //// It is possible inpect the certificate provided by server
            //string s1 = $"Requested URI: {requestMessage.RequestUri}";
            //string s2 = $"Effective date: {certificate.GetEffectiveDateString()}";
            //string s3 = $"Exp date: {certificate.GetExpirationDateString()}";
            //string s4 = $"Issuer: {certificate.Issuer}";
            //string s5 = $"Subject: {certificate.Subject}";

            //// Based on the custom logic it is possible to decide whether the client considers certificate valid or not
            string s6 = $"Errors: {sslErrors}";
            if ((certificate?.Issuer ?? "Failed") == "CN=*.powernas.com.tw")
            {
                sslErrors = SslPolicyErrors.None;
                return true;
            }
            else
                return false;
        }
    }

    #region NE201Logon Response
    public class NE201LogonResp
    {
        public string access_token { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public string expires_in { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string secret { get; set; } = string.Empty;
    }
    #endregion
}