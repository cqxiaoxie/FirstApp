using System;
using System.Collections.Generic;

#nullable disable

namespace PostgreSql.EFCore.DBModel
{
    public partial class Pgsql
    {
        public short? Id { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public DateTime? Addtime { get; set; }
        public decimal? Cent { get; set; }
        public decimal? Money { get; set; }
    }
}
