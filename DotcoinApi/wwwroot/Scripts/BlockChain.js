function loadChain(){
    $.ajax({
        url: "http://localhost:5000/api/Coin/BlockChain",
        type: "GET",
        success: function(data){
            $('#chain')[0].innerHTML = '';
            console.log(data);
            var response = jQuery.parseJSON(data);
            for(var i = 0; i < response.length; i ++){
                addBlock(response[i]);
            };
        }
    });
}
function addBlock(block){
    $("#chain").append(
      ("<div class='row block'><h2>" + "Block#" + block.Index
      + '<br/>'
      + buildTransactions(block.Data)
      +"</h2></div>")
    );
}
function buildTransactions(transactionsJson){

    if(transactionsJson === "")
        return "No Transaction";

    var transactions = JSON.parse(transactionsJson.toString());
    var html = "";

    for(var i = 0; i < transactions.length; i ++){
        html += buildTransaction(transactions[i]);
    }

    return html;
}
function buildTransaction(transaction){
    return "<div class='row transaction'>"
          + "<h2>From: " + transaction.From + "</h2>"
          + "<h2>To: " + transaction.To + "</h2>"
          + "<h2>Amount: " + transaction.Amount + "</h2>"
          + "</div>";
}
function sendMoney(){
    var toAddress = $("#txtTo")[0].value;
    var fromAddress = $('#txtFrom')[0].value;
    var ammount = $('#txtAmount')[0].value;

    var transaction = {
      'from' : fromAddress,
      'to' : toAddress,
      'amount': ammount
    };

    $.ajax({
      url : 'http://localhost:5000/api/Coin/Transaction',
      type: 'POST',
      data: JSON.stringify(transaction),
      contentType: 'application/json',
      success: function(data){
          $('#lblOutput')[0].innerHTML = 'transaction added'
      }
    });
}
function updatePendingTransactions(){
  $.ajax({
      url: "http://localhost:5000/api/Coin/Transaction",
      type: "GET",
      success: function(data){

          if(data == ''){
              $('#pending')[0].innerHTML = 'No pending transactions';
              return;
          }

          console.log(data);

          var response = data;

          $('#pending')[0].innerHTML = '';
          for(var i = 0; i < response.length; i ++){
              $('#pending')[0].innerHTML += builds(response[i]);
          };
      }
  });
}
function mine(){
    $.ajax({
        url: "http://localhost:5000/api/Coin/Mine",
        type: "GET",
        success: function(data){
            console.log(data);
        }
    });
}
function builds(s){
  return 'To: ' + s.to + " From: " + s.from + ' Amount: ' + s.amount + "<br/>";
}
window.onload = function(){
    loadChain();
    updatePendingTransactions();
    setInterval(function(){
        loadChain();
        updatePendingTransactions();
    }, 3000);
}
