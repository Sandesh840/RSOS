$(document).ready(function () {
    "use strict";
    $("#basic-datatable").DataTable({
        keys: !0,
        language: {
            paginate: {
                previous: "<i class='mdi mdi-chevron-left'>",
                next: "<i class='mdi mdi-chevron-right'>"
            }
        },
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
        },
        "lengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]]
    }),

        $("#scroll-vertical-datatable").DataTable({
            scrollY: "350px",
            scrollCollapse: !0,
            paging: !1,
            language: {
                paginate: {
                    previous: "<i class='mdi mdi-chevron-left'>",
                    next: "<i class='mdi mdi-chevron-right'>"
                }
            },
            drawCallback: function () {
                $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
            }
        })


});

function BasicDatatable() {
    $(".basic-datatable").DataTable({
        destroy: true,
        keys: !0,
        language: {
            paginate: {
                previous: "<i class='mdi mdi-chevron-left'>",
                next: "<i class='mdi mdi-chevron-right'>"
            }
        },
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
        },
        "lengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]]
    });

    $('[data-bs-toggle="tooltip"]').tooltip();
}

function ScrollVerticalTable() {
    $(".scroll-vertical-datatable").DataTable({
        scrollY: "350px",
        scrollCollapse: !0,
        paging: !1,
        language: {
            paginate: {
                previous: "<i class='mdi mdi-chevron-left'>",
                next: "<i class='mdi mdi-chevron-right'>"
            }
        },
        drawCallback: function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded")
        }
    })
}


