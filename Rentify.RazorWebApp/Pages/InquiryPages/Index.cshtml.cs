using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.InquiryPages
{
    public class IndexModel : PageModel
    {
        private readonly IInquiryService _inquiryService;

        public IndexModel(IInquiryService inquiryService)
        {
            _inquiryService = inquiryService;
        }

        public IList<Inquiry> Inquiry { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Inquiry = (IList<Inquiry>)await _inquiryService.GetAllInquiries();
        }
    }
}
