window.AppSecurityUserRoles = {
  baseUrl: '/Api/Core/UserRole',

  initialise() {
    $('#user-role-refresh-button').on('click', () => this.refresh());
    $('#user-role-create-button').on('click', () => this.create());
    $('#user-role-delete-button').on('click', () => this.delete());
    this.refresh();
  },

  readForm() {
    return {
      userId: $('#user-role-user-id').val(),
      roleId: $('#user-role-role-id').val()
    };
  },

  writeForm(userRole) {
    $('#user-role-user-id').val(userRole.userId || userRole.UserId || '');
    $('#user-role-role-id').val(userRole.roleId || userRole.RoleId || '');
  },

  async refresh() {
    await AppSecuritySite.run('#user-role-result', async () => {
      const userRoles = await AppSecurityApi.list(`${this.baseUrl}?$top=100`);
      AppSecuritySite.renderTable('#user-role-table', userRoles, ['userId', 'roleId'], userRole => this.writeForm(userRole));
      return userRoles;
    });
  },

  async create() {
    await AppSecuritySite.run('#user-role-result', async () => {
      const userRole = await AppSecurityApi.post(this.baseUrl, this.readForm());
      this.writeForm(userRole);
      await this.refresh();
      return userRole;
    });
  },

  async delete() {
    await AppSecuritySite.run('#user-role-result', async () => {
      const userRole = this.readForm();
      const result = await AppSecurityApi.delete(
        `${this.baseUrl}${AppSecurityApi.compositeUserRoleKey(userRole.roleId, userRole.userId)}`);
      await this.refresh();
      return result || 'Deleted';
    });
  }
};
