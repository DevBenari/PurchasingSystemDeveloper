$(document).ready(function () {
    var table = new DataTable('#tblTemplate2', {
        "paging": false,
        "ordering": false,
        "searching": false
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