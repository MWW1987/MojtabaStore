using MojtabaStore.Core.Services.Interfaces;
using MojtabaStore.DataLayer.Context;
using MojtabaStore.DataLayer.Entities.Permissions;
using MojtabaStore.DataLayer.Entities.User;
using System.Collections.Generic;
using System.Linq;

namespace MojtabaStore.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly MojtabaStoreContext context;

        public PermissionService(MojtabaStoreContext context)
        {
            this.context = context;
        }

        public List<Role> GetRoles()
        {
            return context.Roles.ToList();
        }
        public void AddRolesToUser(List<int> roleIds, int userId)
        {
            foreach (int roleId in roleIds)
            {
                context.UserRoles.Add(new UserRole()
                {
                    RoleId = roleId,
                    UserId = userId
                });
            }
            context.SaveChanges();
        }

        public void EditRolesUser(int userId, List<int> rolesId)
        {
            context.UserRoles.Where(r => r.UserId == userId).ToList().ForEach(r => context.UserRoles.Remove(r));
            AddRolesToUser(rolesId, userId);
        }

        public int AddRole(Role role)
        {
            context.Roles.Add(role);
            context.SaveChanges();
            return role.RoleId;

        }

        public Role GetRoleById(int roleId)
        {
            return context.Roles.Find(roleId);
        }

        public void UpdateRole(Role role)
        {
            context.Roles.Update(role);
            context.SaveChanges();
        }

        public void DeleteRole(Role role)
        {
            role.IsDelete = true;
            UpdateRole(role);
        }

        public List<Permission> GetAllPermission()
        {
            return context.Permissions.ToList();
        }

        public void AddPermissionToRole(int roleId, List<int> permission)
        {
            foreach (var item in permission)
            {
                context.RolePermissions.Add(new RolePermission()
                {
                    PermissionId = item,
                    RoleId = roleId
                });
            }
            context.SaveChanges();
        }

        public List<int> PermissionsRole(int roleId)
        {
            return context.RolePermissions.Where(c => c.RoleId == roleId).Select(c => c.PermissionId).ToList();
        }

        public void UpdatePermissionsRole(int roleId, List<int> permissions)
        {
            context.RolePermissions.Where(c => c.RoleId == roleId).ToList().ForEach(c => context.RolePermissions.Remove(c));
            AddPermissionToRole(roleId, permissions);
        }

        public bool ChekcPermission(int permissionId, string userName)
        {
            int userId = context.Users.Single(c => c.UserName == userName).UserId;
            List<int> UserRoles = context.UserRoles.Where(c => c.UserId == userId).Select(c => c.RoleId).ToList();

            if (!UserRoles.Any())
                return false;

            List<int> RolesPermission = context.RolePermissions.Where(c => c.PermissionId == permissionId).Select(c => c.RoleId).ToList();

            return RolesPermission.Any(c => UserRoles.Contains(c));
        }
    }
}
