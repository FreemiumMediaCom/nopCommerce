using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Topics;
using Nop.Services.Events;

namespace Nop.Services.Topics
{
    /// <summary>
    /// Topic template service
    /// </summary>
    public partial class TopicTemplateService : ITopicTemplateService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<TopicTemplate> _topicTemplateRepository;

        #endregion

        #region Ctor

        public TopicTemplateService(IEventPublisher eventPublisher,
            IRepository<TopicTemplate> topicTemplateRepository)
        {
            _eventPublisher = eventPublisher;
            _topicTemplateRepository = topicTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public async virtual Task DeleteTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException(nameof(topicTemplate));

            await _topicTemplateRepository.Delete(topicTemplate);

            //event notification
            _eventPublisher.EntityDeleted(topicTemplate);
        }

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>Topic templates</returns>
        public async virtual Task<IList<TopicTemplate>> GetAllTopicTemplates()
        {
            var query = from pt in _topicTemplateRepository.Table
                        orderby pt.DisplayOrder, pt.Id
                        select pt;

            var templates = await query.ToListAsync();
            return templates;
        }

        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>Topic template</returns>
        public async virtual Task<TopicTemplate> GetTopicTemplateById(int topicTemplateId)
        {
            if (topicTemplateId == 0)
                return null;

            return await _topicTemplateRepository.GetById(topicTemplateId);
        }

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public async virtual Task InsertTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException(nameof(topicTemplate));

            await _topicTemplateRepository.Insert(topicTemplate);

            //event notification
            _eventPublisher.EntityInserted(topicTemplate);
        }

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public async virtual Task UpdateTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException(nameof(topicTemplate));

            await _topicTemplateRepository.Update(topicTemplate);

            //event notification
            _eventPublisher.EntityUpdated(topicTemplate);
        }

        #endregion
    }
}