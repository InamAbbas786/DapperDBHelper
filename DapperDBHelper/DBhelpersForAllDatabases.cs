using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDBHelper
{
    public class DBHelpersForAllDatabases
    {

        private IDbConnection _connection;

        public DBHelpersForAllDatabases(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<Result> QueryAsyncList<T>(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }

                var result = await _connection.QueryAsync<T>(sqlQuery, param, commandType: commandType);
                return new Result() { Data = result.ToList(), Status = ResultStatus.Success, Message = "Success" };
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

                var result = await _connection.QueryAsync<T>(sqlQuery, param, commandType: commandType);
                return new Result() { Data = result.SingleOrDefault(), Status = ResultStatus.Success, Message = "Success" };
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

                var result = _connection.Query<T>(sqlQuery, param, commandType: commandType);
                return new Result() { Data = result.ToList(), Status = ResultStatus.Success, Message = "Success" };
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

                var result = _connection.Query<T>(sqlQuery, param, commandType: commandType);
                return new Result() { Data = result.SingleOrDefault(), Status = ResultStatus.Success, Message = "Success" };
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

                var multi = await _connection.QueryMultipleAsync(sqlQuery, param, commandType: commandType);
                var result = new Dictionary<string, dynamic>();
                int i = 0;
                while (!multi.IsConsumed)
                {
                    var item = await multi.ReadAsync<dynamic>();
                    result.Add(tableNames[i], item);
                    i++;
                }

                return new Result() { Data = result, Status = ResultStatus.Success, Message = "Success" };
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

                var multi = _connection.QueryMultiple(sqlQuery, param, commandType: commandType);
                var result = new Dictionary<string, dynamic>();
                int i = 0;
                while (!multi.IsConsumed)
                {
                    var item = multi.Read<dynamic>();
                    result.Add(tableNames[i], item);
                    i++;
                }

                return new Result() { Data = result, Status = ResultStatus.Success, Message = "Success" };
            }
            catch (Exception ex)
            {
                return new Result() { Data = "Exception", Status = ResultStatus.Warning, Message = ex.Message };
            }
        }

        public async Task<Result> ExecuteAsync(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var result = await _connection.QueryAsync<dynamic>(sqlQuery, param, commandType: commandType);
                         transaction.Commit();
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

        public Result Execute(string sqlQuery, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return new Result() { Status = ResultStatus.Warning, Message = "Query is null", Data = "" };
                }

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var result = _connection.Query<dynamic>(sqlQuery, param, commandType: commandType);
                        transaction.Commit();
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
    }
}


