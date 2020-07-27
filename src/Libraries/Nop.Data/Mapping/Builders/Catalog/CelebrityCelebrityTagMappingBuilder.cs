using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a celebrity celebrity tag mapping entity builder
    /// </summary>
    public partial class CelebrityCelebrityTagMappingBuilder : NopEntityBuilder<CelebrityCelebrityTagMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CelebrityCelebrityTagMapping), nameof(CelebrityCelebrityTagMapping.CelebrityId)))
                    .AsInt32().PrimaryKey().ForeignKey<Celebrity>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CelebrityCelebrityTagMapping), nameof(CelebrityCelebrityTagMapping.CelebrityTagId)))
                    .AsInt32().PrimaryKey().ForeignKey<CelebrityTag>();
        }

        #endregion
    }
}