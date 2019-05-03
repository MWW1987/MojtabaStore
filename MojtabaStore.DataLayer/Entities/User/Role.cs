using MojtabaStore.DataLayer.Entities.Permissions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MojtabaStore.DataLayer.Entities.User
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Display(Name ="عنوان نقش")]
        [Required(ErrorMessage ="وارد کردن {0} الزامی میباشد")]
        [MaxLength(200,ErrorMessage ="{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string RoleTitle { get; set; }
        public bool IsDelete { get; set; }

        public virtual List<UserRole> UserRoles { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }
}
