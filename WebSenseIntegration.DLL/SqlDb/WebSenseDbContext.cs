using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemplify.Integrations.WebSense.SqlDb
{
    public partial class WebSenseContext
    {
        public WebSenseContext(WebSenseDbConfig dbConfig) 
            : base(dbConfig.BuildDbConnectionString())
        { }

        public WebSenseContext(string pass, string dbHost = null, string dbUser = null)
        {
            var dbConfig = new WebSenseDbConfig(pass, dbHost, dbUser);
            Database.Connection.ConnectionString = dbConfig.BuildDbConnectionString();
        }

        private user_names GetUserId(string username)
        {
            Debug.WriteLine(string.Format("Retrieving user id for username {0}", username));
            return user_names.SingleOrDefault(user => user.user_name == username);
        }

        private Task<user_names> GetUserIdAsync(string username)
        {
            Debug.WriteLine(string.Format("Retrieving user id for username {0}", username));
            return user_names.SingleOrDefaultAsync(user => user.user_name == username);
        }

        /// <summary>
        /// Get all the urls which a given user-name has accessed
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>
        /// List of all the urls as strings
        /// Or null if user does not exist
        /// </returns>
        public List<string> GetAllUrlsForUser(string username)
        {
            Debug.WriteLine(string.Format("Retrieving all urls for username {0}", username));

            var user = GetUserId(username);

            if (null == user)
            {
                return null;
            }

            var urls = summary_url.Where(url => url.user_id == user.user_id).ToList();

            return urls.Select(url => url.url).Distinct().ToList();
        }

        public async Task<List<string>> GetAllUrlsForUserAsync(string username)
        {
            Debug.WriteLine(string.Format("Retrieving all urls for username {0}", username));

            var user = await GetUserIdAsync(username);

            if (null == user)
            {
                return null;
            }

            var urls = await summary_url.Where(url => url.user_id == user.user_id).ToListAsync();
            return urls.Select(url => url.url).Distinct().ToList();
        }

        /// <summary>
        /// Query the DB for all the user which match a given url
        /// </summary>
        /// <param name="urlStr">The url as a sting</param>
        /// <returns>
        /// List of all the users matching the given url
        /// Or null if no url was matched for the given string
        /// </returns>
        /// <remarks>
        /// The url is checked in case-sensitive meaning: http://google.com != google.com
        /// These two URLs will return different results
        /// </remarks>
        public List<string> GetAllUsersForUrl(string urlStr)
        {
            Debug.WriteLine(string.Format("Retrieving all users for url {0}", urlStr));

            var urlsByUser = summary_url
                .Join(user_names, url => url.user_id, user => user.user_id, (urlObj, userObj) => new { urlObj, userObj })
                .Where(row => row.urlObj.url.Equals(urlStr))
                .GroupBy(row => row.urlObj.user_id)
                .ToList();

            return 0 == urlsByUser.Count ? null : 
                urlsByUser.Select(group => group.First().userObj.user_name).ToList();
        }

        public async Task<List<string>> GetAllUsersForUrlAsync(string urlStr)
        {
            Debug.WriteLine(string.Format("Retrieving all users for url {0}", urlStr));

            var urlsByUser = await summary_url
                .Join(user_names, url => url.user_id, user => user.user_id, (urlObj, userObj) => new { urlObj, userObj })
                .Where(row => row.urlObj.url.Equals(urlStr))
                .GroupBy(row => row.urlObj.user_id)
                .ToListAsync();

            return 0 == urlsByUser.Count ? null :
                urlsByUser.Select(group => group.First().userObj.user_name).ToList();
        }

    }
}
