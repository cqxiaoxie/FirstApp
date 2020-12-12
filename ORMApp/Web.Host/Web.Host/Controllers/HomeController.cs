using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web.Host.Models;
using PostgreSql.EFCore.DBModel;

namespace Web.Host.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Userinfo> users = null;
            using (pgtestdbContext _context = new pgtestdbContext())
            {
                users = _context.Userinfos.ToList();
            }
            ViewBag.users = users;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CheckSort(string sortid,string preid)
        {
            List<SortClass> sorts = new List<SortClass>() { 
                new SortClass{ ID="a",Sort=1},
                new SortClass{ ID="b",Sort=2},
                new SortClass{ ID="c",Sort=3},
                new SortClass{ ID="d",Sort=4},
                new SortClass{ ID="e",Sort=5},
                new SortClass{ ID="f",Sort=6}
            };
            CustomerSort sort = new CustomerSort();
            var new_sort = sort.CheckSort(sorts,sortid, preid);
            return Json(new_sort);
        }

    }
}
