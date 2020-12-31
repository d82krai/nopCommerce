using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core;
using Nop.Data.Mapping;
using System;

namespace Nop.Plugin.Soft2Print.Data.Entities
{
    public class S2P_ProjectAttributes : BaseEntity
    {
        public class Map : NopEntityTypeConfiguration<S2P_ProjectAttributes>
        {
            #region Methods

            /// <summary>
            /// Configures the entity
            /// </summary>
            /// <param name="builder">The builder to be used to configure the entity</param>
            public override void Configure(EntityTypeBuilder<S2P_ProjectAttributes> builder)
            {
                builder.ToTable(nameof(S2P_ProjectAttributes));
                builder.HasKey(m => m.Id);

                //Map the additional properties
                builder.Property(m => m.Created);
                builder.Property(m => m.ProjectID);
                builder.Property(m => m.ProductID);
                builder.Property(m => m.Attributes);
            }

            #endregion
        }

        public S2P_ProjectAttributes()
        {
            Id = Guid.NewGuid();
            this.Created = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public int ProjectID { get; set; }
        public int ProductID { get; set; }
        public string Attributes { get; set; }
    }
}
