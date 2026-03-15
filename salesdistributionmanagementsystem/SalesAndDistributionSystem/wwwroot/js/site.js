
$('.simple-date2 .input-group.date').datepicker({
    format: 'dd/mm/yyyy',
    todayBtn: 'linked',
    todayHighlight: true,
    autoclose: true,
});

//donot erase this
listree();
// Delegate event handling to a parent element that exists when the page loads
$(document).on("keydown", 'input[type="number"]', function (event) {
    if (event.key === "ArrowUp" || event.key === "ArrowDown" || event.key === "-" || event.key === "E" || event.key === "e") {
        event.preventDefault();
    }
}).on("focus", 'input[type="number"]', function (event) {
    // Select the value of the input field
    event.target.select();
}).on('focus', 'input[type=number]', function (e) {
    $(this).on('wheel.disableScroll', function (e) {
        e.preventDefault();
    });
}).on('blur', 'input[type=number]', function (e) {
    $(this).off('wheel.disableScroll');
});

$("#txt_search").keyup(function () {
    var MENU_NAME = $(this).val();

    if (MENU_NAME != "") {
        $.ajax({
            type: 'post',
            method: 'post',
            url: "/Security/MenuPermission/SearchableMenuLoad",
            data: JSON.stringify(MENU_NAME),
            dataType: 'json',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },

            success: function (data) {
                var len = data.length;
                $("#searchResult").empty();
                for (var i = 0; i < len; i++) {
                    var href = data[i]['HREF'];
                    var name = data[i]['MENU_NAME'];
                    var id_ = JSON.stringify(data[i]['MENU_ID']) + '_' + JSON.stringify(data[i]['PARENT_MENU_ID']) + '_' + JSON.stringify(data[i]['MODULE_ID']);

                    $("#searchResult").append("<li><a class='collapse-item_top' id=" + id_ + " href='/" + href + "'>" + name + "</li>");
                    // binding click event to li

                }


                //$("#searchResult li").bind("click", function () {
                //    setText(this);
                //});
            }
        });
    }

});
//function setText(element) {
//    setCook(element.children[0].id);
//}
//function setCook(this_id) {
//    var Value = this_id;
//    $.ajax({
//        type: 'post',
//        method: 'post',
//        url: '/Security/MenuPermission/MenuCookieHolerSet',
//        dataType: "json",
//        headers: { 'Content-Type': 'application/json; charset=utf-8' },

//        data: JSON.stringify(Value),
//        success: function (data) {

//        }

//    });
//};
//function getnavselection() {

//    $.ajax({
//        type: 'post',
//        method: 'post',
//        url: '/Security/MenuPermission/NavCookieHolderGet',
//        dataType: "json",
//        headers: { 'Content-Type': 'application/json; charset=utf-8' },
//        success: function (data) {
//            //$('#' + data).parent().addClass("active");
//            //$('#' + data).css("display", "block");
//            if (data == 1) {
//                $('.nav_side_show').addClass('toggled');
//                $('.body_resize_show').addClass('sidebar-toggled');


//            } else {
//                $('.nav_side_show').removeClass('toggled');
//                $('.body_resize_show').removeClass('sidebar-toggled');
//            }

//        },
//        error: function (data) {
//            if (data == 1) {
//                $('#accordionSidebar').addClass('toggled');
//                $('#page-top').addClass('sidebar-toggled');


//            } else {
//                $('#accordionSidebar').removeClass('toggled');
//                $('#page-top').removeClass('sidebar-toggled');
//            }
//        }

//    });
//}



//var dropdown = document.getElementsByClassName("dropdown-btn");
//var i;

//for (i = 0; i < dropdown.length; i++) {
//    dropdown[i].addEventListener("click", function () {
//        this.classList.toggle("active");
//        var dropdownContent = this.nextElementSibling;
//        if (dropdownContent.style.display === "block") {
//            dropdownContent.style.display = "none";
//        } else {
//            dropdownContent.style.display = "block";
//        }
//    });
//}
$(document).ready(function () {
    getMenuPermission();
    NotificationLoad();
    NotificationGet();
});
//-------hold menu----------
//$('.holdMenu').click(function () {
//    let value = this.id;
//    $.ajax({
//        type: 'post',
//        method: 'post',
//        url: '/Security/MenuPermission/MenuCookieHolerSet',
//        headers: { 'Content-Type': 'application/json; charset=utf-8' },
//        data: JSON.stringify(value),
//        success: function (data) {
//            console.log('MenuCookieHolerSet success:', data);
//        },
//        error: function (xhr, status, error) {
//            console.error('MenuCookieHolerSet error:', error);
//        }
//    });
//});


//$('#sidebarToggleTop').click(function () {
//    let value = document.getElementById('page-top').className;
//    $.ajax({
//        type: 'post',
//        method: 'post',
//        url: '/Security/MenuPermission/NavCookieHolderSet',
//        headers: { 'Content-Type': 'application/json; charset=utf-8' },
//        data: JSON.stringify(value),
//        success: function (data) {
//            console.log('NavCookieHolderSet success:', data);
//        },
//        error: function (xhr, status, error) {
//            console.error('NavCookieHolderSet error:', error);
//        }
//    });

//    //$.ajax({
//    //    type: 'post',
//    //    method: 'post',
//    //    url: '/Security/MenuPermission/NavCookieHolderSet',
//    //    dataType: "json",
//    //    headers: { 'Content-Type': 'application/json; charset=utf-8' },

//    //    data: '',
//    //    success: function (data) {
//    //    }

//    //});
//});
function findElementIdByUrl(url) {
    // Get all <a> elements inside #accordionSidebar
    const links = document.querySelectorAll('#accordionSidebar a');
    // Loop through all links
    for (let link of links) {
        // Check if the href attribute matches the provided URL
        if (link.getAttribute('href') === url) {
            // Return the id attribute of the matched element
            return link.getAttribute('id');
        }
    }
    // Return null if no match is found
    return null;
}
function getMenuPermission() {
    let pathname = window.location.pathname;
    let data = findElementIdByUrl(pathname);
    $('#' + data).addClass("active").css("display", "block");
    updateParentElements(data)
    // Collapse sidebar elements if toggled
    if ($(".sidebar").hasClass("toggled")) {
        $('.sidebar .collapse').collapse('hide');
    }

    //    $.ajax({
    //        type: 'post',
    //        method: 'post',
    //        url: '/Security/MenuPermission/MenuCookieHolerGet',
    //        headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //        success: function (data) {
    //            $('#' + data).addClass("active").css("display", "block");
    //            updateParentElements(data)
    //            // Collapse sidebar elements if toggled
    //            if ($(".sidebar").hasClass("toggled")) {
    //                $('.sidebar .collapse').collapse('hide');
    //            }
    //        },
    //        error: function (data) {
    //            console.error("An error occurred:", data);
    //            // Handle error here if needed
    //        }
    //    });
}
function updateParentElements(anchorId) {
    let anchorElement = $("#" + anchorId);
    let parentUl = anchorElement.closest("ul.listree-submenu-items");
    let parentDiv = parentUl.closest("li").children(".listree-submenu-heading");
    parentDiv.removeClass("collapsed").addClass("expanded");
    parentUl.css("display", "block");
    if (parentUl.closest("ul.listree-submenu-items").length) {
        let parentId = parentDiv.attr("id");
        updateParentElements(parentId);
    }
    updateLiClass(anchorId)
}
function updateLiClass(liId) {
    $("#" + liId).closest("li.nav-item").addClass("active");
    $("#" + liId).closest("li.nav-item").find("a.nav-link.collapsed").removeClass("collapsed");
    $("#" + liId).closest("li.nav-item").find("div.collapse").addClass("collapse show");

}



function setCook(this_id) {
    var Value = this_id;
    $.ajax({
        type: 'post',
        method: 'post',
        url: '/Security/MenuPermission/MenuCookieHolerSet',
        dataType: "json",
        headers: { 'Content-Type': 'application/json; charset=utf-8' },

        data: JSON.stringify(Value),
        success: function (data) {

        }

    });
};
function ViewedByNotification(this_id) {
    var Value = this_id;
    $.ajax({
        type: 'get',
        method: 'get',
        url: '/Security/Notification/UpdateNotificationViewStatus',
        dataType: "json",
        headers: { 'Content-Type': 'application/json; charset=utf-8' },

        data: { ID: Value },
        success: function (data) {
            NotificationLoad();
        }

    });
};
function ViewedByUserId() {
    $.ajax({
        type: 'post',
        method: 'post',
        url: '/Security/Notification/UpdateNotificationViewStatusByUser',
        dataType: "json",
        headers: { 'Content-Type': 'application/json; charset=utf-8' },

        data: '',
        success: function (data) {
            NotificationLoad();
        }

    });
};
function ViewAllByUserId() {
    //window.location.href = "/Security/Notification/List";
    window.open(
        "/Security/Notification/List", "_blank");

};
function setText(element) {
    setCook(element.children[0].id);
}
function GetNotify(User_list, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

    $.ajax({
        type: 'post',
        method: 'post',
        url: '/Security/Notification/GetUser',
        dataType: "json",
        headers: { 'Content-Type': 'application/json; charset=utf-8' },

        data: '',
        success: function (data) {
            var User_Id = data;
            for (var i = 0; i < User_list.length; i++) {
                if (User_Id == User_list[i]) {
                    NotificationLoad();


                    $.notify({
                        title: 'Live Notify',
                        message: msg,
                        icon: 'glyphicon glyphicon-ok',
                        url: '',
                        target: '_blank'
                    }, {
                        element: 'body',
                        type: "success",
                        showProgressbar: false,
                        placement: {
                            from: "top",
                            align: "right"
                        },
                        offset: 20,
                        spacing: 10,
                        z_index: 1031,
                        delay: 3300,
                        timer: 1000,
                        url_target: '_blank',
                        mouse_over: null,
                        animate: {
                            enter: 'animated fadeInDown',
                            exit: 'animated fadeOutRight'
                        },
                        onShow: null,
                        onShown: null,
                        onClose: null,
                        onClosed: null,
                        icon_type: 'class',
                    });
                }
            }
        }

    });
};
function NotificationGet() {

    var Notification_Data = [];
    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationhub").build();
    connection.start();
    connection.on("ReceiveMessageHandler", function (User_list, message) {

        GetNotify(User_list, message);

    });

}
function NotificationLoad() {

    $.ajax({
        type: 'get',
        method: 'get',
        url: '/Security/Notification/NotificationLoad',
        dataType: "json",
        headers: { 'Content-Type': 'application/json; charset=utf-8' },

        data: '',
        success: function (data) {

            var Notification_Data = data;
            var Notification_Count = Notification_Data.length;
            $('#Notification_Count').html(Notification_Count);
            var list_add = "<h6 class='dropdown-header'>Click on Notification to Mark as Read</h6>";

            for (var i = 0; i < Notification_Data.length; i++) {
                var Notify = "<a class='dropdown-item d-flex align-items-center' onclick='ViewedByNotification(" + Notification_Data[i].ID + ")'>" +
                    "<div class='mr-3'>" +
                    "<div class='icon-circle bg-primary'>" +
                    "<i class='fas fa-file-alt text-white'></i>" +
                    "</div>" +
                    "</div>" +
                    "<div>" +
                    "<div class='small text-gray-500'>" + Notification_Data[i].NOTIFICATION_DATE + "</div>" +

                    "<span class='font-weight-bold'>" + Notification_Data[i].NOTIFICATION_BODY + "</span>" +

                    "</div>" +
                    "</a>"
                list_add = list_add + Notify;
            }
            $('#Notification_body').html(list_add);

        }

    });
}
function setnavselection() {
    $.ajax({
        type: 'post',
        method: 'post',
        url: '/Security/MenuPermission/NavCookieHolderSet',
        dataType: "json",
        headers: { 'Content-Type': 'application/json; charset=utf-8' },

        data: '',
        success: function (data) {
            debugger
        }

    });
}