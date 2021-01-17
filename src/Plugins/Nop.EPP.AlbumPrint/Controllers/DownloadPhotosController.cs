using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.EPP.AlbumPrint.Domain;
using Nop.EPP.AlbumPrint.Helpers;
using Nop.EPP.AlbumPrint.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.EPP.AlbumPrint.Controllers
{
    [AutoValidateAntiforgeryToken]
    //[AuthorizeAdmin]
    //[Area(AreaNames.Admin)]
    public class DownloadPhotosController : BasePluginController
    {
        #region Fields

        private readonly IEppApService _eppApService;
        private readonly IProductService _productService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHelper _webHelper;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IRepository<EppAp> _eppApRepository;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor 

        public DownloadPhotosController(
            IEppApService eppApService,
            IProductService productService,
            CatalogSettings catalogSettings,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            ShoppingCartSettings shoppingCartSettings,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductModelFactory productModelFactory,
            IProductAttributeService productAttributeService,
            ICategoryService categoryService,
            IWebHelper webHelper,
            IProductAttributeParser productAttributeParser,
            IRepository<EppAp> eppApRepository,
            ICustomerService customerService
            )
        {
            _eppApService = eppApService;
            _productService = productService;
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _permissionService = permissionService;
            _urlRecordService = urlRecordService;
            _shoppingCartSettings = shoppingCartSettings;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _storeContext = storeContext;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _productModelFactory = productModelFactory;
            _productAttributeService = productAttributeService;
            _categoryService = categoryService;
            _webHelper = webHelper;
            _productAttributeParser = productAttributeParser;
            _eppApRepository = eppApRepository;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Download(string id)
        {
            //if current user is vendor then
            var record = _eppApRepository.Table.Where(m => m.CloudFolderId == id).FirstOrDefault();

            var customer = await _customerService.GetCustomerByIdAsync(record.CustomerId);
            var product = await _productService.GetProductByIdAsync(record.ProductId);

            DownloadFromS3 downloadFromS3 = new DownloadFromS3();
            var zipFilePath = await downloadFromS3.Download($"AlbumPrints/{record.CloudFolderId}", $"{product.Name}.zip");

            var memory = new MemoryStream();
            using (var stream = new FileStream(zipFilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(zipFilePath), Path.GetFileName(zipFilePath));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                { ".zip", "application/zip"}
            };
        }

        private IActionResult InvokeHttp404()
        {
            Response.StatusCode = 404;
            return new EmptyResult();
        }

        #endregion

    }

    public class DownloadFileVM
    {
        public int Id { get; set; }
        public int CustomerName { get; set; }
        public DateTime AddedToShoppingCartOn { get; set; }
        public string Url { get; set; }
    }
}
