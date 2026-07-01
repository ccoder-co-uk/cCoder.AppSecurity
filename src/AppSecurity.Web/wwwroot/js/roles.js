window.AppSecurityRoles = {
  baseUrl: '/Api/Core/Role',

  initialise() {
    $('#role-refresh-button').on('click', () => this.refresh());
    $('#role-create-button').on('click', () => this.create());
    $('#role-update-button').on('click', () => this.update());
    $('#role-get-button').on('click', () => this.get());
    $('#role-delete-button').on('click', () => this.delete());
    this.refresh();
  },

  readForm() {
    return {
      id: $('#role-id').val() || crypto.randomUUID(),
      appId: Number($('#role-app-id').val()),
      name: $('#role-name').val(),
      description: $('#role-description').val(),
      privs: $('#role-privs').val()
    };
  },

  writeForm(role) {
    $('#role-id').val(role.id || role.Id || '');
    $('#role-app-id').val(role.appId || role.AppId || '');
    $('#role-name').val(role.name || role.Name || '');
    $('#role-description').val(role.description || role.Description || '');
    $('#role-privs').val(role.privs || role.Privs || '');
  },

  async refresh() {
    await AppSecuritySite.run('#role-result', async () => {
      const roles = await AppSecurityApi.list(`${this.baseUrl}?$top=100&$orderby=Name`);
      AppSecuritySite.renderTable('#role-table', roles, ['id', 'appId', 'name', 'privs'], role => this.writeForm(role));
      return roles;
    });
  },

  async create() {
    await AppSecuritySite.run('#role-result', async () => {
      const role = await AppSecurityApi.post(this.baseUrl, this.readForm());
      this.writeForm(role);
      await this.refresh();
      return role;
    });
  },

  async update() {
    await AppSecuritySite.run('#role-result', async () => {
      const role = this.readForm();
      const result = await AppSecurityApi.put(`${this.baseUrl}${AppSecurityApi.guidKey(role.id)}`, role);
      this.writeForm(result);
      await this.refresh();
      return result;
    });
  },

  async get() {
    await AppSecuritySite.run('#role-result', async () => {
      const role = await AppSecurityApi.get(`${this.baseUrl}${AppSecurityApi.guidKey($('#role-id').val())}`);
      this.writeForm(role);
      return role;
    });
  },

  async delete() {
    await AppSecuritySite.run('#role-result', async () => {
      const result = await AppSecurityApi.delete(`${this.baseUrl}${AppSecurityApi.guidKey($('#role-id').val())}`);
      await this.refresh();
      return result || 'Deleted';
    });
  }
};
