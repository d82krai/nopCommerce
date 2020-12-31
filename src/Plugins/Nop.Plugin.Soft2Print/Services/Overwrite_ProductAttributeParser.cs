using Nop.Data;
using Nop.Services.Catalog;

namespace Nop.Plugin.Soft2Print.Services
{
    public partial class Overwrite_ProductAttributeParser : Nop.Services.Catalog.ProductAttributeParser, IProductAttributeParser
    {

        #region Fields
        private readonly IDbContext _context;
        private readonly IProductAttributeService _productAttributeService;
        private readonly Services.ProductAttributeService _s2p_productAttributeService;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">DB context</param>
        /// <param name="productAttributeService">Product attribute service</param>
        public Overwrite_ProductAttributeParser(IDbContext context,
            IProductAttributeService productAttributeService,
            Services.ProductAttributeService s2p_productAttributeService) : base(context, productAttributeService)
        {
            this._context = context;
            this._productAttributeService = productAttributeService;
            this._s2p_productAttributeService = s2p_productAttributeService;
        }

        public override bool AreProductAttributesEqual(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes, bool ignoreQuantity = true)
        {
            var s2pAttributeData1 = this._s2p_productAttributeService.GetS2PProductAttributes(attributesXml1);
            var s2pAttributeData2 = this._s2p_productAttributeService.GetS2PProductAttributes(attributesXml2);
            if (s2pAttributeData1 != null || s2pAttributeData2 != null)
            {
                if (s2pAttributeData1 != null && s2pAttributeData2 != null)
                    if (s2pAttributeData1.ProjectID.HasValue && s2pAttributeData2.ProjectID.HasValue)
                        if (s2pAttributeData1.ProjectID.Value == s2pAttributeData2.ProjectID.Value)
                            return base.AreProductAttributesEqual(attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity);

                return false;
            }

            return base.AreProductAttributesEqual(attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity);
        }

        #endregion


    }
}
