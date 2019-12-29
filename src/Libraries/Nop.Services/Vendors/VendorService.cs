using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Vendors;
using Nop.Core.Html;
using Nop.Services.Events;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorService : IVendorService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorNote> _vendorNoteRepository;

        #endregion

        #region Ctor

        public VendorService(IEventPublisher eventPublisher,
            IRepository<Vendor> vendorRepository,
            IRepository<VendorNote> vendorNoteRepository)
        {
            _eventPublisher = eventPublisher;
            _vendorRepository = vendorRepository;
            _vendorNoteRepository = vendorNoteRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        public async virtual Task<Vendor> GetVendorById(int vendorId)
        {
            if (vendorId == 0)
                return null;

            return await _vendorRepository.GetById(vendorId);
        }

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public async virtual Task DeleteVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            vendor.Deleted = true;
            await UpdateVendor(vendor);

            //event notification
            _eventPublisher.EntityDeleted(vendor);
        }

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        public virtual IPagedList<Vendor> GetAllVendors(string name = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _vendorRepository.Table;
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
            if (!showHidden)
                query = query.Where(v => v.Active);

            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        /// <summary>
        /// Gets vendors
        /// </summary>
        /// <param name="vendorIds">Vendor identifiers</param>
        /// <returns>Vendors</returns>
        public async virtual Task<IList<Vendor>> GetVendorsByIds(int[] vendorIds)
        {
            var query = _vendorRepository.Table;
            if (vendorIds != null)
                query = query.Where(v => vendorIds.Contains(v.Id));

            return await query.ToListAsync();
        }

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public async virtual Task InsertVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            await _vendorRepository.Insert(vendor);

            //event notification
            _eventPublisher.EntityInserted(vendor);
        }

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public async virtual Task UpdateVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            await _vendorRepository.Update(vendor);

            //event notification
            _eventPublisher.EntityUpdated(vendor);
        }

        /// <summary>
        /// Gets a vendor note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>Vendor note</returns>
        public async virtual Task<VendorNote> GetVendorNoteById(int vendorNoteId)
        {
            if (vendorNoteId == 0)
                return null;

            return await _vendorNoteRepository.GetById(vendorNoteId);
        }

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        public async virtual Task DeleteVendorNote(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException(nameof(vendorNote));

            await _vendorNoteRepository.Delete(vendorNote);

            //event notification
            _eventPublisher.EntityDeleted(vendorNote);
        }

        /// <summary>
        /// Formats the vendor note text
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatVendorNoteText(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException(nameof(vendorNote));

            var text = vendorNote.Note;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        #endregion
    }
}