using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Samples.SqlCe.Tests {

    /// <summary>
    /// SqlCEDBHelper courtesy of Ayende Rahien from Rhino.Commons.Helpers
    /// Full code can be found here: https://svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/Helpers/SqlCEDbHelper.cs
    /// </summary>
    internal static class SqlCEDbHelper {
        private static string engineTypeName = "System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe";
        private static Type type;
        private static PropertyInfo localConnectionString;
        private static MethodInfo createDatabase;
    
        internal static void CreateDatabaseFile(string filename) {
            if (File.Exists(filename))
                File.Delete(filename);

            if (type == null) {
                type = Type.GetType(engineTypeName);
    			localConnectionString = type.GetProperty("LocalConnectionString");
                createDatabase = type.GetMethod("CreateDatabase");
            }
    		object engine = Activator.CreateInstance(type);
    		localConnectionString
                .SetValue(engine, string.Format("Data Source='{0}';", filename), null);
    		createDatabase
                .Invoke(engine, new object[0]);
    	}
    }

    /// <summary>
    /// The code below was also supplied by Ayende Rahien from Rhino.Commons.ForTesting
    /// You can find the complete code here: https://svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/ForTesting/NHibernateEmbeddedDBTestFixtureBase.cs
    /// Ayende has more code in the version in his repository, and you can
    /// expand alot more here, but for the sake of argument only the basics are here
    /// </summary>
    public class EmbeddedTestBase {
        public static string DatabaseFilename = "TempDB.sdf";

        protected static ISessionFactory sessionFactory;
        protected static Configuration configuration;

        /// <summary> 
        /// Initialize NHibernate and builds a session factory 
        /// Note, this is a costly call so it will be executed only one. 
        /// </summary> 
        protected void FixtureInitalize(params Assembly[] assemblies) {
            if (sessionFactory != null)
                return;

            Hashtable properties = new Hashtable();
            properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlServerCeDriver");
            properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSqlCeDialect");
            properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            string connectionString = string.Format("Data Source={0};", DatabaseFilename);
            properties.Add("hibernate.connection.connection_string", connectionString);
            properties.Add("hibernate.show_sql", "true");
            properties.Add("hibernate.connection.release_mode", "on_close");

            configuration = new Configuration();
            configuration.Properties = properties;
            foreach (Assembly assembly in assemblies) {
                Console.WriteLine("Adding Assembly:" + assembly);
                configuration = configuration.AddAssembly(assembly);
            }
            sessionFactory = configuration.BuildSessionFactory();

        }

        public void SetupDb() {
            SqlCEDbHelper.CreateDatabaseFile(DatabaseFilename);
            try {
                new SchemaExport(configuration).Execute(false, true, false, true);
            }
            catch (Exception e) {
                Console.WriteLine("Error Message:" + e);
            }
        }

        public ISession CreateSession() {
            SetupDb();
            return sessionFactory.OpenSession();
        }

    }
}