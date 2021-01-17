using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;
using Nop.Data.Migrations;
using Nop.EPP.AlbumPrint.Domain;

namespace Nop.EPP.AlbumPrint.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/05/27 08:40:55:1687541", "Other.ProductViewTracker base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<ProductViewTrackerRecord>(Create);
        }
    }
}
