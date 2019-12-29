using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute service
    /// </summary>
    public partial class ProductAttributeService : IProductAttributeService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<PredefinedProductAttributeValue> _predefinedProductAttributeValueRepository;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ProductAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<PredefinedProductAttributeValue> predefinedProductAttributeValueRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductAttributeValue> productAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _predefinedProductAttributeValueRepository = predefinedProductAttributeValueRepository;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productAttributeMappingRepository = productAttributeMappingRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Product attributes

        /// <summary>
        /// Deletes a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public async virtual Task DeleteProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException(nameof(productAttribute));

            await _productAttributeRepository.Delete(productAttribute);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(productAttribute);
        }

        /// <summary>
        /// Gets all product attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Product attributes</returns>
        public virtual IPagedList<ProductAttribute> GetAllProductAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(NopCatalogDefaults.ProductAttributesAllCacheKey, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from pa in _productAttributeRepository.Table
                            orderby pa.Name
                            select pa;
                var productAttributes = new PagedList<ProductAttribute>(query, pageIndex, pageSize);
                return productAttributes;
            });
        }

        /// <summary>
        /// Gets a product attribute 
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <returns>Product attribute </returns>
        public async virtual Task<ProductAttribute> GetProductAttributeById(int productAttributeId)
        {
            if (productAttributeId == 0)
                return null;

            var key = string.Format(NopCatalogDefaults.ProductAttributesByIdCacheKey, productAttributeId);
            return await _cacheManager.Get(key, async () => await _productAttributeRepository.GetById(productAttributeId));
        }

        /// <summary>
        /// Inserts a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public async virtual Task InsertProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException(nameof(productAttribute));

            await _productAttributeRepository.Insert(productAttribute);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(productAttribute);
        }

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public async virtual Task UpdateProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException(nameof(productAttribute));

            await _productAttributeRepository.Update(productAttribute);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(productAttribute);
        }

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>List of IDs not existing attributes</returns>
        public async virtual Task<int[]> GetNotExistingAttributes(int[] attributeId)
        {
            if (attributeId == null)
                throw new ArgumentNullException(nameof(attributeId));

            var query = _productAttributeRepository.Table;
            var queryFilter = attributeId.Distinct().ToArray();
            var filter = await query.Select(a => a.Id).Where(m => queryFilter.Contains(m)).ToListAsync();
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Product attributes mappings

        /// <summary>
        /// Deletes a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        public async virtual Task DeleteProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

            await _productAttributeMappingRepository.Delete(productAttributeMapping);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(productAttributeMapping);
        }

        /// <summary>
        /// Gets product attribute mappings by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        public async virtual Task<IList<ProductAttributeMapping>> GetProductAttributeMappingsByProductId(int productId)
        {
            IList<ProductAttributeMapping> attributes = null;

            var allCacheKey = string.Format(NopCatalogDefaults.ProductAttributeMappingsAllCacheKey, productId);

            //function to load attributes
            IList<ProductAttributeMapping> loadedAttributes = await _cacheManager.Get(allCacheKey, async () =>
               {
                   var query = from pam in _productAttributeMappingRepository.Table
                               orderby pam.DisplayOrder, pam.Id
                               where pam.ProductId == productId
                               select pam;
                   var productAttributeMappings = await query.ToListAsync();
                   return productAttributeMappings;
               });


            //perfomance optimization
            //We cache a value indicating whether a product has attributes.
            //This way we don't load attributes with each HTTP request
            var hasAttributesCacheKey = string.Format(NopCatalogDefaults.ProductHasProductAttributesCacheKey, productId);
            var hasAttributes = _staticCacheManager.Get(hasAttributesCacheKey, () =>
            {
                //no value in the cache yet
                //let's load attributes and cache the result (true/false)
                attributes = loadedAttributes;
                return attributes.Any();
            });
            if (hasAttributes && attributes == null)
            {
                //cache indicates that a product has attributes
                //let's load them
                attributes = loadedAttributes;
            }
            if (attributes == null)
                attributes = new List<ProductAttributeMapping>();

            return attributes;
        }

        /// <summary>
        /// Gets a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
        /// <returns>Product attribute mapping</returns>
        public async virtual Task<ProductAttributeMapping> GetProductAttributeMappingById(int productAttributeMappingId)
        {
            if (productAttributeMappingId == 0)
                return null;

            var key = string.Format(NopCatalogDefaults.ProductAttributeMappingsByIdCacheKey, productAttributeMappingId);
            return await _cacheManager.Get(key, async () => await _productAttributeMappingRepository.GetById(productAttributeMappingId));
        }

        /// <summary>
        /// Inserts a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        public async virtual Task InsertProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

            await _productAttributeMappingRepository.Insert(productAttributeMapping);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(productAttributeMapping);
        }

        /// <summary>
        /// Updates the product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        public async virtual Task UpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

            await _productAttributeMappingRepository.Update(productAttributeMapping);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(productAttributeMapping);
        }

        #endregion

        #region Product attribute values

        /// <summary>
        /// Deletes a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">Product attribute value</param>
        public async virtual Task DeleteProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException(nameof(productAttributeValue));

            await _productAttributeValueRepository.Delete(productAttributeValue);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(productAttributeValue);
        }

        /// <summary>
        /// Gets product attribute values by product attribute mapping identifier
        /// </summary>
        /// <param name="productAttributeMappingId">The product attribute mapping identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        public async virtual Task<IList<ProductAttributeValue>> GetProductAttributeValues(int productAttributeMappingId)
        {
            var key = string.Format(NopCatalogDefaults.ProductAttributeValuesAllCacheKey, productAttributeMappingId);
            return await _cacheManager.Get(key, async () =>
            {
                var query = from pav in _productAttributeValueRepository.Table
                            orderby pav.DisplayOrder, pav.Id
                            where pav.ProductAttributeMappingId == productAttributeMappingId
                            select pav;
                var productAttributeValues = await query.ToListAsync();
                return productAttributeValues;
            });
        }

        /// <summary>
        /// Gets a product attribute value
        /// </summary>
        /// <param name="productAttributeValueId">Product attribute value identifier</param>
        /// <returns>Product attribute value</returns>
        public async virtual Task<ProductAttributeValue> GetProductAttributeValueById(int productAttributeValueId)
        {
            if (productAttributeValueId == 0)
                return null;

            var key = string.Format(NopCatalogDefaults.ProductAttributeValuesByIdCacheKey, productAttributeValueId);
            return await _cacheManager.Get(key, async () => await _productAttributeValueRepository.GetById(productAttributeValueId));
        }

        /// <summary>
        /// Inserts a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        public async virtual Task InsertProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException(nameof(productAttributeValue));

            await _productAttributeValueRepository.Insert(productAttributeValue);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(productAttributeValue);
        }

        /// <summary>
        /// Updates the product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        public async virtual Task UpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException(nameof(productAttributeValue));

            await _productAttributeValueRepository.Update(productAttributeValue);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(productAttributeValue);
        }

        #endregion

        #region Predefined product attribute values

        /// <summary>
        /// Deletes a predefined product attribute value
        /// </summary>
        /// <param name="ppav">Predefined product attribute value</param>
        public async virtual Task DeletePredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException(nameof(ppav));

            await _predefinedProductAttributeValueRepository.Delete(ppav);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(ppav);
        }

        /// <summary>
        /// Gets predefined product attribute values by product attribute identifier
        /// </summary>
        /// <param name="productAttributeId">The product attribute identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        public async virtual Task<IList<PredefinedProductAttributeValue>> GetPredefinedProductAttributeValues(int productAttributeId)
        {
            var query = from ppav in _predefinedProductAttributeValueRepository.Table
                        orderby ppav.DisplayOrder, ppav.Id
                        where ppav.ProductAttributeId == productAttributeId
                        select ppav;
            var values = await query.ToListAsync();
            return values;
        }

        /// <summary>
        /// Gets a predefined product attribute value
        /// </summary>
        /// <param name="id">Predefined product attribute value identifier</param>
        /// <returns>Predefined product attribute value</returns>
        public async virtual Task<PredefinedProductAttributeValue> GetPredefinedProductAttributeValueById(int id)
        {
            if (id == 0)
                return null;

            return await _predefinedProductAttributeValueRepository.GetById(id);
        }

        /// <summary>
        /// Inserts a predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        public async virtual Task InsertPredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException(nameof(ppav));

            await _predefinedProductAttributeValueRepository.Insert(ppav);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(ppav);
        }

        /// <summary>
        /// Updates the predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        public async virtual Task UpdatePredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            if (ppav == null)
                throw new ArgumentNullException(nameof(ppav));

            await _predefinedProductAttributeValueRepository.Update(ppav);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(ppav);
        }

        #endregion

        #region Product attribute combinations

        /// <summary>
        /// Deletes a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        public async virtual Task DeleteProductAttributeCombination(ProductAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException(nameof(combination));

            await _productAttributeCombinationRepository.Delete(combination);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(combination);
        }

        /// <summary>
        /// Gets all product attribute combinations
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product attribute combinations</returns>
        public async virtual Task<IList<ProductAttributeCombination>> GetAllProductAttributeCombinations(int productId)
        {
            if (productId == 0)
                return new List<ProductAttributeCombination>();

            var key = string.Format(NopCatalogDefaults.ProductAttributeCombinationsAllCacheKey, productId);

            return await _cacheManager.Get(key, async () =>
            {
                var query = from c in _productAttributeCombinationRepository.Table
                            orderby c.Id
                            where c.ProductId == productId
                            select c;
                var combinations = await query.ToListAsync();
                return combinations;
            });
        }

        /// <summary>
        /// Gets a product attribute combination
        /// </summary>
        /// <param name="productAttributeCombinationId">Product attribute combination identifier</param>
        /// <returns>Product attribute combination</returns>
        public async virtual Task<ProductAttributeCombination> GetProductAttributeCombinationById(int productAttributeCombinationId)
        {
            if (productAttributeCombinationId == 0)
                return null;

            return await _productAttributeCombinationRepository.GetById(productAttributeCombinationId);
        }

        /// <summary>
        /// Gets a product attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product attribute combination</returns>
        public async virtual Task<ProductAttributeCombination> GetProductAttributeCombinationBySku(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pac in _productAttributeCombinationRepository.Table
                        orderby pac.Id
                        where pac.Sku == sku
                        select pac;
            var combination = await query.FirstOrDefaultAsync();
            return combination;
        }

        /// <summary>
        /// Inserts a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        public async virtual Task InsertProductAttributeCombination(ProductAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException(nameof(combination));

            await _productAttributeCombinationRepository.Insert(combination);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(combination);
        }

        /// <summary>
        /// Updates a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        public async virtual Task UpdateProductAttributeCombination(ProductAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException(nameof(combination));

            await _productAttributeCombinationRepository.Update(combination);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeMappingsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeValuesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ProductAttributeCombinationsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(combination);
        }

        #endregion

        #endregion
    }
}