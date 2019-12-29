using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Manufacturer template service
    /// </summary>
    public partial class ManufacturerTemplateService : IManufacturerTemplateService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;

        #endregion

        #region Ctor

        public ManufacturerTemplateService(IEventPublisher eventPublisher,
            IRepository<ManufacturerTemplate> manufacturerTemplateRepository)
        {
            _eventPublisher = eventPublisher;
            _manufacturerTemplateRepository = manufacturerTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public async virtual Task DeleteManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            if (manufacturerTemplate == null)
                throw new ArgumentNullException(nameof(manufacturerTemplate));

            await _manufacturerTemplateRepository.Delete(manufacturerTemplate);

            //event notification
            _eventPublisher.EntityDeleted(manufacturerTemplate);
        }

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>Manufacturer templates</returns>
        public async virtual Task<IList<ManufacturerTemplate>> GetAllManufacturerTemplates()
        {
            var query = from pt in _manufacturerTemplateRepository.Table
                        orderby pt.DisplayOrder, pt.Id
                        select pt;

            var templates = await query.ToListAsync();
            return templates;
        }

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>Manufacturer template</returns>
        public async virtual Task<ManufacturerTemplate> GetManufacturerTemplateById(int manufacturerTemplateId)
        {
            if (manufacturerTemplateId == 0)
                return null;

            return await _manufacturerTemplateRepository.GetById(manufacturerTemplateId);
        }

        /// <summary>
        /// Inserts manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public async virtual Task InsertManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            if (manufacturerTemplate == null)
                throw new ArgumentNullException(nameof(manufacturerTemplate));

            await _manufacturerTemplateRepository.Insert(manufacturerTemplate);

            //event notification
            _eventPublisher.EntityInserted(manufacturerTemplate);
        }

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public async virtual Task UpdateManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            if (manufacturerTemplate == null)
                throw new ArgumentNullException(nameof(manufacturerTemplate));

            await _manufacturerTemplateRepository.Update(manufacturerTemplate);

            //event notification
            _eventPublisher.EntityUpdated(manufacturerTemplate);
        }

        #endregion
    }
}