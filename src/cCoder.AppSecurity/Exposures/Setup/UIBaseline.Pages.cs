using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Pages => new()
    {
        Name = "App Security Pages",
        Category = "Account",
        Description = "App Security Pages.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "ResetPassword",
  "Name": "Reset Password",
  "ShowOnMenus": false,
  "Order": 5,
  "LastUpdated": "2024-04-04T15:46:54.0679935+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[PasswordReset]]"
    },
    {
      "CultureId": "en-GB",
      "Name": "body",
      "Html": " [component[PasswordReset]]\n                    "
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Reset Password",
      "Keywords": "password, reset",
      "Title": "Reset Password"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Login",
  "Name": "Login",
  "ShowOnMenus": false,
  "Order": 0,
  "LastUpdated": "2024-04-04T15:47:14.0500973+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[Login]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Login",
      "Keywords": "Login",
      "Title": "Login"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/SSOAdmin",
  "Name": "SSO Admin",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-11-26T16:37:33.6775748+00:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": " [component[SSORoleManagement]]\n                    "
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "SSO Admin",
      "Keywords": "SSO Admin",
      "Title": "SSO Admin"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "AcceptInvite",
  "Name": "Accept Invite",
  "ResourceKey": "",
  "ShowOnMenus": false,
  "Order": 0,
  "LastUpdated": "2026-07-06T00:00:00+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[AcceptInvite]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Accept Invite",
      "Keywords": "invite, account",
      "Title": "Accept Invite"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/UserInvitations",
  "Name": "User Invitations",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 20,
  "LastUpdated": "2026-07-06T00:00:00+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[UserInvitations]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "User invitation administration",
      "Keywords": "invite, account, user",
      "Title": "User Invitations"
    }
  ]
}
"""
            },
        ]
    };
}