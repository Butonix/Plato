﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Plato.Shell.Models
{
    public class ShellContext : IDisposable
    {

        private bool _disposed = false;

        public bool IsActivated;

        public ShellSettings Settings { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public ShellDescriptor Descriptor { get; set; }

        public IServiceScope CreateServiceScope()
        {
            return ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }

        #region IDisposable Support

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                Settings = null;
                ServiceProvider = null;

                // Disposes all the services registered for this shell
                (ServiceProvider as IDisposable)?.Dispose();
                IsActivated = false;

                _disposed = true;
            }
        }

        ~ShellContext()
        {
            Dispose(false);
        }

        #endregion

    }
}
