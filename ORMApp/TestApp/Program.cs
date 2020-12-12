using System;
using System.Linq;
using System.Collections.Generic;
using PostgreSql.EFCore;
using PostgreSql.NhibernateCore;
using Web.Host.Models;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Userinfo> a = new List<Userinfo>();
            IList<Class1> b = new List<Class1>();
            /*
            var type = typeof(IList<Class1>).GetGenericArguments()[0];
            Console.WriteLine(typeof(List<Userinfo>).GetGenericTypeDefinition()==typeof(List<>));
            Console.WriteLine(b.GetType().GetGenericTypeDefinition() == typeof(List<>));
            Console.WriteLine(typeof(List<Userinfo>).GetGenericArguments()[0] ==typeof(Userinfo));
            Console.WriteLine(a.GetType().GetGenericArguments()[0] == typeof(Userinfo));
            Console.WriteLine(typeof(IList<Class1>).GetGenericArguments()[0] == typeof(Class1));
            Console.WriteLine(b.GetType().GetGenericArguments()[0] == typeof(Class1));
            */
            Type class_type = b.GetType().GetGenericArguments()[0];
            T t = Activator.CreateInstance(class_type);
            List<Class1> result = new List<Class1>()
            {
                new Class1 { Name = "展示", Age = 12, AddTime = DateTime.Now,Data=new List<temp_a> { new temp_a { Name="张三",Type=10} } },
                new Class1 { Name = "李四", Age = 15, AddTime = DateTime.Now,Data=new List<temp_a> { new temp_a { Name="历史",Type=10} } }
        };
            
            Class1 class1 = new Class1 { Name = "展示", Age = 12, AddTime = DateTime.Now,Data=new List<temp_a> { new temp_a { Name="张三",Type=10} } };
            var class2 = result.Select(p => { return ConvertHelper.Map<Class1, Class2>(p); }).ToList();
            Console.WriteLine("Hello World!");
        }
    }
}
