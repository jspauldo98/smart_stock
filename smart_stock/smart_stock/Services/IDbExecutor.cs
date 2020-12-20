using System;
using System.Data;

namespace smart_stock.Services
{
   public interface IDbExecutor
   {
      /***
       * Wraps db update/inserts in a connection and transaction
       */
      void Execute(Action<IDbConnection, IDbTransaction> op);

      /***
       * Wraps db queries in a connection
       */
      TResult Query<TResult>(Func<IDbConnection, TResult> op);
   }
}