
var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    if (url.includes("cancelled")) {
                        loadDataTable("cancelled");
                    }
                    else {
                        if (url.includes("shipped")) {
                            loadDataTable("shipped");
                        }
                        else {
                            if (url.includes("refunded")) {
                                loadDataTable("refunded");
                            }
                            else {
                                loadDataTable("all");
                            }
                        }
                    }
                }
            }
        }
    }

});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url:'/admin/order/getall?status=' + status},
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'applicationUser.email', "width": "15%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'paymentStatus', "width": "10%" },
            { data: 'paymentMethod', "width": "15%" },
            { data: 'productTotal', "width": "10%" },
          
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                            
                           </div>`
                },
                "width": "15%" 
            }

        ]

    });
    
}

