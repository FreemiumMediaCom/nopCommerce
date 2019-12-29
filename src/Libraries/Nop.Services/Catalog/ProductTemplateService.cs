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
    /// Product template service
    /// </summary>
    public partial class ProductTemplateService : IProductTemplateService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<ProductTemplate> _productTemplateRepository;

        #endregion

        #region Ctor

        public ProductTemplateService(IEventPublisher eventPublisher,
            IRepository<ProductTemplate> productTemplateRepository)
        {
            _eventPublisher = eventPublisher;
            _productTemplateRepository = productTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public async virtual Task DeleteProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException(nameof(productTemplate));

            await _productTemplateRepository.Delete(productTemplate);

            //event notification
            _eventPublisher.EntityDeleted(productTemplate);
        }

        /// <summary>
        /// Gets all product templates
        /// </summary>
        /// <returns>Product templates</returns>
        public async virtual Task<IList<ProductTemplate>> GetAllProductTemplates()
        {
            var query = from pt in _productTemplateRepository.Table
                        orderby pt.DisplayOrder, pt.Id
                        select pt;

            var templates = await query.ToListAsync();
            return templates;
        }

        /// <summary>
        /// Gets a product template
        /// </summary>
        /// <param name="productTemplateId">Product template identifier</param>
        /// <returns>Product template</returns>
        public async virtual Task<ProductTemplate> GetProductTemplateById(int productTemplateId)
        {
            if (productTemplateId == 0)
                return null;

            return await _productTemplateRepository.GetById(productTemplateId);
        }

        /// <summary>
        /// Inserts product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public async virtual Task InsertProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException(nameof(productTemplate));

            await _productTemplateRepository.Insert(productTemplate);

            //event notification
            _eventPublisher.EntityInserted(productTemplate);
        }

        /// <summary>
        /// Updates the product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public async virtual Task UpdateProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException(nameof(productTemplate));

            await _productTemplateRepository.Update(productTemplate);

            //event notification
            _eventPublisher.EntityUpdated(productTemplate);
        }

        #endregion
    }
}