using BookEcomPractice.DataAccess.Repository.IRepository;
using BookEcomPractice.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookEcomPractice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region APIS
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitOfWork.Category.GetAll();
            return Json(new { data = categoryList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var CategoryInDb = _unitOfWork.Category.Get(id);
            if(CategoryInDb==null)
            {
                return Json(new { success = false, message = "Something Went Wrong While Delete Data!!" });
            }
            _unitOfWork.Category.Remove(CategoryInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Successfully Deleted" });

        }
        #endregion

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if(id==null) return View(category); // to create only

            category=_unitOfWork.Category.Get(id.GetValueOrDefault()); // to update 

            if (category==null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category==null) return NotFound();

            if(!ModelState.IsValid) return View(category);

            if(category.Id==0)
                _unitOfWork.Category.Add(category);
            else
                _unitOfWork.Category.Update(category);

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
            
        }
    }
}

