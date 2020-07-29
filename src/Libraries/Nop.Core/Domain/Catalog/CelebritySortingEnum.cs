namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents the celebrity sorting
    /// </summary>
    public enum CelebritySortingEnum
    {
        /// <summary>
        /// Position (display order)
        /// </summary>
        Position = 0,

        /// <summary>
        /// Name: A to Z
        /// </summary>
        NameAsc = 5,

        /// <summary>
        /// Name: Z to A
        /// </summary>
        NameDesc = 6,

        /// <summary>
        /// Celebrity creation date
        /// </summary>
        CreatedOn = 15,
    }
}