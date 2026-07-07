using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Templates => new()
    {
        Name = "AppSecurity Templates",
        Category = "AppSecurity",
        Description = "AppSecurity Templates.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "ForgotPassword",
  "ResourceKey": "ForgotPassword",
  "RawString": "<html style=\"font-family: [theme[font.family]]; width:800px; margin:0 auto; padding:0;\">\n    <head>\n        <title>[email[subject]]</title>\n        <style>\n            * { font-size: [theme[font.size]]; font-family: [theme[font.family]]; color: #1F2933; }\n            a { color: [theme[colours.links]]; cursor: pointer; }\n            hr { border-top: [theme[border.style]]; }\n        </style>\n    </head>\n    <body style=\"width: 800px; margin: 20px auto; padding: 0; background: white;\">\n        <header style=\"padding: 20px 30px 0;\">\n            <a href=\"[app[root]]\" style=\"font-size: 28px; font-weight: 700; text-decoration: none; color: [theme[colours.primary]];\">cCoder</a>\n            <h2 style=\"background: [theme[colours.primary]]; color: [theme[colours.text2]]; padding: 12px 16px; font-size: 140%; margin-top: 16px;\">Reset your password</h2>\n        </header>\n        <div style=\"margin: 10px auto; padding: 5px 40px 30px;\">\n            <p>[resource_displayname[ForgotPasswordBody]]</p><p>[resource_displayname[PleaseClick]] <a href=\"[app[root]]/ResetPassword?token=[model[EncodedToken]]&uid=[model[CoreUser.Id]]\">[resource_displayname[Here]]</a> [resource_displayname[ToReset]]</p>\n        </div>\n        <div style=\"background-color: [theme[colours.primary]]; color: [theme[colours.text2]]; width: 100%;\">\n            <p style=\"padding: 10px; text-align: right; background: [theme[colours.primary]]; color: [theme[colours.text2]]; margin: 0;\">&copy; 2026, cCoder</p>\n        </div>\n    </body>\n</html>",
  "LastUpdated": "2024-06-12T15:21:02.2427236+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "UserInvite",
  "RawString": "<html style=\"font-family: [theme[font.family]]; width:800px; margin:0 auto; padding:0;\">\n    <head>\n        <title>[email[subject]]</title>\n        <style>\n            * { font-size: [theme[font.size]]; font-family: [theme[font.family]]; color: #1F2933; }\n            a { color: [theme[colours.links]]; cursor: pointer; }\n            hr { border-top: [theme[border.style]]; }\n        </style>\n    </head>\n    <body style=\"width: 800px; margin: 20px auto; padding: 0; background: white;\">\n        <header style=\"padding: 20px 30px 0;\">\n            <a href=\"[app[root]]\" style=\"font-size: 28px; font-weight: 700; text-decoration: none; color: [theme[colours.primary]];\">cCoder</a>\n            <h2 style=\"background: [theme[colours.primary]]; color: [theme[colours.text2]]; padding: 12px 16px; font-size: 140%; margin-top: 16px;\">You have been invited</h2>\n        </header>\n        <div style=\"margin: 10px auto; padding: 5px 40px 30px;\">\n            <p>[resource_description[InvitationStatement]]</p><p>[resource_displayname[Click]] <a href=\"[app[root]]/AcceptInvite?user=[model[SSOUser.Id]]&e=[model[CoreUser.Email]]&t=[model[EncodedToken]]\">[resource_displayname[Here]]</a> to complete your account setup and sign in.</p>\n        </div>\n        <div style=\"background-color: [theme[colours.primary]]; color: [theme[colours.text2]]; width: 100%;\">\n            <p style=\"padding: 10px; text-align: right; background: [theme[colours.primary]]; color: [theme[colours.text2]]; margin: 0;\">&copy; 2026, cCoder</p>\n        </div>\n    </body>\n</html>",
  "LastUpdated": "2022-11-04T11:17:52.8650502+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "ConfirmRegistration",
  "ResourceKey": "Register",
  "RawString": "<html style=\"font-family: [theme[font.family]]; width:800px; margin:0 auto; padding:0;\">\n    <head>\n        <title>[email[subject]]</title>\n        <style>\n            * { font-size: [theme[font.size]]; font-family: [theme[font.family]]; color: #1F2933; }\n            a { color: [theme[colours.links]]; cursor: pointer; }\n            hr { border-top: [theme[border.style]]; }\n        </style>\n    </head>\n    <body style=\"width: 800px; margin: 20px auto; padding: 0; background: white;\">\n        <header style=\"padding: 20px 30px 0;\">\n            <a href=\"[app[root]]\" style=\"font-size: 28px; font-weight: 700; text-decoration: none; color: [theme[colours.primary]];\">cCoder</a>\n            <h2 style=\"background: [theme[colours.primary]]; color: [theme[colours.text2]]; padding: 12px 16px; font-size: 140%; margin-top: 16px;\">Confirm your registration</h2>\n        </header>\n        <div style=\"margin: 10px auto; padding: 5px 40px 30px;\">\n            <p>[resource_displayname[Greeting]] [model[CoreUser.DisplayName]]</p><p>[resource_displayname[Body]]</p><p>[resource_displayname[Click]] <a href=\"[app[root]]/MyRegistrations?u=[model[SSOUser.Id]]&t=[model[Token]]\">[resource_displayname[Here]]</a> [resource_displayname[ToConfirm]]</p>\n        </div>\n        <div style=\"background-color: [theme[colours.primary]]; color: [theme[colours.text2]]; width: 100%;\">\n            <p style=\"padding: 10px; text-align: right; background: [theme[colours.primary]]; color: [theme[colours.text2]]; margin: 0;\">&copy; 2026, cCoder</p>\n        </div>\n    </body>\n</html>",
  "LastUpdated": "2022-11-14T12:20:33.4463482+00:00"
}
"""
            }
        ]
    };
}