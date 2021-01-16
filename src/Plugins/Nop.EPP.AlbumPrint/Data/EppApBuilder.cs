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
    public class EppApBuilder : NopEntityBuilder<EppAp>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(EppAp.CloudFolderId)).AsString(1000)
            .WithColumn(nameof(EppAp.CloudProvider)).AsString(50)
            .WithColumn(nameof(EppAp.CustomerId)).AsInt32()
            .WithColumn(nameof(EppAp.ShoppingCartId)).AsInt32()
            .WithColumn(nameof(EppAp.OrderId)).AsInt32();
        }
    }
}
