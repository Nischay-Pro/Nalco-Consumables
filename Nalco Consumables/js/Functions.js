function ShowMaterial() {
    SuperHide("materials-main");
};
function ShowIssue() {
    SuperHide("issues-main");
};
function ShowPO() {
    SuperHide("po-main");
};
function ShowReport() {
    SuperHide("report-main");
};
function switchTo(b,a) {
    var obj = document.getElementById(a);
    obj.style.display = "none";
    var obj = document.getElementById(b);
    obj.style.display = "block";
};

var sectionsthere = [
    "materials-main", "issues-main", "po-main", "report-main", "materials-create"
];
function SuperHide(antihide) {
    sectionsthere.forEach(function (element) {
        if (antihide != element) {
            var obj = document.getElementById(element);
            obj.style.display = "none";
        }
    })
    var obj = document.getElementById(antihide);
    obj.style.display = "block";;
};



//Startup
function startup() {
    var xmlhttp = new XMLHttpRequest();

    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == XMLHttpRequest.DONE) {
            if (xmlhttp.status == 200) {
                console.log(xmlhttp.responseText);
            }
            else if (xmlhttp.status == 400) {
                alert('There was an error 400');
            }
            else {
                alert('something else other than 200 was returned');
            }
        }
    };

    xmlhttp.open("GET", "/api/Materials", true);
    xmlhttp.send();
};