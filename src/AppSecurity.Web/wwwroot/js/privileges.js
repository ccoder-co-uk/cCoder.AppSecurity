window.AppSecurityPrivileges = {
  baseUrl: '/Api/Core/Privilege',

  initialise() {
    $('#privilege-refresh-button').on('click', () => this.refresh());
    this.refresh();
  },

  async refresh() {
    await AppSecuritySite.run('#privilege-result', async () => {
      const privileges = await AppSecurityApi.list(`${this.baseUrl}?$top=300&$orderby=Id`);
      AppSecuritySite.renderTable('#privilege-table', privileges, ['id', 'type', 'name', 'description']);
      return privileges;
    });
  }
};
