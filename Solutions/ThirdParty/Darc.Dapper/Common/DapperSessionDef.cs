namespace Darc.Dapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using Common;
    using Core.Entities;
    using Expressions;
    using global::Dapper;
    using DbType = Common.DbType;

    public partial class DapperSession
    {
        public T Save<T>(T t, int? commandTimeout = null)
            where T : EntityBase, new()
        {
            return (T) ExecProcess(_context.DbConnection, conn =>
            {
                var tbName = CommonUtil.GetTableName<T>();
                var columns = CommonUtil.GetExecutedColumns(t);
                var primaryKey = CommonUtil.GetPrimaryKey<T>();
                var sequence = CommonUtil.GetSequence<T>();

                DynamicParameters parameters;
                var sql = CreateInsertSql(tbName, columns, _context.ParamPrefix, primaryKey, out parameters);
                string idSql;
                switch (_context.DbType)
                {
                    case DbType.MySql:
                        idSql = @"select last_insert_id() as Id";
                        break;
                    case DbType.SqlServerCe:
                        idSql = @"";
                        break;
                    case DbType.PostgreSql:
                        idSql = @"";
                        break;
                    case DbType.Oracle:
                        idSql = $"select {sequence}.currval as Id from dual";
                        if (t.Id == null && !string.IsNullOrEmpty(sequence))
                        {
                            sql = sql.Replace(_context.ParamPrefix + primaryKey, $"{sequence}.nextval");
                        }
                        break;
                    case DbType.SQLite:
                        idSql = @"select last_insert_rowid() as Id";
                        break;
                    case DbType.SqlServer:
                        idSql = @"SELECT CAST(SCOPE_IDENTITY() as int) as Id";
                        break;
                    default:
                        idSql = @"SELECT CAST(SCOPE_IDENTITY() as int) as Id";
                        break;
                }

                var flag = conn.Execute(sql, parameters, _context.DbTransaction, commandTimeout);

                if (flag != 0)
                {
                    if (t.Id == null)
                    {
                        var data = conn.Query<T>(idSql).SingleOrDefault();
                        if (data != null) t.Id = data.Id;
                    }
                    return t;
                }
                return null;
            });
        }

        public bool SaveBatch<T>(IList<T> dataList, int? commandTimeout = null)
            where T : class, new()
        {
            return (bool) ExecProcess(_context.DbConnection, conn =>
            {
                var result = false;
                var tbName = CommonUtil.GetTableName<T>();
                var primarayKey = CommonUtil.GetPrimaryKey<T>();
                var columns = CommonUtil.GetExecutedColumns<T>();

                DynamicParameters parameters;

                if (_context.DbTransaction == null)
                {
                    var trans = conn.BeginTransaction();

                    try
                    {
                        var flag =
                            conn.Execute(
                                CreateInsertSql(tbName, columns, _context.ParamPrefix, primarayKey, out parameters),
                                dataList, trans, commandTimeout);

                        result = flag > 0;
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }

                    return result;
                }

                var flagT =
                    conn.Execute(CreateInsertSql(tbName, columns, _context.ParamPrefix, primarayKey, out parameters),
                        dataList, _context.DbTransaction, commandTimeout);

                return flagT > 0;
            });
        }

        public bool Delete<T>(Expression<Func<T, bool>> predicate, int? commandTimeout = null) where T : class, new()
        {
            return (bool) ExecProcess(_context.DbConnection, conn =>
            {
                var tbName = CommonUtil.GetTableName<T>();

                var condition = new List<T>().AsQueryable().WhereC(predicate);
                var sql = $"DELETE FROM {tbName} WHERE {condition}";
                var flag = conn.Execute(sql, transaction: _context.DbTransaction, commandTimeout: commandTimeout);

                return flag > 0;
            });
        }

        public bool Update<T>(T t, int? commandTimeout = null)
            where T : class, new()
        {
            return (bool) ExecProcess(_context.DbConnection, conn =>
            {
                var tbName = CommonUtil.GetTableName<T>();
                var columns = CommonUtil.GetExecutedColumns<T>();
                var primaryKey = CommonUtil.GetPrimaryKey<T>();

                var flag = conn.Execute(CreateUpdateSql(tbName, primaryKey, columns, _context.ParamPrefix),
                    t, _context.DbTransaction, commandTimeout);

                return flag > 0;
            });
        }

        public TResult Get<TResult>(object id) where TResult : EntityBase, new()
        {
            return (TResult) ExecProcess(_context.DbConnection, conn =>
            {
                var tbName = CommonUtil.GetTableName<TResult>();
                var columns = CommonUtil.GetExecutedColumns<TResult>();
                var primaryKey = CommonUtil.GetPrimaryKey<TResult>();

                var data = conn.Query<TResult>(CreateGetSql(tbName, primaryKey,
                    columns, _context.ParamPrefix), new TResult {Id = id}, _context.DbTransaction)
                    .SingleOrDefault();
                return data;
            });
        }

        public IList<TResult> Find<TResult>(Expression<Func<TResult, bool>> predicate) where TResult : class, new()
        {
            return (IList<TResult>) ExecProcess(_context.DbConnection, conn =>
            {
                var tbName = CommonUtil.GetTableName<TResult>();
                var columns = CommonUtil.GetExecutedColumns<TResult>();

                var condition = new List<TResult>().AsQueryable().WhereC(predicate);

                var data = conn.Query<TResult>(CreateFindSql(tbName, columns, condition),
                    transaction: _context.DbTransaction).ToList();
                return data;
            });
        }

        public IList<TResult> All<TResult>() where TResult : class, new()
        {
            return (IList<TResult>) ExecProcess(_context.DbConnection, conn =>
            {
                var tbName = CommonUtil.GetTableName<TResult>();
                var columns = CommonUtil.GetExecutedColumns<TResult>();

                var data = conn.Query<TResult>(CreateAllSql(tbName, columns),
                    transaction: _context.DbTransaction).ToList();

                return data;
            });
        }

        /// <summary>
        ///     Execute sql
        /// </summary>
        /// <param name="func"></param>
        public void Call(Action<IDbConnection> func)
        {
            using (var conn = _context.DbConnection)
            {
                func(conn);
            }
        }

        /// <summary>
        ///     Execute sql
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public TResult Call<TResult>(Func<IDbConnection, TResult> func)
        {
            using (var conn = _context.DbConnection)
            {
                return func(conn);
            }
        }

        /// <summary>
        ///     Execute sql
        /// </summary>
        /// <param name="func"></param>
        /// <param name="commandTimeout"></param>
        public void Call(Action<IDbConnection, IDbTransaction> func, int? commandTimeout = null)
        {
            using (var conn = _context.DbConnection)
            {
                var trans = conn.BeginTransaction();

                try
                {
                    func(conn, trans);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                }
            }
        }

        /// <summary>
        ///     Execute sql
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public TResult Call<TResult>(Func<IDbConnection, IDbTransaction, TResult> func,
            int? commandTimeout = null)
        {
            using (var conn = _context.DbConnection)
            {
                var result = default(TResult);
                var trans = conn.BeginTransaction();

                try
                {
                    result = func(conn, trans);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                }

                return result;
            }
        }

        public TResult Procedure<TResult>(string name, SqlMapper.IDynamicParameters parameters)
        {
            return default(TResult);
        }

        private void ExecProcess(IDbConnection conn, Action action)
        {
            if (conn.State == ConnectionState.Open)
            {
                action();
            }
            else
            {
                using (conn)
                {
                    action();
                }
            }
        }

        private object ExecProcess(IDbConnection conn, Func<IDbConnection, object> func)
        {
            if (conn.State == ConnectionState.Open) return func(conn);

            using (conn)
            {
                return func(conn);
            }
        }

        #region Generate Sql

        private static string CreateInsertSql(string tbName, IList<ParamColumnModel> colums,
            string paramPrefix, string primarayKey, out DynamicParameters parameters)
        {
            var sql = new StringBuilder();
            var param = new DynamicParameters();

            sql.Append($"INSERT INTO {tbName}(");
            for (var i = 0; i < colums.Count; i++)
            {
                sql.Append(i == 0 ? colums[i].ColumnName : $",{colums[i].ColumnName}");
            }
            sql.Append(")");

            sql.Append(" VALUES(");
            for (var i = 0; i < colums.Count; i++)
            {
                sql.Append(i == 0
                    ? $"{paramPrefix}{colums[i].FieldName}"
                    : $",{paramPrefix}{colums[i].FieldName}");

                switch (_context.DbType)
                {
                    case DbType.MySql:
                        param.Add(colums[i].FieldName, colums[i].FieldValue);
                        break;
                    case DbType.SqlServerCe:
                        break;
                    case DbType.PostgreSql:
                        break;
                    case DbType.Oracle:
                        if (primarayKey == colums[i].FieldName && !string.IsNullOrEmpty(colums[i].FieldValue))
                            param.Add(colums[i].FieldName, colums[i].FieldValue);
                        else if (primarayKey != colums[i].FieldName)
                            param.Add(colums[i].FieldName, colums[i].FieldValue);
                        break;
                    case DbType.SQLite:
                        break;
                    case DbType.SqlServer:
                        break;
                    default:
                        param.Add(colums[i].FieldName, colums[i].FieldValue);
                        break;
                }
            }
            sql.Append(")");
            parameters = param;

            return sql.ToString();
        }

        private static string CreateUpdateSql(string tbName, string primaryKey,
            IList<ParamColumnModel> colums, string paramPrefix)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE {tbName} SET ");

            for (var i = 0; i < colums.Count; i++)
            {
                sql.Append(i == 0
                    ? $"{colums[i].ColumnName}={paramPrefix}{colums[i].ColumnName}"
                    : $",{colums[i].ColumnName}={paramPrefix}{colums[i].ColumnName}");
            }

            sql.Append($" WHERE {primaryKey}={paramPrefix}{primaryKey}");

            return sql.ToString();
        }

        private static string CreateGetSql(string tbName, string primaryKey,
            IList<ParamColumnModel> colums, string paramPrefix)
        {
            var sql = new StringBuilder();

            sql.Append("SELECT ");
            for (var i = 0; i < colums.Count; i++)
            {
                sql.Append(i == 0 ? colums[i].ColumnName : $",{colums[i].ColumnName}");
            }

            sql.Append($" FROM {tbName} WHERE {primaryKey}={paramPrefix}{primaryKey}");

            return sql.ToString();
        }

        private static string CreateFindSql(string tbName, IList<ParamColumnModel> colums, string condition)
        {
            var sql = new StringBuilder();

            sql.Append("SELECT ");
            for (var i = 0; i < colums.Count; i++)
            {
                sql.Append(i == 0
                    ? colums[i].ColumnName.ToLower()
                    : $",{colums[i].ColumnName.ToLower()}");
            }

            sql.Append($" FROM {tbName} WHERE {condition}");

            return sql.ToString();
        }

        private static string CreateAllSql(string tbName, IList<ParamColumnModel> colums)
        {
            var sql = new StringBuilder();

            sql.Append("SELECT ");
            for (var i = 0; i < colums.Count; i++)
            {
                sql.Append(i == 0
                    ? colums[i].ColumnName.ToLower()
                    : $",{colums[i].ColumnName.ToLower()}");
            }

            sql.Append($" FROM {tbName}");

            return sql.ToString();
        }

        #endregion
    }
}