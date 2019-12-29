using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category template service
    /// </summary>
    public partial class CategoryTemplateService : ICategoryTemplateService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;

        #endregion

        #region Ctor

        public CategoryTemplateService(IEventPublisher eventPublisher,
            IRepository<CategoryTemplate> categoryTemplateRepository)
        {
            _eventPublisher = eventPublisher;
            _categoryTemplateRepository = categoryTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public async virtual Task DeleteCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            if (categoryTemplate == null)
                throw new ArgumentNullException(nameof(categoryTemplate));

            await _categoryTemplateRepository.Delete(categoryTemplate);

            //event notification
            _eventPublisher.EntityDeleted(categoryTemplate);
        }

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category templates</returns>
        public async virtual Task<IList<CategoryTemplate>> GetAllCategoryTemplates()
        {
            var query = from pt in _categoryTemplateRepository.Table
                        orderby pt.DisplayOrder, pt.Id
                        select pt;

            var templates = await query.ToListAsync();
            return templates;
        }

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>Category template</returns>
        public async virtual Task<CategoryTemplate> GetCategoryTemplateById(int categoryTemplateId)
        {
            if (categoryTemplateId == 0)
                return null;

            return await _categoryTemplateRepository.GetById(categoryTemplateId);
        }

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public async virtual Task InsertCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            if (categoryTemplate == null)
                throw new ArgumentNullException(nameof(categoryTemplate));

            await _categoryTemplateRepository.Insert(categoryTemplate);

            //event notification
            _eventPublisher.EntityInserted(categoryTemplate);
        }

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public async virtual Task UpdateCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            if (categoryTemplate == null)
                throw new ArgumentNullException(nameof(categoryTemplate));

            await _categoryTemplateRepository.Update(categoryTemplate);

            //event notification
            _eventPublisher.EntityUpdated(categoryTemplate);
        }

        #endregion
    }
}