using ONROtomotiv.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Web.UI;
using System.Net.Mail;
using System.Net;


namespace ONROtomotiv.Controllers
{
    public class HomeController : Controller
    {
        ONROtomotivEntities db = new ONROtomotivEntities();
        InputControl cs = new InputControl();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.SonEklenenAraclar = db.Vehicles.ToList().OrderByDescending(x => x.ListingDate).Take(9);
            return View();
        }

        public ActionResult Register()
        {
            if (Session["KullaniciID"] != null)
            {
                return Redirect("/Home/Index");
            }
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Email boş geçilemez";
            }
            if (Request.QueryString["islem"] == "hata2")
            {
                ViewBag.Hata2 = "Geçersiz email adresi";
            }
            if (Request.QueryString["islem"] == "hata3")
            {
                ViewBag.Hata3 = "Geçersiz ad";
            }
            if (Request.QueryString["islem"] == "hata4")
            {
                ViewBag.Hata4 = "Geçersiz soyadı";
            }
            if (Request.QueryString["islem"] == "hata5")
            {
                ViewBag.Hata5 = "Eksik telefon numarası";
            }
            if (Request.QueryString["islem"] == "hata6")
            {
                ViewBag.Hata6 = "Hatalı telefon numarası";
            }
            if (Request.QueryString["islem"] == "hata7")
            {
                ViewBag.Hata7 = "Şifre boş geçilemez";
            }

            return View();
        }
        [HttpPost]
        public ActionResult Register(string txtMail, string txtPassword, string txtFirstname, string txtLastname, string txtPhone)
        {
            if (db.Customers.Where(x => x.Mail == txtMail).Count() > 0)
            {
                Response.Write("<script>alert('Bu mail adresi sisteme kayıtlı.');</script>");
                return View();
            }
            if (txtMail == "" || txtMail == null)
            {
                return Redirect("/Home/Register?islem=hata1");
            }
            if (!cs.IsValidEmail(txtMail))
            {
                return Redirect("/Home/Register?islem=hata2");
            }
            if (!cs.IsName(txtFirstname))
            {
                return Redirect("/Home/Register?islem=hata3");
            }
            if (!cs.IsName(txtLastname))
            {
                return Redirect("/Home/Register?islem=hata4");
            }
            if (txtPhone.Length != 11)
            {
                return Redirect("/Home/Register?islem=hata5");
            }
            if (!cs.IsNumeric(txtPhone))
            {
                return Redirect("/Home/Register?islem=hata6");
            }
            if (txtPassword == "" || txtPassword == null)
            {
                return Redirect("/Home/Register?islem=hata7");
            }


            Customer c = new Customer();
            c.Mail = txtMail;
            c.Username = txtMail;
            c.Password = txtPassword;
            c.Firstname = txtFirstname;
            c.Lastname = txtLastname;
            c.Phone = txtPhone;
            c.RegisteredDate = DateTime.Now;
            db.Customers.Add(c);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                Response.Write("<script>alert('Oldu.');</script>");
                Response.Redirect("/Home/Index");
            }
            else
            {
                Response.Write("<script>alert('Bir hata oluştu.');</script>");
            }
            return View();
        }
        Customer girisYapanKullanici;
        public ActionResult Login()
        {
            if (Session["KullaniciID"] != null)
            {
                return Redirect("/Home/Index");
            }
            if (Request.QueryString["islem"] == "hata")
            {
                ViewBag.Hata = "Yetkisiz alan, lütfen giriş yapınız.";
            }
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Geçersiz mail adresi";
            }
            if (Request.QueryString["islem"] == "hata2")
            {
                ViewBag.Hata2 = "Mail boş geçilemez";
            }
            if (Request.QueryString["islem"] == "hata3")
            {
                ViewBag.Hata3 = "Şifre boş geçilemez";
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(string txtMail, string txtPassword)
        {
            if (!cs.IsValidEmail(txtMail))
            {
                return Redirect("/Home/Login?islem=hata1");
            }
            if (txtMail == "" || txtMail == null)
            {
                return Redirect("/Home/Login?islem=hata2");
            }
            if (txtPassword == "" || txtPassword == null)
            {
                return Redirect("/Home/Login?islem=hata3");
            }
            girisYapanKullanici = db.Customers.Where(x => x.Username == txtMail && x.Password == txtPassword).FirstOrDefault();
            if (girisYapanKullanici == null)
            {
                Response.Write("<script>alert('Kullanıcı adını ve şifreyi kontrol et.');</script>");
                return View();
            }
            else
            {
                int aracID = 0;
                if (Session["VListe"] != null)
                {
                    List<Vehicle> liste = (List<Vehicle>)Session["VListe"];
                    foreach (Vehicle item in liste)
                    {
                        aracID = item.ID;
                        var staleItem2 = Url.Action("Vehicles", "Home", new { id = aracID });
                        Response.RemoveOutputCacheItem(staleItem2);
                    }                    
                }
                Session["KullaniciID"] = girisYapanKullanici.ID;
                var staleItem3 = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem3);
                return Redirect("/Home/Index");
            }
        }
        public ActionResult Logout()
        {
            int aracID = 0;
            if (Session["KullaniciID"] == null)
            {
                return Redirect("/Home/Login?islem=hata");
            }
            if (Session["VListe"] != null)
            {
                List<Vehicle> liste = (List<Vehicle>)Session["VListe"];
                foreach (Vehicle item in liste)
                {
                    aracID = item.ID;
                    var staleItem2 = Url.Action("Vehicles", "Home", new { id = aracID });
                    Response.RemoveOutputCacheItem(staleItem2);
                }
            }
            Session.Clear();
            var staleItem3 = Url.Action("Listele", "Home");
            Response.RemoveOutputCacheItem(staleItem3);
            Response.Redirect("/Home/Index");
            return View();
        }
        public ActionResult UserDetails()
        {
            if (Session["KullaniciID"] == null)
            {
                return Redirect("/Home/Login?islem=hata");
            }
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Mail boş geçilemez";
            }
            if (Request.QueryString["islem"] == "hata2")
            {
                ViewBag.Hata2 = "Geçersiz mail adresi";
            }
            if (Request.QueryString["islem"] == "hata3")
            {
                ViewBag.Hata3 = "Geçersiz ad";
            }
            if (Request.QueryString["islem"] == "hata4")
            {
                ViewBag.Hata4 = "Geçersiz soyad";
            }
            if (Request.QueryString["islem"] == "hata5")
            {
                ViewBag.Hata5 = "Eksik telefon numarası";
            }
            if (Request.QueryString["islem"] == "hata6")
            {
                ViewBag.Hata6 = "Hatalı telefon numarası"; 
            }
            if (Request.QueryString["islem"] == "hata7")
            {
                ViewBag.Hata7 = "Şifre boş geçilemez";
            }
            Customer c = db.Customers.Find(Session["KullaniciID"]);
            ViewBag.Adresler = c.Addresses.Select(x => x.AddressName).ToList();
            return View(c);
        }
        [HttpPost]
        public ActionResult UserDetails(string txtMail, string txtPassword, string txtFirstname, string txtLastname, string txtPhone)
        {
            Customer c = db.Customers.Find(Session["KullaniciID"]);
            ViewBag.Adresler = c.Addresses.Select(x => x.AddressName).ToList();

            if (txtMail == "" || txtMail == null)
            {
                return Redirect("/Home/UserDetails?islem=hata1");
            }
            if (!cs.IsValidEmail(txtMail))
            {
                return Redirect("/HomeUserDetails?islem=hata2");
            }
            if (!cs.IsName(txtFirstname))
            {
                return Redirect("/Home/UserDetails?islem=hata3");
            }
            if (!cs.IsName(txtLastname))
            {
                return Redirect("/Home/UserDetails?islem=hata4");
            }
            if (txtPhone.Length != 11)
            {
                return Redirect("/Home/UserDetails?islem=hata5");
            }
            if (!cs.IsNumeric(txtPhone))
            {
                return Redirect("/Home/UserDetails?islem=hata6");
            }
            if (txtPassword == "" || txtPassword == null)
            {
                return Redirect("/Home/UserDetails?islem=hata7");
            }

            c.Mail = txtMail;
            c.Password = txtPassword;
            c.Firstname = txtFirstname;
            c.Lastname = txtLastname;
            c.Phone = txtPhone;

            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                Response.Redirect("/Home/UserDetails");
                return View(c);
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem...');</script>");
                return View(c);
            }
        }
        public ActionResult AdresEkle()
        {
            if (Session["KullaniciID"] == null)
            {
                return Redirect("/Home/Login?islem=hata");
            }
            Customer c = db.Customers.Find(Session["KullaniciID"]);
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Adres adı boş geçilemez";
            }
            if (Request.QueryString["islem"] == "hata2")
            {
                ViewBag.Hata2 = "Geçersiz ad";
            }
            if (Request.QueryString["islem"] == "hata3")
            {
                ViewBag.Hata3 = "Geçersiz soyad";
            }
            if (Request.QueryString["islem"] == "hata4")
            {
                ViewBag.Hata4 = "Hatalı il girdiniz";
            }
            if (Request.QueryString["islem"] == "hata5")
            {
                ViewBag.Hata5 = "Hatalı ilçe girdiniz";
            }
            if (Request.QueryString["islem"] == "hata6")
            {
                ViewBag.Hata6 = "Hatalı posta kodu";
            }
            if (Request.QueryString["islem"] == "hata7")
            {
                ViewBag.Hata7 = "Adres boş geçilemez";
            }
            return View();
        }
        [HttpPost]
        public ActionResult AdresEkle(string txtAddressName, string txtFirstname, string txtLastname, string txtCity, string txtTown, string txtPostalCode, string txtAddress)
        {
            Customer c = db.Customers.Find(Session["KullaniciID"]);


            if (txtAddressName == "" || txtAddressName == null)
            {
                return Redirect("/Home/AdresEkle?islem=hata1");
            }
            if (!cs.IsName(txtFirstname))
            {
                return Redirect("/Home/AdresEkle?islem=hata2");
            }
            if (!cs.IsName(txtLastname))
            {
                return Redirect("/Home/AdresEkle?islem=hata3");
            }
            if (!cs.IsName(txtCity))
            {
                return Redirect("/Home/AdresEkle?islem=hata4");
            }
            if (!cs.IsName(txtTown))
            {
                return Redirect("/Home/AdresEkle?islem=hata5");
            }
            if (!cs.IsNumeric(txtPostalCode))
            {
                return Redirect("/Home/AdresEkle?islem=hata6");
            }
            if (txtAddress == "" || txtAddress == null)
            {
                return Redirect("/Home/AdresEkle?islem=hata7");
            }


            Address a = new Address();
            a.CustomerID = c.ID;
            a.AddressName = txtAddressName;
            a.Firstname = txtFirstname;
            a.Lastname = txtLastname;
            a.City = txtCity;
            a.Town = txtTown;
            a.PostalCode = txtPostalCode;
            a.Address1 = txtAddress;
            db.Addresses.Add(a);

            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                Response.Redirect("/Home/UserDetails/");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem...');</script>");
            }
            return View();
        }
        List<Vehicle> vListeCache;
        [OutputCache(Duration = 600, VaryByHeader = "X-Requested-With", Location = OutputCacheLocation.Server, VaryByParam = "id")]
        public ActionResult Vehicles(int id)
        {
            Vehicle v = db.Vehicles.Find(id);
            if (Session["VListe"] == null)
            {
                vListeCache = new List<Vehicle>();
                vListeCache.Add(v);
                Session["VListe"] = vListeCache;
            }
            else
            {
                vListeCache = (List<Vehicle>)Session["VListe"];
                vListeCache.Add(v);
                Session["VListe"] = vListeCache;
            }                       
            ViewBag.Resimler = v.Photos.Select(x => x.PhotoPath).ToList();
            ViewBag.BenzerAraclar = db.Vehicles.Where(x => x.BrandID == v.BrandID).ToList();
            return View(v);
        }
        public ActionResult Listele()
        {
            ViewBag.Yillar = db.Vehicles.Select(x => x.Year).Distinct().ToList();
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.YakitTipleri = db.FuelTypes.Select(x => x.FuelTypeName).Distinct().ToList();
            ViewBag.VitesTipleri = db.TransmissionTypes.Select(x => x.TransmissionTypeName).Distinct().ToList();
            ViewBag.KasaTipleri = db.BodyTypes.Select(x => x.BodyTypeName).Distinct().ToList();
            ViewBag.CekisTipleri = db.WheelDriveTypes.Select(x => x.WheelDriveTypeName).Distinct().ToList();
            return View(db.Vehicles.ToList());
        }
        [HttpGet]
        [OutputCache(Duration=600,VaryByHeader="X-REquested-With",Location=OutputCacheLocation.Server)]
        public ActionResult Listele(int sayfa = 1, string yil = "", string kategori = "", string marka = "", string yakit = "", string vites = "", string kasa = "", string cekis = "")
        {
            ViewBag.Yil = yil;
            ViewBag.Kategori = kategori;
            ViewBag.Marka = marka;
            ViewBag.Yakit = yakit;
            ViewBag.Vites = vites;
            ViewBag.Kasa = kasa;
            ViewBag.Cekis = cekis;

            int yilint;
            if (yil == "")
            {
                yilint = 0;
            }
            else
            {
                yilint = Convert.ToInt32(yil);
            }
            ViewBag.Yillar = db.Vehicles.Select(x => x.Year).Distinct().ToList();
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.YakitTipleri = db.FuelTypes.Select(x => x.FuelTypeName).Distinct().ToList();
            ViewBag.VitesTipleri = db.TransmissionTypes.Select(x => x.TransmissionTypeName).Distinct().ToList();
            ViewBag.KasaTipleri = db.BodyTypes.Select(x => x.BodyTypeName).Distinct().ToList();
            ViewBag.CekisTipleri = db.WheelDriveTypes.Select(x => x.WheelDriveTypeName).Distinct().ToList();

            List<Vehicle> filtrelenmisAraclar = new List<Vehicle>();
            if (yil == null && kategori == null && marka == null && yakit == null && vites == null && kasa == null && cekis == null)
            {
                filtrelenmisAraclar = db.Vehicles.ToList();
            }
            else
            {
                //filtrelenmisAraclar = db.Vehicles.Where(x => x.Year == yilint && x.Category.CategoryName == kategori && x.Brand.BrandName == marka && x.FuelType.FuelTypeName == yakit && x.TransmissionType.TransmissionTypeName == vites && x.BodyType.BodyTypeName == kasa && x.WheelDriveType.WheelDriveTypeName == cekis).ToList();
                filtrelenmisAraclar = db.Vehicles.Where(x => x.Year.ToString().Contains(yil) && x.Category.CategoryName.Contains(kategori) && x.Brand.BrandName.Contains(marka) && x.FuelType.FuelTypeName.Contains(yakit) && x.TransmissionType.TransmissionTypeName.StartsWith(vites) && x.BodyType.BodyTypeName.Contains(kasa) && x.WheelDriveType.WheelDriveTypeName.Contains(cekis)).ToList();
            }
           
            return View(filtrelenmisAraclar.ToPagedList(sayfa, 6));
        }
        public ActionResult TeklifVer(int id)
        {
            if (Session["KullaniciID"] == null)
            {
                return Redirect("/Home/Login?islem=hata");
            }
            return View();
        }
        [HttpPost]
        public ActionResult TeklifVer(int id, string ddlTaksit)
        {
            Vehicle v = db.Vehicles.Find(id);
            int userID = (int)Session["KullaniciID"];
            Customer c = db.Customers.Find(userID);
            SoldOrder so = new SoldOrder();
            so.CustomerID = c.ID;
            so.VehicleID = v.ID;
            so.SoldPrice = v.SaleInfos.Where(x => x.VehicleID == v.ID).FirstOrDefault().ListingPrice;
            so.SoldDate = DateTime.Now;
            if (ddlTaksit == "1")
            {
                so.MonthlyPaypments = so.SoldPrice;
                so.MonthlyPaypmentDate = DateTime.Now;
            }
            else
            {
                decimal taksit = Convert.ToDecimal(ddlTaksit);
                decimal hesap = so.SoldPrice / taksit;
                so.MonthlyPaypments = hesap;
                so.MonthlyPaypmentDate = DateTime.Now.AddMonths(Convert.ToInt32(taksit));
            }           
            db.SoldOrders.Add(so);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                return Redirect("/Home/Tekliflerim");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult Tekliflerim()
        {
            if (Session["KullaniciID"] == null)
            {
                return Redirect("/Home/Login?islem=hata");
            }
            int userID = (int)Session["KullaniciID"];
            List<SoldOrder> soListe = db.SoldOrders.Where(x => x.CustomerID == userID).ToList();
            return View(soListe);
        }
        public ActionResult TeklifSil(int id)
        {
            if (Session["KullaniciID"] == null)
            {
                return Redirect("/Home/Login?islem=hata");
            }
            SoldOrder so = db.SoldOrders.Find(id);
            db.SoldOrders.Remove(so);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                return Redirect("/Home/Tekliflerim");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult Karsilastirma()
        {
            //if (Session["KullaniciID"] == null)   eski yapı
            //{
            //    return Redirect("/Home/Login?islem=hata");
            //}
            //ViewBag.Araclar = db.Vehicles.ToList();
            //int userID = (int)Session["KullaniciID"];
            //List<Vehicle> vListe = new List<Vehicle>();
            //if (db.ComparedVehicles.Where(x => x.CustomerID == userID).ToList().Count == 2)
            //{
            //    int sayi1 = db.ComparedVehicles.Where(x => x.CustomerID == userID).Select(x => x.VehicleID).FirstOrDefault();
            //    int sayi2 = db.ComparedVehicles.Where(x => x.CustomerID == userID).Select(x => x.VehicleID).ToArray().Last();
            //    Vehicle v1 = db.Vehicles.Find(sayi1);
            //    Vehicle v2 = db.Vehicles.Find(sayi2);
            //    vListe.Add(v1);
            //    vListe.Add(v2);
            //}
            //if (db.ComparedVehicles.Where(x => x.CustomerID == userID).ToList().Count == 1)
            //{
            //    int sayi = db.ComparedVehicles.Where(x => x.CustomerID == userID).Select(x => x.VehicleID).FirstOrDefault();
            //    Vehicle v = db.Vehicles.Find(sayi);
            //    vListe.Add(v);
            //}
            //return View(vListe);
            ViewBag.Araclar = db.Vehicles.ToList();
            return View();
        }
        public ActionResult KarsilastirmadanSil(int id)
        {
            //int userID = (int)Session["KullaniciID"]; eski yapı
            //ComparedVehicle cv = db.ComparedVehicles.Where(x => x.VehicleID == id && x.CustomerID == userID).FirstOrDefault();
            //db.ComparedVehicles.Remove(cv);
            //int sayi = db.SaveChanges();
            //if (sayi > 0)
            //{
            //    return Redirect("/Home/Karsilastirma");
            //}
            //else
            //{
            //    Response.Redirect("<script>alert('Hatalı işlem');</script>");
            //}
            //return View();
            vListeSession = (List<Vehicle>)Session["AracListe"];
            //Vehicle v = db.Vehicles.Find(id);
            vListeSession.RemoveAll(x => x.ID == id);
            Session["AracListe"] = vListeSession;
            return Redirect("/Home/Karsilastirma");
        }
        List<Vehicle> vListeSession;
        [HttpPost]
        public ActionResult KarsilastirmaEkle(string ddlArac)
        {
            if (Session["AracListe"] == null)
            {
                vListeSession = new List<Vehicle>();
                Vehicle v = db.Vehicles.Where(x => x.ProductName == ddlArac).FirstOrDefault();
                vListeSession.Add(v);
                Session["AracListe"] = vListeSession;
                return Redirect("/Home/Karsilastirma");
            }
            else
            {
                vListeSession = (List<Vehicle>)Session["AracListe"];
                Vehicle v = db.Vehicles.Where(x => x.ProductName == ddlArac).FirstOrDefault();
                vListeSession.Add(v);
                Session["AracListe"] = vListeSession;

                return Redirect("/Home/Karsilastirma");
            }
            //ComparedVehicle cv = new ComparedVehicle();  eski yapı
            //cv.VehicleID = v.ID;
            //cv.CustomerID = (int)Session["KullaniciID"];
            //db.ComparedVehicles.Add(cv);
            //int sayi = db.SaveChanges();
            //if (sayi > 0)
            //{
            //    return Redirect("/Home/Karsilastirma");
            //}
            //else
            //{
            //    Response.Write("<script>alert('Hatalı işlem');</script>");
            //}            
        }
        public ActionResult KarsilastirmaEkle(int id)
        {
            if (Session["AracListe"] == null)
            {
                vListeSession = new List<Vehicle>();
                Vehicle v = db.Vehicles.Where(x => x.ID == id).FirstOrDefault();
                vListeSession.Add(v);
                Session["AracListe"] = vListeSession;
                return Redirect("/Home/Karsilastirma");
            }
            else
            {
                vListeSession = (List<Vehicle>)Session["AracListe"];
                if (vListeSession.Count == 2)
                {
                    vListeSession.Clear();
                }
                Vehicle v = db.Vehicles.Where(x => x.ID == id).FirstOrDefault();
                vListeSession.Add(v);
                Session["AracListe"] = vListeSession;

                return Redirect("/Home/Karsilastirma");
            }      
        }
        public ActionResult Contact()
        {
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Ad boş geçilemez";
            }
            if (Request.QueryString["islem"] == "hata2")
            {
                ViewBag.Hata2 = "Geçersiz mail adresi";
            }
            if (Request.QueryString["islem"] == "hata3")
            {
                ViewBag.Hata3 = "Mesaj boş geçilemez";
            }
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string txtAd, string txtEmail, string txtAciklama)
        {
            if (txtAd == "" || txtAd == null)
            {
                return Redirect("/Home/Contact?islem=hata1");
            }
            if (!cs.IsValidEmail(txtEmail))
            {
                return Redirect("/Home/Contact?islem=hata2");
            }
            if (txtAciklama == "" || txtAciklama == null)
            {
                return Redirect("/Home/Contact?islem=hata3");
            }
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("alpererez@yedioutlet.com");
                mail.To.Add("alpererez@hotmail.com");
                mail.IsBodyHtml = true;
                mail.Subject = "İletişim Formu";
                mail.Body = string.Format("Gönderen: {0} <br> Adı: {1} <br> <br> İçerik: <br> {2}", txtEmail, txtAd, txtAciklama);

                SmtpClient sunucu = new SmtpClient();
                sunucu.Host = "smtp.isimtescil.net";
                sunucu.EnableSsl = false;
                sunucu.Port = 587;
                sunucu.Credentials = new NetworkCredential("alpererez@yedioutlet.com", "Sd9e9wsS");
                sunucu.Send(mail);

                Response.Write("<script>alert('Gönderildi');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
    }
}