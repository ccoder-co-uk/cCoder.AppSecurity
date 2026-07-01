window.AppSecurityUsers = {
  baseUrl: '/Api/Core/User',

  initialise() {
    $('#user-refresh-button').on('click', () => this.refresh());
    $('#user-create-button').on('click', () => this.create());
    $('#user-update-button').on('click', () => this.update());
    $('#user-get-button').on('click', () => this.get());
    $('#user-delete-button').on('click', () => this.delete());
    $('#user-me-button').on('click', () => this.me());
    this.refresh();
  },

  readForm() {
    return {
      id: $('#user-id').val(),
      displayName: $('#user-display-name').val(),
      email: $('#user-email').val(),
      defaultCultureId: $('#user-culture').val(),
      isActive: $('#user-active').is(':checked')
    };
  },

  writeForm(user) {
    $('#user-id').val(user.id || user.Id || '');
    $('#user-display-name').val(user.displayName || user.DisplayName || '');
    $('#user-email').val(user.email || user.Email || '');
    $('#user-culture').val(user.defaultCultureId || user.DefaultCultureId || '');
    $('#user-active').prop('checked', user.isActive ?? user.IsActive ?? false);
  },

  async refresh() {
    await AppSecuritySite.run('#user-result', async () => {
      const users = await AppSecurityApi.list(`${this.baseUrl}?$top=100&$orderby=DisplayName`);
      AppSecuritySite.renderTable('#user-table', users, ['id', 'displayName', 'email', 'isActive'], user => this.writeForm(user));
      return users;
    });
  },

  async create() {
    await AppSecuritySite.run('#user-result', async () => {
      const user = await AppSecurityApi.post(this.baseUrl, this.readForm());
      this.writeForm(user);
      await this.refresh();
      return user;
    });
  },

  async update() {
    await AppSecuritySite.run('#user-result', async () => {
      const user = this.readForm();
      const result = await AppSecurityApi.put(`${this.baseUrl}${AppSecurityApi.stringKey(user.id)}`, user);
      this.writeForm(result);
      await this.refresh();
      return result;
    });
  },

  async get() {
    await AppSecuritySite.run('#user-result', async () => {
      const user = await AppSecurityApi.get(`${this.baseUrl}${AppSecurityApi.stringKey($('#user-id').val())}`);
      this.writeForm(user);
      return user;
    });
  },

  async delete() {
    await AppSecuritySite.run('#user-result', async () => {
      const result = await AppSecurityApi.delete(`${this.baseUrl}${AppSecurityApi.stringKey($('#user-id').val())}`);
      await this.refresh();
      return result || 'Deleted';
    });
  },

  async me() {
    await AppSecuritySite.run('#user-result', async () => {
      const user = await AppSecurityApi.get(`${this.baseUrl}/Me`);
      this.writeForm(user);
      return user;
    });
  }
};
