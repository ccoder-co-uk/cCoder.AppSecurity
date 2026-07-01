window.AppSecurityGrids = {
    apiRoot: "/Api/Core",
    context: {
        appId: null,
        roleId: null,
        userId: null
    },
    appRows: [],
    roleRows: [],
    userRows: [],
    privilegeRows: [],
    privilegesLoaded: false,
    loadedWorkspaces: {},

    workspaces: [
        {
            name: "App",
            title: "Apps",
            description: "Aggregate roots for app security",
            key: "Id",
            readOnly: true,
            fields: {
                Id: { type: "number", editable: false },
                Name: { type: "string" },
                Domain: { type: "string" },
                TenantId: { type: "string" },
                DefaultCultureId: { type: "string" },
                DefaultTheme: { type: "string" }
            },
            columns: ["Id", "Name", "Domain", "TenantId", "DefaultCultureId", "DefaultTheme"]
        },
        {
            name: "Role",
            title: "Roles",
            description: "Roles owned by the selected App",
            key: "Id",
            keyType: "guid",
            context: { type: "app", field: "AppId" },
            fields: {
                Id: { type: "string" },
                AppId: { type: "number" },
                Name: { type: "string" },
                Description: { type: "string" },
                Privs: { type: "string" }
            },
            columns: ["Id", "AppId", "Name", "Description"]
        },
        {
            name: "User",
            title: "Users",
            description: "App-local users available for role assignment",
            key: "Id",
            keyType: "string",
            fields: {
                Id: { type: "string" },
                DisplayName: { type: "string" },
                Email: { type: "string" },
                DefaultCultureId: { type: "string" },
                IsActive: { type: "boolean" }
            },
            columns: ["Id", "DisplayName", "Email", "DefaultCultureId", "IsActive"]
        },
    ],

    init: function () {
        this.buildWorkspaces();
        this.workspaces
            .forEach(config => this.createGrid(config));
    },

    buildWorkspaces: function () {
        const nav = document.getElementById("workspace-nav");
        const surfaces = document.getElementById("workspace-surfaces");

        this.workspaces.forEach((config, index) => {
            const surfaceId = this.surfaceId(config);
            const button = document.createElement("button");
            button.className = `as-nav-item${index === 0 ? " active" : ""}`;
            button.type = "button";
            button.dataset.workspaceTarget = surfaceId;
            button.innerHTML = `<span class="k-icon k-i-table"></span>${config.title}`;
            button.addEventListener("click", () => this.showSurface(button));
            nav.appendChild(button);

            const section = document.createElement("section");
            section.id = surfaceId;
            section.className =
                `as-surface${config.name === "Role" ? " as-role-surface" : ""}${index === 0 ? " active" : ""}`;
            section.innerHTML = this.gridHtml(config);
            surfaces.appendChild(section);
        });

        document
            .querySelectorAll("[data-context-type='app']")
            .forEach(select => select.addEventListener("change", event => this.setAppContext(event.target.value)));

        document
            .querySelectorAll("[data-context-type='role']")
            .forEach(select => select.addEventListener("change", event => this.setRoleContext(event.target.value)));

        document
            .querySelectorAll("[data-context-type='user']")
            .forEach(select => select.addEventListener("change", event => this.setUserContext(event.target.value)));
    },

    gridHtml: function (config) {
        return `<div class="as-toolbar">` +
            `<div><h2>${config.title}</h2><span>${config.description}</span></div>` +
            this.contextHtml(config) +
            `</div>` +
            `<div id="${this.gridId(config)}" class="as-grid"></div>`;
    },

    contextHtml: function (config) {
        if (!config.context) {
            return "";
        }

        if (config.context.type === "app") {
            return this.selectHtml("App", "app");
        }

        if (config.context.type === "role") {
            return this.selectHtml("Role", "role");
        }

        return this.selectHtml("User", "user");
    },

    selectHtml: function (label, type) {
        return `<label class="as-context">` +
            `<span>${label}</span>` +
            `<select class="form-select form-select-sm ${type}-context" data-context-type="${type}">` +
            `<option value="">Select ${label}</option>` +
            `</select>` +
            `</label>`;
    },

    showSurface: function (button) {
        const target = button.dataset.workspaceTarget;
        const config = this.workspaces.find(workspace => this.surfaceId(workspace) === target);

        document
            .querySelectorAll("[data-workspace-target]")
            .forEach(item => item.classList.toggle("active", item === button));

        document
            .querySelectorAll(".as-surface")
            .forEach(surface => surface.classList.toggle("active", surface.id === target));

        if (config) {
            this.loadWorkspace(config);
        }
    },

    loadWorkspace: function (config) {
        this.loadedWorkspaces[config.name] = true;
        this.refreshGrid(config.name);
    },

    createGrid: function (config) {
        $(`#${this.gridId(config)}`).kendoGrid({
            dataSource: {
                transport: {
                    read: options => this.read(config, options),
                    create: options => this.create(config, options),
                    update: options => this.update(config, options),
                    destroy: options => this.destroy(config, options)
                },
                schema: {
                    model: {
                        id: config.key,
                        fields: this.modelFields(config)
                    }
                },
                pageSize: 20
            },
            toolbar: config.readOnly ? [] : [{ name: "create", text: `Create ${config.title}` }],
            editable: config.readOnly ? false : {
                mode: "popup",
                confirmation: false,
                window: {
                    width: "720px"
                }
            },
            pageable: true,
            sortable: true,
            filterable: true,
            resizable: true,
            reorderable: true,
            scrollable: true,
            selectable: "row",
            autoBind: config.name === "App",
            columns: this.columns(config),
            detailTemplate: config.name === "Role" ? this.roleDetailTemplate() : undefined,
            detailInit: config.name === "Role" ? event => this.onRoleDetailInit(event) : undefined,
            noRecords: true,
            messages: {
                noRecords: this.noRecordsMessage(config)
            },
            change: () => this.onSelectionChanged(config),
            edit: event => this.onEdit(config, event),
            save: () => AppSecurityApi.notify("Saving..."),
            remove: () => AppSecurityApi.notify("Deleting..."),
            dataBound: () => this.onDataBound(config)
        });
    },

    modelFields: function (config) {
        const fields = Object.assign({}, config.fields);

        if (config.key && config.keyType === "guid") {
            fields[config.key] = Object.assign({}, fields[config.key], { editable: false });
        }

        if (config.context) {
            fields[config.context.field] = Object.assign({}, fields[config.context.field], { editable: false });
        }

        return fields;
    },

    columns: function (config) {
        const columns = config.columns.map(field => ({
            field: field,
            title: this.label(field),
            width: this.widthFor(field)
        }));

        if (!config.readOnly) {
            columns.push({
                command: [
                    { name: "edit", text: "Edit" },
                    { name: "destroy", text: "Delete" }
                ],
                title: "Actions",
                width: 180
            });
        }

        return columns;
    },

    read: async function (config, options) {
        try {
            if (config.context && !this.contextValue(config.context.type)) {
                options.success([]);
                return;
            }

            const body = await AppSecurityApi.get(this.readUrl(config));
            const rows = AppSecurityApi.unwrapCollection(body).map(row => this.withRowState(config, row));
            options.success(rows);
        } catch (error) {
            options.error(error);
        }
    },

    readUrl: function (config) {
        let url = `${this.apiRoot}/${config.name}?$top=500`;

        if (config.name === "Role" && this.context.appId) {
            url += `&$filter=${encodeURIComponent(`AppId eq ${this.context.appId}`)}`;
        }

        return url;
    },

    create: async function (config, options) {
        try {
            if (config.context && !this.contextValue(config.context.type)) {
                throw new Error(`Select a ${config.context.type} before creating ${config.title}.`);
            }

            const payload = this.preparePayload(config, options.data, true);
            const result = await AppSecurityApi.post(`${this.apiRoot}/${config.name}`, payload);
            options.success(this.withRowState(config, result ?? payload));
            AppSecurityApi.notify(`${config.title} created`);
        } catch (error) {
            options.error(error);
        }
    },

    update: async function (config, options) {
        try {
            const payload = this.preparePayload(config, options.data, false);
            let result;

            result = await AppSecurityApi.put(
                `${this.apiRoot}/${config.name}(${this.formatKey(config, options.data[config.key])})`,
                payload);

            options.success(this.withRowState(config, result ?? payload));
            AppSecurityApi.notify(`${config.title} updated`);
        } catch (error) {
            options.error(error);
        }
    },

    destroy: async function (config, options) {
        try {
            await AppSecurityApi.delete(
                `${this.apiRoot}/${config.name}(${this.formatKey(config, options.data[config.key])})`);

            options.success(options.data);
            AppSecurityApi.notify(`${config.title} deleted`);
        } catch (error) {
            options.error(error);
        }
    },

    preparePayload: function (config, data, isCreate) {
        const payload = {};

        Object.keys(config.fields).forEach(field => {
            const value = data[field];

            if (value !== undefined) {
                payload[field] = value;
            }
        });

        if (config.keyType === "guid" && isCreate && !payload[config.key]) {
            payload[config.key] = crypto.randomUUID();
        }

        if (config.name === "Role") {
            payload.AppId = Number(this.context.appId);
            payload.Privs = payload.Privs || "";
        }

        if (config.name === "User") {
            payload.IsActive = Boolean(payload.IsActive);
        }

        return payload;
    },

    onEdit: function (config, event) {
        if (config.name === "Role") {
            event.model.set("AppId", Number(this.context.appId));
        }

    },

    onSelectionChanged: function (config) {
        const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
        const row = grid.dataItem(grid.select());

        if (!row) {
            return;
        }

        if (config.name === "App") {
            this.setAppContext(row.Id);
        }

        if (config.name === "Role") {
            this.setRoleContext(row.Id);
            grid.expandRow(grid.select());
        }

        if (config.name === "User") {
            this.setUserContext(row.Id);
        }
    },

    onDataBound: function (config) {
        this.loadedWorkspaces[config.name] = true;

        if (config.name === "App") {
            this.appRows = this.gridRows(config);
            this.refreshAppSelectors();

            if (!this.context.appId && this.appRows.length > 0) {
                this.setAppContext(this.appRows[0].Id, false);
            }
        }

        if (config.name === "Role") {
            this.roleRows = this.gridRows(config);
            this.refreshRoleSelectors();
        }

        if (config.name === "User") {
            this.userRows = this.gridRows(config);
            this.refreshUserSelectors();
        }

        this.updateCreateButtons();
        AppSecurityApi.notify("Ready");
    },

    gridRows: function (config) {
        const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
        return grid?.dataSource?.data()?.toJSON?.() ?? [];
    },

    setAppContext: function (value, refresh = true) {
        const appId = value ? Number(value) : null;

        if (this.context.appId === appId) {
            return;
        }

        this.context.appId = appId;
        this.context.roleId = null;
        this.roleRows = [];
        this.refreshAppSelectors();
        this.refreshRoleSelectors();

        if (refresh) {
            this.refreshLoadedGrid("Role");
        }
    },

    setRoleContext: function (value, refresh = true) {
        const roleId = value || null;

        if (this.context.roleId === roleId) {
            return;
        }

        this.context.roleId = roleId;
        this.refreshRoleSelectors();
    },

    setUserContext: function (value) {
        this.context.userId = value || null;
        this.refreshUserSelectors();
    },

    refreshGrid: function (name) {
        const config = this.workspaces.find(workspace => workspace.name === name);
        const grid = config ? $(`#${this.gridId(config)}`).data("kendoGrid") : null;

        if (grid) {
            grid.dataSource.read();
        }
    },

    refreshLoadedGrid: function (name) {
        if (this.loadedWorkspaces[name]) {
            this.refreshGrid(name);
        }
    },

    refreshAppSelectors: function () {
        this.fillContextSelectors(
            ".app-context",
            this.appRows,
            this.context.appId,
            app => `${app.Id} - ${app.Name ?? app.Domain ?? "App"}`);
    },

    refreshRoleSelectors: function () {
        this.fillContextSelectors(
            ".role-context",
            this.roleRows,
            this.context.roleId,
            role => `${role.Name ?? "Role"} - ${role.Id}`);
    },

    refreshUserSelectors: function () {
        this.fillContextSelectors(
            ".user-context",
            this.userRows,
            this.context.userId,
            user => `${user.DisplayName ?? user.Id} - ${user.Id}`);
    },

    fillContextSelectors: function (selector, rows, selectedValue, labelFactory) {
        document.querySelectorAll(selector).forEach(select => {
            const current = selectedValue ? String(selectedValue) : "";
            select.innerHTML = `<option value="">Select ${select.dataset.contextType}</option>`;

            rows.forEach(row => {
                const option = document.createElement("option");
                option.value = row.Id;
                option.textContent = labelFactory(row);
                select.appendChild(option);
            });

            select.value = current;
        });
    },

    updateCreateButtons: function () {
        this.workspaces
            .filter(config => !config.custom)
            .forEach(config => {
                const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
                const button = grid?.wrapper?.find(".k-grid-add");

                if (!button?.length || !config.context) {
                    return;
                }

                const enabled = Boolean(this.contextValue(config.context.type));
                button.toggleClass("k-disabled", !enabled);
                button.attr("aria-disabled", String(!enabled));
            });
    },

    ensurePrivilegesLoaded: async function () {
        if (this.privilegesLoaded) {
            return true;
        }

        try {
            const body = await AppSecurityApi.get(`${this.apiRoot}/Privilege?$top=500`);
            this.privilegeRows = AppSecurityApi.unwrapCollection(body);
            this.privilegesLoaded = true;
            return true;
        } catch {
            this.privilegeRows = [];
            this.privilegesLoaded = false;
            return false;
        }
    },

    roleDetailTemplate: function () {
        return `<div class="as-role-detail">` +
            `<div class="as-role-tabs">` +
            `<ul>` +
            `<li class="k-active"><span class="k-icon k-i-user"></span> Users</li>` +
            `<li><span class="k-icon k-i-check"></span> Privileges</li>` +
            `</ul>` +
            `<div>` +
            `<div class="as-detail-toolbar">` +
            `<input class="form-control form-control-sm role-user-id" placeholder="User Id">` +
            `<button class="k-button k-button-sm k-rounded-md k-button-solid k-button-solid-primary add-role-user" type="button">` +
            `<span class="k-icon k-i-plus"></span>Add user</button>` +
            `</div>` +
            `<div class="role-users-grid as-child-grid"></div>` +
            `</div>` +
            `<div>` +
            `<div class="as-detail-toolbar">` +
            `<button class="k-button k-button-sm k-rounded-md k-button-solid k-button-solid-primary save-role-privileges" type="button">` +
            `<span class="k-icon k-i-save"></span>Save privileges</button>` +
            `<span class="as-detail-note">Tick the privileges that belong to this role.</span>` +
            `</div>` +
            `<div class="role-privileges-grid as-child-grid"></div>` +
            `</div>` +
            `</div>` +
            `</div>`;
    },

    onRoleDetailInit: function (event) {
        const role = event.data;
        const detail = event.detailRow;

        detail.find(".as-role-tabs").kendoTabStrip({
            activate: tabEvent => {
                if ($(tabEvent.item).index() === 1) {
                    detail.find(".role-privileges-grid").data("kendoGrid").dataSource.read();
                }
            }
        });
        this.createRoleUsersGrid(detail, role);
        this.createRolePrivilegesGrid(detail, role);
    },

    createRoleUsersGrid: function (detail, role) {
        const gridElement = detail.find(".role-users-grid");

        gridElement.kendoGrid({
            dataSource: {
                transport: {
                    read: options => this.readRoleUsers(role, options)
                },
                pageSize: 10
            },
            pageable: true,
            sortable: true,
            filterable: true,
            columns: [
                { field: "UserId", title: "User Id", width: 220 },
                { field: "DisplayName", title: "Display Name", width: 220 },
                { field: "Email", title: "Email", width: 260 },
                { command: [{ text: "Delete", click: event => this.removeRoleUser(event, role) }], width: 130 }
            ]
        });

        detail.find(".add-role-user").on("click", async () => {
            const input = detail.find(".role-user-id");
            const userId = input.val()?.trim();

            if (!userId) {
                AppSecurityApi.notify("Enter a user id.", true);
                return;
            }

            await AppSecurityApi.post(`${this.apiRoot}/UserRole`, { roleId: role.Id, userId });
            input.val("");
            gridElement.data("kendoGrid").dataSource.read();
            AppSecurityApi.notify("User added to role");
        });
    },

    readRoleUsers: async function (role, options) {
        try {
            const roleUsers = await this.getRoleUsers(role.Id);
            const usersById = await this.getUsersById();
            const rows = roleUsers.map(userRole => {
                const user = usersById[userRole.UserId] ?? {};

                return {
                    RoleId: userRole.RoleId,
                    UserId: userRole.UserId,
                    DisplayName: user.DisplayName ?? "",
                    Email: user.Email ?? ""
                };
            });

            options.success(rows);
        } catch (error) {
            options.error(error);
        }
    },

    getRoleUsers: async function (roleId) {
        const filter = encodeURIComponent(`RoleId eq ${roleId}`);
        const body = await AppSecurityApi.get(`${this.apiRoot}/UserRole?$top=500&$filter=${filter}`);

        return AppSecurityApi.unwrapCollection(body);
    },

    getUsersById: async function () {
        const body = await AppSecurityApi.get(`${this.apiRoot}/User?$top=500`);
        const users = AppSecurityApi.unwrapCollection(body);

        return users.reduce((map, user) => {
            map[user.Id] = user;
            return map;
        }, {});
    },

    removeRoleUser: async function (event, role) {
        event.preventDefault();
        const grid = $(event.currentTarget).closest(".k-grid").data("kendoGrid");
        const userRole = grid.dataItem($(event.currentTarget).closest("tr"));

        await AppSecurityApi.delete(
            `${this.apiRoot}/UserRole(RoleId=${role.Id},UserId='${this.escapeKey(userRole.UserId)}')`);

        grid.dataSource.read();
        AppSecurityApi.notify("User removed from role");
    },

    createRolePrivilegesGrid: function (detail, role) {
        const gridElement = detail.find(".role-privileges-grid");

        gridElement.kendoGrid({
            dataSource: {
                transport: {
                    read: options => this.readRolePrivileges(role, options)
                },
                schema: {
                    model: {
                        id: "Id",
                        fields: {
                            Assigned: { type: "boolean" },
                            Id: { type: "string", editable: false },
                            Type: { type: "string", editable: false },
                            Operation: { type: "string", editable: false },
                            Description: { type: "string", editable: false }
                        }
                    }
                },
                pageSize: 20
            },
            autoBind: false,
            pageable: true,
            sortable: true,
            filterable: true,
            columns: [
                {
                    field: "Assigned",
                    title: " ",
                    width: 64,
                    template: "<input class='role-privilege-toggle' type='checkbox' #= Assigned ? 'checked' : '' #>"
                },
                { field: "Type", title: "Type", width: 180 },
                { field: "Operation", title: "Operation", width: 160 },
                { field: "Description", title: "Description" }
            ]
        });

        gridElement.on("change", ".role-privilege-toggle", event => {
            const grid = gridElement.data("kendoGrid");
            const row = $(event.currentTarget).closest("tr");
            const privilege = grid.dataItem(row);
            privilege.set("Assigned", event.currentTarget.checked);
        });

        detail.find(".save-role-privileges").on("click", async () => {
            const grid = gridElement.data("kendoGrid");
            const privileges = grid.dataSource.data().toJSON();
            const privilegeIds = privileges
                .filter(privilege => privilege.Assigned)
                .map(privilege => privilege.Id);

            await this.saveRolePrivileges(role, privilegeIds);
        });
    },

    readRolePrivileges: async function (role, options) {
        try {
            const loaded = await this.ensurePrivilegesLoaded();

            if (!loaded) {
                options.success([]);
                return;
            }

            const assignedPrivilegeIds = this.rolePrivilegeIds(role);
            const rows = this.privilegeRows.map(privilege => ({
                Assigned: assignedPrivilegeIds.includes(privilege.Id),
                Id: privilege.Id,
                Type: privilege.Type,
                Operation: privilege.Operation,
                Description: privilege.Description
            }));

            options.success(rows);
        } catch (error) {
            options.error(error);
        }
    },

    saveRolePrivileges: async function (role, privilegeIds) {
        const privilegeList = [...new Set(privilegeIds)].join(",");
        const roleJson = role.toJSON ? role.toJSON() : Object.assign({}, role);

        roleJson.Privs = privilegeList;
        await AppSecurityApi.put(`${this.apiRoot}/Role(${role.Id})`, roleJson);

        if (role.set) {
            role.set("Privs", privilegeList);
        } else {
            role.Privs = privilegeList;
        }

        AppSecurityApi.notify("Role privileges updated");
    },

    rolePrivilegeIds: function (role) {
        return (role?.Privs ?? "")
            .split(",")
            .map(privilegeId => privilegeId.trim())
            .filter(Boolean);
    },

    contextValue: function (contextType) {
        if (contextType === "app") {
            return this.context.appId;
        }

        if (contextType === "role") {
            return this.context.roleId;
        }

        return this.context.userId;
    },

    noRecordsMessage: function (config) {
        if (!config.context) {
            return `No ${config.title} found.`;
        }

        const contextName = config.context.type === "app" ? "App" : "Role";
        return `Select a ${contextName} to manage ${config.title}.`;
    },

    withRowState: function (config, row) {
        const copy = Object.assign({}, row);

        return copy;
    },

    formatKey: function (config, value) {
        if (config.keyType === "string") {
            return `'${this.escapeKey(value)}'`;
        }

        return value;
    },

    escapeKey: function (value) {
        return encodeURIComponent(String(value).replace(/'/g, "''"));
    },

    surfaceId: function (config) {
        return `surface-${config.name.toLowerCase()}`;
    },

    gridId: function (config) {
        return `grid-${config.name.toLowerCase()}`;
    },

    label: function (field) {
        return field.replace(/([a-z])([A-Z])/g, "$1 $2");
    },

    widthFor: function (field) {
        if (field === "Id") {
            return 260;
        }

        if (field.endsWith("Id")) {
            return 220;
        }

        if (["Privs", "Description", "ConfigJson"].includes(field)) {
            return 360;
        }

        return 180;
    }
};

document.addEventListener("DOMContentLoaded", () => window.AppSecurityGrids.init());
