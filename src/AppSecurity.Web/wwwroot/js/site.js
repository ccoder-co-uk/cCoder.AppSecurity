window.AppSecuritySite = {
  async initialise() {
    await this.refreshHealth();
    AppSecurityRoles.initialise();
    AppSecurityUsers.initialise();
    AppSecurityUserRoles.initialise();
    AppSecurityPrivileges.initialise();
  },

  async refreshHealth() {
    try {
      const health = await AppSecurityApi.get('/Health');
      $('#health-status').text(health);
    } catch (error) {
      $('#health-status').text('Unavailable');
    }
  },

  async run(resultSelector, operation) {
    const result = $(resultSelector);
    result.text('Working');

    try {
      const value = await operation();
      result.text(AppSecurityApi.format(value));
      return value;
    } catch (error) {
      result.text(error.message);
      return null;
    }
  },

  renderTable(selector, rows, columns, rowSelected) {
    const table = $(selector);
    table.empty();

    const header = $('<thead>');
    const headerRow = $('<tr>');
    columns.forEach(column => headerRow.append($('<th>').text(column)));
    header.append(headerRow);

    const body = $('<tbody>');
    (rows || []).forEach(row => {
      const bodyRow = $('<tr>');

      columns.forEach(column => {
        const value = this.readProperty(row, column);
        bodyRow.append($('<td>').text(value ?? ''));
      });

      if (rowSelected) {
        bodyRow.on('click', () => rowSelected(row));
      }

      body.append(bodyRow);
    });

    table.append(header);
    table.append(body);
  },

  readProperty(row, property) {
    if (!row) {
      return null;
    }

    const pascalProperty = property.charAt(0).toUpperCase() + property.slice(1);
    return row[property] ?? row[pascalProperty];
  }
};

$(function () {
  AppSecuritySite.initialise();
});
