using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Common
{
    /// <summary>
    /// Generic attribute service
    /// </summary>
    public partial class GenericAttributeService : IGenericAttributeService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        #endregion

        #region Ctor

        public GenericAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<GenericAttribute> genericAttributeRepository)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _genericAttributeRepository = genericAttributeRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        public async virtual Task DeleteAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            await _genericAttributeRepository.Delete(attribute);

            //cache
            _cacheManager.RemoveByPrefix(NopCommonDefaults.GenericAttributePrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(attribute);
        }

        /// <summary>
        /// Deletes an attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        public async virtual Task DeleteAttributes(IList<GenericAttribute> attributes)
        {
            if (attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            await _genericAttributeRepository.Delete(attributes);

            //cache
            _cacheManager.RemoveByPrefix(NopCommonDefaults.GenericAttributePrefixCacheKey);

            //event notification
            foreach (var attribute in attributes)
            {
                _eventPublisher.EntityDeleted(attribute);
            }
        }

        /// <summary>
        /// Gets an attribute
        /// </summary>
        /// <param name="attributeId">Attribute identifier</param>
        /// <returns>An attribute</returns>
        public async virtual Task<GenericAttribute> GetAttributeById(int attributeId)
        {
            if (attributeId == 0)
                return null;

            return await _genericAttributeRepository.GetById(attributeId);
        }

        /// <summary>
        /// Inserts an attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        public async virtual Task InsertAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            await _genericAttributeRepository.Insert(attribute);

            //cache
            _cacheManager.RemoveByPrefix(NopCommonDefaults.GenericAttributePrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(attribute);
        }

        /// <summary>
        /// Updates the attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        public async virtual Task UpdateAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            await _genericAttributeRepository.Update(attribute);

            //cache
            _cacheManager.RemoveByPrefix(NopCommonDefaults.GenericAttributePrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(attribute);
        }

        /// <summary>
        /// Get attributes
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="keyGroup">Key group</param>
        /// <returns>Get attributes</returns>
        public async virtual Task<IList<GenericAttribute>> GetAttributesForEntity(int entityId, string keyGroup)
        {
            var key = string.Format(NopCommonDefaults.GenericAttributeCacheKey, entityId, keyGroup);
            return await _cacheManager.Get(key, async () =>
            {
                var query = from ga in _genericAttributeRepository.Table
                            where ga.EntityId == entityId &&
                            ga.KeyGroup == keyGroup
                            select ga;
                var attributes = await query.ToListAsync();
                return attributes;
            });
        }

        /// <summary>
        /// Save attribute value
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier; pass 0 if this attribute will be available for all stores</param>
        public async virtual Task SaveAttribute<TPropType>(BaseEntity entity, string key, TPropType value, int storeId = 0)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var keyGroup = entity.GetUnproxiedEntityType().Name;

            var props = (await GetAttributesForEntity(entity.Id, keyGroup))
                .Where(x => x.StoreId == storeId)
                .ToList();
            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            var valueStr = CommonHelper.To<string>(value);

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                {
                    //delete
                    await DeleteAttribute(prop);
                }
                else
                {
                    //update
                    prop.Value = valueStr;
                    await UpdateAttribute(prop);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(valueStr)) 
                    return;

                //insert
                prop = new GenericAttribute
                {
                    EntityId = entity.Id,
                    Key = key,
                    KeyGroup = keyGroup,
                    Value = valueStr,
                    StoreId = storeId
                };

                await InsertAttribute(prop);
            }
        }

        /// <summary>
        /// Get an attribute of an entity
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="storeId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Attribute</returns>
        public async virtual Task<TPropType> GetAttribute<TPropType>(BaseEntity entity, string key, int storeId = 0, TPropType defaultValue = default(TPropType))
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var keyGroup = entity.GetUnproxiedEntityType().Name;

            var props = await GetAttributesForEntity(entity.Id, keyGroup);

            //little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
            if (props == null)
                return defaultValue;

            props = props.Where(x => x.StoreId == storeId).ToList();
            if (!props.Any())
                return defaultValue;

            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            if (prop == null || string.IsNullOrEmpty(prop.Value))
                return defaultValue;

            return CommonHelper.To<TPropType>(prop.Value);
        }

        #endregion
    }
}