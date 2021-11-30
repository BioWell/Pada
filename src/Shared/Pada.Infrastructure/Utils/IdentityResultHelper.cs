﻿using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Pada.Infrastructure.Utils
{
    public static class IdentityResultHelper
    {
        public static IdentityResult LogResult(this IdentityResult identityResult, string successMessage,
            string errorMessage = null)
        {
            if (identityResult.Succeeded)
                Log.Logger.Information(successMessage);
            else
            {
                if (errorMessage is not null)
                {
                    Log.Logger.Error(errorMessage);
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        Log.Logger.Error(error.Description);
                    }
                }
            }

            return identityResult;
        }
    }
}