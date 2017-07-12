﻿var username, password;
if (getCookie('username') === "") {
    document.getElementById("mainpage").style.display = "block";
}
else {
    username = getCookie('username');
    password = getCookie('password');
    CheckUser(false);
}
if (typeof Array.prototype.forEach !== 'function') {
    Array.prototype.forEach = function (callback) {
        for (var i = 0; i < this.length; i++) {
            callback.apply(this, [this[i], i, this]);
        }
    };
}

function KeyPress(event) {
    if (event.which == 13 || event.keyCode == 13) {
        ShowMain();
    }
}

function IssuesCreate(event) {
    if (event.which == 13 || event.keyCode == 13) {
        CreateCentralStorageIssue();
    }
}

function IssuesCreateSub(event) {
    if (event.which == 13 || event.keyCode == 13) {
        CheckFormIssuesSub();
    }
}

function IssuesApprove(event) {
    if (event.which == 13 || event.keyCode == 13) {
        CheckFormIssuesApprove();
    }
}

function Add(event) {
    isNumber(event);
    if (event.which == 13 || event.keyCode == 13) {
        CheckFormMaterials();
    }
    return isNumber(event);
}

function Update(event) {
    isNumber(event);
    if (event.which == 13 || event.keyCode == 13) {
        CheckFormMaterialsUpdate();
    }
    return isNumber(event);
}

function AddVendor(event) {
    if (event.which == 13 || event.keyCode == 13) {
        CheckFormVendor();
    }
}

function UpdateVendor(event) {
    if (event.which == 13 || event.keyCode == 13) {
        CheckFormVendorUpdate();
    }
}

function ClearAddMaterials() {
    SwitchTo('materials-create', 'materials-main');
    document.getElementById("AddMaterialsForm").reset();
}

function ClearUpdateMaterials() {
    SwitchTo('materials-update', 'materials-main');
    document.getElementById("UpdateMaterialsForm").reset();
    document.getElementById("UpdateForm").style.display = "none";
}

function ClearCreateIssueCS() {
    SwitchTo('issues-create', 'issues-main');
    document.getElementById("CreateIssuesCSForm").reset();
}

function ClearCreateIssueSS() {
    SwitchTo('issues-substore-create', 'issues-main');
    document.getElementById("CreateIssuesSSForm").reset();
}

function ClearApproveIssue() {
    SwitchTo('issues-approve', 'issues-main');
    document.getElementById("ApproveIssuesForm").reset();
}

function ClearAddVendor() {
    SwitchTo('po-vendor-create', 'po-vendor');
    document.getElementById("AddVendorForm").reset();
}

function ClearUpdateVendor() {
    SwitchTo('po-vendor-update', 'po-vendor');
    document.getElementById("UpdateVendorForm").reset();
}

function ShowMain() {
    username = document.getElementById("username").value;
    password = document.getElementById("password").value;
    CheckUser(true);
}
function ShowMaterialsMain() {
    SwitchTo('materials-main', 'dashboard-nav');
    //document.getElementById("dashboard-nav").style.display = "none";
    //document.getElementById("materials-main").style.display = "block";
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
    evt = evt ? evt : window.event;
    var charCode = evt.which ? evt.which : evt.keyCode;
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
            document.getElementById("mainpage").style.display = "block";
            if (request.responseText === "\"Authentication Success\"") {
                document.getElementById("dashboard").style.display = "block";
                document.getElementById("login").style.display = "none";
                if (first === true) {
                    setCookie("username", document.getElementById("username").value, 1);
                    setCookie("password", document.getElementById("password").value, 1);
                }
                document.getElementById("username_holder").innerHTML = '<span class="glyphicon glyphicon-user"></span> ' + getCookie("username");
                GetUserName();
            }
            else {
                document.getElementById("mainpage").style.display = "block";
                document.getElementById("invalidcreds").appendChild(CreateError('danger', '<strong>Oh snap!</strong> Your Email or Password is incorrect.'));
            }
        }
    };
}
function GetUserName() {
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('GET', 'api/Employ/' + Number(username).pad(5), true);
    request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send();
    request.onreadystatechange = function () {
        if (request.readyState === 4) {
            if (JSON.parse(request.responseText).Message === "Authorization has been denied for this request.") {
            }
            else {
                document.getElementById("username_holder").innerHTML = '<span class="glyphicon glyphicon-user"></span> ' + Number(getCookie("username")).pad(5) + ' - ' + toTitleCase(JSON.parse(request.responseText).data[0].employ_name);
            }
        }
    }
}
function VendorList() {
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('GET', 'api/Vendor', true);
    request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send();
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).message !== 'Authorization has been denied for this request.') {
            var data = JSON.parse(request.responseText);
            $('#povendorlist').bootstrapTable({
                columns: [{
                    field: 'vendor_code',
                    title: 'Vendor Code'
                }, {
                    field: 'vendor_name',
                    title: 'Vendor Name'
                }, {
                    field: 'vendor_contact',
                    title: 'Vendor Contact Details'
                }],
                data: data['data']
            });
            $('#povendorlist').bootstrapTable("load", data['data']);
        }
    };
}
function POList() {
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('GET', 'api/POMin', true);
    request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send();
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).message !== 'Authorization has been denied for this request.') {
            var data = JSON.parse(request.responseText);
            $('#polist').bootstrapTable({
                columns: [{
                    field: 'po_number',
                    title: 'PO Number'
                }, {
                    field: 'po_vendor_code',
                    title: 'PO Vendor Code'
                }, {
                    field: 'po_inspection_report_no',
                    title: 'PO Inspection Report Number'
                }, {
                    field: 'po_approved_by',
                    title: 'PO Approved By'
                }, {
                    field: 'po_material_code',
                    title: 'PO Material Code'
                }, {
                    field: 'po_material_quantity',
                    title: 'PO Material Quantity'
                }],
                data: data['data']
            });
            $('#polist').bootstrapTable("load", data['data']);
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
        if (request.readyState === 4 && JSON.parse(request.responseText).message !== 'Authorization has been denied for this request.') {
            var data = JSON.parse(request.responseText);
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
                data: data['data']
            });
            $('#materialslist').bootstrapTable("load", data['data']);
        }
    };
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
            MaterialsList();
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'exists') {
            document.getElementById("materialerror").appendChild(CreateError('danger', 'Material already exists.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("materialerror").appendChild(CreateError('danger', JSON.parse(request.responseText).message));
        }
    };
}
function CheckFormVendor() {
    var data1 = { data: {} };
    data1.data['createquery'] = true;
    data1.data['vendorcode'] = document.getElementById('VendorCode').value;
    data1.data['vendorname'] = document.getElementById('VendorName').value;
    data1.data['vendorcontact'] = document.getElementById('VendorContact').value;
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('POST', 'api/Vendor', true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    var datatobesent = JSON.stringify(data1);
    request.send(datatobesent);
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).status === 'created') {
            document.getElementById("povendorerror").appendChild(CreateError('success', 'Successfully added Vendor.'));
            VendorList();
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'exists') {
            document.getElementById("povendorerror").appendChild(CreateError('danger', 'Vendor already exists.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("povendorerror").appendChild(CreateError('danger', JSON.parse(request.responseText).message));
        }
    };
}
function CheckFormVendorUpdate() {
    var data1 = { data: {} };
    data1.data['createquery'] = false;
    data1.data['vendorcode'] = document.getElementById('VendorCodeUpdate').value;
    data1.data['vendorname'] = document.getElementById('VendorNameUpdate').value;
    data1.data['vendorcontact'] = document.getElementById('VendorContactUpdate').value;
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('POST', 'api/Vendor', true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    var datatobesent = JSON.stringify(data1);
    request.send(datatobesent);
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).status === 'updated') {
            document.getElementById("vendorerrorupdate").appendChild(CreateError('success', 'Successfully updated Vendor.'));
            VendorList();
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'exists') {
            document.getElementById("vendorerrorupdate").appendChild(CreateError('danger', 'Vendor does not exist.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("vendorerrorupdate").appendChild(CreateError('danger', JSON.parse(request.responseText).message));
        }
    };
}
function CheckFormMaterialsUpdate() {
    var data1 = { data: {} };
    data1.data['createquery'] = false;
    data1.data['materialcode'] = document.getElementById('inputMaterialCodeUpdate').value;
    data1.data['materialdescription'] = document.getElementById('inputMaterialDescriptionUpdate').value;
    data1.data['materialprinter'] = document.getElementById('printerupdate').checked;
    if (document.getElementById('inputPrinterDescriptionUpdate').value !== null) {
        data1.data['materialprinterdescription'] = document.getElementById('inputPrinterDescriptionUpdate').value;
        document.getElementById('printer').checked = true;
        document.getElementById('printer-description-update').display = "block";
    }
    else {
        data1.data['materialprinterdescription'] = "";
        document.getElementById('printer').checked = false;
        document.getElementById('printer-description-update').display = "none";
    }
    data1.data['materialprintercount'] = 12;
    data1.data['materialquantity'] = document.getElementById('inputMaterialQuantityUpdate').value;
    data1.data['materialcriticalflag'] = document.getElementById('criticalflagupdate').checked;
    data1.data['materialreorderlevel'] = document.getElementById('inputMaterialReorderLevelUpdate').value;
    var e = document.getElementById('selectstorageupdate');
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
        if (request.readyState === 4 && JSON.parse(request.responseText).status === 'updated') {
            document.getElementById("materialerrorupdate").appendChild(CreateError('success', 'Successfully updated Material.'));
            MaterialsList();
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'exists') {
            document.getElementById("materialerrorupdate").appendChild(CreateError('danger', 'Material does not exist.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("materialerrorupdate").appendChild(CreateError('danger', JSON.parse(request.responseText).message));
        }
    };
}
function CheckFormMaterialsDelete() {
    smoke.confirm("Are you sure you want to delete?", function (e) {
        if (e) {
            var authorizationBasic = window.btoa(username + ':' + password);
            var request = new XMLHttpRequest();
            request.open('DELETE', 'api/Materials/' + document.getElementById('inputMaterialCodeUpdate').value, true);
            request.setRequestHeader('Content-Type', 'application/json');
            request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
            request.setRequestHeader('Accept', 'application/json');
            request.send();
            request.onreadystatechange = function () {
                if (request.readyState === 4 && JSON.parse(request.responseText).status === 'deleted') {
                    document.getElementById("materialerrorupdate").appendChild(CreateError('success', 'Successfully deleted Material.'));
                    MaterialsList();
                }
                else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'not exists') {
                    document.getElementById("materialerrorupdate").appendChild(CreateError('danger', 'Material does not exist.'));
                }
                else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
                    document.getElementById("materialerrorupdate").appendChild(CreateError('danger', JSON.parse(request.responseText).message));
                }
            };
        }
    }, {
        ok: "Yes",
        cancel: "No",
        classname: "custom-class",
        reverseButtons: true
    });
}
function CreateError(type, message) {
    var element = document.createElement("div");
    element.innerHTML += '<div class="alert alert-dismissible alert-' + type + '" id="message"><button type="button" class="close" data-dismiss="alert">&times;</button>' + message + '</div>';
    $(document).ready(function () {
        window.setTimeout(function () {
            $(".alert").fadeTo(1500, 0).slideUp(500, function () {
                $(this).remove();
            });
        }, 2000);
    });
    return element;
}

function LoadMaterialsUpdate() {
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('GET', 'api/Materials/' + document.getElementById('inputMaterialCodeUpdate').value, true);
    request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send();
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).status === 'not exists') {
            document.getElementById("materialerrorupdate").appendChild(CreateError('danger', 'Material does not exist.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("materialerrorupdate").appendChild(CreateError('danger', JSON.parse(JSON.parse(request.responseText)).message));
        }
        else if (request.readyState === 4) {
            var data = JSON.parse(request.responseText);
            document.getElementById('inputMaterialDescriptionUpdate').value = data.material_description;
            document.getElementById('printerupdate').checked = data.material_printer;
            document.getElementById('inputPrinterDescriptionUpdate').value = data.printer_description;
            document.getElementById('inputMaterialQuantityUpdate').value = data.material_quantity;
            document.getElementById('criticalflagupdate').checked = data.material_critical_flag;
            document.getElementById('inputMaterialReorderLevelUpdate').value = data.material_reorder_level;
            document.getElementById('UpdateForm').style.display = "block";
            if (data.material_printer) {
                document.getElementById('printer').checked = true;
                $('#printer-description-update').removeClass('hidden');
            }
            else {
                document.getElementById('printer').checked = false;
                $('#printer-description-update').addClass('hidden');
            }
            var textToFind = data.material_storage;
            var dd = document.getElementById('selectstorageupdate');
            for (var i = 0; i < dd.options.length; i++) {
                if (dd.options[i].text.split(" ").join("").indexOf(textToFind) !== -1) {
                    dd.selectedIndex = i;
                    break;
                }
            }
        }
    };
}
function LoadVendorUpdate() {
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('GET', 'api/Vendor/' + document.getElementById('VendorCodeUpdate').value, true);
    request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send();
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).status === 'not exists') {
            document.getElementById("vendorerrorupdate").appendChild(CreateError('danger', 'Vendor does not exist.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("vendorerrorupdate").appendChild(CreateError('danger', JSON.parse(JSON.parse(request.responseText)).message));
        }
        else if (request.readyState === 4) {
            var data = JSON.parse(request.responseText);
            document.getElementById('VendorNameUpdate').value = data.vendor_name;
            document.getElementById('VendorContactUpdate').value = data.vendor_contact;
            document.getElementById('UpdateFormVendor').style.display = "block";
        }
    };
}
function ShowIssuesMain() {
    document.getElementById('dashboard-nav').style.display = "none";
    document.getElementById('issues-main').style.display = "block";
}

function ShowPOMain() {
    SwitchTo('po-main', 'dashboard-nav');
    POList();
    VendorList();
}

function ShowReportsMain() {
    SwitchTo('reports-main', 'dashboard-nav');
}
function CheckPOReceiptMain() {
    var data1 = { data: {} };
    data1.data['createquery'] = true;
    data1.data['ponumber'] = document.getElementById('PONumber').value;
    data1.data['podate'] = document.getElementById('datePO').value;
    data1.data['poinspectionnumber'] = document.getElementById('POReportNumber').value;
    data1.data['povendorcode'] = document.getElementById('POVendor').value;
    data1.data['poapprovedby'] = Number(username).pad(5);
    data1.data['pomaterials'] = QuerifyPOReceipts();
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('POST', 'api/PO', true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    var datatobesent = JSON.stringify(data1);
    request.send(datatobesent);
    //console.log(datatobesent);
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).status === 'created') {
            document.getElementById("poerror").appendChild(CreateError('success', 'Successfully added PO Receipt.'));
            POList();
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'exists') {
            document.getElementById("poerror").appendChild(CreateError('danger', 'PO Receipt already exists.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'cannot be updated') {
            document.getElementById("poerror").appendChild(CreateError('danger', 'You cannot update PO Receipts.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("poerror").appendChild(CreateError('danger', JSON.parse(request.responseText).message));
        }
    };
}
function CreateCentralStorageIssue() {
    var data1 = { data: {} };
    data1.data['createquery'] = true;
    data1.data['substore'] = false;
    data1.data['sapvoucher'] = document.getElementById('CSIssueVoucherNumber').value;
    data1.data['materialcode'] = document.getElementById('CSIssueMaterialCode').value;
    data1.data['issuedate'] = document.getElementById('CSIssueDate').value;
    data1.data['issuequantity'] = document.getElementById('CSIssueQuantity').value;
    data1.data['issuecollectedby'] = document.getElementById('CSIssueCollectedBy').value;
    data1.data['issueto'] = document.getElementById('CSIssueTO').value;
    data1.data['issueapprovedby'] = Number(username).pad(5);
    data1.data['issuedepartment'] = document.getElementById('CSIssueDepartment').value;
    data1.data['issuelocation'] = document.getElementById('CSIssueLocation').value;
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('POST', 'api/Issues', true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    var datatobesent = JSON.stringify(data1);
    request.send(datatobesent);
    //console.log(datatobesent);
    request.onreadystatechange = function () {
        if (request.readyState === 4 && JSON.parse(request.responseText).status === 'created') {
            document.getElementById("csmaterialerror").appendChild(CreateError('success', 'Successfully filed an Issue Request.'));
            POList();
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'exists') {
            document.getElementById("csmaterialerror").appendChild(CreateError('danger', 'Issue already exists.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'cannot be updated') {
            document.getElementById("csmaterialerror").appendChild(CreateError('danger', 'You cannot update Issue Request.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'enough material not available') {
            document.getElementById("csmaterialerror").appendChild(CreateError('danger', 'Not enough material available to file an issue.'));
        }
        else if (request.readyState === 4 && JSON.parse(request.responseText).status === 'error') {
            document.getElementById("csmaterialerror").appendChild(CreateError('danger', JSON.parse(request.responseText).message));
        }
    };
}

function QuerifyPOReceipts() {
    var arrayobj = [];
    var objects = document.getElementsByClassName('clonedInput');
    var i = 0;
    Array.prototype.forEach.call(objects, function (element) {
        arrayobj[i] = {};
        var objectsra = element.getElementsByClassName('form-control');
        Array.prototype.forEach.call(objectsra, function (element2) {
            arrayobj[i][element2.id] = element2.value;
        });
        i = i + 1;
    });
    return arrayobj;
}

if (typeof Array.prototype.forEach !== 'function') {
    Array.prototype.forEach = function (callback) {
        for (var i = 0; i < this.length; i++) {
            callback.apply(this, [this[i], i, this]);
        }
    };
}
function toTitleCase(str) {
    return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
}
Number.prototype.pad = function (size) {
    var s = String(this);
    while (s.length < (size || 2)) { s = "0" + s; }
    return s;
}

Selectify('.js-data-example-ajax-1', null);
Selectify('.material-update-select', null);
Selectify('#CSIssueMaterialCode', JSON.stringify({ data: { storage: true, centralstorage: true } }));
SelectifyDepartment('#CSIssueDepartment', JSON.stringify({ data: { dept: true } }));
SelectifyDepartment('#CSIssueLocation', JSON.stringify({ data: { dept: false, deptcode: document.getElementById("CSIssueDepartment").value } }));

$('.material-update-select').on("select2:select", function (e) {
    LoadMaterialsUpdate();
});
$('#CSIssueMaterialCode').on("select2:select", function (e) {
    LoadMaterialCode();
});

$('#CSIssueDepartment').on("select2:select", function (e) {
    document.getElementById('CSIsssueCleaner').innerHTML = '<select id="CSIssueLocation" class="department-code-issue-substore form-control"> <option disabled selected>Select your Working Location</option> </select> <button type="button" class="btn btn-primary btn-xs" onclick="ClearIssueForm()">Clear Location</button>';
    SelectifyDepartment('#CSIssueLocation', JSON.stringify({ data: { dept: false, deptcode: document.getElementById("CSIssueDepartment").value } }));
    $('#CSIssueLocationHolder').removeClass('hidden');
});

function Selectify(classname, dataheader) {
    $(classname).select2({
        ajax: {
            url: function (params) {
                return "http://localhost:56797/api/MaterialsSearch/" + params.term;
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Basic " + window.btoa(username + ":" + password));
            },
            data: dataheader,
            dataType: 'json',
            delay: 250,
            allowClear: true,
            processResults: function (data, params) {
                console.log(data);
                data.data.forEach(function (entry, index) {
                    entry.id = '' + entry.material_code;
                });
                return {
                    results: data.data,
                    pagination: true
                };
            },
            formatNoResults: function () {
                return "No results found";
            },
            formatAjaxError: function () {
                return "Connection Error";
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; },
        minimumInputLength: 1,
        templateResult: formatRepo,
        templateSelection: formatRepoSelection
    });
};
function formatRepo(repo) {
    if (repo.loading) return repo.text;

    var markup = "<div class='select2-result-repository clearfix' style='color:#464545!important'>" +
        "<div class='select2-result-repository__meta'>" +
        "<div class='select2-result-repository__title'>" + repo.material_code + "</div>";

    if (repo.material_description) {
        markup += "<div class='select2-result-repository__description'>" + repo.material_description + "</div>";
    }

    markup += "<div class='select2-result-repository__statistics'>" +
        "<div class='select2-result-repository__forks'><i class='fa fa-archive'></i> Quantity: " + repo.material_quantity + "</div>" +
        "<div class='select2-result-repository__stargazers'><i class='fa fa-level-up'></i> Reorder Level: " + repo.material_reorder_level + "</div>" +
        "<div class='select2-result-repository__watchers'><i class='fa fa-location-arrow'></i> Storage: " + repo.material_storage + "</div>" +
        "</div></div>";
    return markup;
}

function formatRepoSelection(repo) {
    return repo.full_name || repo.text || repo.material_code + ' - ' + repo.material_description;
}

function SelectifyDepartment(classname, dataheader) {
    $(classname).select2({
        ajax: {
            url: function (params) {
                return "http://localhost:56797/api/EmploySearch/" + params.term;
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Basic " + window.btoa(username + ":" + password));
                xhr.setRequestHeader("content-type", "application/json");
            },
            type: "POST",
            data: dataheader,
            dataType: 'json',
            delay: 250,
            processResults: function (data, params) {
                data.data.forEach(function (entry, index) {
                    entry.id = '' + entry.employ_dept_cd;
                });
                return {
                    results: data.data,
                    pagination: true
                };
            },
            formatNoResults: function () {
                return "No results found";
            },
            formatAjaxError: function () {
                return "Connection Error";
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; },
        minimumInputLength: 1,
        templateResult: formatRepoDepartment,
        templateSelection: formatRepoSelectionDepartment
    });
};
function formatRepoDepartment(repo) {
    if (repo.loading) return repo.text;

    var markup = "<div class='select2-result-repository clearfix' style='color:#464545!important'>" +
        "<div class='select2-result-repository__meta'>" +
        "<div class='select2-result-repository__title'>";
    if (repo.employ_loc_cd) {
        markup += repo.employ_loc_cd + "</div>";
    }
    else {
        markup += repo.employ_dept_cd + "</div>";
    }
    if (repo.employ_loc_name) {
        markup += "<div class='select2-result-repository__description'>" + repo.employ_loc_name + "</div>";
    }
    else {
        markup += "<div class='select2-result-repository__description'>" + repo.employ_dept_name + "</div>";
    }
    return markup;
}

function formatRepoSelectionDepartment(repo) {
    if (repo.employ_loc_name) {
        return repo.employ_loc_cd + ' - ' + repo.employ_loc_name;
    }
    return repo.full_name || repo.text || repo.employ_dept_cd + ' - ' + repo.employ_dept_name;
}

function ClearIssueForm() {
    document.getElementById('CSIsssueCleaner').innerHTML = '<select id="CSIssueLocation" class="department-code-issue-substore form-control"> <option disabled selected>Select your Working Location</option> </select> <button type="button" class="btn btn-primary btn-xs" onclick="ClearIssueForm()">Clear Location</button>';
    SelectifyDepartment('#CSIssueLocation', JSON.stringify({ data: { dept: false, deptcode: document.getElementById("CSIssueDepartment").value } }));
}

function LoadMaterialCode() {
    var tablestructure = '<table class="table table-striped table-hover "><thead><tr> <th>Material Properties</th></tr></thead><tbody> ';
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    request.open('GET', 'api/Materials/' + document.getElementById('CSIssueMaterialCode').value, true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send();
    request.onreadystatechange = function () {
        if (request.readyState === 4) {
            var result = request.responseText;
            tablestructure += CleanseData(result, 'material_code', 'Material Code');
            tablestructure += CleanseData(result, 'material_description', 'Material Description');
            tablestructure += CleanseData(result, 'material_quantity', 'Material Quantity');
            tablestructure += CleanseData(result, 'material_critical_flag', 'Material Critical?');
            tablestructure += CleanseData(result, 'material_reorder_level', 'Material Reorder Level');
            tablestructure += '</tbody></table>';
            document.getElementById('CSIssueMaterialTableHolder').innerHTML = '';
            document.getElementById('CSIssueMaterialTableHolder').innerHTML = tablestructure
        }
    }
}

function CleanseData(json, jsonname, fieldname) {
    var jsonserialize = JSON.parse(json)
    var code = '<tr><td>' + fieldname + '</td> <td>' + jsonserialize[jsonname] + '</td></tr>'
    return code;
}

function Experiment() {
    var authorizationBasic = window.btoa(username + ':' + password);
    var request = new XMLHttpRequest();
    var data1 = { data: {} }
    data1.data["description"] = null;
    data1.data["department"] = null;
    data1.data["location"] = null;
    request.open('POST', 'api/UsersSearch', true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.setRequestHeader('Authorization', 'Basic ' + authorizationBasic);
    request.setRequestHeader('Accept', 'application/json');
    request.send(JSON.stringify(data1));
    request.onreadystatechange = function () {
        if (request.readyState === 4) {
            console.log(request.responseText);
        }
    }
}