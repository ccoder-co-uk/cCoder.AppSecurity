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
        {
            name: "UserRole",
            title: "User Roles",
            description: "User-role links inside the selected App and Role",
            composite: true,
            context: { type: "role", field: "RoleId" },
            fields: {
                RoleId: { type: "string" },
                UserId: { type: "string" }
            },
            columns: ["RoleId", "UserId"]
        },
    ],

    init: function () {
        this.buildWorkspaces();
        this.workspaces
            .forEach(config => this.createGrid(config));
        this.bindRolePrivilegeEditor();
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
        const childHtml = config.name === "Role"
            ? this.rolePrivilegesHtml()
            : "";

        return `<div class="as-toolbar">` +
            `<div><h2>${config.title}</h2><span>${config.description}</span></div>` +
            this.contextHtml(config) +
            `</div>` +
            `<div id="${this.gridId(config)}" class="as-grid"></div>` +
            childHtml;
    },

    rolePrivilegesHtml: function () {
        return `<div class="as-child-panel">` +
            `<div class="as-child-header">` +
            `<div><h3>Privilege assignment</h3><span id="selected-role-label">Select a role to manage its privileges.</span></div>` +
            `<button id="refresh-role-privileges" class="k-button k-button-sm k-rounded-md k-button-solid k-button-solid-base" type="button">` +
            `<span class="k-icon k-i-refresh"></span>Refresh</button>` +
            `</div>` +
            `<div class="as-role-privileges">` +
            `<section>` +
            `<h4>Assigned privileges</h4>` +
            `<div id="role-assigned-privileges-grid" class="as-grid"></div>` +
            `</section>` +
            `<section>` +
            `<h4>Available privileges</h4>` +
            `<div id="role-available-privileges-grid" class="as-grid"></div>` +
            `</section>` +
            `</div>` +
            `</div>`;
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
                        id: config.composite ? "_rowKey" : config.key,
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
        const fields = Object.assign(
            config.composite ? { _rowKey: { editable: false } } : {},
            config.fields);

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

        if (config.name === "UserRole" && this.context.roleId) {
            url += `&$filter=${encodeURIComponent(`RoleId eq ${this.context.roleId}`)}`;
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

            if (config.composite) {
                await this.deleteComposite(config, options.data._original ?? options.data);
                result = await AppSecurityApi.post(`${this.apiRoot}/${config.name}`, payload);
            } else {
                result = await AppSecurityApi.put(
                    `${this.apiRoot}/${config.name}(${this.formatKey(config, options.data[config.key])})`,
                    payload);
            }

            options.success(this.withRowState(config, result ?? payload));
            AppSecurityApi.notify(`${config.title} updated`);
        } catch (error) {
            options.error(error);
        }
    },

    destroy: async function (config, options) {
        try {
            if (config.composite) {
                await this.deleteComposite(config, options.data._original ?? options.data);
            } else {
                await AppSecurityApi.delete(
                    `${this.apiRoot}/${config.name}(${this.formatKey(config, options.data[config.key])})`);
            }

            options.success(options.data);
            AppSecurityApi.notify(`${config.title} deleted`);
        } catch (error) {
            options.error(error);
        }
    },

    deleteComposite: function (config, data) {
        const payload = this.preparePayload(config, data, false);

        return AppSecurityApi.delete(
            `${this.apiRoot}/${config.name}(RoleId=${payload.RoleId},UserId='${this.escapeKey(payload.UserId)}')`);
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

        if (config.name === "UserRole") {
            payload.RoleId = this.context.roleId;
            payload.UserId = payload.UserId || this.context.userId;
        }

        return payload;
    },

    onEdit: function (config, event) {
        if (config.name === "Role") {
            event.model.set("AppId", Number(this.context.appId));
        }

        if (config.name === "UserRole") {
            event.model.set("RoleId", this.context.roleId);
            event.model.set("UserId", event.model.UserId || this.context.userId || "");
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
            this.refreshRolePrivilegeEditor();
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
            this.refreshLoadedGrid("UserRole");
            this.refreshRolePrivilegeEditor();
        }
    },

    setRoleContext: function (value, refresh = true) {
        const roleId = value || null;

        if (this.context.roleId === roleId) {
            return;
        }

        this.context.roleId = roleId;
        this.refreshRoleSelectors();

        if (refresh) {
            this.refreshLoadedGrid("UserRole");
            this.refreshRolePrivilegeEditor();
        }
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

    bindRolePrivilegeEditor: function () {
        $("#role-assigned-privileges-grid").kendoGrid({
            dataSource: { data: [], pageSize: 20 },
            pageable: true,
            sortable: true,
            filterable: true,
            columns: [
                { field: "Id", title: "Privilege", width: 240 },
                { field: "Type", title: "Type", width: 160 },
                { field: "Operation", title: "Operation", width: 140 },
                { command: [{ text: "Remove", click: event => this.removePrivilege(event) }], width: 130 }
            ]
        });

        $("#role-available-privileges-grid").kendoGrid({
            dataSource: { data: [], pageSize: 20 },
            pageable: true,
            sortable: true,
            filterable: true,
            columns: [
                { field: "Id", title: "Privilege", width: 240 },
                { field: "Type", title: "Type", width: 160 },
                { field: "Operation", title: "Operation", width: 140 },
                { command: [{ text: "Add", click: event => this.addPrivilege(event) }], width: 110 }
            ]
        });

        document
            .getElementById("refresh-role-privileges")
            ?.addEventListener("click", () => this.reloadRolePrivileges());
    },

    reloadRolePrivileges: async function () {
        this.privilegesLoaded = false;
        await this.refreshRolePrivilegeEditor();
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

    refreshRolePrivilegeEditor: async function () {
        const role = this.selectedRole();

        this.setSelectedRoleLabel(role);

        if (!role) {
            this.setGridData("#role-assigned-privileges-grid", []);
            this.setGridData("#role-available-privileges-grid", []);
            return;
        }

        const loaded = await this.ensurePrivilegesLoaded();

        if (!loaded) {
            this.setGridData("#role-assigned-privileges-grid", []);
            this.setGridData("#role-available-privileges-grid", []);
            return;
        }

        const assignedPrivilegeIds = this.rolePrivilegeIds(role);
        const assigned = this.privilegeRows.filter(privilege => assignedPrivilegeIds.includes(privilege.Id));
        const available = this.privilegeRows.filter(privilege => !assignedPrivilegeIds.includes(privilege.Id));

        this.setGridData("#role-assigned-privileges-grid", assigned);
        this.setGridData("#role-available-privileges-grid", available);
    },

    setSelectedRoleLabel: function (role) {
        const label = document.getElementById("selected-role-label");

        if (!label) {
            return;
        }

        label.textContent = role
            ? `${role.Name ?? "Role"} (${role.Id})`
            : "Select a role to manage its privileges.";
    },

    addPrivilege: async function (event) {
        event.preventDefault();
        const privilege = this.gridDataItem(event);

        await this.saveRolePrivileges([
            ...this.rolePrivilegeIds(this.selectedRole()),
            privilege.Id
        ]);
    },

    removePrivilege: async function (event) {
        event.preventDefault();
        const privilege = this.gridDataItem(event);

        await this.saveRolePrivileges(
            this.rolePrivilegeIds(this.selectedRole())
                .filter(privilegeId => privilegeId !== privilege.Id));
    },

    gridDataItem: function (event) {
        const grid = $(event.currentTarget).closest(".k-grid").data("kendoGrid");
        const row = $(event.currentTarget).closest("tr");

        return grid.dataItem(row);
    },

    saveRolePrivileges: async function (privilegeIds) {
        const role = this.selectedRole();

        if (!role) {
            AppSecurityApi.notify("Select a role first.", true);
            return;
        }

        role.Privs = [...new Set(privilegeIds)].join(",");
        await AppSecurityApi.put(`${this.apiRoot}/Role(${role.Id})`, role);
        this.refreshGrid("Role");
        AppSecurityApi.notify("Role privileges updated");
    },

    selectedRole: function () {
        return this.roleRows.find(role => role.Id === this.context.roleId) ?? null;
    },

    rolePrivilegeIds: function (role) {
        return (role?.Privs ?? "")
            .split(",")
            .map(privilegeId => privilegeId.trim())
            .filter(Boolean);
    },

    setGridData: function (selector, rows) {
        const grid = $(selector).data("kendoGrid");

        if (grid) {
            grid.dataSource.data(rows);
        }
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

        if (config.composite) {
            copy._rowKey = this.compositeKey(copy);
            copy._original = this.preparePayload(config, copy, false);
        }

        return copy;
    },

    compositeKey: function (row) {
        return `${row.RoleId}|${row.UserId}`;
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
