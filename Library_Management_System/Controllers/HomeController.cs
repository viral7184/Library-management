using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library_Management_System.Controllers;
using Library_Management_System.Models;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace Library_Management_System.Controllers
{
    public class HomeController : Controller
    {
        library_m_Database1Entities1 db = new library_m_Database1Entities1();

        public ActionResult book()
        {
         
            var abc = islogin();
            if (abc == 0)
            {
                return RedirectToAction("login");
            }
           

            ViewBag.book = db.books.Where(i=>i.IS_DELETE ==false).ToList();
            ViewBag.genre = db.departments.Where(i => i.IS_DELETED == false).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult book(book data)
        {
            try
            {
                if (data.ID > 0)
                {
                    var book = db.books.Where(i => i.ID == data.ID).SingleOrDefault();
                    book.BOOK_NAME = data.BOOK_NAME;
                    book.AUTHOR_NAME = data.AUTHOR_NAME;
                    book.BOOK_CODE = data.BOOK_CODE;
                    book.GENRE = data.GENRE;
                    book.LANGUAGE = data.LANGUAGE;
                    book.SUBMITTED_BY = data.SUBMITTED_BY;
                    book.PRICE = data.PRICE;
                
                    book.AVAILABLE = data.AVAILABLE;
                    db.SaveChanges();
                }
                else
                {
                    data.ENT_DATE = DateTime.Now;
                  
                    var code = db.books.Where(i => i.BOOK_CODE == db.books.Max(j => j.BOOK_CODE)).FirstOrDefault();
                    if (code != null)
                    {
                        var abc = Convert.ToInt16(code.BOOK_CODE);
                         var xyz = abc + 1;                   
                        data.BOOK_CODE =Convert.ToString(xyz);
                    }
                    else
                    {
                        data.BOOK_CODE = "100";
                    }

                    data.IS_DELETE = false;
                    data.AVAILABLE = "Yes";                 
                    db.books.Add(data);
                    db.SaveChanges();
                }
                return RedirectToAction("book");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public JsonResult getbook(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var confi = db.books.Where(i => i.ID == id).Select(i => new
                {
                    i.ID,
                    i.BOOK_NAME,
                    i.AUTHOR_NAME,
                    i.BOOK_CODE,
                    genre = i.department.ID,
                    i.LANGUAGE,
                    i.PRICE,
                    i.SUBMITTED_BY,
                    i.AVAILABLE
                }).FirstOrDefault();
                return Json(new { data = confi }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public int islogin()
        {
           if(Session["login"] == null)
            {
                return 0;
            }
           else
            {
                return 1;
            }
        }


        public ActionResult login()
        {
            Session.Abandon();
            Session.Clear();
            return View();
        }
        [HttpPost]
        public ActionResult login(registration data,issue_book book)
        {
            var login = db.registrations.Where(i => i.EMAIL == data.EMAIL && i.PASSWORD == data.PASSWORD).SingleOrDefault();
            if(login ==null)
            {
                TempData["msg"] = "Email id or Password is wrong";
                return RedirectToAction("login");
            }
            else
            {

                var role = db.user_role.Where(i => i.USER_ID == login.ID && i.IS_DELETED==false).FirstOrDefault();
             
                Session["roll"] = role.USER_ROLE1;
               
                Session["login"] = login.NAME;
                Session["ID"] = login.ID;

                var returnbook = db.issue_book.Where(i=>i.USER_ID==login.ID).ToList();

                foreach(var item in returnbook)
                { 
                if (returnbook != null)
                {                                   
                    var issuedate = Convert.ToDateTime(item.ISSUE_DATE);
                    var expirydate2 =Convert.ToDateTime(item.EXPIRY_DATE);
                    var today2 = expirydate2.AddDays(-1);  
                    if(DateTime.Now.Date==today2.Date)
                    {
                       
                        Session["noti"] = "You have to return a book by Tomorrow";
                        
                    }     
                                       
                    db.SaveChanges();
                }
                }
                return RedirectToAction("book");
            }
        }


        [AllowAnonymous]
        public ActionResult logout()
        {
            try
            {
                Session.Abandon();
                Session.Clear();
            }
            catch (Exception e)
            {

                throw e;
            }

            return RedirectToAction("Login");
        }

        public ActionResult renewbook(registration data)
        {
            var abc = db.registrations.Where(i => i.ID == data.ID).FirstOrDefault();
            var todaydate = DateTime.Now;
            abc.RENEW_DATE = todaydate.AddDays(180);
            
            db.SaveChanges();
            return RedirectToAction("registration");
        }

        //public void openfolder(int id)
        //{
        //    try
        //    {
        //        var confi = db.books.FirstOrDefault();

        //        var fileno = db.books.Where(i => i.ID == id).FirstOrDefault();
        //        var Rno = fileno.BOOK_CODE;
        //        var path = confi.BOOK_CODE;
        //        var year = DateTime.Now.Year;
        //        //  Process.Start("explorer.exe", path + "" + Rno + "-" + year);


        //        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        //        // processStartInfo.WorkingDirectory = @"C:\Users\pcustance\Desktop\";
        //        processStartInfo.WorkingDirectory = path + "" + Rno + "-" + year;
        //        //processStartInfo.FileName = @"notepad.exe";
        //        //processStartInfo.Arguments = "test.txt";
        //        processStartInfo.WindowStyle = ProcessWindowStyle.Maximized;
        //        processStartInfo.CreateNoWindow = true;
        //        Process process = Process.Start("explorer.exe", processStartInfo.WorkingDirectory);


        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public void SendEmail(string receiver, string subject, string message)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("procorner123@gmail.com", "Procorner");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "P@123456";
                    var sub = subject;
                    var body = message;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                   // return View();
                }
            }
            catch (Exception e)
            {
                throw e;
               // ViewBag.Error = "Some Error";
            }
           // return View();
        }

    
       

        public ActionResult registration()
        {
            var abc = islogin();
            if (abc == 0)
            {
                return RedirectToAction("login");
            }
            var depart = db.departments.Where(i=>i.IS_DELETED==false).ToList();
            ViewBag.department = depart;
            var reg = db.registrations.Where(i => i.IS_DELETE == false).ToList();
            ViewBag.register = reg;

            return View();
        }
        [HttpPost]
        public ActionResult registration(registration data)
        {
            if (data.ID > 0)
            {
                var regist = db.registrations.Where(i => i.ID == data.ID).SingleOrDefault();
                if (regist != null)
                {
                    regist.NAME = data.NAME;

                    regist.EMAIL = data.EMAIL;
                    regist.PASSWORD = data.PASSWORD;
                    if (data.ADDRESS != null)
                    {
                        regist.ADDRESS = data.ADDRESS;
                    }

                    if (data.PHONE_NO != null)
                    {
                        regist.PHONE_NO = data.PHONE_NO;
                    }
                    if (data.FEES != null)
                    {
                        regist.FEES = data.FEES;
                    }
                    if (data.ENT_DATE != null)
                    {
                        regist.ENT_DATE = data.ENT_DATE;
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                var regi = db.registrations.Where(i => i.EMAIL == data.EMAIL).FirstOrDefault();
                if(regi !=null)
                {
                    TempData["email"] = "Email Already Exist";
                    return RedirectToAction("registration");
                }
                else
                { 
                data.ENT_DATE = DateTime.Now;
                var date = Convert.ToDateTime(DateTime.Now);
                data.RENEW_DATE = date.AddDays(180);
                data.IS_DELETE = false;
                           
                    if(data.EMAIL !=null)
                    { 
                        
                        SendEmail(data.EMAIL,"registration","You are member of library");
                    }         
                var reg = db.registrations.Add(data);
                db.SaveChanges();
                var lists = new user_role();
                lists.USER_ROLE1 = "Employee";
                lists.USER_ID = data.ID;
                lists.IS_DELETED= false;
                db.user_role.Add(lists);
                db.SaveChanges();
                }
            }
            return RedirectToAction("registration");
            
        }

        public JsonResult getregistration(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var confi = db.registrations.Where(i => i.ID == id).Select(i => new
                {
                    i.ID,
                    i.NAME,
                    i.EMAIL,
                    i.PASSWORD,
                    i.ADDRESS,
                    i.PHONE_NO,
                    i.FEES,
                   
                }).FirstOrDefault();
                return Json(new { data = confi }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw e;
            }

        }


        public ActionResult issuebook()
        {
            var abc = islogin();
            if (abc == 0)
            {
                return RedirectToAction("login");
            }
            var book = db.books.Where(i=>i.AVAILABLE == "Yes").ToList();
            ViewBag.book = book;
            var returnbook = db.issue_book.ToList();
            foreach (var item in returnbook)
            {
                var issuedate = Convert.ToDateTime(item.ISSUE_DATE);
                var expirydate = Convert.ToDateTime(item.EXPIRY_DATE);
                var todaydate = Convert.ToDateTime(DateTime.Now);
                var Difference = todaydate.Subtract(expirydate);
                var ab = Difference.Days;
                if (ab > 0)
                {
                    var delay = ab * 5;
                    item.DELAY_CHARGES = delay.ToString();
                }
                else
                {
                    var zero = 0;
                    item.DELAY_CHARGES = zero.ToString();
                }
                db.SaveChanges();
            }
            var loginid = Convert.ToInt16(Session["ID"]);
            var user_role =Convert.ToString(Session["roll"]).ToString();
            if (user_role == "Admin")
            { 
            var issue = db.issue_book.Where(i=>i.IS_RETURN==false).ToList();
            ViewBag.issuebook = issue;
            }
            if (user_role == "Employee")
            {
                var issue = db.issue_book.Where(i => i.IS_RETURN == false && i.USER_ID == loginid).ToList();
                ViewBag.issuebook = issue;
            }
            var regi = db.registrations.ToList();
            ViewBag.regist = regi;
            return View();
        }

        [HttpPost]
        public ActionResult issuebook(issue_book data)
        {
            if (data.ID > 0)
            {
                var regist = db.issue_book.Where(i => i.ID == data.ID).SingleOrDefault();
              
                if (regist != null)
                {
                    regist.BOOK_ID = data.BOOK_ID;
                    regist.ISSUEBOOK = data.ISSUEBOOK;

                    if (data.ISSUE_DATE != null)
                    {
                        regist.ISSUE_DATE = data.ISSUE_DATE;
                    }

                    //if (data.EXPIRY_DATE != null)
                    //{
                    //    regist.EXPIRY_DATE = data.EXPIRY_DATE;
                    //}
                    if (data.FEES != null)
                    {
                        regist.FEES = data.FEES;
                    }
                    if (data.USER_ID != null)
                    {
                        regist.USER_ID = data.USER_ID;
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                var regist = db.issue_book.Where(i => i.USER_ID == data.USER_ID && i.IS_RETURN==false).SingleOrDefault();
                if (regist !=null)
                {
                    TempData["issuebook"] = "User Have Already issue book";
                    return RedirectToAction("issuebook");
                }
                data.IS_RETURN = false;
                if(data.ISSUE_DATE == null)
                { 
                data.ISSUE_DATE = DateTime.Now;
                    var issuedate = DateTime.Now;
                    data.EXPIRY_DATE = issuedate.AddDays(10);
                }
                else
                {
                    var issuedate = Convert.ToDateTime(data.ISSUE_DATE);
                    data.EXPIRY_DATE = issuedate.AddDays(10);
                }
               
                var reg = db.issue_book.Add(data);
                db.SaveChanges();
                var book = db.books.Where(i => i.ID == data.BOOK_ID).SingleOrDefault();
                book.AVAILABLE = "No";
                db.SaveChanges();
            }

            return RedirectToAction("issuebook");
        }

        public JsonResult getissuebook(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                //  var confi = db.issue_book.Where(i => i.ID == id).SingleOrDefault();


                var confi = db.issue_book.Where(i => i.ID == id).Select(i => new
                {
                    id = i.ID,
                    bookid = i.book.ID,
                    book = i.book.BOOK_NAME,
                    issuedate = i.ISSUE_DATE,
                    expirydate = i.EXPIRY_DATE,
                    fees = i.FEES,
                    userid = i.registration.ID,
                    username = i.registration.NAME,
                }).FirstOrDefault();
                return Json(new { data = confi }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public ActionResult returnbook()
        {
            ViewBag.returnbook = db.issue_book.Where(i => i.IS_RETURN == true).ToList();
            return View();
        }


       
        public ActionResult returnbook1(int? id)
        {
           
            var returnbook = db.issue_book.Where(i => i.ID == id).SingleOrDefault();

            if(returnbook !=null)
            {
                returnbook.IS_RETURN = true;
                returnbook.RETURN_DATE = DateTime.Now;
                var issuedate =Convert.ToDateTime(returnbook.ISSUE_DATE);
                var expirydate = Convert.ToDateTime(returnbook.EXPIRY_DATE);
                var todaydate =Convert.ToDateTime(DateTime.Now);

             var  Difference = todaydate.Subtract(expirydate);
                var ab = Difference.Days;
                if(ab >0)
                {
                    var abc = ab * 5;
                    returnbook.DELAY_CHARGES = abc.ToString();
                }
                else
                {
                    var zero = 0;
                    returnbook.DELAY_CHARGES = zero.ToString();
                }
               
                db.SaveChanges();
            }
           
            var book = db.books.Where(i => i.ID == returnbook.BOOK_ID).SingleOrDefault();
            book.AVAILABLE = "Yes";
            db.SaveChanges();
            return RedirectToAction("returnbook");
        }

        public ActionResult department()
        {
            var abc = islogin();
            if (abc == 0)
            {
                return RedirectToAction("login");
            }
            var depart = db.departments.Where(i => i.IS_DELETED == false).ToList();
            ViewBag.department = depart;

            return View();
        }

        [HttpPost]
        public ActionResult department(department data)
        {
            if (data.ID > 0)
            {
                var regist = db.departments.Where(i => i.ID == data.ID).SingleOrDefault();
                if (regist != null)
                {
                   
                    if (data.DEPARTMENT_NAME != null)
                    {
                        regist.DEPARTMENT_NAME = data.DEPARTMENT_NAME;
                    }                  
                    db.SaveChanges();
                }
            }
            else
            {
                data.IS_DELETED = false;
                var reg = db.departments.Add(data);
                db.SaveChanges();
            }

            return RedirectToAction("department");
        }

        public JsonResult getdepartment(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;                
                var confi = db.departments.Where(i => i.ID == id).SingleOrDefault();               
                return Json(new { data = confi }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw e;
            }

        }



        public ActionResult user_role()
        {
            var abc = islogin();
            if(abc ==0)
            {
                return RedirectToAction("login");
            }
            ViewBag.member = db.registrations.Where(i=>i.IS_DELETE==false).ToList();
            ViewBag.role = db.user_role.Where(i => i.IS_DELETED == false).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult user_role(user_role data)
        {
            if (data.ID > 0)
            {
                var regist = db.user_role.Where(i => i.ID == data.ID).SingleOrDefault();
                if (regist != null)
                {

                    if (data.USER_ID != null)
                    {
                        regist.USER_ID = data.USER_ID;
                    }
                    if (data.USER_ROLE1 != null)
                    {
                        regist.USER_ROLE1 = data.USER_ROLE1;
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                var reg = db.user_role.Add(data);
                db.SaveChanges();
            }

            return RedirectToAction("user_role");
        }

        public JsonResult getuser_role(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var confi = db.user_role.Where(i => i.ID == id).Select(i => new
                {
                    i.ID,
                    i.USER_ROLE1,
                    userid = i.registration.ID,
                }).FirstOrDefault();
                return Json(new { data = confi }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw e;
            }

        }


        public ActionResult help()
        {
            var abc = islogin();
            if (abc == 0)
            {
                return RedirectToAction("login");
            }
            return View();
        }

        public ActionResult requestbook()
        {
            var abc = islogin();
            if (abc == 0)
            {
                return RedirectToAction("login");
            }
            ViewBag.requestbook = db.request_book.Where(i => i.IS_DELETED == false).ToList();
           
            return View();
        }
        [HttpPost]
        public ActionResult requestbook(request_book data)
        {
            if (data.ID > 0)
            {
                var regist = db.request_book.Where(i => i.ID == data.ID).SingleOrDefault();
                if (regist != null)
                {
                    if (data.BOOK_NAME != null)
                    {
                        regist.BOOK_NAME = data.BOOK_NAME;
                    }
                    if (data.AUTHER_NAME != null)
                    {
                        regist.AUTHER_NAME = data.AUTHER_NAME;
                    }
                    if (data.REMARK != null)
                    {
                        regist.REMARK = data.REMARK;
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                data.REQUEST_BY = Session["login"].ToString();
                data.IS_DELETED = false;
                var reg = db.request_book.Add(data);
                db.SaveChanges();
            }

            return RedirectToAction("requestbook");
        }

        public JsonResult getrequestbook(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var confi = db.request_book.Where(i => i.ID == id).FirstOrDefault();
              
                return Json(new { data = confi }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw e;
            }

        }



        public JsonResult delete(int id, string registration_del,string issuebook_del,string book_del,string department_del,string remark_del,string user_role_del)
        {
            try
            {
                if (registration_del != null)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var notification = db.registrations.Where(i => i.ID == id).SingleOrDefault();
                    notification.IS_DELETE = true;
                    db.SaveChanges();
                    return Json(new { data = notification }, JsonRequestBehavior.AllowGet);

                }
                if (issuebook_del != null)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var notification = db.issue_book.Where(i => i.ID == id).SingleOrDefault();
                    db.issue_book.Remove(notification);
                    db.SaveChanges();
                    return Json(new { data = notification }, JsonRequestBehavior.AllowGet);
                }
                if (book_del != null)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var notification = db.books.Where(i => i.ID == id).SingleOrDefault();
                    notification.IS_DELETE = true;
                    db.SaveChanges();
                    return Json(new { data = notification }, JsonRequestBehavior.AllowGet);
                }

                if(department_del !=null)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var notification = db.departments.Where(i => i.ID == id).SingleOrDefault();
                    notification.IS_DELETED = true;
                    db.SaveChanges();
                    return Json(new { data = notification }, JsonRequestBehavior.AllowGet);
                }
                if (remark_del != null)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var notification = db.request_book.Where(i => i.ID == id).SingleOrDefault();
                    notification.IS_DELETED = true;
                    db.SaveChanges();
                    return Json(new { data = notification }, JsonRequestBehavior.AllowGet);
                }
                if(user_role_del !=null)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var notification = db.user_role.Where(i => i.ID == id).SingleOrDefault();
                    notification.IS_DELETED = true;
                    db.SaveChanges();
                    return Json(new { data = notification }, JsonRequestBehavior.AllowGet);
                }

                return Json("");

            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}