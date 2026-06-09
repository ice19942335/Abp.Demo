$(function () {
    var l = abp.localization.getResource('Demo');
    var bookingService = abp.demo.bookings.booking;

    var createModal = new abp.ModalManager(abp.appPath + 'Bookings/CreateModal');

    var statusMap = ['Pending', 'Confirmed', 'Completed', 'Cancelled'];
    var statusBadge = {
        0: 'warning',
        1: 'primary',
        2: 'success',
        3: 'danger'
    };

    var dataTable = $('#BookingsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, 'desc']],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(bookingService.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Confirm'),
                                visible: function (data) {
                                    return data.status === 0;
                                },
                                action: function (data) {
                                    bookingService.confirm(data.record.id).then(function () {
                                        abp.notify.success(l('BookingStatus:Confirmed'));
                                        dataTable.ajax.reload();
                                    });
                                }
                            },
                            {
                                text: l('Complete'),
                                visible: function (data) {
                                    return data.status === 1;
                                },
                                action: function (data) {
                                    bookingService.complete(data.record.id).then(function () {
                                        abp.notify.success(l('BookingStatus:Completed'));
                                        dataTable.ajax.reload();
                                    });
                                }
                            },
                            {
                                text: l('Cancel'),
                                visible: function (data) {
                                    return data.status === 0 || data.status === 1;
                                },
                                confirmMessage: function (data) {
                                    return l('AreYouSure');
                                },
                                action: function (data) {
                                    abp.message.prompt(
                                        l('BookingCancellationReason'),
                                        l('Cancel')
                                    ).then(function (reason) {
                                        if (reason) {
                                            bookingService.cancel(data.record.id, reason).then(function () {
                                                abp.notify.info(l('BookingStatus:Cancelled'));
                                                dataTable.ajax.reload();
                                            });
                                        }
                                    });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('ResourceName'),
                    data: 'resourceName'
                },
                {
                    title: l('BookingStartTime'),
                    data: 'startTime',
                    render: function (data) {
                        return luxon.DateTime.fromISO(data, { locale: abp.localization.currentCulture.name }).toLocaleString(luxon.DateTime.DATETIME_SHORT);
                    }
                },
                {
                    title: l('BookingEndTime'),
                    data: 'endTime',
                    render: function (data) {
                        return luxon.DateTime.fromISO(data, { locale: abp.localization.currentCulture.name }).toLocaleString(luxon.DateTime.DATETIME_SHORT);
                    }
                },
                {
                    title: l('BookingPurpose'),
                    data: 'purpose'
                },
                {
                    title: l('BookingStatus'),
                    data: 'status',
                    render: function (data) {
                        var name = statusMap[data];
                        var badge = statusBadge[data];
                        return '<span class="badge bg-' + badge + '">' + l('BookingStatus:' + name) + '</span>';
                    }
                }
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewBookingButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
