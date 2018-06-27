﻿using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Users;
using Plato.Users.Social.Models;

namespace Plato.Users.Social.Stores
{

    public class SocialLinksStore : ISocialLinksStore
    {

        public const string Key = "Plato.Users.Details";

        private readonly IUserDataStore _userDataStore;

        public SocialLinksStore(
            IUserDataStore userDataStore)
        {
            _userDataStore = userDataStore;
        }

        public async Task<SocialLinks> GetAsync(int userId)
        {
            return await _userDataStore.GetAsync<SocialLinks>(userId, Key);
        }

        public async Task<SocialLinks> UpdateAsync(int userId, SocialLinks value)
        {
            return await _userDataStore.UpdateAsync<SocialLinks>(userId, Key, value);
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            return await _userDataStore.DeleteAsync(userId, Key);
        }
    }
}