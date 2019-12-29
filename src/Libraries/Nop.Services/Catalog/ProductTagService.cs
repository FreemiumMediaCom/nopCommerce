using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Data.Extensions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product tag service
    /// </summary>
    public partial class ProductTagService : IProductTagService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductService _productService;
        private readonly IRepository<ProductProductTagMapping> _productProductTagMappingRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ProductTagService(CatalogSettings catalogSettings,
            ICacheManager cacheManager,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            IProductService productService,
            IRepository<ProductProductTagMapping> productProductTagMappingRepository,
            IRepository<ProductTag> productTagRepository,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _cacheManager = cacheManager;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
            _productService = productService;
            _productProductTagMappingRepository = productProductTagMappingRepository;
            _productTagRepository = productTagRepository;
            _staticCacheManager = staticCacheManager;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get product count for each of existing product tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Dictionary of "product tag ID : product count"</returns>
        private async Task<Dictionary<int, int>> GetProductCount(int storeId, bool showHidden)
        {
            string allowedCustomerRolesIds = "";
            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                //pass customer role identifiers as comma-delimited string
                allowedCustomerRolesIds = string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds());
            }

            var key = string.Format(NopCatalogDefaults.ProductTagCountCacheKey, storeId, allowedCustomerRolesIds, showHidden);
            return await _staticCacheManager.Get(key, async () =>
            {
                //prepare input parameters
                var pStoreId = _dataProvider.GetInt32Parameter("StoreId", storeId);
                var pAllowedCustomerRoleIds = _dataProvider.GetStringParameter("AllowedCustomerRoleIds", allowedCustomerRolesIds);

                //invoke stored procedure
                return await _dbContext.QueryFromSql<ProductTagWithCount>("ProductTagCountLoadAll",
                        pStoreId,
                        pAllowedCustomerRoleIds)
                    .ToDictionaryAsync(item => item.ProductTagId, item => item.ProductCount);
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public async virtual Task DeleteProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            await _productTagRepository.Delete(productTag);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductTagPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductTagPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(productTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <returns>Product tags</returns>
        public async virtual Task<IList<ProductTag>> GetAllProductTags()
        {
            var query = _productTagRepository.Table;
            var productTags = await query.ToListAsync();
            return productTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product tags</returns>
        public async virtual Task<IList<ProductTag>> GetAllProductTagsByProductId(int productId)
        {
            var key = string.Format(NopCatalogDefaults.ProductTagAllByProductIdCacheKey, productId);
            return await _cacheManager.Get(key, async() =>
            {
                var query = from pt in _productTagRepository.Table
                            join ppt in _productProductTagMappingRepository.Table on pt.Id equals ppt.ProductTagId
                            where ppt.ProductId == productId
                            orderby pt.Id
                            select pt;

                var productTags = await query.ToListAsync();
                return productTags;
            });
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public async virtual Task<ProductTag> GetProductTagById(int productTagId)
        {
            if (productTagId == 0)
                return null;

            return await _productTagRepository.GetById(productTagId);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>Product tag</returns>
        public async virtual Task<ProductTag> GetProductTagByName(string name)
        {
            var query = from pt in _productTagRepository.Table
                        where pt.Name == name
                        select pt;

            var productTag = await query.FirstOrDefaultAsync();
            return productTag;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public async virtual Task InsertProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            await _productTagRepository.Insert(productTag);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductTagPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductTagPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(productTag);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public async virtual Task UpdateProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            await _productTagRepository.Update(productTag);

            var seName = _urlRecordService.ValidateSeName(productTag, string.Empty, productTag.Name, true);
            _urlRecordService.SaveSlug(productTag, seName, 0);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductTagPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductTagPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(productTag);
        }

        /// <summary>
        /// Get number of products
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Number of products</returns>
        public async virtual Task<int> GetProductCount(int productTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetProductCount(storeId, showHidden);
            if (dictionary.ContainsKey(productTagId))
                return dictionary[productTagId];

            return 0;
        }

        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        public async virtual Task UpdateProductTags(Product product, string[] productTags)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //product tags
            var existingProductTags = await GetAllProductTagsByProductId(product.Id);
            var productTagsToRemove = new List<ProductTag>();
            foreach (var existingProductTag in existingProductTags)
            {
                var found = false;
                foreach (var newProductTag in productTags)
                {
                    if (!existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                {
                    productTagsToRemove.Add(existingProductTag);
                }
            }

            foreach (var productTag in productTagsToRemove)
            {
                //product.ProductTags.Remove(productTag);
                product.ProductProductTagMappings
                    .Remove(product.ProductProductTagMappings.FirstOrDefault(mapping => mapping.ProductTagId == productTag.Id));
                await _productService.UpdateProduct(product);
            }

            foreach (var productTagName in productTags)
            {
                ProductTag productTag;
                var productTag2 = await GetProductTagByName(productTagName);
                if (productTag2 == null)
                {
                    //add new product tag
                    productTag = new ProductTag
                    {
                        Name = productTagName
                    };
                    await InsertProductTag(productTag);
                }
                else
                {
                    productTag = productTag2;
                }

                if (!_productService.ProductTagExists(product, productTag.Id))
                {
                    //product.ProductTags.Add(productTag);
                    product.ProductProductTagMappings.Add(new ProductProductTagMapping { ProductTag = productTag });
                    await _productService.UpdateProduct(product);
                }

                var seName = _urlRecordService.ValidateSeName(productTag, string.Empty, productTag.Name, true);
                _urlRecordService.SaveSlug(productTag, seName, 0);
            }

            //cache
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductTagPrefixCacheKey);
        }

        #endregion
    }
}