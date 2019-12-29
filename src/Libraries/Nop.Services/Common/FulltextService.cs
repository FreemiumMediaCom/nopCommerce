using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Domain.Common;
using Nop.Data;

namespace Nop.Services.Common
{
    /// <summary>
    /// Full-Text service
    /// </summary>
    public partial class FulltextService : IFulltextService
    {
        #region Fields

        private readonly IDbContext _dbContext;

        #endregion

        #region Ctor

        public FulltextService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets value indicating whether Full-Text is supported
        /// </summary>
        /// <returns>Result</returns>
        public async virtual Task<bool> IsFullTextSupported()
        {
            var result = await _dbContext
                .QueryFromSql<IntQueryType>("EXEC [FullText_IsSupported]")
                .Select(intValue => intValue.Value).FirstOrDefaultAsync();
            return result > 0;
        }

        /// <summary>
        /// Enable Full-Text support
        /// </summary>
        public async virtual Task EnableFullText()
        {
            await _dbContext.ExecuteSqlCommand("EXEC [FullText_Enable]", true);
        }

        /// <summary>
        /// Disable Full-Text support
        /// </summary>
        public async virtual Task DisableFullText()
        {
            await _dbContext.ExecuteSqlCommand("EXEC [FullText_Disable]", true);
        }

        #endregion
    }
}