using System;
using System.Data;
using MySqlConnector;

namespace smart_stock.Services
{
   public class MySqlExecutor : IDbExecutor
   {
      private readonly string dbConnString;

      public MySqlExecutor(string dbConnString)
      {
         this.dbConnString = dbConnString;
      }

      public void Execute(Action<IDbConnection, IDbTransaction> op)
      {
         using (MySqlConnection connection = new MySqlConnection(dbConnString))
         {
            connection.Open();
            using (var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead))
            {
               try
               {
                  op.Invoke(connection, transaction);
                  transaction.Commit();
               }
               catch
               {
                  transaction.Rollback();
                  throw;
               }
            }
         }
      }

      public TResult Query<TResult>(Func<IDbConnection, TResult> op)
      {
         using (MySqlConnection connection = new MySqlConnection(dbConnString))
            return op.Invoke(connection);
      }
   }
}