using Migrator.Framework;
using System.Data;

namespace Samples.SqlCe.Migrations
{
    [Migration(201103212048000)]
    public class CreateUserTable : Migration
    {
        public override void Up()
        {
            Database.AddTable("\"User\"",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("FirstName", DbType.String),
                new Column("LastName", DbType.String),
                new Column("EmailAddress", DbType.String),
                new Column("Username", DbType.String)
            );
        }

        public override void Down()
        {
            Database.RemoveTable("\"User\"");
        }
    }
}
