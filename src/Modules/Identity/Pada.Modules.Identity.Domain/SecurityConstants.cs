namespace Pada.Modules.Identity.Domain
{
    public static class SecurityConstants
    {
        public static class Permission
        {
            public static class Users
            {
                public const string View = "user:view";
                public const string Create = "user:create";
                public const string Edit = "user:edit";
                public const string Delete = "user:delete";
                public const string Search = "user:search";
                public const string Export = "user:export";
            }

            public static class Roles
            {
                public const string View = "role:view";
                public const string Create = "role:create";
                public const string Edit = "role:edit";
                public const string Delete = "role:delete";
                public const string Search = "role:search";
            }

            public static class Security
            {
                public const string VerifyEmail = "security:verify_email";
                public const string ApiAccess = "security:api_access";
            }
        }
        
        public static class Role
        {
            public static class Admin
            {
                public const string Name = "admin";
                public const string Description = "Admin Role.";
            }

            public static class Customer
            {
                public const string Name = "customer";
                public const string Description = "Customer Role.";
            }
        }
        
    }
}