namespace Darc.Domain.Common
{
    using Dapper;
    using Dapper.Attributes;

    public class Entity : EntityBase
    {
        [PrimaryKey]
        public override object Id { get; set; }
    }
}