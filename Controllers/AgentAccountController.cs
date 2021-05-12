using Account.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Account.Controllers
{
    public class AgentAccountController : Controller
    {
        // GET: Account

        public ActionResult Index()
        {
            return View("Index", "Account");
        }

        public ActionResult AgentRegister()
        {
            return View();
        }

        //The form's data in Register view is posted to this method.
        //We have binded the Register View with Register ViewModel, so we can accept object of Register class as parameter.
        //This object contains all the values entered in the form by the user.
        [HttpPost]
        public ActionResult SaveAgentRegisterDetails(AgentRegister registerDetails)
        {
            //We check if the model state is valid or not. We have used DataAnnotation attributes.
            //If any form value fails the DataAnnotation validation the model state becomes invalid.
            if (ModelState.IsValid)
            {
                //create database context using Entity framework
                using (var databaseContext = new AccountContext())
                {
                    //If the model state is valid i.e. the form values passed the validation then we are storing the User's details in DB.
                    AgentRegister reglog = new AgentRegister();

                    //Save all details in RegitserUser object

                    reglog.AgentFirstName = registerDetails.AgentFirstName;
                    reglog.AgentLastName = registerDetails.AgentLastName;
                    reglog.AgentDoB = registerDetails.AgentDoB;
                    reglog.AgentGender = registerDetails.AgentGender;
                    reglog.AgentContactNumber = registerDetails.AgentContactNumber;
                    reglog.AgentId = registerDetails.AgentId;
                    reglog.AgentPassword = registerDetails.AgentPassword;
                    reglog.Department = registerDetails.Department;

                    int count = databaseContext.AgentRegisters.Where(x => x.AgentId.Equals(reglog.AgentId)).Count();
                    if (count == 0)
                    {
                        //Calling the SaveDetails method which saves the details.
                        databaseContext.AgentRegisters.Add(reglog);
                    }
                    else
                    {
                        ViewBag.Message = "Agent User ID Already Exist";
                        return View("AgentRegister");
                    }

                    try
                    {
                        // Your code...
                        // Could also be before try if you know the exception occurs in SaveChanges

                        databaseContext.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }

                ViewBag.Message = "Agent User Details Saved";
                return View("AgentRegister");
            }
            else
            {
                //If the validation fails, we are returning the model object with errors to the view, which will display the error messages.
                return View("AgentRegister", registerDetails);
            }
        }

        public ActionResult AgentLogin()
        {
            return View();
        }

        //The login form is posted to this method.
        [HttpPost]
        public ActionResult AgentLogin(AgentLogin model)
        {
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {
                //Validating the user, whether the user is valid or not.
                var isValidUser = IsValidUser(model);

                //If user is valid & present in database, we are redirecting it to Welcome page.
                if (isValidUser != null)
                {
                    if (isValidUser.Department == "Pickup")
                    {
                        Session["empid"] = isValidUser.AgentId;
                        FormsAuthentication.SetAuthCookie(model.AgentUserId, false);
                        return View("PickupNewLoans", model);
                    }
                    else if (isValidUser.Department == "Verification")
                    {
                        Session["empid"] = isValidUser.AgentId;
                        FormsAuthentication.SetAuthCookie(model.AgentUserId, false);
                        return View("VerificationNewLoans", model);
                    }
                    // if (isValidUser.Department == "Legal")
                    else
                    {
                        Session["empid"] = isValidUser.AgentId;
                        FormsAuthentication.SetAuthCookie(model.AgentUserId, false);
                        return View("LegalNewLoans", model);
                    }
                    //newrequest page
                }
                else
                {
                    //If the username and password combination is not present in DB then error message is shown.
                    ModelState.AddModelError("Failure", "Wrong Username and password combination !");
                    return View();
                }
            }
            else
            {
                //If model state is not valid, the model with error message is returned to the View.
                return View(model);
            }
        }

        //function to check if User is valid or not
        public AgentRegister IsValidUser(AgentLogin model)
        {
            using (var dataContext = new AccountContext())
            {
                //Retireving the user details from DB based on username and password enetered by user.
                AgentRegister user = dataContext.AgentRegisters.Where(query => query.AgentId.Equals(model.AgentUserId) && query.Department.Equals(model.Department) && query.AgentPassword.Equals(model.AgentPassword)).FirstOrDefault();
                //If user is present, then true is returned.
                if (user == null)
                    return null;
                //If user is not present false is returned.
                else
                    return user;
            }
        }

        public ActionResult ApprovalAgency()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ApprovalAgency(ApprovalAgency details)
        {
            if (ModelState.IsValid)
            {
                if (details.AdminId.Equals("approvalagencyadmin") && details.Password.Equals("admin"))
                {
                    FormsAuthentication.SetAuthCookie(details.AdminId, false);

                    return View("ApprovalAgencyMainPage");
                }
                else
                {
                    ViewBag.Message = "Invalid AdminId and Password!----";
                }
            }

            return View();
        }

        public ActionResult NewLoans()
        {
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.ApprovalAgencyStatus == null || s.ApprovalAgencyStatus == "DisApproved");
            //context.SaveChanges();
            return View(details);
        }

        public ActionResult ShowLoanDetails(int loanid, string loantype)
        {
            Loan details = new Loan();
            using (var context = new AccountContext())
            {
                details = context.Loans.Where(x => x.LoanId.Equals(loanid) && x.LoanType == loantype).FirstOrDefault();
                context.SaveChanges();
            }
            Session["loanid"] = details.LoanId;
            return View(details);
        }

        public ActionResult approve()
        {
            ViewBag.approvemsg = "The Loan is Approved and Forwarded to The Loan Agency";

            using (var context = new AccountContext())
            {
                int SelectedLoanId = Convert.ToInt32(Session["loanid"]);
                var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == SelectedLoanId);
                selectedloan.ApprovalAgencyStatus = "Approved";
                context.SaveChanges();
            }
            return View("ShowLoanDetails");
        }

        public ActionResult disapprove()
        {
            ViewBag.approvemsg = "The Loan is Disapproved";
            //string SelectedUserId = Session["userid"].ToString();
            using (var context = new AccountContext())
            {
                int SelectedLoanId = Convert.ToInt32(Session["loanid"]);
                var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == SelectedLoanId);
                selectedloan.ApprovalAgencyStatus = "DisApproved";
                context.SaveChanges();
            }
            return View("ShowLoanDetails");
        }

        public ActionResult LoanAgencyAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoanAgencyAdmin(LoanAgencyAdmin details)
        {
            if (ModelState.IsValid)
            {
                if (details.LoanAgencyAdminId.Equals("loanagencyadmin") && details.LoanAgencyAdminPassword.Equals("admin"))
                {
                    ViewBag.Message = "LoanAgencyAdmin Login Successfull!-----";
                    FormsAuthentication.SetAuthCookie(details.LoanAgencyAdminId, false);
                    return View("LoanAgencyAdminMainPage");
                }
                else
                {
                    ViewBag.Message = "Invalid LoanAgencyAdmin ID and Password!----";
                }
            }

            return View();
        }

        public ActionResult LoanAgencyAdminMainPage()
        {
            //Session.Clear();
            return View();
        }

        public ActionResult LoanAgencyAdminNewLoans()
        {
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.ApprovalAgencyStatus == "Approved" && s.PickUpStatus == null);
            return View(details);
        }

        public ActionResult LoanAgencyAdminShowLoanDetails(int loanid)
        {
            Loan loandetails = new Loan();
            //Register Registerdetails = new Register();
            using (var context = new AccountContext())
            {
                loandetails = context.Loans.Where(x => x.LoanId.Equals(loanid)).FirstOrDefault();
                context.SaveChanges();
            }
            Session["loanid"] = loandetails.LoanId;
            return View(loandetails);
        }

        public ActionResult MappingEmployee(string loanid)
        {
            var context = new AccountContext();
            //EMployees of Pickup Departments
            var all = from ag in context.AgentRegisters
                      where ag.AgentId == ag.AgentId && ag.Department == "Pickup"
                      select ag.AgentId;
            List<string> lsall = new List<string>();
            foreach (var item in all)
            {
                lsall.Add(item);
            }
            //For Employee with 3 or more assignments
            var query = from Agentreg in context.AgentRegisters.Where(x => x.Department.Equals("Pickup"))
                        join ln in context.Loans on Agentreg.AgentId equals ln.PickUpStatus
                        where ln.PickUpStatus != null && ln.PickUpStatus != "PhysicallyVerified"
                        group ln by ln.PickUpStatus into g
                        where g.Count() >= 3
                        select new Lessthan3Employee
                        {
                            id = g.Key
                        };
            List<string> lsall3 = new List<string>();
            foreach (var item in query)
            {
                lsall3.Add(item.id);
            }
            //final
            List<string> Finalresult = new List<string>();
            var resset = all.Except(lsall3);
            foreach (var item in resset)
            {
                Finalresult.Add(item);
            }

            Session["loanid"] = loanid;

            return View(Finalresult);
        }

        // UPDATE the database with mapping a employee
        public ActionResult Map(string agentid)
        {
            ViewBag.approvemsg = "Mapping Done Successfully";
            var context = new AccountContext();
            int loanid = Convert.ToInt32(Session["loanid"]);
            var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == loanid);
            selectedloan.PickUpStatus = agentid;
            context.SaveChanges();
            return View();
        }

        public ActionResult PickupNewLoansAll()
        {
            string Empid = Session["empid"].ToString();
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.PickUpStatus == Empid);
            return View(details);
        }

        public ActionResult PickupLoanDetails(int loanid, string loantype)
        {
            Loan details = new Loan();
            using (var context = new AccountContext())
            {
                details = context.Loans.Where(x => x.LoanId.Equals(loanid) && x.LoanType == loantype).FirstOrDefault();
                context.SaveChanges();
            }
            Session["loanid"] = details.LoanId;
            return View(details);
        }

        public ActionResult Pickupapprove()
        {
            ViewBag.approvemsg = "Physically Verified Successfully";
            using (var context = new AccountContext())
            {
                int SelectedLoanId = Convert.ToInt32(Session["loanid"]);
                var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == SelectedLoanId);
                selectedloan.PickUpStatus = "PhysicallyVerified";

                context.SaveChanges();
            }
            return View("PickupLoanDetails");
        }

        public ActionResult LoanAgencyPhysicallyVerified()
        {
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.PickUpStatus == "PhysicallyVerified" && s.VerificationStatus == null);
            return View(details);
        }

        public ActionResult LoanAgencyPhysicallyVerifiedShowLoanDetails(int loanid)
        {
            Loan loandetails = new Loan();
            using (var context = new AccountContext())
            {
                loandetails = context.Loans.Where(x => x.LoanId.Equals(loanid)).FirstOrDefault();
                context.SaveChanges();
            }
            Session["loanid"] = loandetails.LoanId;
            return View(loandetails);
        }

        public ActionResult LoanAgencyPhysicallyVerifiedMappingEmployee(string loanid)
        {
            var context = new AccountContext();
            //EMployees of Pickup Departments
            var all = from ag in context.AgentRegisters
                      where ag.AgentId == ag.AgentId && ag.Department == "Verification"
                      select ag.AgentId;
            List<string> lsall = new List<string>();
            foreach (var item in all)
            {
                lsall.Add(item);
            }
            //For Employee with 3 or more assignments
            var query = from Agentreg in context.AgentRegisters.Where(x => x.Department.Equals("Verification"))
                        join ln in context.Loans on Agentreg.AgentId equals ln.VerificationStatus
                        where ln.VerificationStatus != null && ln.VerificationStatus != "Verified"
                        group ln by ln.VerificationStatus into g
                        where g.Count() >= 3
                        select new Lessthan3Employee
                        {
                            id = g.Key
                        };
            List<string> lsall3 = new List<string>();
            foreach (var item in query)
            {
                lsall3.Add(item.id);
            }
            //final
            List<string> Finalresult = new List<string>();
            var resset = all.Except(lsall3);
            foreach (var item in resset)
            {
                Finalresult.Add(item);
            }

            Session["loanid"] = loanid;

            return View(Finalresult);
        }

        // UPDATE the database with mapping a employee
        public ActionResult LoanAgencyPhysicallyVerifiedMap(string agentid)
        {
            ViewBag.approvemsg = "Mapping Done Successfully";
            var context = new AccountContext();
            int loanid = Convert.ToInt32(Session["loanid"]);
            var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == loanid);
            selectedloan.VerificationStatus = agentid;
            context.SaveChanges();
            return View();
        }

        public ActionResult VerificationNewLoansAll()
        {
            string Empid = Session["empid"].ToString();
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.VerificationStatus == Empid);
            return View(details);
        }

        public ActionResult VerificationLoanDetails(int loanid, string loantype)
        {
            Loan details = new Loan();
            using (var context = new AccountContext())
            {
                details = context.Loans.Where(x => x.LoanId.Equals(loanid) && x.LoanType == loantype).FirstOrDefault();
                context.SaveChanges();
            }
            Session["loanid"] = details.LoanId;
            return View(details);
        }

        public ActionResult Verificationapprove()
        {
            ViewBag.approvemsg = "Verified Successfully";
            using (var context = new AccountContext())
            {
                int SelectedLoanId = Convert.ToInt32(Session["loanid"]);
                var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == SelectedLoanId);
                selectedloan.VerificationStatus = "Verified";
                context.SaveChanges();
            }

            return View("VerificationLoanDetails");
        }

        public ActionResult LoanAgencyVerified()
        {
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.VerificationStatus == "Verified" && s.LegalStatus == null);
            return View(details);
        }

        public ActionResult LoanAgencyVerifiedShowLoanDetails(int loanid)
        {
            Loan loandetails = new Loan();
            //Register Registerdetails = new Register();
            using (var context = new AccountContext())
            {
                loandetails = context.Loans.Where(x => x.LoanId.Equals(loanid)).FirstOrDefault();
                context.SaveChanges();
            }
            Session["loanid"] = loandetails.LoanId;
            return View(loandetails);
        }

        public ActionResult LoanAgencyVerifiedMappingEmployee(string loanid)
        {
            var context = new AccountContext();
            //EMployees of Pickup Departments
            var all = from ag in context.AgentRegisters
                      where ag.AgentId == ag.AgentId && ag.Department == "Legal"
                      select ag.AgentId;
            List<string> lsall = new List<string>();
            foreach (var item in all)
            {
                lsall.Add(item);
            }
            //For Employee with 3 or more assignments
            var query = from Agentreg in context.AgentRegisters.Where(x => x.Department.Equals("Legal"))
                        join ln in context.Loans on Agentreg.AgentId equals ln.LegalStatus
                        where ln.LegalStatus != null && ln.LegalStatus != "LegallyVerified"
                        group ln by ln.LegalStatus into g
                        where g.Count() >= 3
                        select new Lessthan3Employee
                        {
                            id = g.Key
                        };
            List<string> lsall3 = new List<string>();
            foreach (var item in query)
            {
                lsall3.Add(item.id);
            }
            //final
            List<string> Finalresult = new List<string>();
            var resset = all.Except(lsall3);
            foreach (var item in resset)
            {
                Finalresult.Add(item);
            }

            Session["loanid"] = loanid;

            return View(Finalresult);
        }

        // UPDATE the database with mapping a employee
        public ActionResult LoanAgencyVerifiedMap(string agentid)
        {
            ViewBag.approvemsg = "Mapping Done Successfully";
            var context = new AccountContext();
            int loanid = Convert.ToInt32(Session["loanid"]);
            var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == loanid);
            selectedloan.LegalStatus = agentid;
            context.SaveChanges();
            return View();
        }

        public ActionResult LegalNewLoansAll()
        {
            string Empid = Session["empid"].ToString();
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.LegalStatus == Empid);
            return View(details);
        }

        public ActionResult LegalNewLoanDetails(int loanid, string loantype)
        {
            Loan details = new Loan();
            using (var context = new AccountContext())
            {
                details = context.Loans.Where(x => x.LoanId.Equals(loanid) && x.LoanType == loantype).FirstOrDefault();
                context.SaveChanges();
            }
            Session["loanid"] = details.LoanId;
            return View(details);
        }

        public ActionResult Legalapprove()
        {
            ViewBag.approvemsg = "Legally Verified Successfully";
            using (var context = new AccountContext())
            {
                int SelectedLoanId = Convert.ToInt32(Session["loanid"]);
                var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == SelectedLoanId);
                selectedloan.LegalStatus = "LegallyVerified";

                context.SaveChanges();
            }
            return View("LegalNewLoanDetails");
        }

        public ActionResult LoanAgencyLegallyVerified()
        {
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.LegalStatus == "LegallyVerified" && s.FinalStatus == null);
            return View(details);
        }

        public ActionResult LoanAgencyLegallyShowLoanDetails(int loanid)
        {
            Loan loandetails = new Loan();

            using (var context = new AccountContext())
            {
                loandetails = context.Loans.Where(x => x.LoanId.Equals(loanid)).FirstOrDefault();
                context.SaveChanges();
            }
            Session["loanid"] = loandetails.LoanId;
            return View(loandetails);
        }

        public ActionResult FinalApprove()
        {
            ViewBag.approvemsg = "The Loan is Approved";
            using (var context = new AccountContext())
            {
                int SelectedLoanId = Convert.ToInt32(Session["loanid"]);
                var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == SelectedLoanId);
                selectedloan.FinalStatus = "Approved";
                context.SaveChanges();
            }
            return View("LoanAgencyLegallyShowLoanDetails");
        }

        public ActionResult FinalDisapprove()
        {
            ViewBag.approvemsg = "The Loan is Rejected";
            using (var context = new AccountContext())
            {
                int SelectedLoanId = Convert.ToInt32(Session["loanid"]);
                var selectedloan = context.Loans.FirstOrDefault(a => a.LoanId == SelectedLoanId);
                selectedloan.FinalStatus = "DisApproved";
                context.SaveChanges();
            }
            return View("LoanAgencyLegallyShowLoanDetails");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "AgentAccount");
        }

        //public ActionResult NewRequest(AgentLogin model)
        //{
        //    return View(model);
        //}

        //public ActionResult ProcessRequest()
        //{
        //    List<Loan> details = new List<Loan>();

        //    using (var context = new AccountContext())
        //    {
        //        details = context.Loans.ToList();
        //        context.SaveChanges();
        //    }
        //    return View(details);
        //}
    }
}