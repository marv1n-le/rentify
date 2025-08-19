namespace Rentify.BusinessObjects.Enum;

public enum InquiryStatus
{
    Open,            // user vừa gửi yêu cầu
    Quoted,          // đã tạo báo giá (Rental ở trạng thái Quoted)
    ClosedAccepted,  // user đồng ý -> sẽ/đang tiến hành rental
    ClosedRejected,  // từ chối / hết hàng / hết hạn
    ClosedDuplicate
}