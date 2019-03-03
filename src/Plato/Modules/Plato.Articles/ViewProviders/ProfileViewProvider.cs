﻿using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Articles.ViewProviders
{
    public class ProfileViewProvider : BaseViewProvider<Profile>
    {

        private readonly IFeatureFacade _featureFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public ProfileViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IFeatureFacade featureFacade)
        {
            _platoUserStore = platoUserStore;
            _featureFacade = featureFacade;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Profile profile, IViewProviderContext context)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(profile.Id);

            // Ensure user exists
            if (user == null)
            {
                return await BuildIndexAsync(profile, context);
            }
            
            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");

            // Ensure feature exists
            if (feature == null)
            {
                return await BuildIndexAsync(profile, context);
            }

            // Build view model
            var viewModel = new EntityIndexViewModel<Article>()
            {
                Options = new EntityIndexOptions()
                { 
                    FeatureId = feature.Id,
                    CreatedByUserId = user.Id
                },
                Pager = new PagerOptions()
                {
                    Page = 1,
                    PageSize = 5,
                    Enabled = false
                }
            };

            return Views(
                    View<EntityIndexViewModel<Article>>("Profile.Articles.Display.Content", model => viewModel).Zone("content")
                );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
