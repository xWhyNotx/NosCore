﻿using Microsoft.EntityFrameworkCore;
using NosCore.Configuration;
using NosCore.Database;
using NosCore.Shared.Logger;
using System;
using System.Data.SqlClient;

namespace NosCore.DAL
{
    public class DataAccessHelper
    {
        private static DataAccessHelper instance;

        private DataAccessHelper() { }

        public static DataAccessHelper Instance
        {
            get
            {
                return instance ?? (instance = new DataAccessHelper());
            }
        }

        #region Members

        private SqlConnectionConfiguration _conn;

        #endregion

        #region Methods

        /// <summary>
        /// Creates new instance of database context.
        /// </summary>
        public NosCoreContext CreateContext()
        {
            return new NosCoreContext(_conn);
        }

        public void Initialize(SqlConnectionConfiguration Database)
        {
            _conn = Database;
            using (NosCoreContext context = CreateContext())
            {
                try
                {
                    context.Database.Migrate();
                    context.Database.GetDbConnection().Open();
                    Logger.Log.Info(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.DATABASE_INITIALIZED));
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("Database Error", ex);
                    Logger.Log.Error(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.DATABASE_NOT_UPTODATE));
                    throw;
                }
            }
        }

        public void EnsureDeleted(SqlConnectionConfiguration Database)
        {
            _conn = Database;
            using (NosCoreContext context = CreateContext())
            {
                context.Database.EnsureDeleted();
            }
        }

        #endregion
    }
}