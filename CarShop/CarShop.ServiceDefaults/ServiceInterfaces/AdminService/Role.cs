namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService;

public static class Role
{
    public static class Admin
    {
        public static class Car
        {
            public const string Add = "admin.car.add";
            public const string Edit = "admin.car.edit";
            public const string Delete = "admin.car.delete";
        }

        public static class Account
        {
            public const string Create = "admin.account.create";

            public static class ChangePassword
            {
                public const string Own = "admin.account.change-password.own";
                public const string Other = "admin.account.change-password.other";
            }

            public static class Ban
            {
                public const string Own = "admin.account.ban.own";
                public const string Other = "admin.account.ban.other";
            }

            public static class Role
            {
                public const string Give = "admin.account.role.give";
                public const string Take = "admin.account.role.take";
            }

            public const string Unban = "admin.account.unban";
        }
    }
}
