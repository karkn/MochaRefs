using PocketSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Integration.Pocket
{
    public class PocketAccount
    {
        private const string ConsumerKey = "23124-afd5deb9a474e869d0620834";

        private PocketClient _client;

        public string CallbackUri
        {
            get { return _client.CallbackUri; }
            set { _client.CallbackUri = value; }
        }

        public PocketAccount()
        {
            _client = new PocketClient(ConsumerKey);
        }

        public PocketAccount(string accessCode)
        {
            _client = new PocketClient(ConsumerKey, accessCode);
        }

        public async Task<string> GetRequestCode()
        {
            return await _client.GetRequestCode();
        }

        public Uri Auth(string callbackUri)
        {
            _client.CallbackUri = callbackUri;
            return _client.GenerateAuthenticationUri();
        }

        public async Task<PocketUser> GetUser(string reqCode)
        {
            try
            {
                var user = await _client.GetUser(reqCode);
                return new PocketUser(user.Username, user.Code);
            }
            catch (PocketException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<PocketItem>> GetItems()
        {
            var items = await _client.Get();
            return items.Select(i => new PocketItem(i.Uri, i.Title)).ToList();
        }
    }
}
