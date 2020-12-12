using NHibernate.Mapping.Attributes;
using System;

namespace PostgreSql.NhibernateCore
{
    [Class(Table ="userinfo")]
    public partial class Userinfo
    {
        [Property(Column ="id")]
        public virtual short Id { get; set; }
        [Property(Column = "name",Length =255)]
        public virtual string Name { get; set; }
        [Property(Column = "age")]
        public virtual int? Age { get; set; }
        [Property(Column = "addtime")]
        public virtual DateTime? Addtime { get; set; }
        [Property(Column = "cent")]
        public virtual decimal Cent { get; set; }
        [Property(Column = "money")]
        public virtual decimal Money { get; set; }
    }
}
