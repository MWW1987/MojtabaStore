using MojtabaStore.DataLayer.Entities.Permissions;
using MojtabaStore.DataLayer.Entities.User;
using System.Collections.Generic;

namespace MojtabaStore.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        List<Role> GetRoles();
        void AddRolesToUser(List<int> roleIds, int userId);
        void EditRolesUser(int userId, List<int> rolesId);

        int AddRole(Role role);
        Role GetRoleById(int roleId);
        void UpdateRole(Role role);
        void DeleteRole(Role role);

        List<Permission> GetAllPermission();
        void AddPermissionToRole(int roleId, List<int> permission);
        List<int> PermissionsRole(int roleId);
        void UpdatePermissionsRole(int roleId, List<int> permissions);
        bool ChekcPermission(int permissionId, string userName);
    }
}
