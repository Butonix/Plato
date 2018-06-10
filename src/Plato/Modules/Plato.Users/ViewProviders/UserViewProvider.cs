﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Layout.ViewProviders;
using Plato.Layout.ModelBinding;
using Plato.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;

        public UserViewProvider(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        public override async Task<IViewProviderResult> BuildDisplayAsync(User user, IUpdateModel updater)
        {

            return Views(
                View<User>("User.Display", model => user).Zone("header").Order(3),
                View<User>("User.Display.2", model => user).Zone("header").Order(2),
                View<User>("User.Display.3", model => user).Zone("header").Order(1),
                View<User>("User.Meta", model => user).Zone("meta").Order(1),
                View<User>("User.Meta", model => user).Zone("meta").Order(2),
                View<User>("User.Display.2", model => user).Zone("meta").Order(3),
                View<User>("User.Display", model => user).Zone("content").Order(1),
                View<User>("User.Display.2", model => user).Zone("content").Order(2),
                View<User>("User.Display.3", model => user).Zone("content").Order(3),
                View<User>("User.Display", model => user).Zone("sidebar").Order(1),
                View<User>("User.Display.2", model => user).Zone("sidebar").Order(2),
                View<User>("User.Display.3", model => user).Zone("sidebar").Order(3),
                View<User>("User.Display", model => user).Zone("footer").Order(1),
                View<User>("User.Display", model => user).Zone("footer").Order(2),
                View<User>("User.Actions", model => user).Zone("actions").Order(2),
                View<User>("User.Tools", model => user).Zone("tools").Order(2)
            );


        }

        public override async Task<IViewProviderResult> BuildIndexAsync(User user, IUpdateModel updater)
        {
            return Views(
                View<User>("User.List", model => user).Zone("header").Order(3)
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {
            
            return Views(
                View<User>("User.Display", model => user),
                View<User>("User.Display", model => user),
                View<User>("User.Display.2", model => user),
                View<EditUserViewModel>("User.Edit", model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    return model;
                }),
                View<EditUserViewModel>("User.Edit.2", model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    return model;
                })
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditUserViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, updater);
            }

            model.UserName = model.UserName?.Trim();
            model.Email = model.Email?.Trim();

            if (updater.ModelState.IsValid)
            {

                await _userManager.SetUserNameAsync(user, model.UserName);
                await _userManager.SetEmailAsync(user, model.Email);

                var result = await _userManager.UpdateAsync(user);

                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            
            return await BuildEditAsync(user, updater);

        }
    }
}
