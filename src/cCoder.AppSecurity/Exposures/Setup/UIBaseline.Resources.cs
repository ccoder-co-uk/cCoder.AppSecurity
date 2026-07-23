// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Resources => new()
    {
        Name = "AppSecurity Resources",
        Category = "AppSecurity",
        Description = "AppSecurity Resources.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "remove",
  "DisplayName": "Remove",
  "ShortDisplayName": "Remove",
  "Description": "Remove",
  "LastUpdated": "2022-03-18T10:41:54.1891378+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "newrole",
  "DisplayName": "New Role",
  "ShortDisplayName": "New Role",
  "Description": "New Role",
  "LastUpdated": "2022-03-18T10:41:54.1891472+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "name",
  "DisplayName": "Nom et Prénom",
  "ShortDisplayName": "Nom et Prénom",
  "Description": "Nom et Prénom",
  "LastUpdated": "2022-03-18T10:41:54.189239+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "passwordrules",
  "DisplayName": "Les mots de passe doivent avoir une longueur minimale de 6 caractères et contenir au moins 1 lettre majuscule, au moins 1 lettre minuscule, au moins 1 chiffre, au moins 1 caractère non alphanumérique",
  "ShortDisplayName": "Les mots de passe doivent avoir une longueur minimale de 6 caractères et contenir au moins 1 lettre majuscule, au moins 1 lettre minuscule, au moins 1 chiffre, au moins 1 caractère non alphanumérique",
  "Description": "Les mots de passe doivent avoir une longueur minimale de 6 caractères et contenir au moins 1 lettre majuscule, au moins 1 lettre minuscule, au moins 1 chiffre, au moins 1 caractère non alphanumérique",
  "LastUpdated": "2022-03-18T10:41:54.1892433+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "passwordrulesheading",
  "DisplayName": "Password Rules",
  "ShortDisplayName": "Password Rules",
  "Description": "Password Rules",
  "LastUpdated": "2022-03-18T10:41:54.1892713+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "passwordresetemailtitle",
  "DisplayName": "Password Reset Email",
  "ShortDisplayName": "Password Reset Email",
  "Description": "Password Reset Email",
  "LastUpdated": "2022-03-18T10:41:54.1892757+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "passwordrules",
  "DisplayName": "Password Rules",
  "ShortDisplayName": "Password Rules",
  "Description": "Password Rules",
  "LastUpdated": "2022-03-18T10:41:54.1893124+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "resetfailed",
  "DisplayName": "Échec de la réinitialisation",
  "ShortDisplayName": "Échec de la réinitialisation",
  "Description": "Échec de la réinitialisation",
  "LastUpdated": "2022-03-18T10:41:54.1893183+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "login",
  "DisplayName": "Login",
  "ShortDisplayName": "Login",
  "Description": "Login",
  "LastUpdated": "2022-03-18T10:41:54.1893228+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "passworddoesmeetrequirements",
  "DisplayName": "Password **DOES** meet requirements",
  "ShortDisplayName": "Password DOES meet requirements",
  "Description": "Password DOES meet requirements",
  "LastUpdated": "2022-03-18T10:41:54.1893273+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "passwordmeetsrequirements",
  "DisplayName": "Password meets requirements",
  "ShortDisplayName": "Password meets requirements",
  "Description": "Password meets requirements",
  "LastUpdated": "2022-03-18T10:41:54.1893317+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "deletethisapptitle",
  "DisplayName": "Delete This App",
  "ShortDisplayName": "Delete This App",
  "Description": "Delete This App",
  "LastUpdated": "2022-03-18T10:41:54.1893361+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "new",
  "DisplayName": "New",
  "ShortDisplayName": "New",
  "Description": "New",
  "LastUpdated": "2022-03-18T10:41:54.1893404+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "environment",
  "DisplayName": "Environment",
  "ShortDisplayName": "Environment",
  "Description": "Environment",
  "LastUpdated": "2022-03-18T10:41:54.1893448+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "createapptitle",
  "DisplayName": "Create App",
  "ShortDisplayName": "Create App",
  "Description": "Create App",
  "LastUpdated": "2022-03-18T10:41:54.1893491+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "passwordrulescontent",
  "DisplayName": "<div class=\"value\">          <ul>             <li>Must be at least 6 characters</li>             <li>At least 1 digit</li>             <li>At least 1 capital letter</li>             <li>At least 1 lowercase letter</li>             <li>At least 1 special character</li>            </ul>       \t</div>",
  "ShortDisplayName": "<div class=\"value\">          <ul>             <li>Must be at least 6 characters</li>             <li>At least 1 digit</li>             <li>At least 1 capital letter</li>             <li>At least 1 lowercase letter</li>             <li>At least 1 special character</li>            </ul>       \t</div>",
  "Description": "<div class=\"value\">          <ul>             <li>Must be at least 6 characters</li>             <li>At least 1 digit</li>             <li>At least 1 capital letter</li>             <li>At least 1 lowercase letter</li>             <li>At least 1 special character</li>            </ul>       \t</div>",
  "LastUpdated": "2022-03-18T10:41:54.1893727+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "login",
  "DisplayName": "Se connecter",
  "ShortDisplayName": "Se connecter",
  "Description": "Se connecter",
  "LastUpdated": "2022-03-18T10:41:54.189377+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "passwordsdontmatch",
  "DisplayName": "Passwords do not match",
  "ShortDisplayName": "Passwords do not match",
  "Description": "Passwords do not match",
  "LastUpdated": "2022-03-18T10:41:54.1893915+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "confirmpassword",
  "DisplayName": "Confirm Password",
  "ShortDisplayName": "Confirm Password",
  "Description": "confirmpassword",
  "LastUpdated": "2022-03-18T10:41:54.189396+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "logout",
  "DisplayName": "Logout",
  "ShortDisplayName": "Logout",
  "Description": "Logout",
  "LastUpdated": "2022-03-18T10:41:54.1894003+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "phone",
  "DisplayName": "Phone number",
  "ShortDisplayName": "Phone number",
  "Description": "Phone number",
  "LastUpdated": "2022-03-18T10:41:54.1894047+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "name",
  "DisplayName": "Name",
  "ShortDisplayName": "Name",
  "Description": "Name",
  "LastUpdated": "2022-03-18T10:41:54.1894091+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "logout",
  "DisplayName": "Se déconnecter",
  "ShortDisplayName": "Se déconnecter",
  "Description": "Se déconnecter",
  "LastUpdated": "2022-03-18T10:41:54.1894134+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "confirmationemailsentto",
  "DisplayName": "Un e-mail de confirmation a été envoyé à",
  "ShortDisplayName": "Un e-mail de confirmation a été envoyé à",
  "Description": "Un e-mail de confirmation a été envoyé à",
  "LastUpdated": "2022-03-18T10:41:54.1894326+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "passwordsdontmatch",
  "DisplayName": "Pas de correspondance des mots de passe",
  "ShortDisplayName": "Pas de correspondance des mots de passe",
  "Description": "Pas de correspondance des mots de passe",
  "LastUpdated": "2022-03-18T10:41:54.189437+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "confirmationemailsentto",
  "DisplayName": "A confirmation email has been sent to",
  "ShortDisplayName": "A confirmation email has been sent to",
  "Description": "A confirmation email has been sent to",
  "LastUpdated": "2022-03-18T10:41:54.1894414+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "pleaseresolvevalidationissues",
  "DisplayName": "Please resolve the validation issues",
  "ShortDisplayName": "Please resolve the validation issues",
  "Description": "Please resolve the validation issues",
  "LastUpdated": "2022-03-18T10:41:54.1894458+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "passwordrulesheading",
  "DisplayName": "Règles pour le mot de passe",
  "ShortDisplayName": "Règles pour le mot de passe",
  "Description": "Règles pour le mot de passe",
  "LastUpdated": "2022-03-18T10:41:54.1894502+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "passwordrequirementsline3",
  "DisplayName": "Au moins 1 lettre majuscule",
  "ShortDisplayName": "Au moins 1 lettre majuscule",
  "Description": "Au moins 1 lettre majuscule",
  "LastUpdated": "2022-03-18T10:41:54.1894604+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "confirmpassword",
  "DisplayName": "Confirmez le mot de passe",
  "ShortDisplayName": "Confirmez le mot de passe",
  "Description": "Confirmez le mot de passe",
  "LastUpdated": "2022-03-18T10:41:54.1894778+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "pleaseresolvevalidationissues",
  "DisplayName": "Veuillez résoudre l'(es) anomalie(s) pour la validation ",
  "ShortDisplayName": "Veuillez résoudre l'(es) anomalie(s) pour la validation",
  "Description": "Veuillez résoudre l'(es) anomalie(s) pour la validation",
  "LastUpdated": "2022-03-18T10:41:54.1895421+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "passworddoesmeetrequirements",
  "DisplayName": "Le mot de passe répond aux exigences",
  "ShortDisplayName": "Le mot de passe répond aux exigences",
  "Description": "Le mot de passe répond aux exigences",
  "LastUpdated": "2022-03-18T10:41:54.1895612+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "passwordmeetsrequirements",
  "DisplayName": "Le mot de passe répond aux exigences",
  "ShortDisplayName": "Le mot de passe répond aux exigences",
  "Description": "Le mot de passe répond aux exigences",
  "LastUpdated": "2022-03-18T10:41:54.1895656+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "passwordresetemailtitle",
  "DisplayName": "E-mail de réinitialisation du mot de passe",
  "ShortDisplayName": "E-mail de réinitialisation du mot de passe",
  "Description": "E-mail de réinitialisation du mot de passe",
  "LastUpdated": "2022-03-18T10:41:54.18957+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "phone",
  "DisplayName": "Numéro de téléphone",
  "ShortDisplayName": "Numéro de téléphone",
  "Description": "Numéro de téléphone",
  "LastUpdated": "2022-03-18T10:41:54.1895743+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "passwordrequirementsline3",
  "DisplayName": "At least 1 capital letter",
  "ShortDisplayName": "At least 1 capital letter",
  "Description": "At least 1 capital letter",
  "LastUpdated": "2022-03-18T10:41:54.189583+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "resetfailed",
  "DisplayName": "Reset Failed",
  "ShortDisplayName": "Reset Failed",
  "Description": "Reset Failed",
  "LastUpdated": "2022-03-18T10:41:54.1898141+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr",
  "Key": "AppSecurity",
  "Name": "tacmessage",
  "DisplayName": "Assurez vous d'avoir lu nos conditions d'utilisation",
  "ShortDisplayName": "Assurez vous d'avoir lu nos conditions d'utilisation",
  "Description": "Assurez vous d'avoir lu nos conditions d'utilisation",
  "LastUpdated": "2022-03-18T10:41:54.1898272+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "DPPTACNotice",
  "DisplayName": "DPPTACNotice",
  "ShortDisplayName": "DPPTACNotice",
  "Description": "By checking these boxes you are acknowledging that you have read, understood, and accept our Terms & Conditions and Data Protection Policy, and that you consent to our use of your personal data as described in our Data Protection Policy (required to register).",
  "LastUpdated": "2022-03-18T10:41:54.1898332+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "ourtac",
  "DisplayName": "<a href='https://[app[domain]]/Documentation'>Platform documentation</a>",
  "ShortDisplayName": "Platform documentation",
  "Description": "Platform documentation",
  "LastUpdated": "2022-03-18T10:41:54.1898376+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "ourdpp",
  "DisplayName": "<a href='https://[app[domain]]/Documentation'>Platform documentation</a>",
  "ShortDisplayName": "Platform documentation",
  "Description": "Platform documentation",
  "LastUpdated": "2022-03-18T10:41:54.189842+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "tacmessage",
  "DisplayName": "Please make sure you have read and checked Terms & Conditions.",
  "ShortDisplayName": "Please make sure you have read and checked Terms & Conditions.",
  "Description": "Please make sure you have read and checked Terms & Conditions.",
  "LastUpdated": "2022-03-18T10:41:54.1898464+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "dppmessage",
  "DisplayName": "Please make sure you have read and checked Data Protection Policy.",
  "ShortDisplayName": "Please make sure you have read and checked Data Protection Policy.",
  "Description": "Please make sure you have read and checked Data Protection Policy.",
  "LastUpdated": "2022-03-18T10:41:54.1898508+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr",
  "Key": "AppSecurity",
  "Name": "DPPTACNotice",
  "DisplayName": "DPPTACNotice",
  "ShortDisplayName": "DPPTACNotice",
  "Description": "En cliquant, dans ces 2 espaces, vous acceptez avoir lu, compris et accepté nos conditions d'utilisation ainsi que notre politique de protection des données et que consentez à l'utilisation de vos données personnelles telles que décrites dans le document de Politique de Protection des Données\"",
  "LastUpdated": "2022-03-18T10:41:54.1898553+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "notice",
  "DisplayName": "Notice",
  "ShortDisplayName": "Notice",
  "Description": "Notice",
  "LastUpdated": "2022-03-18T10:41:54.1898597+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr",
  "Key": "AppSecurity",
  "Name": "ourtac",
  "DisplayName": "<a href='https://[app[domain]]/Documentation'>Documentation de la plateforme</a>",
  "ShortDisplayName": "Documentation de la plateforme",
  "Description": "Documentation de la plateforme",
  "LastUpdated": "2022-03-18T10:41:54.1898656+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr",
  "Key": "AppSecurity",
  "Name": "ourdpp",
  "DisplayName": "<a href='https://[app[domain]]/Documentation'>Documentation de la plateforme</a>",
  "ShortDisplayName": "Documentation de la plateforme",
  "Description": "Documentation de la plateforme",
  "LastUpdated": "2022-03-18T10:41:54.1898702+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr",
  "Key": "AppSecurity",
  "Name": "dppmessage",
  "DisplayName": "Assurez-vous d'avoir lu notre politique de protection des données",
  "ShortDisplayName": "Assurez-vous d'avoir lu notre politique de protection des données",
  "Description": "Assurez-vous d'avoir lu notre politique de protection des données",
  "LastUpdated": "2022-03-18T10:41:54.1898747+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "furtherinformationvisit",
  "DisplayName": "Open documentation",
  "ShortDisplayName": "Open documentation",
  "Description": "Open documentation",
  "LastUpdated": "2022-03-18T10:41:54.1898836+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "newrole",
  "DisplayName": "New Role",
  "ShortDisplayName": "New Role",
  "Description": "New Role",
  "LastUpdated": "2022-03-18T10:41:54.189888+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "passwordrulescontent",
  "DisplayName": "<div class=\"value\">           \t<ul> \t             <li>La longueur minimale est de 6 caractères</li> \t             <li>Au moins 1 chiffre</li> \t             <li>Au moins 1 lettre majuscule</li> \t             <li>Au moins 1 lettre minuscule</li> \t             <li>Au moins 1 caractère spécial non alphanumérique</li> \t</ul> </div>",
  "ShortDisplayName": "<div class=\"value\">           \t<ul> \t             <li>La longueur minimale est de 6 caractères</li> \t             <li>Au moins 1 chiffre</li> \t             <li>Au moins 1 lettre majuscule</li> \t             <li>Au moins 1 lettre minuscule</li> \t             <li>Au moins 1 caractère spécial non alphanumérique</li> \t</ul> </div>",
  "Description": "<div class=\"value\">           \t<ul> \t             <li>La longueur minimale est de 6 caractères</li> \t             <li>Au moins 1 chiffre</li> \t             <li>Au moins 1 lettre majuscule</li> \t             <li>Au moins 1 lettre minuscule</li> \t             <li>Au moins 1 caractère spécial non alphanumérique</li> \t</ul> </div>",
  "LastUpdated": "2022-03-18T10:41:54.1899326+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "furtherinformationlogin",
  "DisplayName": "Want to learn more about the platform? Open the documentation below.",
  "ShortDisplayName": "Want to learn more about the platform? Open the documentation below.",
  "Description": "Want to learn more about the platform? Open the documentation below.",
  "LastUpdated": "2022-03-18T10:41:54.1899619+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "furtherinformationlogin",
  "DisplayName": "Souhaitez-vous en savoir plus sur la plateforme ? Ouvrez la documentation ci-dessous.",
  "ShortDisplayName": "Souhaitez-vous en savoir plus sur la plateforme ? Ouvrez la documentation ci-dessous.",
  "Description": "Souhaitez-vous en savoir plus sur la plateforme ? Ouvrez la documentation ci-dessous.",
  "LastUpdated": "2022-03-18T10:41:54.1906461+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "AppSecurity",
  "Name": "furtherinformationvisit",
  "DisplayName": "Ouvrir la documentation",
  "ShortDisplayName": "Ouvrir la documentation",
  "Description": "Ouvrir la documentation",
  "LastUpdated": "2022-03-18T10:41:54.1906562+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "AppSecurity",
  "Name": "Account",
  "DisplayName": "Select a user to add...",
  "ShortDisplayName": "Select a user to add...",
  "Description": "Select a user to add...",
  "LastUpdated": "2022-03-18T10:41:54.1905129+00:00"
}
"""
            }
        ]
    };
}