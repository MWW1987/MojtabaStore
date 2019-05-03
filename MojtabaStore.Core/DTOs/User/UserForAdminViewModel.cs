using MojtabaStore.DataLayer.Entities.User;
using System.Collections.Generic;

namespace MojtabaStore.Core.DTOs.User
{
    public class UserForAdminViewModel
    {
        public List<MojtabaStore.DataLayer.Entities.User.User> Users { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
    }
}
