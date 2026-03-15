function openCity(e, t) {
    var n, a, o;
    document.cookie = "_modname=" + t + "; max-age=" + 1 * 24 * 60 * 60;
    //document.cookie =  t + "; max-age=" + 1 * 24 * 60 * 60;;
    for (a = document.getElementsByClassName("tabcontent"),
        n = 0; n < a.length; n++)a[n].style.display = "none";
    for (o = document.getElementsByClassName("tablinks"),
        n = 0; n < o.length; n++)o[n].className = o[n].className.replace(" active", "");
    document.getElementById(t).style.setProperty("display", "block", "important"),
        document.cookie = t
    $.ajax({
        method: 'post', url: "/BBase/SetMenuName", data: { value:t }
    }).done((result) => {
        if (result.length > 0)
            document.getElementById(result).style.setProperty("display", "block", "important");
    });
}
//document.getElementById(document.cookie).style.setProperty("display", "block", "important"),
function setBranch(value, value2) {
    if (confirm('Are you sure to change your branch?')) {
        $.ajax({
            method: 'post', url: "/BBase/SetBranchName", data: { value, value2 }
        }).done((result) => {
            if (result.length > 0) {
                $('#branch_name').text(result)
                window.location.reload();
            }
        });
    }
}

function getBranch() {
    $.ajax({
        method: 'get', url: '/branch/GetBranchesByUser'
    }).done((result) => {
        let branches = JSON.parse(result);
        let branchesDiv = document.querySelector('#branches');
        let listOfBranches = Branches(branches);
        branchesDiv.innerHTML = (listOfBranches);   
    })
}

function Branches(branches) {
    return branches.map((branch) => {
        if (branch.branch_name !== null && branch.branch_id !== null)
            return `<li><a href="javascript:void(0)" class="dropdown-item branch" onclick="setBranch('${branch.branch_name}', ${branch.branch_id})">${branch.branch_name}</a></li>`
        else {
            return `<li><a href="javascript:void(0)" class="dropdown-item branch")">No Branch found</a></li>`;
        }
    }).join('');
};
function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

$(document).ready(function (e) { var t = $("nav").height() + 1; $(".o_control_panel").animate({ marginTop: t > 45 ? 45 : t, marginBottom: 0 }) });
getBranch(); 