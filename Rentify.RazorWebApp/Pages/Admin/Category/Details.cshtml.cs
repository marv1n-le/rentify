using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp
{
    public class DetailsModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;

        public DetailsModel(ICategoryService categoryService, IUnitOfWork unitOfWork)
        {
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryById(id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                Category = category;
            }
            return Page();
        }
    }
}
