using ChampionshipMvc3.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChampionshipMvc3.Controllers
{
    public class HomeController : Controller
    {
        private IPlayfieldRepository playfieldRepository;

        public HomeController(IPlayfieldRepository playfieldRepoParam)
        {
            playfieldRepository = playfieldRepoParam;
        }

        public ActionResult Index()
        {
            //var model = playfieldRepository.GetAllPlayfields();

            return View();
        }

        public ActionResult FootballSearchPage()
        {
            return View("FootballSearchView");
        }

        public ActionResult TennisSearchPage()
        {
            return View("TennisSearchView");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult PlayfieldDetails()
        {
            return View();
        }
    }
}
