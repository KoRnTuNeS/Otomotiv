using ONROtomotiv.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace ONROtomotiv.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        ONROtomotivEntities db = new ONROtomotivEntities();
        InputControl cs = new InputControl();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            if (Request.QueryString["islem"] == "hata")
            {
                ViewBag.Hata = "Yetkisiz alan, lütfen giriş yapınız.";
            }
            if (Session["AdminID"] != null)
            {
                return Redirect("/Admin/Index");
            }
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Kullanıcı adı boş geçilemez";
            }
            if (Request.QueryString["islem"] == "hata2")
            {
                ViewBag.Hata2 = "Şifre boş geçilemez";
            }
            return View();
        }
        
        Admin girisYapanAdmin;
        [HttpPost]
        public ActionResult Login(string txtKullaniciAdi, string txtSifre)
        {
            if (txtKullaniciAdi == "" || txtKullaniciAdi == null)
            {
                return Redirect("/Admin/Login?islem=hata1");
            }
            if (txtSifre == "" || txtSifre == null)
            {
                return Redirect("/Admin/Login?islem=hata2");
            }
            girisYapanAdmin = db.Admins.Where(x => x.Username == txtKullaniciAdi && x.Password == txtSifre).FirstOrDefault();
            if (girisYapanAdmin == null)
            {
                Response.Write("<script>alert('Hatalı giriş');</script>");
                return View();
            }
            else
            {
                Session["AdminID"] = girisYapanAdmin.ID;
                return Redirect("/Admin/Index");
            }
        }
        public ActionResult Logout()
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Session.Clear();
            Response.Redirect("/Admin/Index");
            return View();
        }
        public ActionResult KategoriEkle()
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Kategori adı boş geçilemez";
            }
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(string txtKategoriAdi)
        {
            if (txtKategoriAdi == "" || txtKategoriAdi == null)
            {
                return Redirect("/Admin/KategoriEkle?islem=hata1");
            }
            Category c = new Category();
            c.CategoryName = txtKategoriAdi;
            db.Categories.Add(c);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Write("<script>alert('Eklendi');</script>");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</scriipt>");
            }
            return View();
        }
        public ActionResult KategoriListele(int sayfa = 1)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            List<Category> kListe = db.Categories.ToList();
            return View(kListe.ToPagedList(sayfa, 3));
        }
        public ActionResult KategoriSil(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Category c = db.Categories.Find(id);
            if (c == null)
            {
                return Redirect("/Admin/Index");
            }
            db.Categories.Remove(c);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/KategoriListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult KategoriDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Category c = db.Categories.Find(id);
            if (c == null)
            {
                return Redirect("/Admin/Index");
            }
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Kategori adı boş geçilemez";
            }
            return View(c);
        }
        [HttpPost]
        public ActionResult KategoriDuzenle(int id, string txtKategoriAdi)
        {
            Category c = db.Categories.Find(id);

            if (txtKategoriAdi == "" || txtKategoriAdi == null)
            {
                return Redirect("/Admin/KategoriDuzenle/"+ id +"?islem=hata1");
            }

            c.CategoryName = txtKategoriAdi;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/KategoriListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View(c);
        }
        public ActionResult MarkaEkle()
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Marka adı boş geçilemez";
            }
            return View();
        }
        [HttpPost]
        public ActionResult MarkaEkle(string ddlKategori, string txtMarkaAdi)
        {
            ViewBag.Kategoriler = db.Categories.ToList();
            Category c = db.Categories.Where(x => x.CategoryName == ddlKategori).FirstOrDefault();

            if (txtMarkaAdi == "" || txtMarkaAdi == null)
            {
                return Redirect("/Admin/MarkaEkle?islem=hata1");
            }

            Brand b = new Brand();
            b.CategoryID = c.ID;
            b.BrandName = txtMarkaAdi;
            db.Brands.Add(b);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Write("<script>alert('Eklendi');</script>");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult MarkaListele(int sayfa = 1)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            List<Brand> bListe = db.Brands.ToList();
            return View(bListe.ToPagedList(sayfa, 3));
        }
        public ActionResult MarkaSil(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Brand b = db.Brands.Find(id);
            if (b == null)
            {
                return Redirect("/Admin/Index");
            }
            db.Brands.Remove(b);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/MarkaListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult MarkaDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Brand b = db.Brands.Find(id);
            if (b == null)
            {
                return Redirect("/Admin/Index");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Marka adı boş geçilemez";
            }
            return View(b);
        }
        [HttpPost]
        public ActionResult MarkaDuzenle(int id, Brand postB, string txtMarkaAdi)
        {
            Brand b = db.Brands.Find(id);
            ViewBag.Kategoriler = db.Categories.ToList();

            if (txtMarkaAdi == "" || txtMarkaAdi == null)
            {
                return Redirect("/Admin/MarkaDuzenle/" + id + "?islem=hata1");
            }

            b.CategoryID = postB.CategoryID;
            b.BrandName = txtMarkaAdi;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                List<Vehicle> vListe = db.Vehicles.Where(x => x.BrandID == b.ID).ToList();
                foreach (Vehicle item in vListe)
                {
                    var staleItem = Url.Action("Vehicles", "Home", new { id = item.ID });
                    Response.RemoveOutputCacheItem(staleItem);
                }
                var staleItem2 = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem2);
                Response.Redirect("/Admin/MarkaListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script");
            }
            return View(b);
        }
        public ActionResult SeriEkle()
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Seri adı boş geçilemez";
            }
            return View();
        }
        [HttpPost]
        public ActionResult SeriEkle(string ddlKategori, string ddlMarka, string txtSeriAdi)
        {
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            Category c = db.Categories.Where(x => x.CategoryName == ddlKategori).FirstOrDefault();
            Brand b = db.Brands.Where(x => x.BrandName == ddlMarka).FirstOrDefault();
            
            if (txtSeriAdi == "" || txtSeriAdi == null)
            {
                return Redirect("Admin/SeriEkle?islem=hata1");
            }

            Series s = new Series();
            s.CategoryID = c.ID;
            s.BrandID = b.ID;
            s.SerieName = txtSeriAdi;
            db.Series.Add(s);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Write("<script>alert('Eklendi');</script>");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem);</script>");
            }
            return View();
        }
        public ActionResult SeriListele(int sayfa = 1)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            List<Series> sListe = db.Series.ToList();
            return View(sListe.ToPagedList(sayfa, 3));
        }
        public ActionResult SeriSil(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Series s = db.Series.Find(id);
            if (s == null)
            {
                return Redirect("/Admin/Index");
            }
            db.Series.Remove(s);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/SeriListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult SeriDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Series s = db.Series.Find(id);
            if (s == null)
            {
                return Redirect("/Admin/Index");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Seri adı boş geçilemez";
            }
            return View(s);
        }
        [HttpPost]
        public ActionResult SeriDuzenle(int id, Series postS, string txtSeriAdi)
        {
            Series s = db.Series.Find(id);
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();

            if (txtSeriAdi == "" || txtSeriAdi == null)
            {
                return Redirect("/Admin/SeriDuzenle/" + id + "?islem=hata1");
            }

            s.CategoryID = postS.CategoryID;
            s.BrandID = postS.BrandID;
            s.SerieName = txtSeriAdi;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/SeriListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View(s);
        }
        public ActionResult ModelEkle()
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Model adı boş geçilemez";
            }
            return View();
        }
        [HttpPost]
        public ActionResult ModelEkle(string ddlKategori, string ddlMarka, string ddlSeri, string txtModelAdi)
        {
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            Category c = db.Categories.Where(x => x.CategoryName == ddlKategori).FirstOrDefault();
            Brand b = db.Brands.Where(x => x.BrandName == ddlMarka).FirstOrDefault();
            Series s = db.Series.Where(x => x.SerieName == ddlSeri).FirstOrDefault();

            if (txtModelAdi == "" || txtModelAdi == null)
            {
                return Redirect("/Admin/ModelEkle?islem=hata1");
            }

            Model m = new Model();
            m.CategoryID = c.ID;
            m.BrandID = b.ID;
            m.SerieID = s.ID;
            m.ModelName = txtModelAdi;
            db.Models.Add(m);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Write("<script>alert('Eklendi');</script>");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem);</script>");
            }
            return View();
        }
        public ActionResult ModelListele(int sayfa = 1)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            List<Model> mListe = db.Models.ToList(); 
            return View(mListe.ToPagedList(sayfa, 3));
        }
        public ActionResult ModelSil(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Model m = db.Models.Find(id);
            if (m == null)
            {
                return Redirect("/Admin/Index");
            }
            db.Models.Remove(m);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/ModelListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult ModelDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Model m = db.Models.Find(id);
            if (m == null)
            {
                return Redirect("/Admin/Index");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Marka adı boş geçilemez";
            }
            return View(m);
        }
        [HttpPost]
        public ActionResult ModelDuzenle(int id, Model postM, string txtModelAdi)
        {
            Model m = db.Models.Find(id);
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();

            if (txtModelAdi == "" || txtModelAdi == null)
            {
                return Redirect("/Admin/ModelDuzenle/" + id + "?islem=hata1");
            }

            m.CategoryID = postM.CategoryID;
            m.BrandID = postM.BrandID;
            m.SerieID = postM.SerieID;
            m.ModelName = txtModelAdi;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                List<Vehicle> vListe = db.Vehicles.Where(x => x.ModelID == m.ID).ToList();
                foreach (Vehicle item in vListe)
                {
                    var staleItem = Url.Action("Vehicles", "Home", new { id = item.ID });
                    Response.RemoveOutputCacheItem(staleItem);
                }
                var staleItem2 = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem2);
                Response.Redirect("/Admin/ModelListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View(m);
        }
        public ActionResult AltModelEkle()
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            ViewBag.Modeller = db.Models.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Alt model adı boş geçilemez";
            }
            return View();
        }
        [HttpPost]
        public ActionResult AltModelEkle(string ddlKategori, string ddlMarka, string ddlSeri, string ddlModel, string txtAltModelAdi)
        {
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            ViewBag.Modeller = db.Models.ToList();
            Category c = db.Categories.Where(x => x.CategoryName == ddlKategori).FirstOrDefault();
            Brand b = db.Brands.Where(x => x.BrandName == ddlMarka).FirstOrDefault();
            Series s = db.Series.Where(x => x.SerieName == ddlSeri).FirstOrDefault();
            Model m = db.Models.Where(x => x.ModelName == ddlModel).FirstOrDefault();

            if (txtAltModelAdi == "" || txtAltModelAdi == null)
            {
                return Redirect("/Admin/AltModelEkle?islem=hata1");
            }

            Sub_Model sm = new Sub_Model();
            sm.CategoryID = c.ID;
            sm.BrandID = b.ID;
            sm.SerieID = s.ID;
            sm.ModelID = m.ID;
            sm.Sub_ModelName = txtAltModelAdi;
            db.Sub_Model.Add(sm);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Write("<script>alert('Eklendi');</script>");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult AltModelListele(int sayfa = 1)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            List<Sub_Model> smListe = db.Sub_Model.ToList();
            return View(smListe.ToPagedList(sayfa, 3));
        }
        public ActionResult AltModelSil(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Sub_Model sm = db.Sub_Model.Find(id);
            if (sm == null)
            {
                return Redirect("/Admin/Index");
            }
            db.Sub_Model.Remove(sm);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/AltModelListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult AltModelDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Sub_Model sm = db.Sub_Model.Find(id);
            if (sm == null)
            {
                return Redirect("/Admin/Index");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            ViewBag.Modeller = db.Models.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Alt model adı boş geçilemez";
            }
            return View(sm);
        }
        [HttpPost]
        public ActionResult AltModelDuzenle(int id, Sub_Model PostSm, string txtAltModelAdi)
        {
            Sub_Model sm = db.Sub_Model.Find(id);
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            ViewBag.Modeller = db.Models.ToList();

            if (txtAltModelAdi == "" || txtAltModelAdi == null)
            {
                return Redirect("/Admin/AltModelDuzenle/" + id + "?islem=hata1");
            }

            sm.CategoryID = PostSm.CategoryID;
            sm.BrandID = PostSm.BrandID;
            sm.SerieID = PostSm.SerieID;
            sm.ModelID = PostSm.ModelID;
            sm.Sub_ModelName = txtAltModelAdi;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                List<Vehicle> vListe = db.Vehicles.Where(x => x.Sub_ModelID == sm.ID).ToList();
                foreach (Vehicle item in vListe)
                {
                    var staleItem = Url.Action("Vehicles", "Home", new { id = item.ID });
                    Response.RemoveOutputCacheItem(staleItem);
                }
                var staleItem2 = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem2);
                Response.Redirect("/Admin/AltModelListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View(sm);
        }
        public ActionResult AracEkle()
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            ViewBag.Modeller = db.Models.ToList();
            ViewBag.AltModeller = db.Sub_Model.ToList();
            ViewBag.YakitTipleri = db.FuelTypes.ToList();
            ViewBag.VitesTipleri = db.TransmissionTypes.ToList();
            ViewBag.KasaTipleri = db.BodyTypes.ToList();
            ViewBag.CekisTipleri = db.WheelDriveTypes.ToList();
            ViewBag.Renkler = db.Colors.ToList();
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Başlık boş geçilemez";
            }
            if (Request.QueryString["islem"] == "hata2")
            {
                ViewBag.Hata2 = "Geçersiz yıl";
            }
            if (Request.QueryString["islem"] == "hata3")
            {
                ViewBag.Hata3 = "Geçersiz KM";
            }
            if (Request.QueryString["islem"] == "hata4")
            {
                ViewBag.Hata4 = "Geçersiz motor kapasitesi";
            }
            if (Request.QueryString["islem"] == "hata5")
            {
                ViewBag.Hata5 = "Geçersiz motor gücü";
            }
            if (Request.QueryString["islem"] == "hata6")
            {
                ViewBag.Hata6 = "Geçersiz bölge";
            }
            if (Request.QueryString["islem"] == "hata7")
            {
                ViewBag.Hata7 = "Açıklamayı boş geçemezsiniz";
            }
            if (Request.QueryString["islem"] == "hata8")
            {
                ViewBag.Hata8 = "Fotoğraf ekleyiniz";
            }
            return View();
        }
        [HttpPost]
        public ActionResult AracEkle(string ddlKategori, string ddlMarka, string ddlSeri, string ddlModel, string ddlAltModel, string ddlYakitTipi, string ddlVitesTipi, string ddlKasaTipi, string ddlCekisTipi, string ddlRenk, string txtUrunAdi, int? txtYil, int? txtKM, int? txtMotorKapasitesi, int? txtMotorGucu, string ddlGaranti, string txtBolge, string ddlTakas, string ddlDurum, string txtAciklama, HttpPostedFileBase fuPhotoPath)
        {
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            ViewBag.Modeller = db.Models.ToList();
            ViewBag.AltModeller = db.Sub_Model.ToList();
            ViewBag.YakitTipleri = db.FuelTypes.ToList();
            ViewBag.VitesTipleri = db.TransmissionTypes.ToList();
            ViewBag.KasaTipleri = db.BodyTypes.ToList();
            ViewBag.CekisTipleri = db.WheelDriveTypes.ToList();
            ViewBag.Renkler = db.Colors.ToList();



            if (txtUrunAdi == "" || txtUrunAdi == null)
            {
                return Redirect("/Admin/AracEkle?islem=hata1");
            }
            if (!cs.IsNumeric(Convert.ToString(txtYil)))
            {
                return Redirect("/Admin/AracEkle?islem=hata2");
            }
            if (!cs.IsNumeric(Convert.ToString(txtKM)))
            {
                return Redirect("/Admin/AracEkle?islem=hata3");
            }
            if (!cs.IsNumeric(Convert.ToString(txtMotorKapasitesi)))
            {
                return Redirect("/Admin/AracEkle?islem=hata4");
            }
            if (!cs.IsNumeric(Convert.ToString(txtMotorGucu)))
            {
                return Redirect("/Admin/AracEkle?islem=hata5");
            }
            if (!cs.IsName(txtBolge))
            {
                return Redirect("/Admin/AracEkle?islem=hata6");
            }
            if (txtAciklama == "" || txtAciklama == null)
            {
                return Redirect("/Admin/AracEkle?islem=hata7");
            }
            if (fuPhotoPath == null)
            {
                return Redirect("/Admin/AracEkle?islem=hata8");
            }



            Random rnd = new Random();
            string benzersizIsim = rnd.Next(10, 55555555) + DateTime.Now.Day + "_" + DateTime.Now.Month + fuPhotoPath.FileName;

            if (fuPhotoPath != null && fuPhotoPath.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/upload"), benzersizIsim);
                fuPhotoPath.SaveAs(path);
            }


            Category c = db.Categories.Where(x => x.CategoryName == ddlKategori).FirstOrDefault();
            Brand b = db.Brands.Where(x => x.BrandName == ddlMarka).FirstOrDefault();
            Series s = db.Series.Where(x => x.SerieName == ddlSeri).FirstOrDefault();
            Model m = db.Models.Where(x => x.ModelName == ddlModel).FirstOrDefault();
            Sub_Model sm = db.Sub_Model.Where(x => x.Sub_ModelName == ddlAltModel).FirstOrDefault();
            FuelType ft = db.FuelTypes.Where(x => x.FuelTypeName == ddlYakitTipi).FirstOrDefault();
            TransmissionType tt = db.TransmissionTypes.Where(x => x.TransmissionTypeName == ddlVitesTipi).FirstOrDefault();
            BodyType bt = db.BodyTypes.Where(x => x.BodyTypeName == ddlKasaTipi).FirstOrDefault();
            WheelDriveType wdt = db.WheelDriveTypes.Where(x => x.WheelDriveTypeName == ddlCekisTipi).FirstOrDefault();
            Color col = db.Colors.Where(x => x.ColorName == ddlRenk).FirstOrDefault();


            Vehicle v = new Vehicle();
            v.CategoryID = c.ID;
            v.BrandID = b.ID;
            v.SerieID = s.ID;
            v.ModelID = m.ID;
            v.Sub_ModelID = sm.ID;
            v.FuelTypeID = ft.ID;
            v.TransmissionTypeID = tt.ID;
            v.BodyTypeID = bt.ID;
            v.WheelDriveTypeID = wdt.ID;
            v.ColorID = col.ID;

            v.ProductName = txtUrunAdi;
            v.Year = (int)txtYil;
            v.ListingDate = DateTime.Now;
            v.KM = (int)txtKM;
            v.EngineCapacity = (int)txtMotorKapasitesi;
            v.EngineHP = (int)txtMotorGucu;
            if (ddlGaranti == "Var")
            {
                v.Warranty = true;
            }
            else
            {
                v.Warranty = false;
            }
            v.Region = txtBolge;
            if (ddlTakas == "Var")
            {
                v.Trade = true;
            }
            else
            {
                v.Trade = false;
            }
            if (ddlDurum == "Sıfır")
            {
                v.Condition = false;
            }
            else
            {
                v.Condition = true;
            }
            v.Description = txtAciklama;
            v.MasterPhotoPath = "/upload/" + benzersizIsim;

            db.Vehicles.Add(v);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                foreach (Vehicle item in db.Vehicles.ToList())
                {
                    var staleItem2 = Url.Action("Vehicles", "Home", new { id = item.ID });
                    Response.RemoveOutputCacheItem(staleItem2);
                }
                return Redirect("/Admin/AracListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }

        public ActionResult AracListele(int sayfa = 1)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            List<Vehicle> vListe = db.Vehicles.ToList();
            return View(vListe.ToPagedList(sayfa, 3));
        }
        [HttpGet]
        public ActionResult AracListele(int sayfa = 1, string txtID = "", string txtBaslik = "")
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            List<Vehicle> vListe = db.Vehicles.Where(x => x.ID.ToString().Contains(txtID) && x.ProductName.Contains(txtBaslik)).ToList();
            return View(vListe.ToPagedList(sayfa, 3));
        }
        public ActionResult FiyatBilgisiDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Vehicle v = db.Vehicles.Find(id);
            if (v == null)
            {
                return Redirect("/Admin/Index");
            }


            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Geçersiz fiyat";
            }


            SaleInfo si;
            if (db.SaleInfos.Where(x => x.VehicleID == v.ID).FirstOrDefault() == null)
            {
                si = new SaleInfo();
                si.VehicleID = v.ID;
                db.SaleInfos.Add(si);
                db.SaveChanges();
            }
            else
            {
                si = db.SaleInfos.Where(x => x.VehicleID == v.ID).FirstOrDefault();
            }
            return View(si);
        }
        [HttpPost]
        public ActionResult FiyatBilgisiDuzenle(int id, decimal? txtListeFiyati, decimal txtIndirimliFiyat)
        {
            if (txtIndirimliFiyat == 0)
            {
                txtIndirimliFiyat = (decimal)txtListeFiyati;
            }
            Vehicle v = db.Vehicles.Find(id);
            SaleInfo si = db.SaleInfos.Where(x => x.VehicleID == v.ID).FirstOrDefault();
            if (!cs.IsDecimal(Convert.ToString(txtListeFiyati)))
            {
                return Redirect("/Admin/FiyatBilgisiDuzenle/" + id + "?islem=hata1");
            }
            si.ListingPrice = (decimal)txtListeFiyati;
            si.DiscountPrice = txtIndirimliFiyat;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Vehicles", "Home", new { id = v.ID });
                Response.RemoveOutputCacheItem(staleItem);
                var staleItem2 = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem2);
                return Redirect("/Admin/AracListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
                return View(si);
            }
            
        }
        public ActionResult ResimGalerisi(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Vehicle v = db.Vehicles.Find(id);
            if (v == null)
            {
                return Redirect("/Admin/Index");
            }
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Fotoğraf ekleyiniz";
            }
            List<Photo> resimler = db.Photos.Where(x => x.VehicleID == id).ToList();
            return View(resimler);
        }
        [HttpPost]
        public ActionResult ResimGalerisi(int id, HttpPostedFileBase fuResim)
        {
            List<Photo> resimler = db.Photos.Where(x => x.VehicleID == id).ToList();


            if (fuResim == null)
            {
                return Redirect("/Admin/ResimGalerisi/" + id + "?islem=hata1");
            }


            Random rnd = new Random();
            string benzersizIsim = rnd.Next(10, 55555555) + DateTime.Now.Day + "_" + DateTime.Now.Month + fuResim.FileName;

            if (fuResim != null && fuResim.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/upload/" + id), benzersizIsim);
                if (!Directory.Exists(Server.MapPath("~/upload/" + id)))
                {
                    DirectoryInfo di = Directory.CreateDirectory(Server.MapPath("~/upload/" + id));
                }
                fuResim.SaveAs(path);
            }


            Photo p = new Photo();
            p.VehicleID = id;
            p.PhotoPath = "/upload/" + id + "/" + benzersizIsim;
            db.Photos.Add(p);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Vehicles", "Home", new { id = p.VehicleID });
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/ResimGalerisi/" + id);
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View(resimler);
        }
        public ActionResult ResmiDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Photo p = db.Photos.Find(id);
            if (p == null)
            {
                return Redirect("/Admin/Index");
            }
            return View(p);
        }
        public ActionResult ResimSil(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Photo p = db.Photos.Find(id);
            if (p == null)
            {
                return Redirect("/Admin/Index");
            }
            db.Photos.Remove(p);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Vehicles", "Home", new { id = p.VehicleID });
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/ResimGalerisi/" + p.VehicleID);
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult GuvenlikDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Vehicle v = db.Vehicles.Find(id);
            if (v == null)
            {
                return Redirect("/Admin/Index");
            }
            SecuritySpec ss;
            if (db.SecuritySpecs.Where(x => x.VehicleID == id).FirstOrDefault() == null)
            {
                ss = new SecuritySpec();
                ss.VehicleID = id;
                db.SecuritySpecs.Add(ss);
                db.SaveChanges();
            }
            else
            {
                ss = db.SecuritySpecs.Where(x => x.VehicleID == id).FirstOrDefault();
            }
            return View(ss);
        }
        [HttpPost]
        public ActionResult GuvenlikDuzenle(int id, SecuritySpec postSs)
        {
            SecuritySpec ss = db.SecuritySpecs.Where(x => x.VehicleID == id).FirstOrDefault();
            ss.ABC = postSs.ABC;
            ss.ABS = postSs.ABS;
            ss.Airbag_Diz = postSs.Airbag_Diz;
            ss.Airbag_Perde = postSs.Airbag_Perde;
            ss.Airbag_Surucu = postSs.Airbag_Surucu;
            ss.Airbag_Tavan = postSs.Airbag_Tavan;
            ss.Airbag_Yan = postSs.Airbag_Yan;
            ss.Airbag_Yolcu = postSs.Airbag_Yolcu;
            ss.Airmatic = postSs.Airmatic;
            ss.Alarm = postSs.Alarm;
            ss.ASR = postSs.ASR;
            ss.BAS = postSs.BAS;
            ss.Distronic = postSs.Distronic;
            ss.EBA = postSs.EBA;
            ss.EBD = postSs.EBD;
            ss.EDL = postSs.EDL;
            ss.ESP = postSs.ESP;
            ss.GeceGorus = postSs.GeceGorus;
            ss.Immobilizer = postSs.Immobilizer;
            ss.Isofix = postSs.Isofix;
            ss.KorNoktaUyariSistemi = postSs.KorNoktaUyariSistemi;
            ss.LastikArizaGostergesi = postSs.LastikArizaGostergesi;
            ss.MerkeziKilit = postSs.MerkeziKilit;
            ss.SeritDegistirmeYardimcisi = postSs.SeritDegistirmeYardimcisi;
            ss.SerittenAyrilmaIkazi = postSs.SerittenAyrilmaIkazi;
            ss.TSC = postSs.TSC;
            ss.YokusKalkisDestegi = postSs.YokusKalkisDestegi;
            ss.YorgunlukTespitSistemi = postSs.YorgunlukTespitSistemi;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Vehicles", "Home", new { id = ss.VehicleID });
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/GuvenlikDuzenle/" + id);
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View(ss);
        }
        public ActionResult IcDonanimDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Vehicle v = db.Vehicles.Find(id);
            if (v == null)
            {
                return Redirect("/Admin/Index");
            }
            InnerEquipment ie;
            if (db.InnerEquipments.Where(x => x.VehicleID == id).FirstOrDefault() == null)
            {
                ie = new InnerEquipment();
                ie.VehicleID = id;
                db.InnerEquipments.Add(ie);
                db.SaveChanges();
            }
            else
            {
                ie = db.InnerEquipments.Where(x => x.VehicleID == id).FirstOrDefault();
            }
            return View(ie);
        }
        [HttpPost]
        public ActionResult IcDonanimDuzenle(int id, InnerEquipment postIe)
        {
            InnerEquipment ie = db.InnerEquipments.Where(x => x.VehicleID == id).FirstOrDefault();
            ie.AdaptiveCruiseControl = postIe.AdaptiveCruiseControl;
            ie.AhsapDireksiyon = postIe.AhsapDireksiyon;
            ie.AhsapKapama = postIe.AhsapKapama;
            ie.AltiIleriVites = postIe.AltiIleriVites;
            ie.AnahtarsizCalistirma = postIe.AnahtarsizCalistirma;
            ie.ArkaKolDayama = postIe.ArkaKolDayama;
            ie.AyarlanabilirDireksiyon = postIe.AyarlanabilirDireksiyon;
            ie.Deri_KumasKoltuk = postIe.Deri_KumasKoltuk;
            ie.DeriDireksiyon = postIe.DeriDireksiyon;
            ie.DeriKoltuk = postIe.DeriKoltuk;
            ie.ElektrikliArkaCamlar = postIe.ElektrikliArkaCamlar;
            ie.ElektrikliOnCamlar = postIe.ElektrikliOnCamlar;
            ie.FonksiyonelDireksiyon = postIe.FonksiyonelDireksiyon;
            ie.GeceGorusKamerasi = postIe.GeceGorusKamerasi;
            ie.HeadUpDisplay = postIe.HeadUpDisplay;
            ie.HidrolikDireksiyon = postIe.HidrolikDireksiyon;
            ie.HizSabitleyici = postIe.HizSabitleyici;
            ie.IsitmaliDireksiyon = postIe.IsitmaliDireksiyon;
            ie.Klima_Analog = postIe.Klima_Analog;
            ie.Klima_Digital = postIe.Klima_Digital;
            ie.Koltuklar_ArkaIsitmali = postIe.Koltuklar_ArkaIsitmali;
            ie.Koltuklar_Elektirkli = postIe.Koltuklar_Elektirkli;
            ie.Koltuklar_Hafizali = postIe.Koltuklar_Hafizali;
            ie.Koltuklar_Katlanır = postIe.Koltuklar_Katlanır;
            ie.Koltuklar_OnIsitmali = postIe.Koltuklar_OnIsitmali;
            ie.Koltuklar_Sogutmali = postIe.Koltuklar_Sogutmali;
            ie.KromKaplama = postIe.KromKaplama;
            ie.KumasKoltuk = postIe.KumasKoltuk;
            ie.OnGorusKamerasi = postIe.OnGorusKamerasi;
            ie.OnKolDayama = postIe.OnKolDayama;
            ie.OtmKararanDikizAynasi = postIe.OtmKararanDikizAynasi;
            ie.SogutmaliTorpido = postIe.SogutmaliTorpido;
            ie.Start_Stop = postIe.Start_Stop;
            ie.UcuncuSiraKoltuk = postIe.UcuncuSiraKoltuk;
            ie.YediIleriVites = postIe.YediIleriVites;
            ie.YolBilgisayari = postIe.YolBilgisayari;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Vehicles", "Home", new { id = ie.VehicleID });
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/IcDonanimDuzenle/" + id);
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View(ie);
        }
        public ActionResult DisDonanimDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Vehicle v = db.Vehicles.Find(id);
            if (v == null)
            {
                return Redirect("/Admin/Index");
            }
            OuterEquipment oe;
            if (db.OuterEquipments.Where(x => x.VehicleID == id).FirstOrDefault() == null)
            {
                oe = new OuterEquipment();
                oe.VehicleID = id;
                db.OuterEquipments.Add(oe);
                db.SaveChanges();
            }
            else
            {
                oe = db.OuterEquipments.Where(x => x.VehicleID == id).FirstOrDefault();
            }
            return View(oe);
        }
        [HttpPost]
        public ActionResult DisDonanimDuzenle(int id, OuterEquipment postOe)
        {
            OuterEquipment oe = db.OuterEquipments.Where(x => x.VehicleID == id).FirstOrDefault();
            oe.AkilliBagajKapagi = postOe.AkilliBagajKapagi;
            oe.AlasimliJant = postOe.AlasimliJant;
            oe.ArkaCamBuzCozucu = postOe.ArkaCamBuzCozucu;
            oe.Aynalar_Elektrikli = postOe.Aynalar_Elektrikli;
            oe.Aynalar_Hafizali = postOe.Aynalar_Hafizali;
            oe.Aynalar_Isitmali = postOe.Aynalar_Isitmali;
            oe.Aynalar_Katlanir = postOe.Aynalar_Katlanir;
            oe.Far_Adaptif = postOe.Far_Adaptif;
            oe.Far_BiXenon = postOe.Far_BiXenon;
            oe.Far_Halojen = postOe.Far_Halojen;
            oe.Far_LED = postOe.Far_LED;
            oe.Far_Sis = postOe.Far_Sis;
            oe.Far_Xenon = postOe.Far_Xenon;
            oe.Far_Yikama = postOe.Far_Yikama;
            oe.FarGeceSensoru = postOe.FarGeceSensoru;
            oe.Hardtop = postOe.Hardtop;
            oe.PanoramikCamTavan = postOe.PanoramikCamTavan;
            oe.PanoramikOnCam = postOe.PanoramikOnCam;
            oe.ParkSensoru_Arka = postOe.ParkSensoru_Arka;
            oe.ParkSensoru_On = postOe.ParkSensoru_On;
            oe.RomorkCekiDemiri = postOe.RomorkCekiDemiri;
            oe.RomorkCekiDemiri = postOe.RomorkCekiDemiri;
            oe.Sunroof = postOe.Sunroof;
            oe.YagmurSensoru = postOe.YagmurSensoru;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Vehicles", "Home", new { id = oe.VehicleID });
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/DisDonanimDuzenle/" + id);
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View(oe);
        }
        public ActionResult MultimedyaDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Vehicle v = db.Vehicles.Find(id);
            if (v == null)
            {
                return Redirect("/Admin/Index");
            }
            Multimedia m;
            if (db.Multimedias.Where(x => x.VehicleID == id).FirstOrDefault() == null)
            {
                m = new Multimedia();
                m.VehicleID = id;
                db.Multimedias.Add(m);
                db.SaveChanges();
            }
            else
            {
                m = db.Multimedias.Where(x => x.VehicleID == id).FirstOrDefault();
            }
            return View(m);
        }
        [HttpPost]
        public ActionResult MultimedyaDuzenle(int id, Multimedia postM)
        {
            Multimedia m = db.Multimedias.Where(x => x.VehicleID == id).FirstOrDefault();
            m.AltiArtiHoparlor = postM.AltiArtiHoparlor;
            m.ArkaEglencePaketi = postM.ArkaEglencePaketi;
            m.AUX = postM.AUX;
            m.Bluetooth_Telefon = postM.Bluetooth_Telefon;
            m.CDDegistirici = postM.CDDegistirici;
            m.DVDDegistirici = postM.DVDDegistirici;
            m.iPodBaglantisi = postM.iPodBaglantisi;
            m.Radyo_CDCalar = postM.Radyo_CDCalar;
            m.Radyo_Kasetçalar = postM.Radyo_Kasetçalar;
            m.Radyo_MP3Calar = postM.Radyo_MP3Calar;
            m.TV_Navigasyon = postM.TV_Navigasyon;
            m.USB_AUX = postM.USB_AUX;
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Vehicles", "Home", new { id = m.VehicleID });
                Response.RemoveOutputCacheItem(staleItem);
                Response.Redirect("/Admin/MultimedyaDuzenle/" + id);
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult AracDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            ViewBag.Modeller = db.Models.ToList();
            ViewBag.AltModeller = db.Sub_Model.ToList();
            ViewBag.YakitTipleri = db.FuelTypes.ToList();
            ViewBag.VitesTipleri = db.TransmissionTypes.ToList();
            ViewBag.KasaTipleri = db.BodyTypes.ToList();
            ViewBag.CekisTipleri = db.WheelDriveTypes.ToList();
            ViewBag.Renkler = db.Colors.ToList();

            Vehicle v = db.Vehicles.Find(id);
            if (v == null)
            {
                return Redirect("/Admin/Index");
            }
            if (Request.QueryString["islem"] == "hata1")
            {
                ViewBag.Hata1 = "Başlık boş geçilemez";
            }
            if (Request.QueryString["islem"] == "hata2")
            {
                ViewBag.Hata2 = "Geçersiz yıl";
            }
            if (Request.QueryString["islem"] == "hata3")
            {
                ViewBag.Hata3 = "Geçersiz KM";
            }
            if (Request.QueryString["islem"] == "hata4")
            {
                ViewBag.Hata4 = "Geçersiz motor kapasitesi";
            }
            if (Request.QueryString["islem"] == "hata5")
            {
                ViewBag.Hata5 = "Geçersiz motor gücü";
            }
            if (Request.QueryString["islem"] == "hata6")
            {
                ViewBag.Hata6 = "Geçersiz bölge";
            }
            if (Request.QueryString["islem"] == "hata7")
            {
                ViewBag.Hata7 = "Açıklama boş geçilemez";
            }
            return View(v);
        }
        [HttpPost]
        public ActionResult AracDuzenle(int id, Vehicle postV, string txtUrunAdi, int? txtYil, int? txtKM, int? txtMotorKapasitesi, int? txtMotorGucu, string ddlGaranti, string txtBolge, string ddlTakas, string ddlDurum, string txtAciklama, HttpPostedFileBase fuPhotoPath)
        {
            ViewBag.Kategoriler = db.Categories.ToList();
            ViewBag.Markalar = db.Brands.ToList();
            ViewBag.Seriler = db.Series.ToList();
            ViewBag.Modeller = db.Models.ToList();
            ViewBag.AltModeller = db.Sub_Model.ToList();
            ViewBag.YakitTipleri = db.FuelTypes.ToList();
            ViewBag.VitesTipleri = db.TransmissionTypes.ToList();
            ViewBag.KasaTipleri = db.BodyTypes.ToList();
            ViewBag.CekisTipleri = db.WheelDriveTypes.ToList();
            ViewBag.Renkler = db.Colors.ToList();

            Vehicle v = db.Vehicles.Find(id);

            string fotografyolu = v.MasterPhotoPath;
            string benzersizIsim = "";



            if (txtUrunAdi == "" || txtUrunAdi == null)
            {
                return Redirect("/Admin/AracDuzenle/" + id + "?islem=hata1");
            }
            if (!cs.IsNumeric(Convert.ToString(txtYil)))
            {
                return Redirect("/Admin/AracDuzenle/" + id + "?islem=hata2");
            }
            if (!cs.IsNumeric(Convert.ToString(txtKM)))
            {
                return Redirect("/Admin/AracDuzenle/" + id + "?islem=hata3");
            }
            if (!cs.IsNumeric(Convert.ToString(txtMotorKapasitesi)))
            {
                return Redirect("/Admin/AracDuzenle/" + id + "?islem=hata4");
            }
            if (!cs.IsNumeric(Convert.ToString(txtMotorGucu)))
            {
                return Redirect("/Admin/AracDuzenle/" + id + "?islem=hata5");
            }
            if (!cs.IsName(txtBolge))
            {
                return Redirect("/Admin/AracDuzenle/" + id + "?islem=hata6");
            }
            if (txtAciklama == "" || txtAciklama == null)
            {
                return Redirect("/Admin/AracDuzenle/" + id + "?islem=hata7");
            }



            if (fuPhotoPath != null && fuPhotoPath.ContentLength > 0)
            {
                Random rnd = new Random();
                benzersizIsim = rnd.Next(10, 55555555) + DateTime.Now.Day + "_" + DateTime.Now.Month + fuPhotoPath.FileName;

                var path = Path.Combine(Server.MapPath("~/upload"), benzersizIsim);
                fuPhotoPath.SaveAs(path);
                v.MasterPhotoPath = "/upload/" + benzersizIsim;
            }
            else
            {
                v.MasterPhotoPath = fotografyolu;
            }
            
            v.CategoryID = postV.CategoryID;
            v.BrandID = postV.BrandID;
            v.SerieID = postV.SerieID;
            v.ModelID = postV.ModelID;
            v.Sub_ModelID = postV.Sub_ModelID;
            v.FuelTypeID = postV.FuelTypeID;
            v.TransmissionTypeID = postV.TransmissionTypeID;
            v.BodyTypeID = postV.BodyTypeID;
            v.WheelDriveTypeID = postV.WheelDriveTypeID;
            v.ColorID = postV.ColorID;

            v.ProductName = txtUrunAdi;
            v.Year = (int)txtYil;
            v.KM = (int)txtKM;
            v.EngineCapacity = (int)txtMotorKapasitesi;
            v.EngineHP = (int)txtMotorGucu;
            if (ddlGaranti == "Var")
            {
                v.Warranty = true;
            }
            else
            {
                v.Warranty = false;
            }
            v.Region = txtBolge;
            if (ddlTakas == "Var")
            {
                v.Trade = true;
            }
            else
            {
                v.Trade = false;
            }
            if (ddlDurum == "İkinci El")
            {
                v.Condition = true;
            }
            else
            {
                v.Condition = false;
            }
            v.Description = txtAciklama;           

            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                foreach (Vehicle item in db.Vehicles.ToList())
                {
                    var staleItem = Url.Action("Vehicles", "Home", new { id = item.ID });
                    Response.RemoveOutputCacheItem(staleItem);
                }
                var staleItem2 = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem2);
                Response.Redirect("/Admin/AracListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }

            return View(v);
        }
        public ActionResult AracSil(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            Vehicle v = db.Vehicles.Find(id);
            if (v == null)
            {
                return Redirect("/Admin/Index");
            }
            List<Photo> resimListe = db.Photos.Where(x => x.VehicleID == id).ToList();
            db.Photos.RemoveRange(resimListe);

            string mappedPath1 = Server.MapPath("~"+v.MasterPhotoPath);

            if (System.IO.File.Exists(mappedPath1))
            {
                System.IO.File.Delete(mappedPath1);
            }
            //foreach (Photo item in resimListe)
            //{
            //    if (System.IO.File.Exists(item.PhotoPath))
            //    {
            //    System.IO.File.Delete(item.PhotoPath);
            //    }
            //}
            //System.IO.DirectoryInfo di = new DirectoryInfo("~/upload/"+ id);
            string mappedPath2 = Server.MapPath("~/upload/" + id);
            if (System.IO.Directory.Exists(mappedPath2))
            {
                System.IO.Directory.Delete(mappedPath2, true);
            }

            if (db.SecuritySpecs.Where(x => x.VehicleID == id).FirstOrDefault() != null)
            {
                SecuritySpec ss = db.SecuritySpecs.Where(x => x.VehicleID == id).FirstOrDefault();
                db.SecuritySpecs.Remove(ss);
            }

            if (db.InnerEquipments.Where(x => x.VehicleID == id).FirstOrDefault() != null)
            {
                InnerEquipment ie = db.InnerEquipments.Where(x => x.VehicleID == id).FirstOrDefault();
                db.InnerEquipments.Remove(ie);
            }

            if (db.OuterEquipments.Where(x => x.VehicleID == id).FirstOrDefault() != null)
            {
                OuterEquipment oe = db.OuterEquipments.Where(x => x.VehicleID == id).FirstOrDefault();
                db.OuterEquipments.Remove(oe);
            }

            if (db.Multimedias.Where(x => x.VehicleID == id).FirstOrDefault() != null)
            {
                Multimedia m = db.Multimedias.Where(x => x.VehicleID == id).FirstOrDefault();
                db.Multimedias.Remove(m);
            }

            if (db.SaleInfos.Where(x => x.VehicleID == id).FirstOrDefault() != null)
            {
                SaleInfo si = db.SaleInfos.Where(x => x.VehicleID == id).FirstOrDefault();
                db.SaleInfos.Remove(si);
            }

            db.Vehicles.Remove(v);

            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                var staleItem = Url.Action("Listele", "Home");
                Response.RemoveOutputCacheItem(staleItem);
                foreach (Vehicle item in db.Vehicles.ToList())
                {
                    var staleItem2 = Url.Action("Vehicles", "Home", new { id = item.ID });
                    Response.RemoveOutputCacheItem(staleItem2);
                }
                Response.Redirect("/Admin/AracListele");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }

            return View();
        }
        public ActionResult Teklifler()
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            List<SoldOrder> soListe = db.SoldOrders.ToList();
            return View(soListe);
        }
        public ActionResult TeklifSil(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            SoldOrder so = db.SoldOrders.Find(id);
            db.SoldOrders.Remove(so);
            int sayi = db.SaveChanges();
            if (sayi > 0)
            {
                return Redirect("/Admin/Teklifler");
            }
            else
            {
                Response.Write("<script>alert('Hatalı işlem');</script>");
            }
            return View();
        }
        public ActionResult TeklifDuzenle(int id)
        {
            if (Session["AdminID"] == null)
            {
                return Redirect("/Admin/Login?islem=hata");
            }
            SoldOrder so = db.SoldOrders.Find(id);
            ViewBag.Araclar = db.Vehicles.ToList();
            return View(so);
        }
    }
}