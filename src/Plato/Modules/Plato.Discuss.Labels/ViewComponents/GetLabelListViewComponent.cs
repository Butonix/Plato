﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Labels.Models;
using Plato.Entities.Labels.Services;
using Plato.Entities.Labels.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Labels.ViewComponents
{

    public class GetLabelListViewComponent : ViewComponent
    {
        
        private readonly ILabelService<Label> _labelService;

        public GetLabelListViewComponent(
            ILabelService<Label> labelService)
        {
            _labelService = labelService;
        }

        public async Task<IViewComponentResult> InvokeAsync(LabelIndexOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new LabelIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            return View(await GetViewModel(options, pager));

        }

        async Task<LabelIndexViewModel<Label>> GetViewModel(
            LabelIndexOptions options,
            PagerOptions pager)
        {

            var results = await _labelService
                .GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new LabelIndexViewModel<Label>
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }


}
