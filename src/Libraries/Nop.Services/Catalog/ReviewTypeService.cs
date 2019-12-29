using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Review type service implementation
    /// </summary>
    public partial class ReviewTypeService : IReviewTypeService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<ProductReviewReviewTypeMapping> _productReviewReviewTypeMappingRepository;
        private readonly IRepository<ReviewType> _reviewTypeRepository;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public ReviewTypeService(IEventPublisher eventPublisher,
            IRepository<ProductReviewReviewTypeMapping> productReviewReviewTypeMappingRepository,
            IRepository<ReviewType> reviewTypeRepository,
            IStaticCacheManager cacheManager)
        {
            _eventPublisher = eventPublisher;
            _productReviewReviewTypeMappingRepository = productReviewReviewTypeMappingRepository;
            _reviewTypeRepository = reviewTypeRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        #region Review type

        /// <summary>
        /// Gets all review types
        /// </summary>
        /// <returns>Review types</returns>
        public async virtual Task<IList<ReviewType>> GetAllReviewTypes()
        {
            return await _cacheManager.Get(NopCatalogDefaults.ReviewTypeAllKey, async() =>
            {
                return await _reviewTypeRepository.Table
                    .OrderBy(reviewType => reviewType.DisplayOrder).ThenBy(reviewType => reviewType.Id)
                    .ToListAsync();
            });
        }

        /// <summary>
        /// Gets a review type 
        /// </summary>
        /// <param name="reviewTypeId">Review type identifier</param>
        /// <returns>Review type</returns>
        public async virtual Task<ReviewType> GetReviewTypeById(int reviewTypeId)
        {
            if (reviewTypeId == 0)
                return null;

            var key = string.Format(NopCatalogDefaults.ReviewTypeByIdKey, reviewTypeId);
            return await _cacheManager.Get(key, async() => await _reviewTypeRepository.GetById(reviewTypeId));
        }

        /// <summary>
        /// Inserts a review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        public async virtual Task InsertReviewType(ReviewType reviewType)
        {
            if (reviewType == null)
                throw new ArgumentNullException(nameof(reviewType));

            await _reviewTypeRepository.Insert(reviewType);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ReviewTypeByPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(reviewType);
        }

        /// <summary>
        /// Updates a review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        public async virtual Task UpdateReviewType(ReviewType reviewType)
        {
            if (reviewType == null)
                throw new ArgumentNullException(nameof(reviewType));

            await _reviewTypeRepository.Update(reviewType);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ReviewTypeByPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(reviewType);
        }

        /// <summary>
        /// Delete review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        public async virtual Task DeleteReiewType(ReviewType reviewType)
        {
            if (reviewType == null)
                throw new ArgumentNullException(nameof(reviewType));

            await _reviewTypeRepository.Delete(reviewType);
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.ReviewTypeByPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(reviewType);
        }

        #endregion

        #region Product review type mapping

        /// <summary>
        /// Gets product review and review type mappings by product review identifier
        /// </summary>
        /// <param name="productReviewId">The product review identifier</param>
        /// <returns>Product review and review type mapping collection</returns>
        public async virtual Task<IList<ProductReviewReviewTypeMapping>> GetProductReviewReviewTypeMappingsByProductReviewId(int productReviewId)
        {
            var key = string.Format(NopCatalogDefaults.ProductReviewReviewTypeMappingAllKey, productReviewId);

            return await _cacheManager.Get(key, async () =>
            {
                var query = from pam in _productReviewReviewTypeMappingRepository.Table
                            orderby pam.Id
                            where pam.ProductReviewId == productReviewId
                            select pam;
                var productReviewReviewTypeMappings = await query.ToListAsync();
                return productReviewReviewTypeMappings;
            });
        }

        #endregion

        #endregion
    }
}