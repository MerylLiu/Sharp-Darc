namespace Darc.Domain.Common
{
    using Core.Entities;
    using Dapper.Attributes;

    public class Entity : EntityBase
    {
        [PrimaryKey]
        public override object Id { get; set; }
    }
}