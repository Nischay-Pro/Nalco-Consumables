var username, password;
if (getCookie('username') === "") {
}
else {
    username = getCookie('username');
    password = getCookie('password');
    CheckUser(false);
}
if (typeof Array.prototype.forEach != 'function') {
    Array.prototype.forEach = function (callback) {
        for (var i = 0; i < this.length; i++) {
            callback.apply(this, [this[i], i, this]);
        }
    };
}
function ShowMain() {
    username = document.getElementById("username").value;
    password = document.getElementById("password").value;
    CheckUser(true);
}
function ShowMaterialsMain() {
    document.getElementById("dashboard-nav").style.display = "none";
    document.getElementById("materials-main").style.display = "block";
    MaterialsList();
}
function ShowMaterialCreate(reverse) {
    if (reverse) {
        document.getElementById("materials-main").style.display = "none";
        document.getElementById("materials-create").style.display = "block";
    }
    else
    {
        document.getElementById("materials-main").style.display = "block";
        document.getElementById("materials-create").style.display = "none";
    }
}
var sectionsdata = ['logout', 'materials-main', 'materials-create'];
function SwitchTo(a,b) {
    document.getElementById(a).style.display = "block";
    document.getElementById(b).style.display = "none";
}
function deleteAllCookies() {
    var cookies = document.cookie.split(";");

    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        var eqPos = cookie.indexOf("=");
        var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
}
function LogOut() {
    deleteAllCookies();
    document.getElementById("dashboard").style.display = "none";
    document.getElementById("logout").style.display = "block";
}
function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + exdays * 24 * 60 * 60 * 1000);
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}
function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
function CheckUser(first) {
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('POST', 'api/Users', true);
    request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send();
    request.onreadystatechange = function () {
        if (request.readyState === 4) {
            if (request.responseText === "\"Authentication Success\"") {
                document.getElementById("dashboard").style.display = "block";
                document.getElementById("login").style.display = "none";
                if (first == true) {
                    setCookie("username", document.getElementById("username").value, 1);
                    setCookie("password", document.getElementById("password").value, 1);
                }
                document.getElementById("username_holder").innerText = getCookie("username");
                document.getElementById("invalidcreds").style.display = "none";
            }
            else {
                document.getElementById("invalidcreds").style.display = "block";
            }
        }
    };
}
function MaterialsList() {
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('GET', 'api/Materials', true);
    request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send();
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).message != 'Authorization has been denied for this request.') {
            var data = JSON.parse(request.responseText);
            data = data.replace(/\s/g, "");
            data = JSON.parse(data);
            $('#materialslist').bootstrapTable({
                columns: [{
                    field: 'material_code',
                    title: 'Material Code'
                }, {
                    field: 'material_description',
                    title: 'Material Description'
                }, {
                    field: 'material_printer',
                    title: 'Material Printer'
                }, {
                    field: 'material_printer_description',
                    title: 'Material Printer Description'
                }, {
                    field: 'material_printer_count',
                    title: 'Material Printer Count'
                }, {
                    field: 'material_quantity',
                    title: 'Material Quantity'
                }, {
                    field: 'material_critical_flag',
                    title: 'Material Critical Flag'
                }, {
                    field: 'material_reorder_level',
                    title: 'Material Reorder Level'
                }, {
                    field: 'material_storage',
                    title: 'Material Storage'
                }],
                data: data
            });
        }
    }
}