﻿using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Notifications;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface INotificationManager
    {

        Task<ICommandResult<T>> SendAsync<T>(INotification notification) where T : class;

    }
    
}
