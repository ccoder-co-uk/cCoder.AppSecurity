window.AppSecurityApi = {
  async request(method, url, body) {
    const options = {
      method,
      credentials: 'same-origin',
      headers: {}
    };

    if (body !== undefined && body !== null) {
      options.headers['Content-Type'] = 'application/json';
      options.body = JSON.stringify(body);
    }

    const response = await fetch(url, options);
    const text = await response.text();
    const contentType = response.headers.get('content-type') || '';
    const payload = text && contentType.includes('application/json')
      ? JSON.parse(text)
      : text;

    if (!response.ok) {
      const message = typeof payload === 'object'
        ? JSON.stringify(payload, null, 2)
        : payload;

      throw new Error(message || `${response.status} ${response.statusText}`);
    }

    return payload;
  },

  get(url) {
    return this.request('GET', url);
  },

  post(url, body) {
    return this.request('POST', url, body);
  },

  put(url, body) {
    return this.request('PUT', url, body);
  },

  delete(url) {
    return this.request('DELETE', url);
  },

  list(url) {
    return this.get(url).then(result => result && result.value ? result.value : result);
  },

  format(value) {
    return typeof value === 'string'
      ? value
      : JSON.stringify(value, null, 2);
  },

  stringKey(value) {
    return `('${encodeURIComponent(value)}')`;
  },

  guidKey(value) {
    return `(${value})`;
  },

  compositeUserRoleKey(roleId, userId) {
    return `(RoleId=${roleId},UserId='${encodeURIComponent(userId)}')`;
  }
};
