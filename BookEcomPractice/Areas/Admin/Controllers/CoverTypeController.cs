using BookEcomPractice.DataAccess.Repository.IRepository;
using BookEcomPractice.Models;
using BookEcomPractice.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookEcomPractice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region APIs
        public IActionResult GetAll()
        {
            var coverTypeInDb = _unitOfWork.CoverType.GetAll();

            return Json(new { data = coverTypeInDb });
        }

       
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var CoverTypeInDb = _unitOfWork.CoverType.Get(id);
           
     
            if (CoverTypeInDb==null)
                return Json(new
                {
                    success = false,
                    message = "something went wrong while delete data!!!"
                });
           
            _unitOfWork.CoverType.Remove(CoverTypeInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Delete Successfully!" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null) return View(coverType);
                     
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return NotFound();
            if (!ModelState.IsValid) return View(coverType);
            
            if (coverType.Id == 0)
                _unitOfWork.CoverType.Add(coverType);
            else
                 _unitOfWork.CoverType.Update(coverType);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
        }
      
    }
}
