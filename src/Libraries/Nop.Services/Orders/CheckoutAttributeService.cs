using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Stores;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute service
    /// </summary>
    public partial class CheckoutAttributeService : ICheckoutAttributeService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<CheckoutAttribute> _checkoutAttributeRepository;
        private readonly IRepository<CheckoutAttributeValue> _checkoutAttributeValueRepository;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public CheckoutAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<CheckoutAttribute> checkoutAttributeRepository,
            IRepository<CheckoutAttributeValue> checkoutAttributeValueRepository,
            IStoreMappingService storeMappingService)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _checkoutAttributeRepository = checkoutAttributeRepository;
            _checkoutAttributeValueRepository = checkoutAttributeValueRepository;
            _storeMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        #region Checkout attributes

        /// <summary>
        /// Deletes a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public async virtual Task DeleteCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            await _checkoutAttributeRepository.Delete(checkoutAttribute);

            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(checkoutAttribute);
        }

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="excludeShippableAttributes">A value indicating whether we should exclude shippable attributes</param>
        /// <returns>Checkout attributes</returns>
        public async virtual Task<IList<CheckoutAttribute>> GetAllCheckoutAttributes(int storeId = 0, bool excludeShippableAttributes = false)
        {
            var key = string.Format(NopOrderDefaults.CheckoutAttributesAllCacheKey, storeId, excludeShippableAttributes);
            return await _cacheManager.Get(key, async() =>
            {
                var query = from ca in _checkoutAttributeRepository.Table
                            orderby ca.DisplayOrder, ca.Id
                            select ca;
                var checkoutAttributes = await query.ToListAsync();
                if (storeId > 0)
                {
                    //store mapping
                    checkoutAttributes = checkoutAttributes.Where(ca => _storeMappingService.Authorize(ca)).ToList();
                }

                if (excludeShippableAttributes)
                {
                    //remove attributes which require shippable products
                    checkoutAttributes = checkoutAttributes.Where(x => !x.ShippableProductRequired).ToList();
                }

                return checkoutAttributes;
            });
        }

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute</returns>
        public async virtual Task<CheckoutAttribute> GetCheckoutAttributeById(int checkoutAttributeId)
        {
            if (checkoutAttributeId == 0)
                return null;

            var key = string.Format(NopOrderDefaults.CheckoutAttributesByIdCacheKey, checkoutAttributeId);
            return await _cacheManager.Get(key, async () => await _checkoutAttributeRepository.GetById(checkoutAttributeId));
        }

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public async virtual Task InsertCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            await _checkoutAttributeRepository.Insert(checkoutAttribute);

            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(checkoutAttribute);
        }

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public async virtual Task UpdateCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            await _checkoutAttributeRepository.Update(checkoutAttribute);

            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(checkoutAttribute);
        }

        #endregion

        #region Checkout attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public async virtual Task DeleteCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException(nameof(checkoutAttributeValue));

            await _checkoutAttributeValueRepository.Delete(checkoutAttributeValue);

            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(checkoutAttributeValue);
        }

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <returns>Checkout attribute values</returns>
        public async virtual Task<IList<CheckoutAttributeValue>> GetCheckoutAttributeValues(int checkoutAttributeId)
        {
            var key = string.Format(NopOrderDefaults.CheckoutAttributeValuesAllCacheKey, checkoutAttributeId);
            return await _cacheManager.Get(key, async() =>
            {
                var query = from cav in _checkoutAttributeValueRepository.Table
                            orderby cav.DisplayOrder, cav.Id
                            where cav.CheckoutAttributeId == checkoutAttributeId
                            select cav;
                var checkoutAttributeValues = await query.ToListAsync();
                return checkoutAttributeValues;
            });
        }

        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>Checkout attribute value</returns>
        public async virtual Task<CheckoutAttributeValue> GetCheckoutAttributeValueById(int checkoutAttributeValueId)
        {
            if (checkoutAttributeValueId == 0)
                return null;

            var key = string.Format(NopOrderDefaults.CheckoutAttributeValuesByIdCacheKey, checkoutAttributeValueId);
            return await _cacheManager.Get(key, async() => await _checkoutAttributeValueRepository.GetById(checkoutAttributeValueId));
        }

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public async virtual Task InsertCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException(nameof(checkoutAttributeValue));

            await _checkoutAttributeValueRepository.Insert(checkoutAttributeValue);

            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(checkoutAttributeValue);
        }

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public async virtual Task UpdateCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException(nameof(checkoutAttributeValue));

            await _checkoutAttributeValueRepository.Update(checkoutAttributeValue);

            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopOrderDefaults.CheckoutAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(checkoutAttributeValue);
        }

        #endregion

        #endregion
    }
}