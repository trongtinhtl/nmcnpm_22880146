$(document).ready(function () {

    const tbUsers = $('#tbUsers');
    const mdlUser = $('#mdlUsers');
    const frmUser = mdlUser.find('#frmUser');

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
                            <td>${user.role == 1 ? 'Administrator' : 'User'}</td>
                            <td>${user.blocked ? 'Đã bị khóa' : 'Đang hoạt động'}</td>
                            <td>
                                <a href="javascript:;" class="btn btn-outline-primary btn-sm btnUpdate"><i class="fal fa-edit"></i></a>
                                <a href="javascript:;" class="btn btn-outline-danger btn-sm btnDelete"><i class="fal fa-trash"></i></a>
                            </td>
                        </tr>`

            tbUsers.find('tbody').append(row);
		}
    }

    tbUsers.on('click', 'btnUpdate', function ()
    {
        let row = (this).closest('tr');
        let userId = row.attr('data-id') >> 0;

        let user = LIST_USERS.find(t => t.id == userId);

        if (user)
        {
            Utils.Form.bindFormData(frmUser, user);
		}
    })

    getUsers();
});