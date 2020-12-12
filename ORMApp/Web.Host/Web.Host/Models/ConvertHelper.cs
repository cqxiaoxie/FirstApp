using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Host.Models
{
    public class ConvertHelper
    {
        
        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            TDestination result = Activator.CreateInstance<TDestination>();
            
            var source_propertyinfos = source.GetType().GetProperties();
            var result_propertyinfos = result.GetType().GetProperties();
            foreach (var p in result_propertyinfos)
            {
                if (p.CanWrite && source_propertyinfos.Select(s => s.Name).Contains(p.Name))
                {
                    var temp_p = source_propertyinfos.FirstOrDefault(o => o.Name == p.Name);
                    if (temp_p.CanRead)
                    {
                        p.SetValue(result, temp_p.GetValue(source), null);
                    }
                }
            }
            
            return result;
        }

        public static List<TDestination> Map<TSource, TDestination>(List<TSource> sources)
        {
            List<TDestination> results = Activator.CreateInstance<List<TDestination>>();
            foreach (var source in sources)
            {
                var result = Activator.CreateInstance<TDestination>();
                var source_propertyinfos = source.GetType().GetProperties();
                var result_propertyinfos = result.GetType().GetProperties();
                foreach (var p in result_propertyinfos)
                {
                    if (p.CanWrite && source_propertyinfos.Select(s => s.Name).Contains(p.Name))
                    {
                        var temp_p = source_propertyinfos.FirstOrDefault(o => o.Name == p.Name);
                        if (temp_p.CanRead)
                        {
                            p.SetValue(result, temp_p.GetValue(source), null);
                        }
                    }
                }
                results.Add(result);
            }
            return results;
        }
    }
}
