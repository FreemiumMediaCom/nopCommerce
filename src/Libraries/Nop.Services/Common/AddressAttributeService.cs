using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Services.Events;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address attribute service
    /// </summary>
    public partial class AddressAttributeService : IAddressAttributeService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AddressAttribute> _addressAttributeRepository;
        private readonly IRepository<AddressAttributeValue> _addressAttributeValueRepository;

        #endregion

        #region Ctor

        public AddressAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<AddressAttribute> addressAttributeRepository,
            IRepository<AddressAttributeValue> addressAttributeValueRepository)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _addressAttributeRepository = addressAttributeRepository;
            _addressAttributeValueRepository = addressAttributeValueRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public async virtual Task DeleteAddressAttribute(AddressAttribute addressAttribute)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            await _addressAttributeRepository.Delete(addressAttribute);

            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(addressAttribute);
        }

        /// <summary>
        /// Gets all address attributes
        /// </summary>
        /// <returns>Address attributes</returns>
        public async virtual Task<IList<AddressAttribute>> GetAllAddressAttributes()
        {
            return await _cacheManager.Get(NopCommonDefaults.AddressAttributesAllCacheKey, async () =>
            {
                var query = from aa in _addressAttributeRepository.Table
                            orderby aa.DisplayOrder, aa.Id
                            select aa;
                return await query.ToListAsync();
            });
        }

        /// <summary>
        /// Gets an address attribute 
        /// </summary>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute</returns>
        public async virtual Task<AddressAttribute> GetAddressAttributeById(int addressAttributeId)
        {
            if (addressAttributeId == 0)
                return null;

            var key = string.Format(NopCommonDefaults.AddressAttributesByIdCacheKey, addressAttributeId);
            return await _cacheManager.Get(key, async() => await _addressAttributeRepository.GetById(addressAttributeId));
        }

        /// <summary>
        /// Inserts an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public async virtual Task InsertAddressAttribute(AddressAttribute addressAttribute)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            await _addressAttributeRepository.Insert(addressAttribute);

            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(addressAttribute);
        }

        /// <summary>
        /// Updates the address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public async virtual Task UpdateAddressAttribute(AddressAttribute addressAttribute)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            await _addressAttributeRepository.Update(addressAttribute);

            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(addressAttribute);
        }

        /// <summary>
        /// Deletes an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public async virtual Task DeleteAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            if (addressAttributeValue == null)
                throw new ArgumentNullException(nameof(addressAttributeValue));

            await _addressAttributeValueRepository.Delete(addressAttributeValue);

            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(addressAttributeValue);
        }

        /// <summary>
        /// Gets address attribute values by address attribute identifier
        /// </summary>
        /// <param name="addressAttributeId">The address attribute identifier</param>
        /// <returns>Address attribute values</returns>
        public async virtual Task<IList<AddressAttributeValue>> GetAddressAttributeValues(int addressAttributeId)
        {
            var key = string.Format(NopCommonDefaults.AddressAttributeValuesAllCacheKey, addressAttributeId);
            return await _cacheManager.Get(key, async() =>
            {
                var query = from aav in _addressAttributeValueRepository.Table
                            orderby aav.DisplayOrder, aav.Id
                            where aav.AddressAttributeId == addressAttributeId
                            select aav;
                var addressAttributeValues = await query.ToListAsync();
                return addressAttributeValues;
            });
        }

        /// <summary>
        /// Gets an address attribute value
        /// </summary>
        /// <param name="addressAttributeValueId">Address attribute value identifier</param>
        /// <returns>Address attribute value</returns>
        public async virtual Task<AddressAttributeValue> GetAddressAttributeValueById(int addressAttributeValueId)
        {
            if (addressAttributeValueId == 0)
                return null;

            var key = string.Format(NopCommonDefaults.AddressAttributeValuesByIdCacheKey, addressAttributeValueId);
            return await _cacheManager.Get(key, async () => await _addressAttributeValueRepository.GetById(addressAttributeValueId));
        }

        /// <summary>
        /// Inserts an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public async virtual Task InsertAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            if (addressAttributeValue == null)
                throw new ArgumentNullException(nameof(addressAttributeValue));

            await _addressAttributeValueRepository.Insert(addressAttributeValue);

            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(addressAttributeValue);
        }

        /// <summary>
        /// Updates the address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public async virtual Task UpdateAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            if (addressAttributeValue == null)
                throw new ArgumentNullException(nameof(addressAttributeValue));

            await _addressAttributeValueRepository.Update(addressAttributeValue);

            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCommonDefaults.AddressAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(addressAttributeValue);
        }

        #endregion
    }
}