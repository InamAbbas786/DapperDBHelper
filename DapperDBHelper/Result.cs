using System;
using System.Collections.Generic;
using System.Text;

namespace DapperDBHelper
{
    public struct Result
    {
        public ResultStatus Status { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }
    public enum ResultStatus
    {
        Success = 100,
        Error = 200,
        NotFound = 300,
        Warning = 400,
        InProcess = 500,
        AlreadyExist = 600
    };

    public enum RDBMSProvider 
    { 
        MSSQLServer, 
        MySQL, 
        PostgreSQL, 
        Oracle, 
        Firebird, 
        SQLite 
    }
    public enum DataRetriveTypeEnum
    {
        FirstOrDefault,
        List
    }
}
