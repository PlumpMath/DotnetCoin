function loadWallet() {
    var address = $('#txtAddress').val();
    $.ajax({
        url: "http://localhost:5000/api/Coin/Wallet?walletAddress=" + address,
        type: "GET",
        success: function(data){
            console.log(data);
            var response = jQuery.parseJSON(data);
            displayWallet(response.Address, response.Balance);
        }
    });
}
function displayWallet(address, balance) {
    //do stuff
    $('#walletAddress')[0].innerHTML = 'Wallet Address: ' + address;
    $('#walletBalance')[0].innerHTML = 'Wallet Balance: ' + balance;
}
