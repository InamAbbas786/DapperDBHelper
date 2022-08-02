using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperDBHelper
{
    public class DBHelpers
    {

        private string ConnectionString = "";
        private RDBMSProvider provider = RDBMSProvider.MSSQLServer;
        private bool Trans = false;
        private bool buffer = true;
        private int CmdTimeOut = 0;
        public DBHelpers(string connectionString, RDBMSProvider DBProviders = RDBMSProvider.MSSQLServer, bool transaction = false, bool buffered = true, int commandTimeOut = 0)
        {
            this.Trans = transaction;
            this.provider = DBProviders;
            this.buffer = buffered;
            this.CmdTimeOut = commandTimeOut;
            this.ConnectionString = connectionString;
        }
        public async Task<Result> QueryAsyncList<T>(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var result = await conn.QueryAsync<T>(sqlQuery, param);
                    await conn.CloseAsync();
                    if (result != null)
                        return new Result() { Data = result.ToList(), Status = ResultStatus.Success, Message = "Success" };
                    else
                        return new Result() { Data = null, Status = ResultStatus.NotFound, Message = "Data Not Found" };
                }
            }
            catch (Exception ex)
            {
                return new Result() { Data = "Exception", Status = ResultStatus.Warning, Message = ex.Message };
            }
        }
        public async Task<Result> QueryAsync<T>(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var result = await conn.QueryAsync<T>(sqlQuery, param);
                    await conn.CloseAsync();
                    if (result != null)
                        return new Result() { Data = result.SingleOrDefault(), Status = ResultStatus.Success, Message = "Success" };

                    else
                        return new Result() { Data = null, Status = ResultStatus.NotFound, Message = "Data Not Found" };
                }
            }
            catch (Exception ex)
            {
                return new Result() { Data = "Exception", Status = ResultStatus.Warning, Message = ex.Message };
            }
        }
        public Result QueryList<T>(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var result = conn.Query<T>(sqlQuery, param);
                    if (result != null)
                        return new Result() { Data = result.ToList(), Status = ResultStatus.Success, Message = "Success" };
                    else
                        return new Result() { Data = null, Status = ResultStatus.NotFound, Message = "Data Not Found" };
                }
            }
            catch (Exception ex)
            {
                return new Result() { Data = "Exception", Status = ResultStatus.Warning, Message = ex.Message };
            }
        }
        public Result Query<T>(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var result = conn.Query<T>(sqlQuery, param);
                    if (result != null)
                        return new Result() { Data = result.SingleOrDefault(), Status = ResultStatus.Success, Message = "Success" };
                    else
                        return new Result() { Data = null, Status = ResultStatus.NotFound, Message = "Data Not Found" };
                }
            }
            catch (Exception ex)
            {
                return new Result() { Data = "Exception", Status = ResultStatus.Warning, Message = ex.Message };
            }
        }
        public async Task<Result> QueryMultipleAsync(string sqlQuery, object param = null, List<string> tableNames = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var multi = await conn.QueryMultipleAsync(sqlQuery, param);
                    var result = new Dictionary<string, dynamic>();
                    int i = 0;
                    while (multi.IsConsumed == false)
                    {
                        var item = await multi?.ReadAsync<dynamic>();
                        result.Add(tableNames[i], item);
                        i++;
                    }
                    await conn.CloseAsync();
                    if (result != null)
                        return new Result() { Data = result, Status = ResultStatus.Success, Message = "Success" };

                    else
                        return new Result() { Data = null, Status = ResultStatus.NotFound, Message = "Data Not Found" };
                }
            }
            catch (Exception ex)
            {
                return new Result() { Data = "Exception", Status = ResultStatus.Warning, Message = ex.Message };
            }
        }
        public Result QueryMultiple(string sqlQuery, object param = null, List<string> tableNames = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var multi = conn.QueryMultiple(sqlQuery, param);
                    var result = new Dictionary<string, dynamic>();
                    int i = 0;
                    while (multi.IsConsumed == false)
                    {
                        var item = multi?.Read<dynamic>();
                        result.Add(tableNames[i], item);
                        i++;
                    }
                    conn.Close();
                    if (result != null)
                        return new Result() { Data = result, Status = ResultStatus.Success, Message = "Success" };

                    else
                        return new Result() { Data = null, Status = ResultStatus.NotFound, Message = "Data Not Found" };
                }
            }
            catch (Exception ex)
            {
                return new Result() { Data = "Exception", Status = ResultStatus.Warning, Message = ex.Message };
            }
        }
        /// <summary>
        ///  Execute Method use for INSERT UPDATE DELETE
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="param"></param>
        /// <returns>After Execution Return Single Table is Best Practices</returns>
        public async Task<Result> ExecuteAsync(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        await conn.OpenAsync();
                        using (var transaction = await conn.BeginTransactionAsync())
                        {
                            try
                            {
                                var result = await conn.QueryAsync<dynamic>(sqlQuery, param);
                                await transaction.CommitAsync();
                                await conn.CloseAsync();
                                return new Result()
                                {
                                    Data = result.SingleOrDefault(),
                                    Status = ResultStatus.Success,
                                    Message = "Success"
                                };
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                return new Result()
                                {
                                    Data = "Exception",
                                    Status = ResultStatus.Warning,
                                    Message = ex.Message

                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    Data = "Exception",
                    Status = ResultStatus.Warning,
                    Message = ex.Message

                };
            }
        }
        /// <summary>
        /// Execute Method use for INSERT UPDATE DELETE
        /// </summary>
        /// <param name="sqlQuery"> EXEC SP @parameter  and query like INSERT INTO TABLE (columns) VALUES(@columns), select * from table</param>
        /// <param name="param">Patameter pass param: new { ID = ID } </param>
        /// <returns>After Execution Return Single Table is Best Practices</returns>
        public Result Execute(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        using (var transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                var result = conn.Query<dynamic>(sqlQuery, param);
                                transaction.Commit();
                                conn.Close();
                                return new Result()
                                {
                                    Data = result.SingleOrDefault(),
                                    Status = ResultStatus.Success,
                                    Message = "Success"
                                };
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return new Result()
                                {
                                    Data = "Exception",
                                    Status = ResultStatus.Warning,
                                    Message = ex.Message

                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    Data = "Exception",
                    Status = ResultStatus.Warning,
                    Message = ex.Message

                };
            }
        }
        private static IDbConnection CreateConnection(RDBMSProvider rdbmsProvider, string connectionString)
        {
            IDbConnection connection = null;

            switch (rdbmsProvider)
            {
                case RDBMSProvider.MSSQLServer:
                    connection = new SqlConnection(connectionString);
                    break;
                case RDBMSProvider.MySQL:
                    connection = new MySqlConnection(connectionString);
                    break;
                case RDBMSProvider.PostgreSQL:
                    connection = new NpgsqlConnection(connectionString);
                    break;
                case RDBMSProvider.Oracle:
                    //connection = new OracleConnection(connectionString);
                    break;
                case RDBMSProvider.Firebird:
                    connection = new FbConnection(connectionString);
                    break;
                case RDBMSProvider.SQLite:
                    connection = new SqliteConnection(connectionString);
                    break;
                default:
                    break;
            }

            return connection;
        }
    }
}


