//create connection
var connectionUserCount = new signalR.HubConnectionBuilder()
    //.configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .withUrl("/userhub", signalR.HttpTransportType.WebSockets).build();

//connect to methods that hub invokes aka receive notfications from hub
connectionUserCount.on("updateTotalViews", (value) => {
   
});

//invoke hub methods aka send notification to hub
function newWindowLoadedOnClient() {
    connectionUserCount.invoke("NewWindowLoaded", parseInt(document.getElementById("userId").value), document.getElementById("orderId").value).then(function (obj) {
        const parsedObject = JSON.parse(obj);
        var sparkleTextElements = document.getElementsByClassName("sparkle-text");
        setInterval(function () {
            sparkleTextElements[0].style.opacity =
                (sparkleTextElements[0].style.opacity == 0 ? 1 : 0);
        }, 1500);
        if (parsedObject.userInfo != null) {
            var alertBox = document.getElementById("myAlert");
            alertBox.style.display = "block";
            if (parsedObject.userId == parseInt(document.getElementById("userId").value)) {
                sparkleTextElements[0].innerHTML = " [ This order is already loaded in your browser. ]"
            } else {
                sparkleTextElements[0].innerHTML = " [ We would like to inform you that " + parsedObject.userInfo.Email + " is currently working on this order. ]"
            }
        }
    });
}

//start connection
function fulfilled() {
    //do something on start
    console.log("Connection to User Hub Successful");
    newWindowLoadedOnClient();
}
function rejected() {
    //rejected logs
}

//connectionUserCount.onclose((error) => {
//    document.body.style.background = "red";
//});

//connectionUserCount.onreconnected((connectionId) => {
//    document.body.style.background = "green";
//});

//connectionUserCount.onreconnecting((error) => {
//    document.body.style.background = "orange";
//});
connectionUserCount.start().then(fulfilled, rejected);