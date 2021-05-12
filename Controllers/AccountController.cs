using Account.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Account.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        //The form's data in Register view is posted to this method.
        //We have binded the Register View with Register ViewModel, so we can accept object of Register class as parameter.
        //This object contains all the values entered in the form by the user.
        [HttpPost]
        public ActionResult SaveRegisterDetails(Register registerDetails)
        {
            //We check if the model state is valid or not. We have used DataAnnotation attributes.
            //If any form value fails the DataAnnotation validation the model state becomes invalid.
            if (ModelState.IsValid)
            {
                //create database context using Entity framework
                using (var databaseContext = new AccountContext())
                {
                    //If the model state is valid i.e. the form values passed the validation then we are storing the User's details in DB.
                    Register reglog = new Register();

                    reglog.FirstName = registerDetails.FirstName;
                    reglog.LastName = registerDetails.LastName;
                    reglog.DoB = registerDetails.DoB;
                    reglog.Gender = registerDetails.Gender;
                    reglog.ContactNumber = registerDetails.ContactNumber;
                    reglog.Email = registerDetails.Email;
                    reglog.UserId = registerDetails.UserId;
                    reglog.Password = registerDetails.Password;
                    //Save all details in RegitserUser object
                    if (databaseContext.Registers.Where(x => x.UserId.Equals(registerDetails.UserId)).FirstOrDefault() == null)
                    {
                        //Calling the SaveDetails method which saves the details.
                        databaseContext.Registers.Add(reglog);

                        databaseContext.SaveChanges();
                        ViewBag.Message = "User Details Saved";
                        return View("Register");
                    }
                    else
                    {
                        ViewBag.Message = "UserId already exists. Try another!--------";
                        return View("Register");
                    }
                }
            }
            else
            {
                //If the validation fails, we are returning the model object with errors to the view, which will display the error messages.
                return View("Register", registerDetails);
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        //The login form is posted to this method.
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {
                //Validating the user, whether the user is valid or not.
                var isValidUser = IsValidUser(model);

                //If user is valid & present in database, we are redirecting it to Welcome page.
                if (isValidUser != null)
                {
                    FormsAuthentication.SetAuthCookie(model.UserId, false);
                    Session["userid"] = model.UserId;
                    return RedirectToAction("Welcome", model);
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
        public Register IsValidUser(LoginViewModel model)
        {
            using (var dataContext = new AccountContext())
            {
                //Retireving the user details from DB based on username and password enetered by user.
                Register user = dataContext.Registers.Where(query => query.UserId.Equals(model.UserId) && query.Password.Equals(model.Password)).FirstOrDefault();
                //If user is present, then true is returned.
                if (user == null)
                    return null;
                //If user is not present false is returned.
                else
                    return user;
            }
        }

        public ActionResult Welcome(LoginViewModel model)
        {
            return View(model);
        }

        public ActionResult ApplyLoan()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ApplyLoan(Loan info)
        {
            int count = 0;
            string userid = Session["userid"].ToString();
            Loan loan = new Loan();
            if (ModelState.IsValid)
            {
                using (var context = new AccountContext())
                {
                    if ((context.Registers.Where(x => x.UserId.Equals(info.UserId)) != null))
                    {
                        loan.UserId = userid;
                        loan.Occupation = info.Occupation;
                        loan.LoanType = info.LoanType;
                        loan.AnnualIncome = info.AnnualIncome;
                        loan.LoanAmount = info.LoanAmount;

                        //check for userid and loan type already exists in database
                        count = context.Loans.Where(x => x.UserId.Equals(loan.UserId) && x.LoanType.Equals(loan.LoanType)).Count();
                        if (count == 0)
                        {
                            context.Loans.Add(loan);
                            context.SaveChanges();
                            ViewBag.Message = "loan details saved!";
                        }
                        else
                            ViewBag.Message = "Already saved details into database";
                        return View(loan);
                    }
                    else
                    {
                        ViewBag.Message = "Invalid User Id. Try again!---";
                        return View(info);
                    }
                }
            }

            return View(info);
        }

        public ActionResult ViewLoans()
        {
            string userid = Session["userid"].ToString();
            var context = new AccountContext();
            var details = context.Loans.Where(s => s.UserId == userid);
            //context.SaveChanges();
            return View(details);
        }

        public ActionResult LoanStatus(int loanid)
        {
            string userid = Session["userid"].ToString();
            var context = new AccountContext();
            int approvedcount = context.Loans.Where(x => x.LoanId == loanid && x.FinalStatus == "Approved").Count();
            int DisApprovedcount = context.Loans.Where(x => x.LoanId == loanid && x.FinalStatus == "DisApproved").Count();

            if (approvedcount != 0)
            {
                ViewBag.Message = "Congratulations! Your Loan is Approved";
            }
            else if (DisApprovedcount != 0)
            {
                ViewBag.Message = "Sorry!!! Your Loan is Rejected";
            }
            else
            {
                ViewBag.Message = "Your Loan is Under Verification";
            }

            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Account");
        }
    }
}