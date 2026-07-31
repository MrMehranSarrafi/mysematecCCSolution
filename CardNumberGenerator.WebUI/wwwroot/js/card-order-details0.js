
$(document).ready(function () {
    $('#btnModalConfirmExpireDateFa').on('click', setCardExpireDateFa);
    $('#btnModalAllCardsConfirmExpireDateFa').on('click', setAllCardsExpireDateFa);

    let savedCardId = sessionStorage.getItem('selectedCardId');
    if (savedCardId) {
        setCurrentRow(parseInt(savedCardId));
        //پاک کردن پس از استفاده:
        sessionStorage.removeItem('selectedCardId');
    }

    //console.log(cardOrderIsActive)
    if (cardOrderIsActive == false) {
        $('#msgCardOrderDisabled').removeClass('hidden');
    }
   

   

    
    $('#incModalAmount').on('input', function () { ValidateAmount(this); });
    $('#decModalAmount').on('input', function () { ValidateAmount(this); });



   

});