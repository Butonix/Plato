﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewModels;
using Plato.Categories.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Features;

namespace Plato.Articles.Categories.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<Category>
    {
        
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<ArticleCategory> _categoryStore;
        private readonly ICategoryManager<ArticleCategory> _categoryManager;
        private readonly IFeatureFacade _featureFacade;

        public IStringLocalizer S { get; }


        public AdminViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<ArticleCategory> categoryStore,
            ICategoryManager<ArticleCategory> categoryManager,
            IStringLocalizer<AdminViewProvider> stringLocalizer,
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;

            S = stringLocalizer;
            _featureFacade = featureFacade;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(Category category, IViewProviderContext updater)
        {
            //var indexViewModel = await GetIndexModel(category?.Id ?? 0);

            var viewModel = new CategoryIndexViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    ChannelId = category?.Id ?? 0,
                    EnableEdit = true
                }
            };

            return Task.FromResult(Views(
                View<Category>("Admin.Index.Header", model => category).Zone("header").Order(1),
                View<CategoryIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<CategoryIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Category viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Category category, IViewProviderContext updater)
        {

            var defaultIcons = new DefaultIcons();

            EditChannelViewModel editChannelViewModel = null;
            if (category.Id == 0)
            {
                editChannelViewModel = new EditChannelViewModel()
                {
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons,
                    IsNewChannel = true,
                    ParentId = category.ParentId,
                    AvailableChannels = await GetAvailableChannels()
                };
            }
            else
            {
                editChannelViewModel = new EditChannelViewModel()
                {
                    Id = category.Id,
                    ParentId = category.ParentId,
                    Name = category.Name,
                    Description = category.Description,
                    ForeColor = category.ForeColor,
                    BackColor = category.BackColor,
                    IconCss = category.IconCss,
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons,
                    AvailableChannels = await GetAvailableChannels()
                };
            }
            
            return Views(
                View<EditChannelViewModel>("Admin.Edit.Header", model => editChannelViewModel).Zone("header").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Content", model => editChannelViewModel).Zone("content").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Actions", model => editChannelViewModel).Zone("actions").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Footer", model => editChannelViewModel).Zone("footer").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Category category, IViewProviderContext context)
        {

            var model = new EditChannelViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(category, context);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();
  
            if (context.Updater.ModelState.IsValid)
            {

                var iconCss = model.IconCss;
                if (!string.IsNullOrEmpty(iconCss))
                {
                    iconCss = model.IconPrefix + iconCss;
                }

                var result = await _categoryManager.UpdateAsync(new ArticleCategory()
                {
                    Id = category.Id,
                    FeatureId = category.FeatureId,
                    ParentId = model.ParentId,
                    Name = model.Name,
                    Description = model.Description,
                    ForeColor = model.ForeColor,
                    BackColor = model.BackColor,
                    IconCss = iconCss,
                    SortOrder = category.SortOrder
                });

                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(category, context);
            
        }

        #endregion

        #region "Private Methods"

        async Task<IEnumerable<SelectListItem>> GetAvailableChannels()
        {

            var output = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["root channel"],
                    Value = "0"
                }
            };
            
            var feature = await GetcurrentFeature();
            var channels = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            if (channels != null)
            {
                var items = await RecurseChannels(channels);
                foreach (var item in items)
                {
                    output.Add(item);
                }
            }
          
            return output;

        }

        Task<IList<SelectListItem>> RecurseChannels(
            IEnumerable<ICategory> input,
            IList<SelectListItem> output = null,
            int id = 0)
        {

            if (output == null)
            {
                output = new List<SelectListItem>();
            }
            
            var categories = input.ToList();
            foreach (var category in categories)
            {
                if (category.ParentId == id)
                {
                    var indent = "-".Repeat(category.Depth);
                    if (!string.IsNullOrEmpty(indent))
                    {
                        indent += " ";
                    }
                    output.Add(new SelectListItem
                    {
                        Text = indent + category.Name,
                        Value = category.Id.ToString()
                    });
                    RecurseChannels(categories, output, category.Id);
                }
            }

            return Task.FromResult(output);

        }
        
        //async Task<ChannelListViewModel> GetIndexModel(int parentId)
        //{
        //    var feature = await GetcurrentFeature();
        //    var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
        //    return new ChannelListViewModel()
        //    {
        //        Channels = categories?.Where(c => c.ParentId == parentId)
        //    };
        //}
        
        async Task<IShellFeature> GetcurrentFeature()
        {
            var featureId = "Plato.Articles.Categories";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

        #endregion

    }
}
