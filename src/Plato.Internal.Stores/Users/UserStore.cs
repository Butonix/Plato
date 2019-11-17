﻿using System;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Models.Users;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{
    public class UserStore :
        IUserStore<User>,
        IUserRoleStore<User>,
        IUserPasswordStore<User>,
        IUserEmailStore<User>,
        IUserSecurityStampStore<User>,
        IUserLoginStore<User>
    {
        #region "Dispose"

        public void Dispose()
        {
        }

        #endregion

        #region "UserStore"

        private readonly IPlatoUserLoginStore<UserLogin> _platoUserLoginStore;
        private readonly IPlatoUserRoleStore<UserRole> _platoUserRoleStore;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IPlatoRoleStore _platoRoleStore;

        public UserStore(
            IPlatoUserRoleStore<UserRole> platoUserRoleStore,
            IPlatoUserLoginStore<UserLogin> userLoginStore,
            IPlatoUserStore<User> platoUserStore,
            IPlatoRoleStore platoRoleStore)
        {            
            _platoUserRoleStore = platoUserRoleStore;
            _platoUserLoginStore = userLoginStore;
            _platoUserStore = platoUserStore;
            _platoRoleStore = platoRoleStore;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var newUser = await _platoUserStore.CreateAsync(user);
            if ((newUser != null) && (newUser.Id > 0))
            {
                return IdentityResult.Success;
            }                

            return IdentityResult.Failed();

        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                await _platoUserStore.UpdateAsync(user);
            }
            catch
            {
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;

        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            try
            {
                await _platoUserStore.DeleteAsync(user);
            }
            catch
            {
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }
        
        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!int.TryParse(userId, out var id))
            {
                return null;
            }
                
            return await _platoUserStore.GetByIdAsync(id);

        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _platoUserStore.GetByUserNameNormalizedAsync(normalizedUserName);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString(CultureInfo.InvariantCulture));
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;

            return Task.CompletedTask;
        }
        
        #endregion

        #region "IUserPasswordStore"

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                

            return Task.FromResult(user.PasswordHash != null);
        }

        #endregion

        #region "IUserSecurityStampStore"

        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                

            user.SecurityStamp = stamp;

            return Task.CompletedTask;

        }

        public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            return Task.FromResult(user.SecurityStamp);

        }

        #endregion

        #region "IUserEmailStore"

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            user.Email = email;
            return Task.CompletedTask;

        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            return Task.FromResult(user.Email);

        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            return Task.FromResult(user.EmailConfirmed);

        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;

        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _platoUserStore.GetByEmailNormalizedAsync(normalizedEmail);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            return Task.FromResult(user.NormalizedEmail);

        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;

        }

        #endregion

        #region "IUserRoleStore"

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var role = await _platoRoleStore.GetByNameAsync(roleName);
            if (role != null)
            {
                await _platoUserRoleStore.CreateAsync(new UserRole()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                });
            }

        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var role = await _platoRoleStore.GetByNameAsync(roleName);
            if (role != null)
            {
                await _platoUserRoleStore.DeleteUserRole(user.Id, role.Id);
            }

        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _platoRoleStore.GetRolesByUserIdAsync(user.Id);
            if (roles == null)
            {
                return new List<string>();
            }
                
            var output = new List<string>();
            foreach (var role in roles)
            {
                output.Add(role.Name);
            }
                
            return output;

        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            
            cancellationToken.ThrowIfCancellationRequested();

            var roleNames = await GetRolesAsync(user, cancellationToken);

            if (roleNames == null)
            {
                return false;
            }

            if (roleNames.Count == 0)
            {
                return false;
            }

            foreach (var name in roleNames)
            {
                if (string.Equals(name, roleName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                    
            }

            return false;

        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var results  = await _platoUserStore.QueryAsync()
                .Select<UserQueryParams>(q => q.RoleName.Equals(roleName))
                .OrderBy("Id", OrderBy.Desc)
                .ToList();

            return results.Data;

        }

        #endregion

        #region "IUserLoginStore"

        public Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            if (((User)user).LoginInfos.Any(i => i.LoginProvider == login.LoginProvider))
            {
                throw new InvalidOperationException($"Provider {login.LoginProvider} is already linked for {user.UserName}");
            }                

            ((User)user).LoginInfos.Add(new UserLogin(login));

            return Task.CompletedTask;

        }

        public async Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {

            // Has the login provider already been added?
            var logins = await _platoUserLoginStore.QueryAsync()
                .Select<UserLoginQueryParams>(q =>
                {
                    q.LoginProvider.Equals(loginProvider);
                    q.ProviderKey.Equals(providerKey);
                })
                .ToList();

            var userId = logins?.FirstOrDefault().UserId ?? 0;
            if (userId > 0)
            {
                return await _platoUserStore.GetByIdAsync(userId);
            }

            return null;
            
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var infos = new List<UserLoginInfo>();
            foreach (var info in ((User)user).LoginInfos)
            {
                var newInfo = new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName);
                infos.Add(newInfo);
            } 

            return Task.FromResult<IList<UserLoginInfo>>(infos);

        }

        public Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var externalLogins = ((User)user).LoginInfos;
            if (externalLogins != null)
            {
                var item = externalLogins.FirstOrDefault(c => c.LoginProvider == loginProvider && c.ProviderKey == providerKey);
                if (item != null)
                {
                    externalLogins.Remove(item);
                }
            }

            return Task.CompletedTask;

        }

        #endregion

    }

}