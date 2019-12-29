using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Return request service
    /// </summary>
    public partial class ReturnRequestService : IReturnRequestService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<ReturnRequest> _returnRequestRepository;
        private readonly IRepository<ReturnRequestAction> _returnRequestActionRepository;
        private readonly IRepository<ReturnRequestReason> _returnRequestReasonRepository;

        #endregion

        #region Ctor

        public ReturnRequestService(IEventPublisher eventPublisher,
            IRepository<ReturnRequest> returnRequestRepository,
            IRepository<ReturnRequestAction> returnRequestActionRepository,
            IRepository<ReturnRequestReason> returnRequestReasonRepository)
        {
            _eventPublisher = eventPublisher;
            _returnRequestRepository = returnRequestRepository;
            _returnRequestActionRepository = returnRequestActionRepository;
            _returnRequestReasonRepository = returnRequestReasonRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public async virtual Task DeleteReturnRequest(ReturnRequest returnRequest)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            await _returnRequestRepository.Delete(returnRequest);

            //event notification
            _eventPublisher.EntityDeleted(returnRequest);
        }

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>Return request</returns>
        public async virtual Task<ReturnRequest> GetReturnRequestById(int returnRequestId)
        {
            if (returnRequestId == 0)
                return null;

            return await _returnRequestRepository.GetById(returnRequestId);
        }

        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all entries</param>
        /// <param name="customerId">Customer identifier; 0 to load all entries</param>
        /// <param name="orderItemId">Order item identifier; 0 to load all entries</param>
        /// <param name="customNumber">Custom number; null or empty to load all entries</param>
        /// <param name="rs">Return request status; null to load all entries</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Return requests</returns>
        public IPagedList<ReturnRequest> SearchReturnRequests(int storeId = 0, int customerId = 0,
            int orderItemId = 0, string customNumber = "", ReturnRequestStatus? rs = null, DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _returnRequestRepository.Table;
            if (storeId > 0)
                query = query.Where(rr => storeId == rr.StoreId);
            if (customerId > 0)
                query = query.Where(rr => customerId == rr.CustomerId);
            if (rs.HasValue)
            {
                var returnStatusId = (int)rs.Value;
                query = query.Where(rr => rr.ReturnRequestStatusId == returnStatusId);
            }

            if (orderItemId > 0)
                query = query.Where(rr => rr.OrderItemId == orderItemId);

            if (!string.IsNullOrEmpty(customNumber))
                query = query.Where(rr => rr.CustomNumber == customNumber);

            if (createdFromUtc.HasValue)
                query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var returnRequests = new PagedList<ReturnRequest>(query, pageIndex, pageSize, getOnlyTotalCount);
            return returnRequests;
        }

        /// <summary>
        /// Delete a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public async virtual Task DeleteReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException(nameof(returnRequestAction));

            await _returnRequestActionRepository.Delete(returnRequestAction);

            //event notification
            _eventPublisher.EntityDeleted(returnRequestAction);
        }

        /// <summary>
        /// Gets all return request actions
        /// </summary>
        /// <returns>Return request actions</returns>
        public async virtual Task<IList<ReturnRequestAction>> GetAllReturnRequestActions()
        {
            var query = from rra in _returnRequestActionRepository.Table
                        orderby rra.DisplayOrder, rra.Id
                        select rra;
            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets a return request action
        /// </summary>
        /// <param name="returnRequestActionId">Return request action identifier</param>
        /// <returns>Return request action</returns>
        public async virtual Task<ReturnRequestAction> GetReturnRequestActionById(int returnRequestActionId)
        {
            if (returnRequestActionId == 0)
                return null;

            return await _returnRequestActionRepository.GetById(returnRequestActionId);
        }

        /// <summary>
        /// Inserts a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public async virtual Task InsertReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException(nameof(returnRequestAction));

            await _returnRequestActionRepository.Insert(returnRequestAction);

            //event notification
            _eventPublisher.EntityInserted(returnRequestAction);
        }

        /// <summary>
        /// Updates the  return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public async virtual Task UpdateReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException(nameof(returnRequestAction));

            await _returnRequestActionRepository.Update(returnRequestAction);

            //event notification
            _eventPublisher.EntityUpdated(returnRequestAction);
        }

        /// <summary>
        /// Delete a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public async virtual Task DeleteReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException(nameof(returnRequestReason));

            await _returnRequestReasonRepository.Delete(returnRequestReason);

            //event notification
            _eventPublisher.EntityDeleted(returnRequestReason);
        }

        /// <summary>
        /// Gets all return request reasons
        /// </summary>
        /// <returns>Return request reasons</returns>
        public async virtual Task<IList<ReturnRequestReason>> GetAllReturnRequestReasons()
        {
            var query = from rra in _returnRequestReasonRepository.Table
                        orderby rra.DisplayOrder, rra.Id
                        select rra;
            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets a return request reason
        /// </summary>
        /// <param name="returnRequestReasonId">Return request reason identifier</param>
        /// <returns>Return request reason</returns>
        public async virtual Task<ReturnRequestReason> GetReturnRequestReasonById(int returnRequestReasonId)
        {
            if (returnRequestReasonId == 0)
                return null;

            return await _returnRequestReasonRepository.GetById(returnRequestReasonId);
        }

        /// <summary>
        /// Inserts a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public async virtual Task InsertReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException(nameof(returnRequestReason));

            await _returnRequestReasonRepository.Insert(returnRequestReason);

            //event notification
            _eventPublisher.EntityInserted(returnRequestReason);
        }

        /// <summary>
        /// Updates the  return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public async virtual Task UpdateReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException(nameof(returnRequestReason));

            await _returnRequestReasonRepository.Update(returnRequestReason);

            //event notification
            _eventPublisher.EntityUpdated(returnRequestReason);
        }

        #endregion
    }
}