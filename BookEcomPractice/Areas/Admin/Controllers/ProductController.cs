using BookEcomPractice.DataAccess.Repository.IRepository;
using BookEcomPractice.Models;
using BookEcomPractice.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BookEcomPractice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }

        #region APIs

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Product.GetAll() });
        }

        [HttpPost]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var ProductInDb = _unitOfWork.Product.Get(id);
            if (ProductInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data!!" });
            //Image Delete

            var WebRootPath = _webHostEnvironment.WebRootPath;
            var ImagePath = Path.Combine(WebRootPath, ProductInDb.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(ImagePath))
            {
                System.IO.File.Delete(ImagePath);
            }
            //*******
            _unitOfWork.Product.Remove(ProductInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Deleted Successfully!!" });
        }
        #endregion

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(ctl => new SelectListItem()
                {
                    Text = ctl.Name,
                    Value = ctl.Id.ToString()
                })
            };
            if (id == null) return View(productVM);
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null) return NotFound();
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var WebRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    var filename = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var Upload = Path.Combine(WebRootPath, @"Images\Products");
                    if (productVM.Product.Id != 0)
                    {
                        var ImageExists = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = ImageExists;
                    }
                    if (productVM.Product.ImageUrl != null)
                    {
                        var ImagePath = Path.Combine(WebRootPath, productVM.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(Upload, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    productVM.Product.ImageUrl = @"\Images\Products\" + filename + extension;
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        var ImageExists = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = ImageExists;
                    }
                }
                if (productVM.Product.Id == 0)
                    _unitOfWork.Product.Add(productVM.Product);
                else
                    _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                productVM = new ProductVM()
                {
                    Product = new Product(),
                    CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitOfWork.CoverType.GetAll().Select(ctl => new SelectListItem()
                    {
                        Text = ctl.Name,
                        Value = ctl.Id.ToString()
                    })
                };
                if (productVM.Product.Id!=0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
                return View(productVM);
            }
        }

    }
}
