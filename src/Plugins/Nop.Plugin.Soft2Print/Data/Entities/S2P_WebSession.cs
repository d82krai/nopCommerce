using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core;
using Nop.Data.Mapping;
using System;

namespace Nop.Plugin.Soft2Print.Data.Entities
{
    public class S2P_WebSession : BaseEntity
    {
        public class Map : NopEntityTypeConfiguration<S2P_WebSession>
        {
            #region Methods

            /// <summary>
            /// Configures the entity
            /// </summary>
            /// <param name="builder">The builder to be used to configure the entity</param>
            public override void Configure(EntityTypeBuilder<S2P_WebSession> builder)
            {
                builder.ToTable(nameof(S2P_WebSession));
                builder.HasKey(m => m.Id);

                //Map the additional properties
                builder.Property(m => m.CustomerId);
                builder.Property(m => m.Created);
                builder.Property(m => m.Valid);
            }

            #endregion
        }

        public S2P_WebSession()
        {
            this.Id = Guid.NewGuid();
            this.Created = DateTime.Now;
        }

        public virtual Guid Id { get; set; }
        public virtual int CustomerId { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual bool Valid { get; set; }
    }
}
