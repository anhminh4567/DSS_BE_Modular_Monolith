using DiamondShop.Commons;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.ErrorMessages
{
    public class DiamondErrors
    {
        public static NotFoundError DiamondNotFoundError = new NotFoundError("Không tìm thấy kim cương");
        public static Error NotHavingCertificate = new Error("Kim cương không có chứng chỉ");
        public static ConflictError DiamondExistError(string? detail = null) 
        {
            if(detail != null)
                return new ConflictError("Kim cương đã tồn tại, " + detail);
            return new ConflictError("Kim cương đã tồn tại");
        }
        public static ValidationError DiamondNotValid(Dictionary<string, object>? errors = null) {
            if(errors != null )
                return  new ValidationError("Kim cương không hợp lệ", errors);
            return new ValidationError("Kim cương không hợp lệ");
        }
        
        public static ValidationError DiamondNotAvailable = new ValidationError("Kim cương không còn sẵn");
        public static ConflictError UnknownPrice = new ConflictError("Không tìm thấy giá kim cương, vui lòng liên hệ nhân viên để được báo giá riêng nếu có thể");
        public static Error NotInCorrectState = new Error("Kim cương không ở trạng thái yêu cầu");
        public static Error DeleteUnallowed(string? extraDetail = null) 
        {
            if(extraDetail != null)
                return new Error("Không thể xóa kim cương này, lý do " + extraDetail);
            return new Error("Không thể xóa kim cương này, phải trạng thái inactive ");
        }
        public static ConflictError DiamondExistInTransaction = new ConflictError("Kim cương đã được sử dụng trong giao dịch");
        public static ConflictError DiamondAssignedToJewelryAlready(string? jewelryCode = null, string? detail = null) 
        {
            string messageBase = "Kim cương đã được gán vào trang sức";
            if (jewelryCode != null && detail != null)
                return new ConflictError(messageBase+ " với mã code #" + jewelryCode+ ", "+detail);
            if(jewelryCode != null)
                return new ConflictError(messageBase+" với mã code #" + jewelryCode);
            if (detail != null)
                return new ConflictError(messageBase +", " + detail);
            return new ConflictError(messageBase);
        } 
        public static ConflictError DiamondNotExistInAnyCriteria = new ConflictError("Kim cương không thuộc bất kỳ tiêu chí nào trong tiệm, hãy vào bảng giá, thêm khoảng carat range để có thể add");
        public static ConflictError DiamondNotMeetingRequirementSpec => new ConflictError("Kim cương không đáp ứng yêu cầu về thông số");
        public static Error LockPriceNotValid(string? detail = null)
        {
            if(detail != null)
                return new Error("Khóa giá không hợp lệ, " + detail);
            return new Error("Khóa giá không hợp lệ");
        }
        public static Error SoldError(Diamond? soldDiamond = null)
        {
            if(soldDiamond != null)
                return new Error("Kim cương đã được bán, mã code #" + soldDiamond.SerialCode );
            return new Error("Kim cương đã được bán");
        }
        public class LockError
        {
            public static Error SoldError => new Error("Khóa giá không thể thực hiện với kim cương đã bán");
            
        }
    }
}
