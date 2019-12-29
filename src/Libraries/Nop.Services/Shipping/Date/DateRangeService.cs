using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Shipping;
using Nop.Services.Events;

namespace Nop.Services.Shipping.Date
{
    /// <summary>
    /// Represents the date range service
    /// </summary>
    public partial class DateRangeService : IDateRangeService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<DeliveryDate> _deliveryDateRepository;
        private readonly IRepository<ProductAvailabilityRange> _productAvailabilityRangeRepository;

        #endregion

        #region Ctor

        public DateRangeService(IEventPublisher eventPublisher,
            IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<ProductAvailabilityRange> productAvailabilityRangeRepository)
        {
            _eventPublisher = eventPublisher;
            _deliveryDateRepository = deliveryDateRepository;
            _productAvailabilityRangeRepository = productAvailabilityRangeRepository;
        }

        #endregion

        #region Methods

        #region Delivery dates

        /// <summary>
        /// Get a delivery date
        /// </summary>
        /// <param name="deliveryDateId">The delivery date identifier</param>
        /// <returns>Delivery date</returns>
        public async virtual Task<DeliveryDate> GetDeliveryDateById(int deliveryDateId)
        {
            if (deliveryDateId == 0)
                return null;

            return await _deliveryDateRepository.GetById(deliveryDateId);
        }

        /// <summary>
        /// Get all delivery dates
        /// </summary>
        /// <returns>Delivery dates</returns>
        public async virtual Task<IList<DeliveryDate>> GetAllDeliveryDates()
        {
            var query = from dd in _deliveryDateRepository.Table
                        orderby dd.DisplayOrder, dd.Id
                        select dd;
            var deliveryDates = await query.ToListAsync();
            return deliveryDates;
        }

        /// <summary>
        /// Insert a delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        public async virtual Task InsertDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException(nameof(deliveryDate));

            await _deliveryDateRepository.Insert(deliveryDate);

            //event notification
            _eventPublisher.EntityInserted(deliveryDate);
        }

        /// <summary>
        /// Update the delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        public async virtual Task UpdateDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException(nameof(deliveryDate));

            await _deliveryDateRepository.Update(deliveryDate);

            //event notification
            _eventPublisher.EntityUpdated(deliveryDate);
        }

        /// <summary>
        /// Delete a delivery date
        /// </summary>
        /// <param name="deliveryDate">The delivery date</param>
        public async virtual Task DeleteDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException(nameof(deliveryDate));

            await _deliveryDateRepository.Delete(deliveryDate);

            //event notification
            _eventPublisher.EntityDeleted(deliveryDate);
        }

        #endregion

        #region Product availability ranges

        /// <summary>
        /// Get a product availability range
        /// </summary>
        /// <param name="productAvailabilityRangeId">The product availability range identifier</param>
        /// <returns>Product availability range</returns>
        public async virtual Task<ProductAvailabilityRange> GetProductAvailabilityRangeById(int productAvailabilityRangeId)
        {
            return productAvailabilityRangeId != 0 ? await _productAvailabilityRangeRepository.GetById(productAvailabilityRangeId) : null;
        }

        /// <summary>
        /// Get all product availability ranges
        /// </summary>
        /// <returns>Product availability ranges</returns>
        public async virtual Task<IList<ProductAvailabilityRange>> GetAllProductAvailabilityRanges()
        {
            var query = from par in _productAvailabilityRangeRepository.Table
                        orderby par.DisplayOrder, par.Id
                        select par;
            return await query.ToListAsync();
        }

        /// <summary>
        /// Insert the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public async virtual Task InsertProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            if (productAvailabilityRange == null)
                throw new ArgumentNullException(nameof(productAvailabilityRange));

            await _productAvailabilityRangeRepository.Insert(productAvailabilityRange);

            //event notification
            _eventPublisher.EntityInserted(productAvailabilityRange);
        }

        /// <summary>
        /// Update the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public async virtual Task UpdateProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            if (productAvailabilityRange == null)
                throw new ArgumentNullException(nameof(productAvailabilityRange));

            await _productAvailabilityRangeRepository.Update(productAvailabilityRange);

            //event notification
            _eventPublisher.EntityUpdated(productAvailabilityRange);
        }

        /// <summary>
        /// Delete the product availability range
        /// </summary>
        /// <param name="productAvailabilityRange">Product availability range</param>
        public async virtual Task DeleteProductAvailabilityRange(ProductAvailabilityRange productAvailabilityRange)
        {
            if (productAvailabilityRange == null)
                throw new ArgumentNullException(nameof(productAvailabilityRange));

            await _productAvailabilityRangeRepository.Delete(productAvailabilityRange);

            //event notification
            _eventPublisher.EntityDeleted(productAvailabilityRange);
        }

        #endregion

        #endregion
    }
}