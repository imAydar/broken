using System;
using System.Collections.Generic;

using BrokenCode.Etc;
using BrokenCode.Model;
using BrokenCode.Model.Enums;

namespace BrokenCode.Helpers
{
    public static class LicenseHelper
    {
        /// <summary>
        /// Get a license type as a string.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guidsToLicences"></param>
        /// <returns></returns>
        public static LicenseType GetLicenseType(User user, Dictionary<Guid, LicenseInfo> guidsToLicences)
        {
            var licenseType = new LicenseInfo();

            if (guidsToLicences.TryGetValue(user.Id, out licenseType))
            {
                return licenseType.IsTrial ? LicenseType.Trial : LicenseType.Paid;
            }

            return LicenseType.None;
        }
    }
}
