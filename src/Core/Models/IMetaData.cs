﻿using System;
using System.Collections.Generic;
using PlatoCore.Abstractions;

namespace PlatoCore.Models
{

    /// <summary>
    /// Represents some abstract meta data that can be stored on types that implement this interface.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IMetaData<TModel> where TModel : class
    {
        IDictionary<Type, ISerializable> MetaData { get; }

        IEnumerable<TModel> Data { get; set; }

        void AddOrUpdate<T>(T obj) where T : class;

        void AddOrUpdate(Type type, ISerializable obj);

        T GetOrCreate<T>() where T : class;

    }

}
