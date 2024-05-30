$(document).ready(function () {

    let containerCrawlers = $("#containerCrawlers");
    let btnAddCrawler = $("#btnAddCrawler");
    let frmCrawler = $("#frmCrawler");
    let mdlCrawler = $("#mdlCrawler");
    let mdlTitle = mdlCrawler.find('#mdlTitle');
    let btnSaveCrawler = mdlCrawler.find("#btnSaveCrawler")

    let isClearForm = false;
    let LIST_CRAWLERS = [];

    const getCrawlers = function () {
        Utils.Ajax.callAjax("Admin/GetCrawler", "GET", null, function (res) {
            console.log(res)
            generateCrawler(res?.value);

            let crawlers = res?.value || [];
            LIST_CRAWLERS = crawlers;

            $.each(crawlers, function (index, item){
                generateCrawler(item);
            })
        })
    }

    const generateCrawler = function (data) {
        if (data?.crawler) {

            let { crawler, aggregations } = data;

            let icon = '<i class="fa fa-server"></i>';

            if (crawler.crawler == 1) {
                icon = '<img src="assets/img/devevents.png" />';
            }
            else if (crawler.crawler == 2) {
                icon = '<img src="assets/img/polytechnique.png" />';
            }

            var lastModified = Utils.DateTime.Parser.fromDateTimeString(aggregations?.lastModified)

            let template = `<div class="crawler row" data-id="${crawler.id}">
                                        <div class="col-md-3 left-content">
                                            ${icon}
                                        </div>
                                        <div class="col-md-9 right-content">
                                            <h4 class="crawler-name">${crawler.crawlerName}</h4>
                                            <p class="crawler-url">${crawler.crawlerUrl || ''}</p>
                                            <div class="main-button">
                                                ${crawler.crawlerUrl ? '<a href="javascript:;" class="btnCrawler"><i class="fal fa-cloud-download"></i> Crawler</a>' : ''}
                                            </div>
                                            <ul class="info">
                                              <li><i class="fa fa-database"></i><span class="total"> ${aggregations?.count || 0} </span> conferences</li>
                                              <li><i class="fa fa-calendar-alt"></i>updated at <span class="lastModified"> ${lastModified?.toString(true) || '___'} </span></li>
                                              <li>
                                                <a class="text-danger btnDeleteData" href="javascript:;" style="display:${aggregations?.count ? 'block' : 'none'}">
                                                    <i class="fa fa-times"></i> Delete data
                                                </a>
                                              </li>
                                            </ul>
                                            <div class="actions">
                                                <div>
                                                    <a class="btnUpdate btn btn-outline-primary btn-sm" href="javascript:;">
                                                        <i class="fal fa-edit"></i> Update
                                                    </a>
                                                    <a class="btn btn-outline-danger btn-sm btnDelete" href="javascript:;">
                                                        <i class="fal fa-trash"></i> Delete
                                                    </a>
                                                </div>
                                                ${crawler.crawlerUrl ? `<a class="crawler-link" href="${crawler.crawlerUrl}" target="_blank">Go to website  <i class="fa fa-arrow-right"></i></a>` : ''}
                                            </div>
                                        </div>
                                    <div>`

            containerCrawlers.append(template);
        }
    }

    const addCrawler = function (data) {
        if (data) {
            Utils.Ajax.callAjax("Admin/AddCrawler", "POST", data, function (res) {
                if (res && res.value)
                {
                    Utils.Alertify.success("bl", "Add crawler successfull");

                    let vmData = {
                        crawler: res.value,
                        aggregations: {
                            count: 0,
                            lastModified: null
                        }
                    };

                    generateCrawler(vmData);
                    LIST_CRAWLERS.push(vmData);

                    mdlCrawler.modal('hide');
                }
                else {
                    Utils.Alertify.error("bl", "Add crawler unsuccessfull")
                }
            })
        }
    }


    const updateCrawler = function (data, callback) {
        if (data && data.id) {
            Utils.Ajax.callAjax("Admin/UpdateCrawler", "POST", data, function (res) {
                if (res && res.value) {
                    Utils.Alertify.success("bl", "Update crawler successfull");

                    if (LIST_CRAWLERS.length > 0) {
                        for (var i = 0; i < LIST_CRAWLERS.length; i++) {
                            if (LIST_CRAWLERS[i].crawler && LIST_CRAWLERS[i].crawler['id'] == data.id) {
                                LIST_CRAWLERS[i].crawler = { ...LIST_CRAWLERS[i].crawler, ...{ crawler: data.crawler, crawlerName: data.crawlerName, crawlerUrl: data.crawlerUrl } }
                                break;
                            }
                        }
                    }

                    let element = containerCrawlers.find('.crawler[data-id="' + data.id + '"]');

                    if (element.length == 1)
                    {
                        element.find('.crawler-name').html(data.crawlerName || '')
                        element.find('.crawler-url').html(data.crawlerUrl || '')
                        element.find('.crawler-link').attr('href', data.crawlerUrl || '#')
                    }

                    mdlCrawler.modal('hide');
                }
                else {
                    Utils.Alertify.error("bl", "Update crawler unsuccessfull")
                }
            })
        }
    }


    containerCrawlers.on('click', '.btnCrawler', function () {
        let btn = $(this);
        let element = $(this).closest('.crawler');
        let crawlerId = element.attr('data-id') >> 0;

        if (crawlerId)
        {
            btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>  Crawling...')
            btn.prop('disabled', true);
            Utils.Ajax.callAjax("Admin/Crawler", "POST", { crawlerId }, function (res) {
                if (res?.value && res.value.success)
                {
                    if (LIST_CRAWLERS.length > 0) {
                        for (var i = 0; i < LIST_CRAWLERS.length; i++) {
                            if (LIST_CRAWLERS[i].crawler && LIST_CRAWLERS[i].crawler['id'] == crawlerId)
                            {
                                LIST_CRAWLERS[i].aggregations = { ...LIST_CRAWLERS[i].aggregations, ...{ count: res.value.total || 0, lastModified: res.value.lastModified } }
                                break;
                            }
                        }
                    }

                    element.find('.total').html(res.value.total || 0);
                    element.find('.lastModified').html(res.value.lastModified?.toString(true));
                    element.find('.btnDeleteData').show();
                }

                btn.html('Crawler');
                btn.prop('disabled', false);
            })
        }
    })

    containerCrawlers.on('click', '.btnDeleteData', function () {
        let btnDelete = $(this);
        let element = $(this).closest('.crawler');

        if (element.attr('data-id')) {
            let crawlerId = element.attr('data-id') >> 0;
            Utils.Msg.show({
                title: "Confirm delete data",
                msg: `Are you sure delete this data?`,
                icon: Utils.Msg.WARNING,
                buttons: Utils.Msg.YESNO,
                fn: function (btn) {
                    if (btn === 'YES') {
                        btnDelete.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>  Deleting...')
                        btnDelete.prop('disabled', true);
                        Utils.Ajax.callAjax("Admin/DeleteData", "POST", { crawlerId }, function (res)
                        {
                            if (res?.value) {
                                element.find('.total').html( 0);
                                element.find('.lastModified').html('---');
                            }

                            setTimeout(function () {
                                btnDelete.html('<i class="fa fa-times"></i> Delete data</a>');
                                btnDelete.prop('disabled', false);
                                btnDelete.hide();
                            }, 1000)
                        })
                    }
                }
            });
        }
    })

    containerCrawlers.on('click', '.btnUpdate', function () {
        let element = $(this).closest('.crawler');
        let crawlerId = element.attr('data-id') >> 0;

        if (crawlerId && LIST_CRAWLERS.length)
        {
            let vmCrawler = LIST_CRAWLERS.find(t => t.crawler && t.crawler['id'] === crawlerId);

            if (vmCrawler) {
                Utils.Form.bindFormData(frmCrawler, vmCrawler.crawler);
                frmCrawler.find('select').attr('disabled', true);
                btnSaveCrawler.attr('data-mode', 'update').removeClass('disabled');
                mdlTitle.html('Update crawler');
                mdlCrawler.modal('show');
            }
        }
    })

    containerCrawlers.on('click', '.btnDelete', function () {
        let btnDelete = $(this);
        let element = $(this).closest('.crawler');
        let crawlerId = element.attr('data-id') >> 0;

        if (crawlerId) {
            Utils.Msg.show({
                title: "Confirm delete crawler",
                msg: `Are you sure delete this crawler?`,
                icon: Utils.Msg.WARNING,
                buttons: Utils.Msg.YESNO,
                fn: function (btn) {
                    if (btn === 'YES') {
                        btnDelete.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>  Deleting...')
                        btnDelete.prop('disabled', true);
                        Utils.Ajax.callAjax("Admin/DeleteCrawler", "POST", { id : crawlerId }, function (res) {

                            if (res && res.value) {

                                Utils.Alertify.success("bl", "Delete crawler successfull");

                                if (LIST_CRAWLERS.length > 0) {
                                    for (var i = 0; i < LIST_CRAWLERS.length; i++) {
                                        if (LIST_CRAWLERS[i].crawler && LIST_CRAWLERS[i].crawler['id'] == crawlerId) {
                                            LIST_CRAWLERS.splice(i, 1);
                                            break;
                                        }
                                    }
                                }

                                setTimeout(function () {
                                    btnDelete.html('<i class="fal fa-trash"></i> Delete');
                                    element.remove();
                                }, 1000)
                            }
                            else
                            {
                                Utils.Alertify.error("bl", "Delete crawler unsuccessfull");
                            }
                        })
                    }
                }
            });
        }
    })

    btnAddCrawler.on('click', function () {
        Utils.Form.clearFormData(frmCrawler);
        frmCrawler.find('select').attr('disabled', false);
        btnSaveCrawler.attr('data-mode', 'add').addClass('disabled');
        mdlTitle.html('Add crawler')
        mdlCrawler.modal('show');
    });

    frmCrawler.on('change', 'input', function () {

        if (Utils.Form.IsUpdateForm) return false;

        if (this.value) {
            $(this).removeClass('is-invalid');
            $(this).siblings('.invalid-feedback').hide();
        }
        else {
            $(this).addClass('is-invalid');
            $(this).siblings('.invalid-feedback').show();
        }

        var elements = frmCrawler.find('input.is-invalid');

        if (elements.length) {
            btnSaveCrawler.addClass('disabled');
        }
        else {
            btnSaveCrawler.removeClass('disabled');
        }
    })

    btnSaveCrawler.on('click', function () {
        let mode = this.dataset.mode;
        let data = Utils.Form.getFormValue(frmCrawler);

        if (mode === 'add') {
            addCrawler(data);
        }
        else if (mode == 'update')
        {
            updateCrawler(data);
        }
    })

    getCrawlers();
});