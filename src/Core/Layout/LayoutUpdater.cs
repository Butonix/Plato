﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout
{

    public interface ILayoutUpdater
    {

        Task<ILayoutUpdater> GetLayoutAsync(Controller controller);

        Task<ILayoutUpdater> UpdateLayoutAsync(
            Func<LayoutViewModel, Task<LayoutViewModel>> configure);

        Task<List<ILayoutZoneView>> UpdateZoneAsync(
            IEnumerable<ILayoutZoneView> zone,
            Action<List<ILayoutZoneView>> configure);
        
        Task<List<ILayoutZoneView>> GetZoneAsync(string zoneName);

    }

    public class LayoutUpdater : ILayoutUpdater
    {

        private LayoutViewModel _model;
        private Controller _controller;

        private readonly ILogger<LayoutUpdater> _logger;
     
        public LayoutUpdater(
            ILogger<LayoutUpdater> logger)
        {
            _logger = logger;
        }

        public Task<ILayoutUpdater> GetLayoutAsync(Controller controller)
        {

            _controller = controller;

            // Ensure our controller implements LayoutViewModel
            var model = _controller.ViewData.Model as LayoutViewModel;
            _model = model ?? null;

            return Task.FromResult((ILayoutUpdater)this);

        }

        public async Task<ILayoutUpdater> UpdateLayoutAsync(
            Func<LayoutViewModel, Task<LayoutViewModel>> configure)
        {
            
            // We always need a model to invoke configuration
            if (_model == null)
            {
                return this;
            }

            // Configure model
            if (configure != null)
            {
                _model = await configure.Invoke(_model);
            }

            // Attempt to update controllers model to reflect configuration
            try
            {
                await _controller.TryUpdateModelAsync(_model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            return this;

        }

        public Task<List<ILayoutZoneView>> GetZoneAsync(string zoneName)
        {

            // We always need a model to invoke configuration
            if (_model == null)
            {
                return null;
            }

            // Use reflection to get models property value
            var propValue = _model
                .GetType()
                .GetProperty(zoneName)
                ?.GetValue(_model, null);

            // Ensure we are working with a list of positioned views
            if (propValue is IEnumerable<ILayoutZoneView> views)
            {
                return Task.FromResult(views.ToList());
            }

            return Task.FromResult(new List<ILayoutZoneView>());

        }

        public Task<List<ILayoutZoneView>> UpdateZoneAsync(
            IEnumerable<ILayoutZoneView> zone,
            Action<List<ILayoutZoneView>> configure)
        {

            // The zone may not have been initialized
            if (zone == null)
            {
                zone = new List<ILayoutZoneView>();
            }

            // Ensures we can easily perform work within configure
            var list = zone.ToList();

            // Configure the zone
            configure(list);

            // Order configured zone
            var orderedViews = list.ToList().OrderBy(v => v.Position.Order);

            // Return the ordered zone
            return Task.FromResult(orderedViews.ToList());

        }

    }

}
