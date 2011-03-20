using System;
using System.IO;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Samples.SqlCe.Tests
{

    /// <summary>
    /// SqlCEDBHelper courtesy of Ayende Rahien from Rhino.Commons.Helpers
    /// Full code can be found here: https://svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/Helpers/SqlCEDbHelper.cs
    /// </summary>
    internal static class SqlCeDbHelper
    {
        private const string EngineTypeName = "System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe";
        private static Type _type;
        private static PropertyInfo _localConnectionString;
        private static MethodInfo _createDatabase;

        internal static void CreateDatabaseFile(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            if (_type == null)
            {
                _type = Type.GetType(EngineTypeName);
                _localConnectionString = _type.GetProperty("LocalConnectionString");
                _createDatabase = _type.GetMethod("CreateDatabase");
            }
            object engine = Activator.CreateInstance(_type);
            _localConnectionString
                .SetValue(engine, string.Format("Data Source='{0}';", filename), null);
            _createDatabase
                .Invoke(engine, new object[0]);
        }
    }

    /// <summary>
    /// The code below was also supplied by Ayende Rahien from Rhino.Commons.ForTesting
    /// You can find the complete code here: https://svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/ForTesting/NHibernateEmbeddedDBTestFixtureBase.cs
    /// Ayende has more code in the version in his repository, and you can
    /// expand alot more here, but for the sake of argument only the basics are here
    /// </summary>
    public class EmbeddedTestBase
    {
        public static string DatabaseFilename = "TempTestDB.sdf";

        protected static ISessionFactory SessionFactory;
        protected static FluentConfiguration Config;

        public FluentConfiguration Configure()
        {
            return Config ?? (Config =
                Fluently.Configure()
                // Bug fix: http://stackoverflow.com/questions/2361730/assertionfailure-null-identifier-fluentnh-sqlserverce
                .ExposeConfiguration(x => x.SetProperty("connection.release_mode", "on_close"))
                .Database(
                    MsSqlCeConfiguration.Standard
                    .ConnectionString(string.Format("Data Source={0};", DatabaseFilename))
                    .ShowSql()
                )
            );
        }

        /// <summary> 
        /// Initialize NHibernate and builds a session factory.
        /// Note, this is a costly call so it will be executed only one. 
        /// </summary> 
        protected void Initialize(params Assembly[] assemblies)
        {
            if (SessionFactory != null)
                return;

            if (Config == null)
            {
                Configure();
                foreach (var assembly in assemblies)
                {
                    var asm = assembly;
                    Config.Mappings(m => m.AutoMappings.Add(
                        AutoMap.Assembly(asm).Where(t => t.Namespace != null && t.Namespace.EndsWith("DomainObjects"))
                    ));
                }
            }

            SetupDb();

            SessionFactory = Config.BuildSessionFactory();
        }

        public void SetupDb()
        {
            SqlCeDbHelper.CreateDatabaseFile(DatabaseFilename);
            new SchemaExport(Config.BuildConfiguration()).Execute(true, true, false);
        }

        public ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }

    }
}