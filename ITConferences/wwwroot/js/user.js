$(document).ready(function () {

    const tbUsers = $('#tbUsers');
    const mdlUser = $('#mdlUser');
    const frmUser = mdlUser.find('#frmUser');
    const btnSaveUser = mdlUser.find('#btnSaveUser');
    const btnAddUser = $('#btnAddUser');

    let LIST_USERS = [];

    const getUsers = function () {

        let query = "";
        Utils.Ajax.callAjax(_WEB_URL + "Admin/GetUsers", "POST", { query  }, function (res) {
            LIST_USERS = res?.value || [];
            $.each(LIST_USERS, function (index, user) {
                generateRow(index + 1, user);
            })
        })
    }

    const generateRow = function (index, user)
    {
        if (user && typeof user == 'object') {

            var row = `<tr data-id="${user.id}">
                            <td>${index}</td>
                            <td>${user.userName}</td>
                            <td>${user.email}</td>
                            <td class="role">${user.role == 1 ? 'Administrator' : 'User'}</td>
                            <td class="blocked">${user.blocked ? 'Đã bị khóa' : 'Đang hoạt động'}</td>
                            <td>
                                <a href="javascript:;" class="btn btn-outline-primary btn-sm btnUpdate"><i class="fal fa-edit"></i></a>
                                <a href="javascript:;" class="btn btn-outline-danger btn-sm btnDelete"><i class="fal fa-trash"></i></a>
                            </td>
                        </tr>`

            tbUsers.find('tbody').append(row);
		}
    }

    const changeModal = function (mode) {
        if (mode === 'add')
        {
            btnSaveUser.attr('data-mode', mode);
            mdlUser.find('.modal-title').html('Create account')
            frmUser.find('.password-container').show();
            frmUser.find('[name="userName"]').attr('disabled', false);
            frmUser.find('[name="email"]').attr('disabled', false);
        }
        else if (mode == 'update')
        {
            btnSaveUser.attr('data-mode', mode);
            mdlUser.find('.modal-title').html('Update account');
            frmUser.find('.password-container').hide();
            frmUser.find('[name="userName"]').attr('disabled', true);
            frmUser.find('[name="email"]').attr('disabled', true);
        }
    };

    const addUser = function (user) {
        if (user) {
            Utils.Ajax.callAjax(_WEB_URL + "Admin/AddUser", "POST", user, function (res) {
                if (res && res.value) {
                    Utils.Alertify.success("bl", "Add account successfull");

                    LIST_USERS.push(res.value);
                    generateRow(LIST_USERS.length, res.value);

                    mdlUser.modal('hide');
                }
                else {
                    Utils.Alertify.error("bl", res.error ? res.error : "Add account unsuccessfull")
                }
            })
        }
    }


    const updateUser = function (data) {
        if (data && data.id) {
            Utils.Ajax.callAjax(_WEB_URL + "Admin/UpdateUser", "POST", data, function (res) {
                if (res && res.value) {
                    Utils.Alertify.success("bl", "Update account successfull");

                    if (LIST_USERS.length > 0) {
                        for (var i = 0; i < LIST_USERS.length; i++) {
                            if (LIST_USERS[i]['id'] == data.id)
                            {
                                LIST_USERS[i] = { ...LIST_USERS[i], ...{ role: data.role, blocked: data.blocked } }
                                break;
                            }
                        }
                    }

                    let element = tbUsers.find('tr[data-id="' + data.id + '"]');

                    if (element.length == 1) {
                        element.find('td.role').html(data.role == 1 ? 'Administrator' : 'User')
                        element.find('td.blocked').html(data.blocked ? 'Đã bị khóa' : 'Đang hoạt động')
                    }

                    mdlUser.modal('hide');
                }
                else {
                    Utils.Alertify.error("bl", res.error ? res.error : "Update crawler unsuccessfull")
                }
            })
        }
    }

    tbUsers.on('click', '.btnUpdate', function ()
    {
        let row = $(this).closest('tr');
        let userId = row.attr('data-id') >> 0;

        let user = LIST_USERS.find(t => t.id == userId);

        if (user)
        {
            changeModal('update');
            Utils.Form.bindFormData(frmUser, user);
            mdlUser.modal('show');
		}
    })

    tbUsers.on('click', '.btnDelete', function () {
        let btnDelete = $(this);
        let row = $(this).closest('tr');
        let userId = row.attr('data-id') >> 0;

        if (userId)
        {
            Utils.Msg.show({
                title: "Confirm delete account",
                msg: `Are you sure delete this account?`,
                icon: Utils.Msg.WARNING,
                buttons: Utils.Msg.YESNO,
                fn: function (btn) {
                    if (btn === 'YES') {
                        btnDelete.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>')
                        btnDelete.prop('disabled', true);

                        Utils.Ajax.callAjax(_WEB_URL + "Admin/DeleteUser", "POST", { id: userId }, function (res) {

                            if (res && res.value) {

                                Utils.Alertify.success("bl", "Delete account successfull");

                                if (LIST_USERS.length > 0) {
                                    for (var i = 0; i < LIST_USERS.length; i++) {
                                        if (LIST_USERS[i]['id'] == userId) {
                                            LIST_USERS.splice(i, 1);
                                            break;
                                        }
                                    }
                                }

                                setTimeout(function () {
                                    btnDelete.html('<i class="fal fa-trash"></i> Delete');
                                    row.remove();
                                },500)
                            }
                            else {
                                Utils.Alertify.error("bl", "Delete account unsuccessfull");
                            }
                        })
                    }
                }
            });
        }
    })

    btnAddUser.on('click', function () {
        Utils.Form.clearFormData(frmUser);
        changeModal('add');
        mdlUser.modal('show');
    })

    btnSaveUser.on('click', function () {

        let mode = this.dataset.mode;
        let user = Utils.Form.getFormValue(frmUser);

        if (mode == 'add')
        {
            addUser(user)
        }
        else if (mode == 'update')
        {
            updateUser(user)
        }
    })

    getUsers();
});