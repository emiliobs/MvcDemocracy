using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MvcDemocracy.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
//
using MvcDemocracy.Classes;

namespace MvcDemocracy.Controllers.API
{
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private MvcDemocracyContext db = new MvcDemocracyContext();


        [HttpPost]
        [Route("Login")]
        public  IHttpActionResult Login(JObject form)
        {
            dynamic jsonObject = form;

            var email = string.Empty;
            var password = string.Empty;

            try
            {
                email = jsonObject.email.Value;//Pilas co V(Value) en mayuscula:

            }
            catch 
            {                             
            }

            if (string.IsNullOrEmpty(email))
            {
                return this.BadRequest("Icorrect Call.");
            }

            try
            {
                password = jsonObject.password.Value; //Pilas co V(Value) en mayuscula:
            }
            catch 
            {                                  
            }

            if (string.IsNullOrEmpty(password))
            {
                return this.BadRequest("Incorrect Password.");
            }

            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.Find(email, password);

            if (userASP == null)
            {
                return this.BadRequest("Incorrect user or Password.");
            }

            var user = db.Users.Where(u => u.UserName.ToLower() == email.ToLower()).FirstOrDefault();

            if (user == null)
            {
                return this.BadRequest("Problem better call saul.");
            }


          

            return this.Ok(user);
        }

      //GET: api/Users
       public IQueryable<User> GetUsers()
        {
           return db.Users;
       }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

       // PUT: api/Users/5

        [ResponseType(typeof(void))]

        [HttpPut]
        public IHttpActionResult PutUser(int id, UserChange user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }

            //verificu si el usuario cambio:
            var currentUser = db.Users.Find(id);
            if (currentUser == null)
            {
                return this.BadRequest("User not Found.");
            }

            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = userManager.Find(currentUser.UserName, user.CurrentPassword);
            
            if (userASP == null)
            {

                return this.BadRequest("Password Wrong.");
            }

            if (currentUser.UserName != user.UserName)
            {
                Utilities.ChangeUserName(currentUser.UserName ,user);
            }

            var userModel = new User
            {
                Address = user.Address,
                FirstName = user.FirstName,
                Grade = user.Grade,
                Group = user.Group,
                lastName =user.lastName,
                Phone = user.Phone,
                Photo = user.Photo,
                UserId = user.UserId,
                UserName = user.UserName,
            };

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message.ToString()); 
            }

            return this.Ok(user);
        }

        [HttpPost]
        public IHttpActionResult PostUser(RegisterUserView userView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Address = userView.Address,
                FirstName = userView.FirstName,
                Grade = userView.Grade,
                Group = userView.Group,
                lastName = userView.lastName,
                Phone = userView.Phone,
                UserName = userView.UserName,
                

            };

            db.Users.Add(user);
            db.SaveChanges();

            Utilities.CreateASPuser(userView);

           
            return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
        }

         [HttpPut]
        //[Route("changePassword/{userId}")]
        [Route("changePassword")]
        public IHttpActionResult changePassword(/*int userId*/ JObject form)
        {
            int userId = 0;
            var oldPassword = string.Empty;
            var newPassword = string.Empty;
            dynamic jsonObject = form;

            try
            {
                userId = (int)jsonObject.userId.Value;
                oldPassword = jsonObject.oldPassword.Value;
                newPassword = jsonObject.newPassword.Value;

            }
             catch(Exception e)
            {

                return this.BadRequest("Incorrect CAll." + e.Message);
            }

            var user = db.Users.Find(userId);
            if (user == null)
            {
                return this.BadRequest("User Not Found");
            }

            //si lo encuentro, cambio el password:
            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.Find(user.UserName, oldPassword);

            if (userASP == null)
            {
                return this.BadRequest("Incorrect Old Password.");

            }

           // userManager.RemovePassword(userASP.Id);

            var response = userManager.ChangePassword(userASP.Id, oldPassword, newPassword);

            if (response.Succeeded)
            {
                return this.Ok<object>(new
                {
                    Message = "The Password was changed Successfully"
                });
            }           
            else
            {
                return this.BadRequest(response.Errors.ToString());
            }

           
        }

        //// POST: api/Users
        //[ResponseType(typeof(User))]
        //public IHttpActionResult PostUser(User user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Users.Add(user);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
        //}

        //// DELETE: api/Users/5
        //[ResponseType(typeof(User))]
        //public IHttpActionResult DeleteUser(int id)
        //{
        //    User user = db.Users.Find(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Users.Remove(user);
        //    db.SaveChanges();

        //    return Ok(user);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.UserId == id) > 0;
        }
    }
}