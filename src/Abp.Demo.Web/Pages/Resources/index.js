$(function () {
    var l = abp.localization.getResource('Demo');
    var resourceService = abp.demo.resources.resource;

    var createModal = new abp.ModalManager(abp.appPath + 'Resources/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Resources/EditModal');

    var dataTable = $('#ResourcesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, 'asc']],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(resourceService.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('BookingSystem.Resources.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('BookingSystem.Resources.Delete'),
                                confirmMessage: function (data) {
                                    return l('AreYouSure');
                                },
                                action: function (data) {
                                    resourceService.delete(data.record.id).then(function () {
                                        abp.notify.info(l('SuccessfullyDeleted'));
                                        dataTable.ajax.reload();
                                    });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('ResourceName'),
                    data: 'name'
                },
                {
                    title: l('ResourceType'),
                    data: 'type',
                    render: function (data) {
                        return l('ResourceType:' + ['MeetingRoom', 'Workspace', 'Car'][data]);
                    }
                },
                {
                    title: l('ResourceLocation'),
                    data: 'location'
                },
                {
                    title: l('ResourceCapacity'),
                    data: 'capacity'
                },
                {
                    title: l('ResourceIsActive'),
                    data: 'isActive',
                    render: function (data) {
                        return data
                            ? '<i class="fa fa-check text-success"></i>'
                            : '<i class="fa fa-times text-danger"></i>';
                    }
                }
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewResourceButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
