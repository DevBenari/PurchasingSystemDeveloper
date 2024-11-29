// Inisialisasi koneksi SignalR
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

// Event saat koneksi mulai
connection.start().then(function () {
    console.log("SignalR connected.");
}).catch(function (err) {
    return console.error(err.toString());
});

// Fungsi untuk mengambil data
function refreshData() {
    $.ajax({
        url: '@Url.Action("CountNotifikasi", "Home")', // Sesuaikan URL dengan controller Anda
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                // Menampilkan total Approval Notif
                $("#notifikasiCount").text(response.totalJsonAllNotification);
                // Menyembunyikan atau menampilkan notifikasiCount
                if (response.totalJsonAllNotification === 0) {
                    $("#notifikasiCount").hide(); // Sembunyikan jika 0
                } else {
                    $("#notifikasiCount").show(); // Tampilkan jika tidak 0
                }

                // Mengambil data logger dari response
                var loggerDataPR = response.loggerDataJsonPR;
                var loggerDataQtyDiff = response.loggerDataJsonQtyDiff;
                var loggerDataUnitReq = response.loggerDataJsonUnitReq;

                // Bersihkan konten sebelumnya, kecuali footer
                $("#loggerContainer").empty();

                const combineData = loggerDataPR.concat(loggerDataQtyDiff, loggerDataUnitReq);

                combineData.forEach(item => {
                    item.dateObj = new Date(item.createdDate);  // tambahkan properti dateObj untuk pengurutan
                });

                // combineData.sort();
                combineData.sort((a, b) => b.dateObj - a.dateObj);

                combineData.forEach(item => {
                    const day = String(item.dateObj.getDate()).padStart(2, '0');
                    const month = String(item.dateObj.getMonth() + 1).padStart(2, '0'); // Month is 0-indexed
                    const year = item.dateObj.getFullYear();
                    const hours = String(item.dateObj.getHours()).padStart(2, '0');
                    const minutes = String(item.dateObj.getMinutes()).padStart(2, '0');
                    item.formattedDate = `${day}/${month}/${year} ${hours}:${minutes}`; // Simpan dalam format dd-mm-yyyy
                });

                combineData.forEach(item => {
                    if (item.purchaseRequestNumber != undefined) {
                        var urlPR = '@Url.Action("DetailApproval", "Approval", new { area = "Order", id = "__id__" })'.replace("__id__", item.approvalId);
                        var content =
                            `<a href="${urlPR}" class="dropdown-item">
                                        <div class="media">
                                            <div class="media-body">
                                                <div class="info-box-content">
                                                    <p class="text-sm text-muted">Approval Purchase Request</p>
                                                    <h3 class="dropdown-item-title" style="color:#347FC4"><b>${item.purchaseRequestNumber} Has Been Created</b></h3>
                                                    <p class="text-sm text-muted">By ${item.createdBy} || ${item.formattedDate}</p>
                                                </div>
                                            </div>
                                        </div>
                                    </a>
                                    <div class="dropdown-divider"></div>`;

                        // Append ke loggerContainer
                        $("#loggerContainer").append(content);
                    }
                    else if (item.qtyDifferenceNumber != undefined) {
                        var urlQtyDiff = '@Url.Action("DetailApprovalQtyDifference", "ApprovalQtyDifference", new { area = "Order", id = "__id__" })'.replace("__id__", item.approvalId);
                        var content =
                            `<a href="${urlQtyDiff}" class="dropdown-item">
                                        <div class="media">
                                            <div class="media-body">
                                                <div class="info-box-content">
                                                    <p class="text-sm text-muted">Approval Qty Difference</p>
                                                    <h3 class="dropdown-item-title" style="color:#758E4F"><b>${item.qtyDifferenceNumber} Has Been Created</b></h3>
                                                    <p class="text-sm text-muted">By ${item.createdBy} || ${item.formattedDate}</p>
                                                </div>
                                            </div>
                                        </div>
                                    </a>
                                    <div class="dropdown-divider"></div>`;

                        // Append ke loggerContainer
                        $("#loggerContainer").append(content);
                    }
                    else if (item.unitRequestNumber != undefined) {
                        var urlUnitReq = '@Url.Action("DetailApprovalUnitRequest", "ApprovalUnitRequest", new { area = "Warehouse", id = "__id__" })'.replace("__id__", item.approvalId);
                        var content =
                            `<a href="${urlUnitReq}" class="dropdown-item">
                                        <div class="media">
                                            <div class="media-body">
                                                <div class="info-box-content">
                                                    <p class="text-sm text-muted">Approval Unit Request</p>
                                                    <h3 class="dropdown-item-title" style="color:#F26419"><b>${item.unitRequestNumber} Has Been Created</b></h3>
                                                    <p class="text-sm text-muted">By ${item.createdBy} || ${item.formattedDate}</p>
                                                </div>
                                            </div>
                                        </div>
                                    </a>
                                    <div class="dropdown-divider"></div>`;

                        // Append ke loggerContainer
                        $("#loggerContainer").append(content);
                    }
                });

                // Menambahkan tautan "See All Messages" setelah loop
                if (loggerDataPR.length > 0 && loggerDataQtyDiff.length == 0 && loggerDataUnitReq.length == 0) {
                    var footerLink =
                        `<a href="${'@Url.Action("Index", "Approval", new { area = "Order" })'}" class="dropdown-item dropdown-footer"><strong>See All Messages</strong></a>`;
                } else if (loggerDataQtyDiff.length > 0 && loggerDataPR.length == 0 && loggerDataUnitReq.length == 0) {
                    var footerLink =
                        `<a href="${'@Url.Action("Index", "ApprovalQtyDifference", new { area = "Order" })'}" class="dropdown-item dropdown-footer"><strong>See All Messages</strong></a>`;
                } else if (loggerDataUnitReq.length > 0 && loggerDataPR.length == 0 && loggerDataQtyDiff.length == 0) {
                    var footerLink =
                        `<a href="${'@Url.Action("Index", "ApprovalUnitRequest", new { area = "Warehouse" })'}" class="dropdown-item dropdown-footer"><strong>See All Message</strong></a>`;
                } else if (loggerDataPR.length > 0 || loggerDataQtyDiff.length > 0 || loggerDataUnitReq.length > 0) {
                    var footerLink =
                        `<a href="#" class="dropdown-item dropdown-footer"><strong>Select data to your needs</strong></a>`;
                } else {
                    var footerLink =
                        `<a href="#" class="dropdown-item dropdown-footer"><strong>No Messages</strong></a>`;
                }
                $("#loggerContainer").append(footerLink);
            } else {
                console.log(response.message); // Jika ada pesan error
            }
        },
        error: function (xhr, status, error) {
            console.log("Error: " + error); // Menangani error
        }
    });
}

// Ketika menerima update dari SignalR
connection.on("UpdateDataCount", function () {
    // Panggil fungsi refreshData untuk mendapatkan data terbaru
    refreshData();
});

$(document).ready(function () {
    // Panggil refreshData saat halaman dimuat pertama kali
    refreshData();
});