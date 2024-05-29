$(document).ready(function () {

    const MAX_LENGTH = 10;
    let PROGRESSING = false;

    let CurrentRequest = {
        query: null,
        start: 0,
        length: 10
    }

    let aggregations = $("#aggregations");
    let container = $("#conferences");
    let loading = $("#loading");

    const getConferences = function(page)
    {
        if (page > 0) {
            const start = (page - 1) * MAX_LENGTH;
            const length = MAX_LENGTH;

            const formData = Utils.Form.getFormValue(aggregations);

            CurrentRequest = { ...formData, start, length };

            Utils.Ajax.callAjax("Home/GetITConferences", "POST", { request: CurrentRequest }, function (res)
            {
                generateConferences(res?.value || [], page, res?.totalCount);
                setTimeout(function () {
                    PROGRESSING = false;
                    loading.hide();
                }, 1000)
            })
        }
    }

    const getAggregation = function () {

        const formData = Utils.Form.getFormValue(aggregations);

        //CurrentRequest = { ...formData, start, length };

        Utils.Ajax.callAjax("Home/Aggregation", "POST", { request: formData }, function (res) {
            generateAggregation(res?.value)
        })
    }

    const generateConferences = function (data, page, total)
    {
        if (page == 1) container.empty();

        if (data?.length) {
            $.each(data, function (index, item) {
                let template = `<div class="conference row">
                                    <div class="left col-md-4">
                                        ${generateTime(item.startDate, item.endDate)}
                                    </div>
                                    <div class="right col-md-8">
                                        <a class="name" href="${item.link}" target="_blank">${item.name}</a>
                                        <div class="description">${item.description}</div>
                                        <ul class="info">
                                          <li><i class="fa fa-map-marker-alt"></i> ${item.location || '---'}</li>
                                        </ul>
                                    </div>
                                </div>`

                container.append(template);
            })

            let count = (page - 1) * MAX_LENGTH + data.length;
            let counter = `<div class="counter" ${count < total ? 'data-next="' + (page + 1) + '"' : ''}>Hiển thị ${count} trong tổng số ${total}</div>`

            container.append(counter);
        }
        else if (page == 1)
        {
            let empty = `<div class="empty-data">
                            <img src="assets/img/no_thing_here.png" />
                            <p>Data not found! Please contact administrator for infomation</p>
                         </div>`

            container.append(empty);
        }
    }

    const generateTime = function (startDate, endDate)
    {
        if (startDate && endDate)
        {
            if (startDate.toString() == endDate.toString()) {
                return startDate.toString();
            }

            let startYear = startDate.getFullYear();
            let startMonth = startDate.getMonth();
            let startDay = startDate.getDate();

            let endYear = endDate.getFullYear();
            let endMonth = endDate.getMonth();

            if (startYear == endYear) {

                if (startMonth == endMonth) {
                    return String(startDay).padStart(2, "0") + ' - ' + endDate.toString();
                }
                return String(startDay).padStart(2, "0") + "/" + String(startMonth).padStart(2, "0") + " - " + endDate.toString();
            }
            else
            {
                return startDate.toString() + " - " + endDate.toString();
            }
        }
        else if (startDate)
        {
            return startDate.toString();
        }
        else if (endDate)
        {
            return endDate.toString();
        }

        return "";
    }

    const generateAggregation = function (data)
    {
        aggregations.empty();

        if (data && data.length)
        {
            $.each(data, function (index, item)
            {
                if (item && typeof item === 'object')
                {
                    let template = $('<div>', { class: 'aggregation' });
                    template.append('<h6>' + item.name + '</h6>');
                    template.append('<ul></ul>');

                    if (item.aggregations && typeof item.aggregations == 'object' && Object.keys(item.aggregations).length > 0)
                    {
                        for (let prop in item.aggregations) {

                            let checked = prop == CurrentRequest[item.value] ? 'checked' : ''

                            let li = `<li class="form-check">
                                        <input class="form-check-input" type="radio" value="${prop}" id="${prop}" name="${item.value}" ${checked} />
                                        <label class="form-check-label" for="${prop}">
                                            <span>${prop}</span>
                                            <small>${item.aggregations[prop]}</small>
                                        </label>
                                    </li>`

                            template.find('ul').append(li)
                        }

                        aggregations.append(template);
                    }
                }
            })
        }
    }

    $(document).scroll(function ()
    {
        var scroll = $(window).scrollTop();
        var height = container.height();

        if ((scroll > height + 300 || scroll + $(window).height() == $(document).height()) && PROGRESSING == false)
        {
            let counter = container.find('.counter:last');

            if (counter.length == 1 && counter.attr("data-next"))
            {
                let page = counter.attr("data-next") >> 0;

                if (page)
                {
                    PROGRESSING = true;
                    loading.show();

                    setTimeout(function () {
                        getConferences(page);
                    }, 1000)
                }
            }
        }
    });

    aggregations.on('click', 'li.form-check small', function () {
        let label = $(this).parent();
        let input = label.siblings('input[type="radio"]');
        if (input.length == 1 && input.prop('checked'))
        {
            input.prop('checked', false);
            input.trigger('change');
            return false;
        }
    })

    aggregations.on('change', 'li.form-check [type="radio"]', function ()
    {
        if (this.name)
        {
            getConferences(1);
            getAggregation();
        }
    })

    getConferences(1);
    getAggregation();
});