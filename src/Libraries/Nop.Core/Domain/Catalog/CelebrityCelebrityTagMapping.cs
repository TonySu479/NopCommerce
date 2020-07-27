namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a celebrity-celebrity tag mapping class
    /// </summary>
    public partial class CelebrityCelebrityTagMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the celebrity identifier
        /// </summary>
        public int CelebrityId { get; set; }

        /// <summary>
        /// Gets or sets the celebrity tag identifier
        /// </summary>
        public int CelebrityTagId { get; set; }
    }
}