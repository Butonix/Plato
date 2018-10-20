﻿using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    #region "EntityReplyQuery"

    public class EntityReplyQuery<TModel> : DefaultQuery<TModel> where TModel : class
    {

        private readonly IStore<TModel> _store;

        public EntityReplyQuery(IStore<TModel> store)
        {
            _store = store;
        }

        public EntityReplyQueryParams Params { get; set; }

        public override IQuery<TModel> Select<TParams>(Action<TParams> configure)
        {
            var defaultParams = new TParams();
            configure(defaultParams);
            Params = (EntityReplyQueryParams)Convert.ChangeType(defaultParams, typeof(EntityReplyQueryParams));
            return this;
        }
        
        public override async Task<IPagedResults<TModel>> ToList()
        {
            var builder = new EntityReplyQueryBuilder<TModel>(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.Keywords.Value
            );

            return data;
        }
    }

    #endregion

    #region "EntityReplyQueryParams"

    public class EntityReplyQueryParams
    {


        private WhereInt _id;
        private WhereInt _entityId;
        private WhereString _keywords;
        private WhereBool _isPrivate;
        private WhereBool _isSpam;
        private WhereBool _isPinned;
        private WhereBool _isDeleted;
        private WhereBool _isClosed;
        private WhereInt _createdUserId;
        private WhereDate _createdDate;
        private WhereInt _modifiedUserId;
        private WhereDate _modifiedDate;
        
        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt EntityId
        {
            get => _entityId ?? (_entityId = new WhereInt());
            set => _entityId = value;
        }
        
        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }

        public WhereBool IsPrivate
        {
            get => _isPrivate ?? (_isPrivate = new WhereBool());
            set => _isPrivate = value;
        }

        public WhereBool IsSpam
        {
            get => _isSpam ?? (_isSpam = new WhereBool());
            set => _isSpam = value;
        }

        public WhereBool IsPinned
        {
            get => _isPinned ?? (_isPinned = new WhereBool());
            set => _isPinned = value;
        }

        public WhereBool IsDeleted
        {
            get => _isDeleted ?? (_isDeleted = new WhereBool());
            set => _isDeleted = value;
        }

        public WhereBool IsClosed
        {
            get => _isClosed ?? (_isClosed = new WhereBool());
            set => _isClosed = value;
        }

        public WhereInt CreatedUserId
        {
            get => _createdUserId ?? (_createdUserId = new WhereInt());
            set => _createdUserId = value;
        }

        public WhereDate CreatedDate
        {
            get => _createdDate ?? (_createdDate = new WhereDate());
            set => _createdDate = value;
        }

        public WhereInt ModifiedUserId
        {
            get => _modifiedUserId ?? (_modifiedUserId = new WhereInt());
            set => _modifiedUserId = value;
        }

        public WhereDate ModifiedDate
        {
            get => _modifiedDate ?? (_modifiedDate = new WhereDate());
            set => _modifiedDate = value;
        }

    }

    #endregion

    #region "EntityReplyQueryBuilder"

    public class EntityReplyQueryBuilder<TModel> : IQueryBuilder where TModel : class
    {
        #region "Constructor"

        private readonly string _entityRepliesTableName;
        private readonly string _usersTableName;

        private readonly EntityReplyQuery<TModel> _query;

        public EntityReplyQueryBuilder(EntityReplyQuery<TModel> query)
        {
            _query = query;
            _entityRepliesTableName = GetTableNameWithPrefix("EntityReplies");
            _usersTableName = GetTableNameWithPrefix("Users");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT ")
                .Append(BuildPopulateSelect())
                .Append(" FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            sb.Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(r.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }
        
        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("r.*,")
                .Append("c.UserName AS CreatedUserName,")
                .Append("c.NormalizedUserName AS CreatedNormalizedUserName,")
                .Append("c.DisplayName AS CreatedDisplayName,")
                .Append("c.FirstName AS CreatedFirstName,")
                .Append("c.LastName AS CreatedLastName,")
                .Append("c.Alias AS CreatedAlias,")
                .Append("m.UserName AS ModifiedUserName,")
                .Append("m.NormalizedUserName AS ModifiedNormalizedUserName,")
                .Append("m.DisplayName AS ModifiedDisplayName,")
                .Append("m.FirstName AS ModifiedFirstName,")
                .Append("m.LastName AS ModifiedLastName,")
                .Append("m.Alias AS ModifiedAlias");
            return sb.ToString();

        }

        string BuildTables()
        {
            
            var sb = new StringBuilder();
            sb.Append(_entityRepliesTableName)
                .Append(" r ");

            // join created user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" c ON r.CreatedUserId = c.Id ");
            
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" m ON r.ModifiedUserId = m.Id");
            
            return sb.ToString();

        }

        #endregion

        #region "Private Methods"

        private string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.TablePrefix)
                ? _query.TablePrefix + tableName
                : tableName;
        }
        
        private string BuildWhereClause()
        {
            var sb = new StringBuilder();
            
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("r.Id"));
            }

            if (_query.Params.EntityId.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("EntityId"));
            }

            if (!string.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("Keywords"));
            }
            
         
            return sb.ToString();

        }


        string GetQualifiedColumnName(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            return columnName.IndexOf('.') >= 0
                ? columnName
                : "r." + columnName;
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(GetQualifiedColumnName(sortColumn.Key));
                if (sortColumn.Value != OrderBy.Asc)
                    sb.Append(" DESC");
                if (i < _query.SortColumns.Count - 1)
                    sb.Append(", ");
                i += 1;
            }
            return sb.ToString();
        }

        #endregion
    }

    #endregion

}
