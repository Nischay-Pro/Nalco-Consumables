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
    else {
        document.getElementById("materials-main").style.display = "block";
        document.getElementById("materials-create").style.display = "none";
    }
}
var sectionsdata = ['logout', 'materials-main', 'materials-create'];
function SwitchTo(a, b) {
    document.getElementById(a).style.display = "block";
    document.getElementById(b).style.display = "none";
}
function MaterialPrinterCheck() {
}
function checkAddress(printer) {
    //if (printer.checked === true) {
    //    document.getElementById('printer-description').style.display = "block";
    //}
    //else {
    //    document.getElementById('printer-description').style.display = "none";
    //    document.getElementById('inputPrinterDescription').value = "";
    //}
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
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode > 31 && charCode < 48) || charCode > 57) {
        return false;
    }
    return true;
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
function RefreshMaterials() {
    SwitchTo('materials-main', 'materials-create');
    MaterialsList();
}
function CheckFormMaterials() {
    var data1 = { data: {} };
    data1.data['createquery'] = true;
    data1.data['materialcode'] = document.getElementById('inputMaterialCode').value;
    data1.data['materialdescription'] = document.getElementById('inputMaterialDescription').value;
    data1.data['materialprinter'] = document.getElementById('printer').checked;
    data1.data['materialprinterdescription'] = document.getElementById('inputPrinterDescription').value;
    data1.data['materialprintercount'] = 12;
    data1.data['materialquantity'] = document.getElementById('inputMaterialQuantity').value;
    data1.data['materialcriticalflag'] = document.getElementById('criticalflag').checked;
    data1.data['materialreorderlevel'] = document.getElementById('inputMaterialReorderLevel').value;
    var e = document.getElementById('selectstorage');
    data1.data['materialstorage'] = e.options[e.selectedIndex].text;
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('POST', 'api/Materials', true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    var datatobesent = JSON.stringify(data1);
    request.send(datatobesent);
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).status === 'created') {
            document.getElementById("materialerror").appendChild(CreateError('success', 'Successfully added Material.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'exists') {
            document.getElementById("materialerror").appendChild(CreateError('danger', 'Material already exists.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("materialerror").appendChild(CreateError('danger', JSON.parse(request.responseText).message));
        }
    };
}
function CreateError(type, message) {
    var element = document.createElement("div");
    element.innerHTML += '<div class="alert alert-dismissible alert-' + type + '" id="message"><button type="button" class="close" data-dismiss="alert">&times;</button>' + message + '</div>';
    return element;
}