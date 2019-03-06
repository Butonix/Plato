﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Categories.ViewComponents
{

    public class CategoryInputViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        public CategoryInputViewComponent(
            ICategoryStore<Category> channelStore,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<int> selectedCategories, string htmlName)
        {
            if (selectedCategories == null)
            {
                selectedCategories = new int[0];
            }
            
            var categories = await BuildChannelSelectionsAsync(selectedCategories);
            var model = new CategoryInputViewModel(categories)
            {
                HtmlName = htmlName
            };

            return View(model);
        }

        private async Task<IList<Selection<Category>>> BuildChannelSelectionsAsync(
            IEnumerable<int> selectedCategories)
        {
            
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");
            var channels = await _channelStore.GetByFeatureIdAsync(feature.Id);
            if (channels != null)
            {

                var items = await RecurseChannels(channels);
          
                var selections = items.Select(c => new Selection<Category>
                    {
                        IsSelected = selectedCategories.Any(v => v == c.Id),
                        Value = c
                    })
                    .OrderBy(r => r.Value)
                    .ToList();

                return selections;

            }

            return null;
        }


        Task<IList<Category>> RecurseChannels(
            IEnumerable<ICategory> input,
            IList<Category> output = null,
            int id = 0)
        {

            if (output == null)
            {
                output = new List<Category>();
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
                    output.Add(new Category
                    {
                        Id = category.Id,
                        Name = indent + category.Name
                       
                    });
                    RecurseChannels(categories, output, category.Id);
                }
            }

            return Task.FromResult(output);

        }

    }

}
