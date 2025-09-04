using BookEcomPractice.DataAccess.Data;
using BookEcomPractice.DataAccess.Repository.IRepository;
using BookEcomPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEcomPractice.DataAccess.Repository
{
    public class CompanyRepository:Repository<Company>,ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
