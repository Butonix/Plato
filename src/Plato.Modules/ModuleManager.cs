﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Plato.Modules.Abstractions;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Threading.Tasks;

namespace Plato.Modules
{
    public class ModuleManager : IModuleManager
    {

        #region "Private Variables"

        readonly IModuleLocator _moduleLocator;
        readonly IModuleLoader _moduleLoader;       
        IEnumerable<IModuleDescriptor> _moduleDescriptors;

        static List<IModuleEntry> _moduleEntries;
        static List<Assembly> _loadedAssemblies;

        readonly string _contentRootPath;
        readonly string _virtualPathToModules;

        #endregion

        #region "Public ReadOnly Propertoes"

        public IEnumerable<IModuleEntry> AvailableModules
        {
            get
            {
                InitializeModules();
                return _moduleEntries;
            }
        }

        public IEnumerable<Assembly> AllAvailableAssemblies
        {
            get
            {
                InitializeModules();
                return _loadedAssemblies;
            }
        }

        #endregion

        #region "Constructor"

        public ModuleManager(
            IHostingEnvironment hostingEnvironment,
            IOptions<ModuleOptions> moduleOptions,
            IModuleLocator moduleLocator,
            IModuleLoader moduleLoader)
        {
            _moduleLocator = moduleLocator;
            _moduleLoader = moduleLoader;
            _contentRootPath = hostingEnvironment.ContentRootPath;
            _virtualPathToModules = moduleOptions.Value.VirtualPathToModulesFolder;            
            InitializeModules();
        }

        #endregion

        #region "Implementation"

        public async Task LoadModulesAsync()
        {

            if (_moduleDescriptors == null)
                throw new NullReferenceException(nameof(_moduleDescriptors));
                      
            foreach (var descriptor in _moduleDescriptors)
            {
                var assemblies = await _moduleLoader.LoadModuleAsync(descriptor);
                _moduleEntries.Add(new ModuleEntry()
                {
                    Descriptor = descriptor,
                    Assmeblies = assemblies                                  
                });
                _loadedAssemblies.AddRange(assemblies);
                
            }

        }

        public Task<IEnumerable<IModuleEntry>> LoadModulesAsync(string[] moduleIds)
        {
            InitializeModules();
            
            var moduless = _moduleEntries.Select(m => m.Descriptor.Id).ToList();

            var loadedModules = _moduleEntries
                .Where(m => moduless.Contains(m.Descriptor.Id));

            return Task.FromResult(loadedModules);
        }

        #endregion

        #region "Private Methods"

        async Task InitializeModules()
        {
            if (_moduleEntries == null)
            {
                _moduleEntries = new List<IModuleEntry>();
                _loadedAssemblies = new List<Assembly>();
                await LoadModuleDescriptors();
                await LoadModulesAsync();
            }
        }

        async Task LoadModuleDescriptors()
        {
            _moduleDescriptors = await _moduleLocator.LocateModulesAsync(
             new string[] { _contentRootPath + "\\" + _virtualPathToModules },
             "Module", "module.txt", false);
        }

        #endregion

    }
}
