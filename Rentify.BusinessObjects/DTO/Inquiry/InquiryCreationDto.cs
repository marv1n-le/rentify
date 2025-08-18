using Rentify.BusinessObjects.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.BusinessObjects.DTO.Inquiry;

public class InquiryCreationDto
{
    public string PostId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public InquiryStatus Status { get; set; } = InquiryStatus.Open;
}
