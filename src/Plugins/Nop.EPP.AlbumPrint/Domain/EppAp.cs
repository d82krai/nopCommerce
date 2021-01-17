using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.EPP.AlbumPrint.Domain
{
    public class EppAp : BaseEntity
    {
        public int ProductId { get; set; }
        public int ShoppingCartId { get; set; }
        public int? OrderId { get; set; }
        public string CloudFolderId { get; set; }
        public string CloudProvider { get; set; }
        public int CustomerId { get; set; }
        public DateTime AddedToShoppingCartOn { get; set; }
        public DateTime? AddedToOrderOn { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
