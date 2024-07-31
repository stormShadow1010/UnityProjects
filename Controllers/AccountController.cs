using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ChampionshipMvc3.Models;
using ChampionshipMvc3.Models.DataContext;
using ChampionshipMvc3.Models.Repositories;
using ChampionshipMvc3.Models.Interfaces;
using ChampionshipMvc3.Models.Enums;
using ChampionshipMvc3.Models.ViewModels;

namespace ChampionshipMvc3.Controllers
{
    public class AccountController : Controller
    {

        private const string registerTeamView = "RegisterTeam";
        private const string registerPlayerView = "_RegisterPlayer";

        private IPlayerRepository playerRepository;
        private ITeamRepository teamRepository;
        private IPlayfieldOwnerRepository ownerRepository;
        //
        // GET: /Account/LogOn

        public AccountController()
        {
            playerRepository = new PlayerRepository();
            teamRepository = new TeamRepository();
            ownerRepository = new PlayfieldOwnerRepository();
        }

        public ActionResult LogIn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogIn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    //if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    //    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    //else
                    //{
                        return RedirectToAction("OwnerPanel", "Admin");
                    //}
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("FootballSearchPage", "Home");
        }

        //
        // GET: /Account/Register


        public ActionResult RegisterOwner()
        {
            RegisterOwnerViewModel regViewModel = new RegisterOwnerViewModel();
            List<PlayfieldOwner> listOfOwners = ownerRepository.GetAllOwners().ToList();

            regViewModel.OwnersSelectList = new List<SelectListItem>();
            foreach (var owner in listOfOwners)
            {
                SelectListItem item = new SelectListItem();
                item.Text = owner.Name;
                item.Value = owner.PlayfieldOwnerID.ToString();
                regViewModel.OwnersSelectList.Add(item);
            }

            return View("RegisterOwnerView", regViewModel);
        }

        [HttpPost]
        public ActionResult RegisterOwner(RegisterModel regModel, RegisterOwnerViewModel ownerViewModel)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus = RegisterUser(regModel);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    Guid userId = ownerRepository.GetUserId(regModel.UserName);
                    ownerRepository.UpdatePlayfieldOwner(ownerViewModel.SelectedId, userId);
                }
            }

            return RedirectToAction("Index", "Home");
        }
        public ActionResult RegisterTeam()
        {
            return PartialView(registerTeamView);
        }
        
        [HttpPost]
        public ActionResult RegisterTeam(Team teamModel, RegisterModel regModel, Player playerModel)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = RegisterUser(regModel);
                
                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(regModel.UserName, false /* createPersistentCookie */);

                    teamRepository.AddNewTeam(teamModel);

                    playerModel.UserId = (Guid)Membership.GetUser(regModel.UserName).ProviderUserKey;
                    playerModel.Team = teamModel;
                    playerModel.PlayerType = PlayerType.Captain.ToString(); //enum necessary
                    playerModel.IsApproved = false;
                    playerRepository.AddNewPlayer(playerModel);
                }
                return RedirectToAction("Index", "Home");
            }

            return PartialView(teamModel);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [HttpGet]
        public ActionResult RegisterPlayer()
        {
            return PartialView(registerPlayerView);
        }

        [HttpPost]
        public ActionResult RegisterPlayer(RegisterModel regModel, Player playerModel, Team teamModel)
        {
            if (ModelState.IsValid)
            {
                //TODO: CHECK TEAM PASSWORD THEN REGISTER PLAYER
                //TODO: SCHEDULE INITIALIZE PLAYER AND TEAM

                var playerTeam = teamRepository.FindTeamByName(teamModel.TeamName);

                //Attempt to register the user
                MembershipCreateStatus createStatus = RegisterUser(regModel);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(regModel.UserName, false /* createPersistentCookie */);
                        
                    playerModel.Team = playerTeam;
                    playerModel.UserId = (Guid)Membership.GetUser(regModel.UserName).ProviderUserKey;
                    playerModel.PlayerType = PlayerType.RegularPlayer.ToString();//enum necessary
                    playerModel.IsApproved = false;
                    playerRepository.AddNewPlayer(playerModel);
                }
                return RedirectToAction("Index", "Home");
            }

            return PartialView(registerPlayerView);
        }

        private static MembershipCreateStatus RegisterUser(RegisterModel regModel)
        {
            MembershipCreateStatus createStatus;
            Membership.CreateUser(regModel.UserName, regModel.Password, regModel.Email, null, null, true, null, out createStatus);
            return createStatus;
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult FilterTeams(string term)
        {
            var teams = teamRepository.GetAllTeams();
            var filteredTeams = teams
                                    .Where(t => t.TeamName.ToLower().Contains(term.ToLower()))
                                    .Take(10)
                                    .Select(t => new { label = t.TeamName });

            return Json(filteredTeams, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
