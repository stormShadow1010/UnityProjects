﻿using ChampionshipMvc3.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChampionshipMvc3.Controllers
{
    public class ReservationController : Controller
    {
        private const string reservationViewName = "CreateReservation";
        private IReservationRepository reservationRepository;

        public ReservationController(IReservationRepository reservationRepoParam)
        {
            reservationRepository = reservationRepoParam;
        }
        //
        // GET: /Reservation/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Reservation/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Reservation/Create

        public ActionResult Create()
        {
            var model = reservationRepository.GetModel();
            return View(reservationViewName, model);
        } 

        //
        // POST: /Reservation/Create

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
        // GET: /Reservation/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Reservation/Edit/5

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
        // GET: /Reservation/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Reservation/Delete/5

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
