using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChampionshipMvc3.Models.Interfaces;
using ChampionshipMvc3.Models.DataContext;
using System.IO;
using ChampionshipMvc3.Models.ViewModels;
using ChampionshipMvc3.Common;
using ChampionshipMvc3.Models.Repositories;

namespace ChampionshipMvc3.Controllers
{

    public class PlayfieldController : Controller
    {
        private const string locationString = "../../Images/";
        private const string relativePath = "../Images/";
        private IPlayfieldRepository playfieldRepository;
        private IPlayfieldOwnerRepository ownerRepository;
        private ITennisPlayfieldOwnerRepository tennisOwnerRepository;
        private ITennisPlayfieldRepository tennisPlayfieldRepository;
    
        private IReservationRepository reservationRepository;
        private IPictureRepository pictureRepository;

        private DateTime StartScheduleDay;

        public PlayfieldController(IPlayfieldRepository playfieldRepoParam, IPlayfieldOwnerRepository ownerRepoParam,
                ITennisPlayfieldOwnerRepository tennisOwnerRepoParam, ITennisPlayfieldRepository tennisPlayfieldRepoParam,
                IReservationRepository reservationRepoParam, IPictureRepository pictureRepoParam)
                                    
        {
            playfieldRepository = playfieldRepoParam;
            ownerRepository = ownerRepoParam;
            tennisOwnerRepository = tennisOwnerRepoParam;
            tennisPlayfieldRepository = tennisPlayfieldRepoParam;
            
            reservationRepository = reservationRepoParam;
            pictureRepository = pictureRepoParam;

        }

        
        public ActionResult Details(Guid id)
        {
            var owner = ownerRepository.GetOwnerById(id);
            
            return View("OwnerDetails", owner);
        }

        #region Football playfields

        public ActionResult CreatePlayfieldOwner()
        {
            return View("CreateOwnerPlayfieldView", ownerRepository.GetModel());
        }

        [HttpPost]
        public ActionResult CreatePlayfieldOwner(PlayfieldOwner ownerModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Picture newPicture = new Picture();
                    newPicture.PictureID = Guid.NewGuid();

                    ownerModel.PlayfieldOwnerID = Guid.NewGuid();
                    newPicture.PlayfieldOwnerID = ownerModel.PlayfieldOwnerID;
                    newPicture.Path = locationString + file.FileName;
                    pictureRepository.AddNewPicture(newPicture);
                    ownerRepository.AddNewOwner(ownerModel);
                    SaveToServer(file);
                }
                catch (Exception ex)
                {
                    //TODO: Log exceptions
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddNewPlayfield()
        {
            PlayfieldViewModel viewModel = new PlayfieldViewModel();

            List<PlayfieldOwner> listOfOwners = ownerRepository.GetAllOwners().ToList();

            viewModel.OwnersSelectList = new List<SelectListItem>();
            for(int index = 0; index < listOfOwners.Count; index++)
            {
                SelectListItem currentItem = new SelectListItem();
                currentItem.Text = listOfOwners[index].Name;
                currentItem.Value = listOfOwners[index].PlayfieldOwnerID.ToString();

                viewModel.OwnersSelectList.Add(currentItem);
            }

             
            return View("AddNewPlayfieldView", viewModel);
        }

        [HttpPost]
        public ActionResult AddNewPlayfield(PlayfieldViewModel playfieldViewModel, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                Playfield playfieldModel = playfieldViewModel.PlayfieldModel;
                playfieldModel.PLayfieldID = Guid.NewGuid();
                PlayfieldOwner playfieldOwner = ownerRepository.GetOwnerById(playfieldViewModel.SelectedId);
                playfieldModel.PlayfieldOwner = playfieldOwner;
                
                foreach (var file in files)
                {
                    Picture newPicture = new Picture();
                    newPicture.PictureID = Guid.NewGuid();
                    newPicture.PlayfieldId = playfieldModel.PLayfieldID;
                    newPicture.Path = locationString + file.FileName;
                    pictureRepository.AddNewPicture(newPicture);
                }

                playfieldRepository.AddNewPlayfield(playfieldModel);

                SaveToServer(files);
    
            }

            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SearchPlayfieldsByCity(string city)
        {
            IList<PlayfieldOwner> playfieldsResult;
            if (city==string.Empty)
            {
                playfieldsResult = playfieldRepository.GetAllPlayfieldsByCityE();
            }
            else
            {
                playfieldsResult = playfieldRepository.GetAllPlayfieldsByCity(city);
            }
            
            return View("PlayfieldOwnerResult", playfieldsResult);
        }

        [HttpPost]
        public ActionResult SearchPlayfieldsByName(string name)
        {
            IList<PlayfieldOwner> playfieldResult = playfieldRepository.GetAllPlayfieldsByName(name);
            return View("PlayfieldOwnerResult", playfieldResult);
        }

        #endregion

        #region TennisPlayfields

        public ActionResult CreateTennisPlayfieldOwner()
        {
            return View("CreateTennisOwnerView", new TennisPlayfieldOwner());
        }

        [HttpPost]
        public ActionResult CreateTennisPlayfieldOwner(TennisPlayfieldOwner tennisOwnerModel, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Picture newPicture = new Picture();
                    newPicture.PictureID = Guid.NewGuid();

                    tennisOwnerModel.TennisPlayfieldOwnerID = Guid.NewGuid();
                    newPicture.TennisOwnerID = tennisOwnerModel.TennisPlayfieldOwnerID;
                    newPicture.Path = locationString + file.FileName;
                    pictureRepository.AddNewPicture(newPicture);

                    tennisOwnerRepository.AddNewOwner(tennisOwnerModel);
                    SaveToServer(file);
                }
                catch (Exception ex)
                {
                    //TODO: Log exception
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddNewTennisPlayfield()
        {
            TennisPlayfieldViewModel viewModel = new TennisPlayfieldViewModel();

            List<TennisPlayfieldOwner> listOfOwners = tennisOwnerRepository
                                                             .GetAllOwners().ToList();

            viewModel.OwnersSelectList = new List<SelectListItem>();
            for (int index = 0; index < listOfOwners.Count; index++)
            {
                SelectListItem currentItem = new SelectListItem();
                currentItem.Text = listOfOwners[index].Name;
                currentItem.Value = listOfOwners[index].TennisPlayfieldOwnerID.ToString();

                viewModel.OwnersSelectList.Add(currentItem);
            }

            return View("AddNewTennisPlayfieldView", viewModel);
        }

        [HttpPost]
        public ActionResult AddNewTennisPlayfield(TennisPlayfieldViewModel playfieldViewModel, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                TennisPlayfield tennisPlayfieldModel = playfieldViewModel.TennisPlayfieldModel;
                tennisPlayfieldModel.TennisPlayfieldID = Guid.NewGuid();
                TennisPlayfieldOwner playfieldOwner = tennisOwnerRepository.GetOwnerById(playfieldViewModel.SelectedId);
                tennisPlayfieldModel.TennisPlayfieldOwner = playfieldOwner;

                foreach (var file in files)
                {
                    Picture newPicture = new Picture();
                    newPicture.PictureID = Guid.NewGuid();
                    newPicture.TennisOwnerID = tennisPlayfieldModel.TennisPlayfieldOwnerID;
                    newPicture.Path = locationString + file.FileName;
                    pictureRepository.AddNewPicture(newPicture);
                }

                tennisPlayfieldRepository.AddNewPlayfield(tennisPlayfieldModel);

                SaveToServer(files);

            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SearchTennisPlayfieldsByCity(string city)
        {

            IList<TennisPlayfieldOwner> playfieldsResult = 
                tennisPlayfieldRepository.GetAllPlayfieldsByCity(city);

            return View("TennisPlayfieldOwnerResult", playfieldsResult);
        }

        [HttpPost]
        public ActionResult SearchTennisPlayfieldsByName(string name)
        {
            IList<TennisPlayfieldOwner> playfieldResult = 
                tennisPlayfieldRepository.GetAllPlayfieldsByName(name);
            
            return View("TennisPlayfieldOwnerResult", playfieldResult);
        }

        #endregion

        public ActionResult PlayfieldDetails(Guid playfieldId)
        {
            var selectedPlayfield = playfieldRepository.GetPlayfieldById(playfieldId);

            return View(selectedPlayfield);
        }

        //
        // POST: /Playfield/Delete/5

        private void SaveToServer(HttpPostedFileBase[] files)
        {
            foreach (var singleFile in files)
            {
                var fileName = Path.GetFileName(singleFile.FileName);
                singleFile.SaveAs(Server.MapPath(relativePath + singleFile.FileName));
            }

            //string location = locationString + fileViewModel.File.FileName;
            //fileViewModel.File.SaveAs(Server.MapPath(location));
        }

        private void SaveToServer(HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);
            file.SaveAs(Server.MapPath(relativePath + file.FileName));
        }

        #region Reservation

        public PartialViewResult GetSchedule(Guid PlayfieldsList)
        {
            Playfield playfield = null;

            if (PlayfieldsList == Guid.Empty)
            {
                playfield = Session["Playfield"] as Playfield;
            }
            else
            {
                playfield = playfieldRepository.GetPlayfieldById(PlayfieldsList);

                StartScheduleDay = DateTime.Now;

                Session["Playfield"] = playfield;
                ViewBag.StartScheduleDay = StartScheduleDay;
                Session["BeginScheduleDay"] = StartScheduleDay;
            }

            
            return PartialView("AnonymousScheduleView", playfield);
        }

        public PartialViewResult GetOwnerSchedule(Guid PlayfieldsList)
        {
            Playfield playfield = null;

            if (PlayfieldsList == Guid.Empty)
            {
                playfield = Session["Playfield"] as Playfield;
            }
            else
            {
                playfield = playfieldRepository.GetPlayfieldById(PlayfieldsList);

                StartScheduleDay = DateTime.Now;

                Session["Playfield"] = playfield;
                ViewBag.StartScheduleDay = StartScheduleDay;
                Session["StartScheduleDay"] = StartScheduleDay;
            }

            return PartialView("PlayfieldScheduleView", playfield);
        }

        public PartialViewResult RefreshScheduleNext()
        {
            StartScheduleDay = ((DateTime)Session["StartScheduleDay"]).AddDays(7);

            ViewBag.StartScheduleDay = StartScheduleDay;

            Session["StartScheduleDay"] = StartScheduleDay;

            return GetOwnerSchedule(Guid.Empty);
        }

        public PartialViewResult RefreshSchedulePrevious()
        {
            StartScheduleDay = ((DateTime)Session["StartScheduleDay"]).AddDays(-7);

            ViewBag.StartScheduleDay = StartScheduleDay;

            Session["StartScheduleDay"] = StartScheduleDay;

            return GetOwnerSchedule(Guid.Empty);
        }

        public PartialViewResult RefreshAnonScheduleNext()
        {
            StartScheduleDay = ((DateTime)Session["BeginScheduleDay"]).AddDays(7);

            ViewBag.StartScheduleDay = StartScheduleDay;

            Session["BeginScheduleDay"] = StartScheduleDay;

            return GetSchedule(Guid.Empty);
        }

        public PartialViewResult RefreshAnonSchedulePrevious()
        {
            StartScheduleDay = ((DateTime)Session["BeginScheduleDay"]).AddDays(-7);

            ViewBag.StartScheduleDay = StartScheduleDay;

            Session["BeginScheduleDay"] = StartScheduleDay;

            return GetSchedule(Guid.Empty);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult ReservePlayfield(string hour, string date, string name, 
                                                    string phone, string weeks)
        {
            Playfield playfield = Session["Playfield"] as Playfield;
            Session.Remove("Playfield");

            int weeksInteger = int.Parse(weeks);
            DateTime nextDate = DateTime.Parse(date + " " + hour + ":00");
            for (int weeksIndex = 0; weeksIndex < weeksInteger; weeksIndex++)
            {
                Reservation reservation = new Reservation();

                reservation.ReservationID = Guid.NewGuid();
                reservation.ReservedDateHour = nextDate;
                reservation.SubmissionDate = DateTime.Now;
                reservation.Name = name;
                reservation.Phone = phone;
                reservation.Playfield = playfield;

                reservationRepository.AddNewReservation(reservation);

                nextDate = nextDate.AddDays(7);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public string ReservationDetails(string hour, string date)
        {
            var reservedDate = DateTime.Parse(date + " " + hour + ":00");

            var reservation = reservationRepository.GetReservationByDateHour(reservedDate);

            string namePhone = reservation.Name + " " + reservation.Phone;

            return namePhone;
        }

        [HttpPost]
        public ActionResult CancelReservation(string hour, string date)
        {
            var reservedDateHour = DateTime.Parse(date + " " + hour + ":00");

            reservationRepository.RemoveReservation(reservedDateHour);

            return RedirectToAction("Index", "Home");
        }

        //public ActionResult SuccessReservation()
        //{
        //    return View("SuccessReservation");
        //}

        #endregion
    }
}

