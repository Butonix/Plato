﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Navigation;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Discuss.Services
{
    
    public interface ITopicService
    {
        Task<IPagedResults<Topic>> Get(TopicIndexOptions options, PagerOptions pager);

    }

    public class TopicService : ITopicService
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly IPlatoRoleStore _roleStore;

        public TopicService(
            IContextFacade contextFacade,
            IEntityStore<Topic> topicStore,
            IFeatureFacade featureFacade,
            IPlatoRoleStore roleStore)
        {
            _contextFacade = contextFacade;
            _topicStore = topicStore;
            _featureFacade = featureFacade;
            _roleStore = roleStore;
        }

        public async Task<IPagedResults<Topic>> Get(TopicIndexOptions options, PagerOptions pager)
        {
            
            if (options == null)
            {
                options = new TopicIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get discuss features
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            var channelFeature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");

            // Get authenticated user for use within view adaptor below
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Get anonymous role for use within view adaptor below
            var anonymousRole = await _roleStore.GetByNameAsync(DefaultRoles.Anonymous);

            // Return tailored results
            return await _topicStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<EntityQueryParams>(q =>
                {

                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }
                    
                    // Restrict results via user role if the channels feature is enabled
                    if (channelFeature != null)
                    {
                        if (user != null)
                        {
                            q.RoleId.IsIn(user.UserRoles?.Select(r => r.Id).ToArray());
                        }
                        else
                        {
                            if (anonymousRole != null)
                            {
                                q.RoleId.Equals(anonymousRole.Id);
                            }
                        }
                    }
                    
                    if (options.ChannelId > 0)
                    {
                        q.CategoryId.Equals(options.ChannelId);
                    }
                    
                    if (options.LabelId > 0)
                    {
                        q.LabelId.Equals(options.LabelId);
                    }


                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();

                    //q.IsPinned.True();
                    
                    //if (!string.IsNullOrEmpty(filterOptions.Search))
                    //{
                    //    q.UserName.IsIn(filterOptions.Search).Or();
                    //    q.Email.IsIn(filterOptions.Search);
                    //}
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

        }

    }

}