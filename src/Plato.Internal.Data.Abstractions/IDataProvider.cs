﻿using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IDataProvider : IDisposable
    {
        IDbConnection Connection { get; }

        //IDataReader ExecuteReader(string sql, params object[] args);

        Task<DbDataReader> ExecuteReaderAsync(string sql, params object[] args);
        
        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        Task<T> ExecuteAsync<T>(string sql, params object[] args);

        void HandleException(Exception x);

        event DbEventHandlers.DbExceptionEventHandler OnException;

    }
}
