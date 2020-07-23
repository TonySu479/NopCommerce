namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a celebrity picture mapping
    /// </summary>
    public partial class CelebrityPicture : BaseEntity
    {
        /// <summary>
        /// Gets or sets the celebrity identifier
        /// </summary>
        public int CelebrityId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
