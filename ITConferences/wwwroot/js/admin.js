$(document).ready(function () {

    let aggregations = $("#aggregations");

    const getAggregation = function () {
        Utils.Ajax.callAjax("Admin/AggregationSource", "GET", null, function (res) {
            console.log(res)
            generateAggregation(res?.value)
        })
    }

    const generateAggregation = function (data) {
        if (data && data.length)
        {
            $.each(data, function (index, item) {

                if (item && typeof item === 'object') {

                    let icon = '<i class="fa fa-server"></i>';

                    if (item.value == 1) {
                        icon = '<img src="assets/img/devevents.png" />';
                    }
                    else if (item.value == 2) {
                        icon = '<img src="assets/img/polytechnique.png" />';
                    }

                    var lastModified = Utils.DateTime.Parser.fromDateTimeString(item.aggregations?.lastModified)

                    let template = `<div class="aggregation-source row" data-type="${item.value}">
                                        <div class="col-md-3 left-content">
                                            ${icon}
                                        </div>
                                        <div class="col-md-9 right-content">
                                            <h4 class="name">${item.name}</h4>
                                            <p>${item.link || ''}</p>
                                            <div class="main-button">
                                                ${item.link ? '<a href="javascript:;" class="btnCrawler">Crawler</a>' : ''}
                                            </div>
                                            <ul class="info">
                                              <li><i class="fa fa-database"></i><span class="total"> ${item.aggregations?.count || 0} </span> conferences</li>
                                              <li><i class="fa fa-calendar-alt"></i>updated at <span class="lastModified"> ${lastModified?.toString(true) || '___'} </span></li>
                                              <li>
                                                <a class="text-danger btnDelete" href="javascript:;" style="display:${item.aggregations?.count ? 'block' : 'none'}">
                                                    <i class="fa fa-trash-alt"></i>Delete
                                                </a>
                                              </li>
                                            </ul>
                                            <div class="text-button">
                                                ${
                                                    item.link ? `<a href="${item.link}" target="_blank">Go to website  <i class="fa fa-arrow-right"></i></a>` : ''
                                                }
                                            </div>
                                        </div>
                                    <div>`

                    aggregations.append(template);
                }
            })
        }
    }

    aggregations.on('click', '.btnCrawler', function () {
        let btn = $(this);
        let element = $(this).closest('.aggregation-source');
        let type = element.attr('data-type') >> 0;

        if (type)
        {
            btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>  Crawling...')
            btn.prop('disabled', true);
            Utils.Ajax.callAjax("Admin/Crawler", "POST", { type }, function (res) {
                if (res?.value && res.value.success)
                {
                    element.find('.total').html(res.value.total || 0);
                    element.find('.lastModified').html(res.value.lastModified?.toString(true));
                    element.find('.btnDelete').show();
                }

                btn.html('Crawler');
                btn.prop('disabled', false);
            })
        }
    })

    aggregations.on('click', '.btnDelete', function () {
        let btnDelete = $(this);
        let element = $(this).closest('.aggregation-source');

        if (element.attr('data-type')) {
            let type = element.attr('data-type') >> 0;
            Utils.Msg.show({
                title: "Confirm delete data",
                msg: `Are you sure delete this data?`,
                icon: Utils.Msg.WARNING,
                buttons: Utils.Msg.YESNO,
                fn: function (btn) {
                    if (btn === 'YES') {
                        btnDelete.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>  Deleting...')
                        btnDelete.prop('disabled', true);
                        Utils.Ajax.callAjax("Admin/Delete", "POST", { type }, function (res)
                        {
                            if (res?.value) {
                                element.find('.total').html( 0);
                                element.find('.lastModified').html('---');
                            }

                            setTimeout(function () {
                                btnDelete.html('<i class="fa fa-trash-alt"></i>Delete</a>');
                                btnDelete.prop('disabled', false);
                                btnDelete.hide();
                            }, 1000)
                        })
                    }
                }
            });
        }
    })

    getAggregation();
});