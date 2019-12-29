using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Events;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer attribute service
    /// </summary>
    public partial class CustomerAttributeService : ICustomerAttributeService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<CustomerAttribute> _customerAttributeRepository;
        private readonly IRepository<CustomerAttributeValue> _customerAttributeValueRepository;

        #endregion

        #region Ctor

        public CustomerAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<CustomerAttribute> customerAttributeRepository,
            IRepository<CustomerAttributeValue> customerAttributeValueRepository)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _customerAttributeRepository = customerAttributeRepository;
            _customerAttributeValueRepository = customerAttributeValueRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public async virtual Task DeleteCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            await _customerAttributeRepository.Delete(customerAttribute);

            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(customerAttribute);
        }

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>Customer attributes</returns>
        public async virtual Task<IList<CustomerAttribute>> GetAllCustomerAttributes()
        {
            return await _cacheManager.Get(NopCustomerServiceDefaults.CustomerAttributesAllCacheKey, async () =>
            {
                var query = from ca in _customerAttributeRepository.Table
                            orderby ca.DisplayOrder, ca.Id
                            select ca;
                return await query.ToListAsync();
            });
        }

        /// <summary>
        /// Gets a customer attribute 
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>Customer attribute</returns>
        public async virtual Task<CustomerAttribute> GetCustomerAttributeById(int customerAttributeId)
        {
            if (customerAttributeId == 0)
                return null;

            var key = string.Format(NopCustomerServiceDefaults.CustomerAttributesByIdCacheKey, customerAttributeId);
            return await _cacheManager.Get(key, async () => await _customerAttributeRepository.GetById(customerAttributeId));
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public async virtual Task InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            await _customerAttributeRepository.Insert(customerAttribute);

            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(customerAttribute);
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public async virtual Task UpdateCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            await _customerAttributeRepository.Update(customerAttribute);

            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(customerAttribute);
        }

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public async virtual Task DeleteCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException(nameof(customerAttributeValue));

            await _customerAttributeValueRepository.Delete(customerAttributeValue);

            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(customerAttributeValue);
        }

        /// <summary>
        /// Gets customer attribute values by customer attribute identifier
        /// </summary>
        /// <param name="customerAttributeId">The customer attribute identifier</param>
        /// <returns>Customer attribute values</returns>
        public async virtual Task<IList<CustomerAttributeValue>> GetCustomerAttributeValues(int customerAttributeId)
        {
            var key = string.Format(NopCustomerServiceDefaults.CustomerAttributeValuesAllCacheKey, customerAttributeId);
            return await _cacheManager.Get(key, async () =>
            {
                var query = from cav in _customerAttributeValueRepository.Table
                            orderby cav.DisplayOrder, cav.Id
                            where cav.CustomerAttributeId == customerAttributeId
                            select cav;
                var customerAttributeValues = await query.ToListAsync();
                return customerAttributeValues;
            });
        }

        /// <summary>
        /// Gets a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValueId">Customer attribute value identifier</param>
        /// <returns>Customer attribute value</returns>
        public async virtual Task<CustomerAttributeValue> GetCustomerAttributeValueById(int customerAttributeValueId)
        {
            if (customerAttributeValueId == 0)
                return null;

            var key = string.Format(NopCustomerServiceDefaults.CustomerAttributeValuesByIdCacheKey, customerAttributeValueId);
            return await _cacheManager.Get(key, async () => await _customerAttributeValueRepository.GetById(customerAttributeValueId));
        }

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public async virtual Task InsertCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException(nameof(customerAttributeValue));

            await _customerAttributeValueRepository.Insert(customerAttributeValue);

            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(customerAttributeValue);
        }

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public async virtual Task UpdateCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException(nameof(customerAttributeValue));

            await _customerAttributeValueRepository.Update(customerAttributeValue);

            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceDefaults.CustomerAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(customerAttributeValue);
        }

        #endregion
    }
}