var btnID = "";
var btnValue = "";

// Get the modal
var modal = document.getElementById("myModal");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

function getBtnID(obj) {
    btnID = obj.id;
}

function selectRow(e) {
    if (e.target) {
        if (e.target.innerHTML.trim() != "ШТРИХ-КОД") {
            if (e.target.innerHTML.trim() != "Без функции") {
                document.getElementById(btnID).title = e.target.innerHTML.trim();
                document.getElementById(btnID).value = e.target.innerHTML.trim();
            }
            else if (e.target.innerHTML.trim() == "Без функции") {
                document.getElementById(btnID).title = "";
                document.getElementById(btnID).value = btnID.substr(3);
            }
        }
        else if (e.target.innerHTML.trim() == "ШТРИХ-КОД") {
            // When the user clicks the button, open the modal 
            modal.style.display = "block";
            document.getElementById("txtBarcodeID").value = "";
            document.getElementById("txtBarcodeID").focus(); s
        }
    }
}

function defaultSetValue() {
    document.getElementById("btn1").value = "1";
    document.getElementById("btn2").value = "2";
    document.getElementById("btn3").value = "3";
    document.getElementById("btn4").value = "4";
    document.getElementById("btn5").value = "5";
    document.getElementById("btn6").value = "6";
    document.getElementById("btn7").value = "7";
    document.getElementById("btn8").value = "8";
    document.getElementById("btn9").value = "9";
    document.getElementById("btn10").value = "10";
    document.getElementById("btn11").value = "11";
    document.getElementById("btn12").value = "12";
    document.getElementById("btn13").value = "13";
    document.getElementById("btn14").value = "14";
    document.getElementById("btn15").value = "15";
    document.getElementById("btn16").value = "ВАУЧЕР";
    document.getElementById("btn17").value = "17";

    document.getElementById("btn30").value = "АННУЛЯЦИЯ";
    document.getElementById("btn31").value = "ВОЗВРАТ";
    document.getElementById("btn32").value = "*ВВЕРХ";
    document.getElementById("btn33").value = "*ТАБ";
    document.getElementById("btn34").value = "ВТОРОЙ ЧЕК";
    document.getElementById("btn35").value = "35"; //PAYMENT
    document.getElementById("btn36").value = "*ВНИЗ";
    document.getElementById("btn37").value = "37"; //ИТОГ/ОПЛАТА
    document.getElementById("btn38").value = "КОЛ-ВО";
    document.getElementById("btn39").value = "ОТМЕНА";
    document.getElementById("btn40").value = "40";
    document.getElementById("btn41").value = "41"; //СКИДКА ПО ЧЕКУ
    document.getElementById("btn42").value = "КАРТА ЛОЯЛЬНОСТИ";
    document.getElementById("btn43").value = "ОПЛАТА ПО КАРТЕ";
    document.getElementById("btn44").value = "44"; //ДИСКОНТНАЯ КАРТА
    document.getElementById("btn45").value = "45"; //СКИДКА % ПО ЧЕКУ
    document.getElementById("btn46").value = "ИТОГ"; //ИТОГ/ОПЛАТА
    document.getElementById("btn47").value = "47";
    document.getElementById("btn48").value = "ЯЩИК";
    document.getElementById("btn49").value = "КОД ТОВАРА";

    document.getElementById("btn50").value = "СБРОС";
    document.getElementById("btn51").value = "51";
    document.getElementById("btn52").value = "52";
    document.getElementById("btn53").value = "53";
    document.getElementById("btn54").value = "54";
    document.getElementById("btn55").value = "55";
    document.getElementById("btn56").value = "56";
    document.getElementById("btn57").value = "57";
    document.getElementById("btn58").value = "58";
    document.getElementById("btn59").value = "59";
    document.getElementById("btn60").value = "60";
    document.getElementById("btn61").value = "61";
    document.getElementById("btn62").value = "62";
    document.getElementById("btn63").value = "63";
    document.getElementById("btn64").value = "64";
    document.getElementById("btn65").value = "65";
    document.getElementById("btn66").value = "66";
    document.getElementById("btn67").value = "67";

    document.getElementById("btn1").title = "1";
    document.getElementById("btn2").title = "2";
    document.getElementById("btn3").title = "3";
    document.getElementById("btn4").title = "4";
    document.getElementById("btn5").title = "5";
    document.getElementById("btn6").title = "6";
    document.getElementById("btn7").title = "7";
    document.getElementById("btn8").title = "8";
    document.getElementById("btn9").title = "9";
    document.getElementById("btn10").title = "10";
    document.getElementById("btn11").title = "11";
    document.getElementById("btn12").title = "12";
    document.getElementById("btn13").title = "13";
    document.getElementById("btn14").title = "14";
    document.getElementById("btn15").title = "15";
    document.getElementById("btn16").title = "ВАУЧЕР";
    document.getElementById("btn17").title = "17";

    document.getElementById("btn30").title = "АННУЛЯЦИЯ";
    document.getElementById("btn31").title = "ВОЗВРАТ";
    document.getElementById("btn32").title = "*ВВЕРХ";
    document.getElementById("btn33").title = "*ТАБ";
    document.getElementById("btn34").title = "ВТОРОЙ ЧЕК";
    document.getElementById("btn35").title = "35"; //PAYMENT
    document.getElementById("btn36").title = "*ВНИЗ";
    document.getElementById("btn37").title = "37"; //ИТОГ/ОПЛАТА
    document.getElementById("btn38").title = "КОЛ-ВО";
    document.getElementById("btn39").title = "ОТМЕНА";
    document.getElementById("btn40").title = "40";
    document.getElementById("btn41").title = "41"; //СКИДКА ПО ЧЕКУ
    document.getElementById("btn42").title = "КАРТА ЛОЯЛЬНОСТИ";
    document.getElementById("btn43").title = "ОПЛАТА ПО КАРТЕ";
    document.getElementById("btn44").title = "44"; //ДИСКОНТНАЯ КАРТА
    document.getElementById("btn45").title = "45"; //СКИДКА % ПО ЧЕКУ
    document.getElementById("btn46").title = "ИТОГ"; //ИТОГ/ОПЛАТА
    document.getElementById("btn47").title = "47";
    document.getElementById("btn48").title = "ЯЩИК";
    document.getElementById("btn49").title = "КОД ТОВАРА";

    document.getElementById("btn50").title = "СБРОС";
    document.getElementById("btn51").title = "51";
    document.getElementById("btn52").title = "52";
    document.getElementById("btn53").title = "53";
    document.getElementById("btn54").title = "54";
    document.getElementById("btn55").title = "55";
    document.getElementById("btn56").title = "56";
    document.getElementById("btn57").title = "57";
    document.getElementById("btn58").title = "58";
    document.getElementById("btn59").title = "59";
    document.getElementById("btn60").title = "60";
    document.getElementById("btn61").title = "61";
    document.getElementById("btn62").title = "62";
    document.getElementById("btn63").title = "63";
    document.getElementById("btn64").title = "64";
    document.getElementById("btn65").title = "65";
    document.getElementById("btn66").title = "66";
    document.getElementById("btn67").title = "67";
}

function setValues() {

    var buttons = document.querySelectorAll("input[type=button]");
    var id = "";

    for (var i = 0; i < buttons.length; i++) {
        if (buttons[i].id != "") {
            id = buttons[i].id + "H";
            document.getElementById(id).value = id + ":" + document.getElementById(buttons[i].id).value.trim();
        }
    }

    $("#submit").click(function () {
        document.forms[0].submit();
        return false;
    });
}

function myFunction() {
    document.getElementById("btn55HH").value = document.getElementById("btn55").value.trim();
    alert(document.getElementById("btn55HH").value);
}

// When the user clicks on <span> (x), close the modal
function clickSpanClose() {
    document.getElementById(btnID).title = document.getElementById("txtBarcodeID").value.trim();
    document.getElementById(btnID).value = document.getElementById("txtBarcodeID").value.trim();

    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

//Press Enter
function runScript(e) {
    //See notes about 'which' and 'key'
    if (e.keyCode == 13) {
        var tb = document.getElementById("txtBarcodeID");
        clickSpanClose();
        return false;
    }
}
