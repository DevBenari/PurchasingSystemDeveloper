$(document).ready(function () {
    var table = new DataTable('#tblTemplate', {
        columnDefs: [
            {
                paging: true,
                searching: true,
                ordering: true,
                info: true,
                pageLength: 10,
                lengthChange: false
            }
        ]
    });

    table
        .on('order.dt search.dt', function () {
            let i = 1;

            table
                .cells(null, 0, { search: 'applied', order: 'applied' })
                .every(function (cell) {
                    this.data(i++);
                });
        }).draw();    
})