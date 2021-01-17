using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Tasks;
using System.Linq;

namespace Nop.Plugin.Soft2Print.ScheduledTask
{
    public class ImageCleanUpTask : IScheduleTask
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly Services.Soft2PrintAPIService _soft2PrintAPIService;
        private readonly Services.PluginSettingService _pluginSettingService;
        private readonly Services.ProductAttributeService _s2p_productAttributeService;

        private readonly IPictureService _pictureService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IRepository<ShoppingCartItem> _sciRepository;


        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILogger _logger;
        #endregion

        #region Ctor

        public ImageCleanUpTask(IRepository<ShoppingCartItem> sciRepository,
            ILocalizationService localizationService,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IDownloadService downloadService,
            IPictureService pictureService,
            IShoppingCartService shoppingCartService,
            Services.Soft2PrintAPIService soft2PrintAPIService,
            Services.PluginSettingService pluginSettingService,
            Services.ProductAttributeService s2p_productAttributeService,
            IProductAttributeParser productAttributeParser,
            ILogger logger)
        {
            this._sciRepository = sciRepository;
            this._localizationService = localizationService;
            this._productService = productService;
            this._productAttributeService = productAttributeService;
            this._downloadService = downloadService;
            this._pictureService = pictureService;
            this._shoppingCartService = shoppingCartService;
            this._soft2PrintAPIService = soft2PrintAPIService;
            this._pluginSettingService = pluginSettingService;
            this._s2p_productAttributeService = s2p_productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._logger = logger;
        }

        #endregion


        public void Execute()
        {
            var allPictures = this._pictureService.GetPictures().ToArray().Where(i => i.SeoFilename != null).Where(i => i.SeoFilename.StartsWith("Project_")).ToArray();
            int startCount = allPictures.Count();
            // Remove pictures from list that is still in shoppingcarts 
            {
                var query = from sci in _sciRepository.Table
                            select sci;

                foreach (var shoppingCartItem in query)
                {
                    var s2pAttr = this._s2p_productAttributeService.GetS2PProductAttributes(shoppingCartItem.AttributesXml);
                    if (s2pAttr != null)
                        if (s2pAttr.ProjectID.HasValue)
                            allPictures = allPictures.Where(i => !i.SeoFilename.Equals("Project_" + s2pAttr.ProjectID.Value)).ToArray();
                }
            }


            // Should we keep preview of ordered jobs



            // Delete images that is still in the list

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("The 'Soft2Print Task - Cleanup images' just completed.");
            sb.AppendLine(" - pictures deleted:" + allPictures.ToArray().Count());
            sb.AppendLine(" - pictures still kept:" + (startCount - allPictures.ToArray().Count()));
            foreach (var picture in allPictures)
                this._pictureService.DeletePicture(picture);

            this._logger.Information(sb.ToString());
        }

    }
}
