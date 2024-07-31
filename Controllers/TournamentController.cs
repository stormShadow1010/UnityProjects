using ChampionshipMvc3.Models.Interfaces;
using ChampionshipMvc3.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChampionshipMvc3.Controllers
{
    public class TournamentController : Controller
    {
        private const string tournamentViewName = "CreateTournament";
        private ITournamentRepository tournamentRepository;

        public TournamentController()
        {
            tournamentRepository = new TournamentRepository();
        }
        //
        // GET: /Tournament/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Tournament/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Tournament/Create

        public ActionResult Create()
        {
            var model = tournamentRepository.GetModel();

            return View(tournamentViewName ,model);
        } 

        //
        // POST: /Tournament/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Tournament/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Tournament/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Tournament/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Tournament/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
