using System.Text;
using System.Xml;
using System.Xml.Linq;

using PowerCloud.Models;
using PowerCloud.ViewModels;
using CommunityToolkit.Maui.Storage;


namespace PowerCloud.Platforms
{
    public class AccountFiler2 : IAccountFiler2
    {
        const string FileName = "UserAccounts.xml";

        public IEnumerable<AccountViewModel> Load()
        {
            XDocument doc = null;

            string filename = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);
            //string filename = Path.Combine(
            //    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            //    "..", "Library", FileName);

            if (File.Exists(filename))
            {
                try
                {
                    doc = XDocument.Load(filename);
                }
                catch
                {
                }
            }

            if (doc == null)
                doc = XDocument.Parse(DefaultData);

            if (doc.Root != null)
            {
                int userC = 0;
                foreach (var entry in doc.Root.Elements("user"))
                {
                    userC++;
                    yield return new AccountViewModel(new Account(
                        entry.Attribute("username").Value,
                        entry.Attribute("access_token").Value,
                        entry.Attribute("refresh_token").Value,
                        entry.Attribute("sequence_no").Value,
                        entry.Attribute("sort_type").Value,
                        entry.Value));
                }
            }
        }

        public void Save(IEnumerable<AccountViewModel> quotes)
        {
            string filename = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);
            //string filename = Path.Combine(
            //    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            //    "..", "Library", FileName);

            if (File.Exists(filename))
                File.Delete(filename);

            XDocument doc = new XDocument(
                new XElement("users",
                    quotes.Select(q =>
                        new XElement("user", new XAttribute("username", q.UserName), new XAttribute("access_token", q.AccessToken),
                                     new XAttribute("refresh_token", q.RefreshToken), new XAttribute("sequence_no", q.UserNasLink),
                                     new XAttribute("sort_type", q.SortType))
                        {
                            Value = q.SystemInfoStr
                        })));


            doc.Save(filename);
        }

        #region Internal Data
        static readonly string DefaultData =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
<users>
    <user username=""Eleanor Roosevelt"" access_token=""unknown"" refresh_token=""unknown"" sequence_no=""1"" sort_type=""1"">Great minds discuss ideas; average minds discuss events; small minds discuss people.</user>
    <user username=""William Shakespeare"" access_token=""unknown"" refresh_token=""unknown"" sequence_no=""1"" sort_type=""1"">Some are born great, some achieve greatness, and some have greatness thrust upon them.</user>
    <user username=""Winston Churchill"" access_token=""unknown"" refresh_token=""unknown"" sequence_no=""1"" sort_type=""1"">All the great things are simple, and many can be expressed in a single word: freedom, justice, honor, duty, mercy, hope.</user>
    <user username=""Ralph Waldo Emerson"" access_token=""unknown"" refresh_token=""unknown"" sequence_no=""1"" sort_type=""1"">Our greatest glory is not in never failing, but in rising up every time we fail.</user>
    <user username=""William Arthur Ward"" access_token=""unknown"" refresh_token=""unknown"" sequence_no=""1"" sort_type=""1"">The mediocre teacher tells. The good teacher explains. The superior teacher demonstrates. The great teacher inspires.</user>
</users>";
        #endregion
    }
}