using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Soft2Print.Services
{
    public partial class Overwrite_ShoppingCartService : Nop.Services.Orders.ShoppingCartService, IShoppingCartService
    {
        #region Fields
        private readonly ILogger _logger;

        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerService _customerService;
        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        private readonly Services.Soft2PrintAPIService _soft2PrintAPIService;
        private readonly Services.ProductAttributeService _s2p_productAttributeService;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sciRepository">Shopping cart repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="productService">Product settings</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeService">Checkout attribute service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="orderSettings">Order settings</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="permissionService">Permission service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="dateRangeService">Date range service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="productAttributeService">Product attribute service</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        public Overwrite_ShoppingCartService(CatalogSettings catalogSettings,
            IAclService aclService,
            IActionContextAccessor actionContextAccessor,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IRepository<ShoppingCartItem> sciRepository,
            IShippingService shippingService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,

            ILogger logger,
            Services.Soft2PrintAPIService soft2PrintAPIService,
            Services.ProductAttributeService s2p_productAttributeService) : base(catalogSettings,aclService,actionContextAccessor,checkoutAttributeParser,checkoutAttributeService,currencyService,customerService,dateRangeService,dateTimeHelper,eventPublisher,genericAttributeService,localizationService,permissionService,priceFormatter,productAttributeParser,productAttributeService,productService,sciRepository,shippingService,storeContext,storeMappingService,urlHelperFactory,urlRecordService,workContext,orderSettings,shoppingCartSettings)
        {
            this._logger = logger;
            this._sciRepository = sciRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._productService = productService;
            this._localizationService = localizationService;
            this._productAttributeParser = productAttributeParser;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._priceFormatter = priceFormatter;
            this._customerService = customerService;
            this._orderSettings = orderSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._dateRangeService = dateRangeService;
            this._storeMappingService = storeMappingService;
            this._genericAttributeService = genericAttributeService;
            this._productAttributeService = productAttributeService;
            this._dateTimeHelper = dateTimeHelper;
            this._soft2PrintAPIService = soft2PrintAPIService;
            this._s2p_productAttributeService = s2p_productAttributeService;
        }

        #endregion


        public override IList<string> GetShoppingCartItemWarnings(Customer customer, ShoppingCartType shoppingCartType, Product product, int storeId, string attributesXml, decimal customerEnteredPrice, DateTime? rentalStartDate = null, DateTime? rentalEndDate = null, int quantity = 1, bool addRequiredProducts = true, int shoppingCartItemId = 0, bool getStandardWarnings = true, bool getAttributesWarnings = true, bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true, bool getRentalWarnings = true)
        {
            var result = base.GetShoppingCartItemWarnings(customer, shoppingCartType, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, shoppingCartItemId, getStandardWarnings, getAttributesWarnings, getGiftCardWarnings, getRequiredProductWarnings, getRentalWarnings);

            var s2pAttr = this._s2p_productAttributeService.GetS2PProductAttributes(attributesXml);
            if (s2pAttr != null)
                if (!s2pAttr.JobID.HasValue)
                    // We do this check if we allow updateing of the shopping cart
                    if (!Environment.StackTrace.Contains("UpdateShoppingCartItem"))
                        result.Add(string.Format(_localizationService.GetResource(ResourceCollection.Key.ShoppingCart.ReconfirmRequired), _localizationService.GetResource("Common.Edit")));

            return result;
        }

        public override void MigrateShoppingCart(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
        {
            this._soft2PrintAPIService.MigrateToUser(fromCustomer.Id, toCustomer.Id);
            base.MigrateShoppingCart(fromCustomer, toCustomer, includeCouponCodes);
        }

        // TODO: overwrite so that attributes from soft2print alwais will survive
        //this.UpdateShoppingCartItem()

    }
}
