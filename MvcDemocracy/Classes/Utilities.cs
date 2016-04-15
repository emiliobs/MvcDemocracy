using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcDemocracy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace MvcDemocracy.Classes
{
   public class Utilities :IDisposable
    {

        private static MvcDemocracyContext db = new MvcDemocracyContext();

        public static string UploadPhoto(HttpPostedFileBase file)
        {
            //Upload Image:
            string path = string.Empty;
            string picture = string.Empty;

            if (file != null)
            {
                picture = Path.GetFileName(file.FileName);
                path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Photos"), picture);
                file.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }

            return picture;
        }

        public static void CreateASPuser(UserView userView)
        {
            //User management:
            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(userContext));

            //Creaate User Role:
            string roleName = "User";

            //Check to see if role Exists, if not create it:
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));

            }

            //Creaate the ASP NET User:
            var userASP = new ApplicationUser
            {
                UserName = userView.UserName,
                Email = userView.UserName,
                PhoneNumber = userView.Phone
            };

            userManager.Create(userASP, userASP.UserName); //la contraseña es el mismo correo:

            //Add user to role:
            userASP = userManager.FindByName(userView.UserName);
            userManager.AddToRole(userASP.Id, "User");
        }

       
        

        public static void ChangeUserName(string currentUserName, UserChange user)
        {
            //User management:
            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(currentUserName);


            if (userASP == null)
            {
               

                return;
            }

            userManager.Delete(userASP);

            userASP = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.UserName,
                PhoneNumber = user.Phone,
            };

            userManager.Create(userASP, user.CurrentPassword);

            userManager.AddToRole(userASP.Id, "User");            
        }

        public static List<Voting> MyVotings(User user)
        {
            //Get event voting for the current time:
            var state = GetState("Open");

            var votings = db.Votings.Where(v => v.StateId == state.StateId &&
                          v.DateTimeStart <= DateTime.Now &&
                          v.DateTimeEnd >= DateTime.Now).
                          Include(v => v.Candidates).
                          Include(v => v.VotingGroups).
                          Include(v => v.States).ToList();

            //Discart evets in the wich the user already vote:
            foreach (var voting in votings.ToList())//cuando a una lista le ponemos tolist(), es una copia en memoria de la lista:
            {
                //int userId = user.UserId;
                //int votingId = voting.VotingId;

                var votingDetail = db.VotingDetails.Where(vd => vd.VotingId == voting.VotingId && vd.UserId == user.UserId).FirstOrDefault();

                if (votingDetail != null)
                {
                    votings.Remove(voting);

                }

               
            }

            //for (int i  = 0; i  < votings.Count; i ++)
            //{
            //    int userId = user.UserId;
            //    int votingId = votings[i].VotingId;

            //    var votingDetail = db.VotingDetails.Where(vd => vd.VotingId == votingId && vd.UserId == userId).FirstOrDefault();

            //    if (votingDetail != null)
            //    {
            //        votings.RemoveAt(i);

            //    }
            //}

            //Discart events by groups in wich the user are not included:
            foreach (var voting in votings.ToList())
            {
                if (!voting.IsForAllUsers)
                {
                    bool userBelongsToGroup = false;

                    foreach (var votinGroup in voting.VotingGroups)
                    {
                        var userGroup = votinGroup.Group.GroupMembers.Where(gm => gm.UserId == user.UserId).FirstOrDefault();

                        if (userGroup != null)
                        {
                            userBelongsToGroup = true;
                            break;
                        }
                    }

                    if (!userBelongsToGroup)
                    {
                        votings.Remove(voting);
                    }
                }
            }

            return votings;
        }

        public static State GetState(string stateName)
        {

            //Aseguro que siempre el estado sea open si no esxiste:
            var state = db.States.Where(s => s.Description == stateName).FirstOrDefault();

            if (state == null)
            {
                state = new State
                {
                    Description = stateName,
                };

                db.States.Add(state);
                db.SaveChanges();

            }

            return state;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
