// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataPrivilege = cCoder.Data.Models.Security.Privilege;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IPrivilegeBroker = cCoder.AppSecurity.Brokers.IPrivilegeBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class PrivilegeServiceTests
{
    private readonly Mock<IPrivilegeBroker> privilegeBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly PrivilegeService privilegeService;

    public PrivilegeServiceTests()
    {
        privilegeBrokerMock = new Mock<IPrivilegeBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        privilegeService = new PrivilegeService(
privilegeBroker: privilegeBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static Privilege CreateRandomPrivilege(string id = null)
    {

        Privilege privilege = Builder<Privilege>
            .CreateNew()
            .With(func: x => x.Id = id ?? $"privilege-{Guid.NewGuid():N}")
            .With(func: x => x.Type = "page")
            .With(func: x => x.Operation = "read")
            .With(func: x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(func: x => x.PortalAdminsOnly = false)
            .Build();

        return privilege;
    }

    private static DataPrivilege ToExternalPrivilege(Privilege item) =>
        item == null
            ? null
            : new DataPrivilege
            {
                Id = item.Id,
                Type = item.Type,
                Operation = item.Operation,
                Description = item.Description,
                PortalAdminsOnly = item.PortalAdminsOnly,
            };
}