using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Localization;

namespace Nop.Services.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<PermissionRecord> _permissionRecordRepository;
        private readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PermissionService(ICacheManager cacheManager,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IRepository<PermissionRecord> permissionRecordRepository,
            IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _cacheManager = cacheManager;
            _customerService = customerService;
            _localizationService = localizationService;
            _permissionRecordRepository = permissionRecordRepository;
            _permissionRecordCustomerRoleMappingRepository = permissionRecordCustomerRoleMappingRepository;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get permission records by customer role identifier
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Permissions</returns>
        protected async virtual Task<IList<PermissionRecord>> GetPermissionRecordsByCustomerRoleId(int customerRoleId)
        {
            var key = string.Format(NopSecurityDefaults.PermissionsAllByCustomerRoleIdCacheKey, customerRoleId);
            return await _cacheManager.Get(key, async () =>
            {
                var query = from pr in _permissionRecordRepository.Table
                            join prcrm in _permissionRecordCustomerRoleMappingRepository.Table on pr.Id equals prcrm.PermissionRecordId
                            where prcrm.CustomerRoleId == customerRoleId
                            orderby pr.Id
                            select pr;

                return await query.ToListAsync();
            });
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        protected async virtual Task<bool> Authorize(string permissionRecordSystemName, int customerRoleId)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var key = string.Format(NopSecurityDefaults.PermissionsAllowedCacheKey, customerRoleId, permissionRecordSystemName);
            return await _staticCacheManager.Get(key, async () =>
            {
                var permissions = await GetPermissionRecordsByCustomerRoleId(customerRoleId);
                foreach (var permission1 in permissions)
                    if (permission1.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public async virtual Task DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            await _permissionRecordRepository.Delete(permission);

            _cacheManager.RemoveByPrefix(NopSecurityDefaults.PermissionsPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopSecurityDefaults.PermissionsPrefixCacheKey);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public async virtual Task<PermissionRecord> GetPermissionRecordById(int permissionId)
        {
            if (permissionId == 0)
                return null;

            return await _permissionRecordRepository.GetById(permissionId);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        public async virtual Task<PermissionRecord> GetPermissionRecordBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _permissionRecordRepository.Table
                        where pr.SystemName == systemName
                        orderby pr.Id
                        select pr;

            var permissionRecord = await query.FirstOrDefaultAsync();
            return permissionRecord;
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public async virtual Task<IList<PermissionRecord>> GetAllPermissionRecords()
        {
            var query = from pr in _permissionRecordRepository.Table
                        orderby pr.Name
                        select pr;
            var permissions = await query.ToListAsync();
            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public async virtual Task InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            await _permissionRecordRepository.Insert(permission);

            _cacheManager.RemoveByPrefix(NopSecurityDefaults.PermissionsPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopSecurityDefaults.PermissionsPrefixCacheKey);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public async virtual Task UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            await _permissionRecordRepository.Update(permission);

            _cacheManager.RemoveByPrefix(NopSecurityDefaults.PermissionsPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopSecurityDefaults.PermissionsPrefixCacheKey);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public async virtual Task InstallPermissions(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            //default customer role mappings
            var defaultPermissions = permissionProvider.GetDefaultPermissions().ToList();

            foreach (var permission in permissions)
            {
                var permission1 = await GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 != null)
                    continue;

                //new permission (install it)
                permission1 = new PermissionRecord
                {
                    Name = permission.Name,
                    SystemName = permission.SystemName,
                    Category = permission.Category
                };

                foreach (var defaultPermission in defaultPermissions)
                {
                    var customerRole = await _customerService.GetCustomerRoleBySystemName(defaultPermission.CustomerRoleSystemName);
                    if (customerRole == null)
                    {
                        //new role (save it)
                        customerRole = new CustomerRole
                        {
                            Name = defaultPermission.CustomerRoleSystemName,
                            Active = true,
                            SystemName = defaultPermission.CustomerRoleSystemName
                        };
                        await _customerService.InsertCustomerRole(customerRole);
                    }

                    var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                                  where p.SystemName == permission1.SystemName
                                                  select p).Any();
                    var mappingExists = (from mapping in customerRole.PermissionRecordCustomerRoleMappings
                                         where mapping.PermissionRecord.SystemName == permission1.SystemName
                                         select mapping.PermissionRecord).Any();
                    if (defaultMappingProvided && !mappingExists)
                    {
                        //permission1.CustomerRoles.Add(customerRole);
                        permission1.PermissionRecordCustomerRoleMappings.Add(new PermissionRecordCustomerRoleMapping { CustomerRole = customerRole });
                    }
                }

                //save new permission
                await InsertPermissionRecord(permission1);

                //save localization
                await _localizationService.SaveLocalizedPermissionName(permission1);
            }
        }

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public async virtual Task UninstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = await GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 == null) 
                    continue;

                await DeletePermissionRecord(permission1);

                //delete permission locales
                await _localizationService.DeleteLocalizedPermissionName(permission1);
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public async virtual Task<bool> Authorize(PermissionRecord permission)
        {
            return await Authorize(permission, _workContext.CurrentCustomer);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public async virtual Task<bool> Authorize(PermissionRecord permission, Customer customer)
        {
            if (permission == null)
                return false;

            if (customer == null)
                return false;

            //old implementation of Authorize method
            //var customerRoles = customer.CustomerRoles.Where(cr => cr.Active);
            //foreach (var role in customerRoles)
            //    foreach (var permission1 in role.PermissionRecords)
            //        if (permission1.SystemName.Equals(permission.SystemName, StringComparison.InvariantCultureIgnoreCase))
            //            return true;

            //return false;

            return await Authorize(permission.SystemName, customer);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public async virtual Task<bool> Authorize(string permissionRecordSystemName)
        {
            return await Authorize(permissionRecordSystemName, _workContext.CurrentCustomer);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public async virtual Task<bool> Authorize(string permissionRecordSystemName, Customer customer)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var customerRoles = customer.CustomerRoles.Where(cr => cr.Active);
            foreach (var role in customerRoles)
                if (await Authorize(permissionRecordSystemName, role.Id))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        #endregion
    }
}