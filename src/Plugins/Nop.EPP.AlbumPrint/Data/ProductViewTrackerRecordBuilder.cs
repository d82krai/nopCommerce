using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.EPP.AlbumPrint.Domain;

namespace Nop.EPP.AlbumPrint.Data
{
    public class ProductViewTrackerRecordBuilder : NopEntityBuilder<ProductViewTrackerRecord>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            //map the primary key (not necessary if it is Id field)
            table.WithColumn(nameof(ProductViewTrackerRecord.Id)).AsInt32().PrimaryKey()
            //map the additional properties as foreign keys
            //.WithColumn(nameof(ProductViewTrackerRecord.ProductId)).AsInt32().ForeignKey<Product>(onDelete: Rule.Cascade)
            //.WithColumn(nameof(ProductViewTrackerRecord.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.Cascade)
            //avoiding truncation/failure
            //so we set the same max length used in the product name
            .WithColumn(nameof(ProductViewTrackerRecord.ProductName)).AsString(400)
            //not necessary if we don't specify any rules
            .WithColumn(nameof(ProductViewTrackerRecord.IpAddress)).AsString()
            .WithColumn(nameof(ProductViewTrackerRecord.IsRegistered)).AsInt32();
        }
    }
}
