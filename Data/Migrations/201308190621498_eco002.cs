namespace Econocom.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eco002 : DbMigration
    {
        public override void Up()
        {
            AddColumn("BENCHMARK.Client", "Statut", c => c.Int(nullable: false));
            DropColumn("BENCHMARK.Client", "Actif");
        }
        
        public override void Down()
        {
            AddColumn("BENCHMARK.Client", "Actif", c => c.Boolean(nullable: false));
            DropColumn("BENCHMARK.Client", "Statut");
        }
    }
}
