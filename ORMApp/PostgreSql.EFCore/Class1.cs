using System;
using System.Collections.Generic;

namespace PostgreSql.EFCore
{
    public class Class1
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime AddTime { get; set; }

        public List<temp_a> Data { get; set; }
    }

    public class Class2
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public int Sort { get; set; }

        public List<temp_a> Data { get; set; }
    }

    public class temp_a
    {
        public string Name { get; set; }

        public int Type { get; set; }
    }
}
