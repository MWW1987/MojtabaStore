using Microsoft.EntityFrameworkCore;
using MojtabaStore.Core.Convertors;
using MojtabaStore.Core.DTOs.User;
using MojtabaStore.Core.Generator;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Services.Interfaces;
using MojtabaStore.DataLayer.Context;
using MojtabaStore.DataLayer.Entities.User;
using MojtabaStore.DataLayer.Entities.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MojtabaStore.Core.Services
{
    public class UserService : IUserService
    {
        private readonly MojtabaStoreContext context;

        public UserService(MojtabaStoreContext context)
        {
            this.context = context;
        }

        public bool ActiveAccount(string activeCode)
        {
            var user = context.Users.SingleOrDefault(c => c.ActiveCode == activeCode);

            if (user == null || user.IsActive)
                return false;

            user.IsActive = true;
            user.ActiveCode = NameGenerator.GenerateUniqueCode();
            context.SaveChanges();
            return true;
        }

        public int AddUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return user.UserId;
        }

        public SideBarUserPanelViewModel GetSideBarUserPanelData(string userName)
        {
            return context.Users.Where(c => c.UserName == userName).Select(c => new SideBarUserPanelViewModel()
            {
                UserName = c.UserName,
                RegisterDate = c.RegisterDate,
                ImageName = c.UserAvatar
            }).Single();
        }

        public EditProfileViewModel GetDataForEditProfileUser(string userName)
        {
            return context.Users.Where(c => c.UserName == userName).Select(c => new EditProfileViewModel()
            {
                UserName = c.UserName,
                Email = c.Email,
                AvatarName = c.UserAvatar
            }).Single();
        }



        public User GetUserByActiveCode(string activeCode)
        {
            return context.Users.SingleOrDefault(c => c.ActiveCode == activeCode);
        }

        public User GetUserByEmail(string email)
        {
            return context.Users.SingleOrDefault(c => c.Email == email);
        }

        public User GetUserByUserName(string userName)
        {
            return context.Users.SingleOrDefault(u => u.UserName == userName);
        }

        public User GetUserById(int userId)
        {
            return context.Users.Find(userId);
        }

        public int GetUserIdByUserName(string userName)
        {
            return context.Users.Single(c => c.UserName == userName).UserId;
        }

        public InformationUserViewModel GetUserInformation(string userName)
        {
            User user = GetUserByUserName(userName);
            InformationUserViewModel information = new InformationUserViewModel();
            information.UserName = user.UserName;
            information.Email = user.Email;
            information.RegisterDate = user.RegisterDate;
            information.Wallet = BalanceUserWallet(userName);

            return information;
        }

        public InformationUserViewModel GetUserInformation(int userId)
        {
            User user = GetUserById(userId);
            InformationUserViewModel information = new InformationUserViewModel();
            information.UserName = user.UserName;
            information.Email = user.Email;
            information.RegisterDate = user.RegisterDate;
            information.Wallet = BalanceUserWallet(user.UserName);

            return information;
        }

        public bool IsExistEmail(string email)
        {
            return context.Users.Any(c => c.Email == email);
        }

        public bool IsExistUserName(string userName)
        {
            return context.Users.Any(c => c.UserName == userName);
        }

        public User LoginUser(LoginViewModel login)
        {
            string hashPassword = PasswordHelper.EncodePasswordMd5(login.Password);
            string email = FixedText.FixEmail(login.Email);
            return context.Users.SingleOrDefault(c => c.Email == email && c.Password == hashPassword);
        }

        public void UpdateUser(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
        }

        public void EditProfile(string userName, EditProfileViewModel profile)
        {

            if (profile.UserAvatar != null)
            {
                string imagePath = "";

                if (profile.AvatarName != "Defult.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);
                    if (File.Exists(imagePath))
                        File.Delete(imagePath);
                }

                profile.AvatarName = NameGenerator.GenerateUniqueCode() + Path.GetExtension(profile.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    profile.UserAvatar.CopyTo(stream);
                }
            }

            var user = GetUserByUserName(userName);
            user.UserName = profile.UserName;
            user.Email = profile.Email;
            user.UserAvatar = profile.AvatarName;
            UpdateUser(user);
        }

        public bool CompareOldPassword(string userName, string oldPassword)
        {
            string hashOldPassword = PasswordHelper.EncodePasswordMd5(oldPassword);
            return context.Users.Any(c => c.UserName == userName && c.Password == hashOldPassword);
        }

        public void ChangeUserPassword(string userName, string newPassword)
        {
            var user = GetUserByUserName(userName);
            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);
            UpdateUser(user);
        }

        public int BalanceUserWallet(string userName)
        {
            int userId = GetUserIdByUserName(userName);

            var settle = context.Wallets.Where(c => c.UserId == userId && c.TypeId == 1 && c.IsPay == true).Select(c => c.Amount).ToList();
            var take = context.Wallets.Where(c => c.UserId == userId && c.TypeId == 2 && c.IsPay == true).Select(c => c.Amount).ToList();

            return settle.Sum() - take.Sum();
        }

        public List<WalletViewModel> GetWalletUser(string userName)
        {
            int userId = GetUserIdByUserName(userName);
            return context.Wallets.Where(c => c.IsPay == true && c.UserId == userId).Select(c => new WalletViewModel()
            {
                Amount = c.Amount,
                DateTime = c.CreateDate,
                Description = c.Description,
                Type = c.TypeId
            }).ToList();
        }

        public int ChargeWallet(string userName, int amount, string description, bool isPay)
        {
            Wallet wallet = new Wallet()
            {
                Amount = amount,
                CreateDate = DateTime.Now,
                Description = description,
                IsPay = isPay,
                TypeId = 1,
                UserId = GetUserIdByUserName(userName)
            };

            return AddWallet(wallet);
        }

        public int AddWallet(Wallet wallet)
        {
            context.Wallets.Add(wallet);
            context.SaveChanges();
            return wallet.WalletId;
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return context.Wallets.Find(walletId);
        }

        public void UpdateWallet(Wallet wallet)
        {
            context.Wallets.Update(wallet);
            context.SaveChanges();
        }

        public UserForAdminViewModel GetUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> result = context.Users;

            if (!string.IsNullOrEmpty(filterEmail))
                result = result.Where(c => c.Email == filterEmail);

            if (!string.IsNullOrEmpty(filterUserName))
                result = result.Where(c => c.UserName == filterUserName);

            int take = 20;
            int skip = (pageId - 1) * take;

            UserForAdminViewModel list = new UserForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderBy(c => c.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public int AddUserFromAdmin(CreateUserViewModel user)
        {
            User addUser = new User();
            addUser.Password = PasswordHelper.EncodePasswordMd5(user.Password);
            addUser.ActiveCode = NameGenerator.GenerateUniqueCode();
            addUser.Email = user.Email;
            addUser.IsActive = true;
            addUser.RegisterDate = DateTime.Now;
            addUser.UserName = user.UserName;

            #region Save Avatar

            if (user.UserAvatar != null)
            {
                string imagePath = "";
                addUser.UserAvatar = NameGenerator.GenerateUniqueCode() + Path.GetExtension(user.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", addUser.UserAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    user.UserAvatar.CopyTo(stream);
                }
            }

            #endregion

            return AddUser(addUser);
        }

        public EditUserViewModel GetUserForShowInEditMode(int userId)
        {
            return context.Users.Where(u => u.UserId == userId)
               .Select(u => new EditUserViewModel()
               {
                   UserId = u.UserId,
                   AvatarName = u.UserAvatar,
                   Email = u.Email,
                   UserName = u.UserName,
                   UserRoles = u.UserRoles.Select(r => r.RoleId).ToList()
               }).Single();
        }

        public void EditUserFromAdmin(EditUserViewModel editUser)
        {
            User user = GetUserById(editUser.UserId);
            user.Email = editUser.Email;
            if (!string.IsNullOrEmpty(editUser.Password))
            {
                user.Password = PasswordHelper.EncodePasswordMd5(editUser.Password);
            }

            if (editUser.UserAvatar != null)
            {
                //Delete old Image
                if (editUser.AvatarName != "Defult.jpg")
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editUser.AvatarName);
                    if (File.Exists(deletePath))
                    {
                        File.Delete(deletePath);
                    }
                }

                //Save New Image
                user.UserAvatar = NameGenerator.GenerateUniqueCode() + Path.GetExtension(editUser.UserAvatar.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", user.UserAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    editUser.UserAvatar.CopyTo(stream);
                }
            }

            context.Users.Update(user);
            context.SaveChanges();
        }

        public UserForAdminViewModel GetDeleteUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> result = context.Users.IgnoreQueryFilters().Where(c => c.IsDelete);

            if (!string.IsNullOrEmpty(filterEmail))
                result = result.Where(c => c.Email == filterEmail);

            if (!string.IsNullOrEmpty(filterUserName))
                result = result.Where(c => c.UserName == filterUserName);

            int take = 20;
            int skip = (pageId - 1) * take;

            UserForAdminViewModel list = new UserForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderBy(c => c.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public void DeleteUser(int userId)
        {
            User user = GetUserById(userId);
            user.IsDelete = true;
            UpdateUser(user);
        }

        
    }
}
