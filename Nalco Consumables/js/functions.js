function ShowMain(obj) {
    document.getElementById("dashboard").style.display = "block";
    document.getElementById("login").style.display = "none";
    setCookie("username", document.getElementById("username").value, 1);
    document.getElementById("username_holder").innerText = getCookie("username");
}

function ShowMaterialsMain() {
    document.getElementById("dashboard-nav").style.display = "none";
    document.getElementById("materials-main").style.display = "block";
}
function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}
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