﻿using System.Threading.Tasks;
using Plato.Categories.Roles.Services;
using Plato.Discuss.Categories.Models;
using Plato.Internal.Features.Abstractions;

namespace Plato.Discuss.Categories.Roles.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        
        private readonly IDefaultCategoryRolesManager<Category> _defaultCategoryRolesManager;
        private readonly IFeatureFacade _featureFacade;

        public FeatureEventHandler(
            IDefaultCategoryRolesManager<Category> defaultCategoryRolesManager,
            IFeatureFacade featureFacade)
        {
            _defaultCategoryRolesManager = defaultCategoryRolesManager;
            _featureFacade = featureFacade;
        }
        
        public override async Task InstalledAsync(IFeatureEventContext context)
        {

            // Install default roles for Plato.Docs.Categories
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Categories");
            if (feature != null)
            {
                await _defaultCategoryRolesManager.InstallAsync(feature.Id);
            }
       
        }

    }

}
