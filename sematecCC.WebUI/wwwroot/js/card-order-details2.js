function decreaseCreditCard() {
    var cardId = $('#decreaseCardCreditModal').data('card-id');
    var amount = $('#decModalAmount').val();
    var description = $('#decModalDescription').val();

    // اعتبارسنجی اولیه
    if (!cardId || cardId === 0) {
        showError('شناسه کارت معتبر نیست', $('#decModalErrorMessageSpan'), $('#decModalErrorMessage'))
        return;
    }

    if (!amount || parseFloat(amount) <= 0) {
        showError('لطفا مبلغ را وارد کنید', $('#decModalErrorMessageSpan'), $('#decModalErrorMessage'))
        return;
    }
    if (description == '') {
        showError('توضیحات آن را وارد نمایید', $('#decModalErrorMessageSpan'), $('#decModalErrorMessage'))
        return;
    }

    // ساخت مدل برای ارسال
    var model = {
        CardId: cardId,
        Amount: parseFloat(amount),
        Description: description || ''
    };
    $('#decModalIncreaseCreditCardBtn').prop('disabled', true);
    // ارسال با AJAX
    $.ajax({
        url: '/CardsManagement/DecreaseCardCredit', // یا مسیر کامل
        type: 'POST',
        data: JSON.stringify(model),//ان طرف باید [FromBody]
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            $('#decModalIncreaseCreditCardBtn').prop('disabled', false);
            if (result.success) {
                // موفقیت
                $('#decreaseCardCreditModal').modal('hide');
                sessionStorage.setItem('selectedCardId', cardId)
                alert(result.message);
                // optionally reload grid or update UI
                location.reload(); // یا refresh جدول  ???

            } else {
                // خطا
                $('#decModalIncreaseCreditCardBtn').prop('disabled', false);
                showError(result.message || 'خطا در کاهش اعتبار', $('#decModalErrorMessageSpan'), $('#decModalErrorMessage'));
            }
        },
        error: function (xhr, status, error) {
            $('#decModalIncreaseCreditCardBtn').prop('disabled', false);

            showError('خطا در ارتباط با سرور', $('#decModalErrorMessageSpan'), $('#decModalErrorMessage'));
            console.error(error);
        }

    });
}
$("#txtSearch").on("keydown", function (e) {
    if (e.key === "Enter") {
        e.preventDefault();
        var value = $(this).val();
        var cardOrderId = $("#cardOrderId").val();//razor
        // GetCardsByOrderIdAndCardNoInitialsAsync
        //"/CardsManagement/CardOrderDetailsSearch"
        console.log(cardOrderId);
        let url = '/CardsManagement/CardOrderDetailsSearch';//'@Url.Action("CardOrderDetailsSearch", "CardsManagement")';
        console.log(url);
        $.get(url, { cardOrderId: cardOrderId, cardNoInitials: value })
            .done(function (data) {
                $("#tbody").html(data);
            })
            .fail(function () {
                alert("خطا در دریافت اطلاعات");
            });
    }
});

function setCardExpireDateFa() {
    
    var cardId = $('#ExpireDateModal').data('cardId');
    var txtmodalExpireDateFa = $('#txtmodalExpireDateFa');
    let ExpireDate = txtmodalExpireDateFa.val();
    const url = '/CardsManagement/SetCardExpireDateFa'; //'@Url.Action("SetCardExpireDateFa", "CardsManagement")';
    const token = getAntiForgeryToken();
    fetch(url,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: new URLSearchParams({
                cardId: cardId,
                expireDateFa: ExpireDate,
                __RequestVerificationToken: token
            })
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                $('#ExpireDateModal').modal('hide');
                sessionStorage.setItem('selectedCardId', cardId);
                alert(data.message);
                location.reload();
            }
            else {
                
               
                $('#divMsgExpireDateModal').removeClass('hidden');
                $('#spnMsgExpireDateModal').text(data.message);
            }
        })
         
        .catch(error => {
            console.error('خطا در ارسال درخواست:', error);
            alert(error.message);
            alert("خطایی در ارتباط با سرور رخ داد. لطفاً دوباره تلاش کنید.");
        });
}
function setAllCardsExpireDateFa() {

    
    var txtmodalAllCardsExpireDateFa = $('#txtmodalAllCardsExpireDateFa');
    let ExpireDate = txtmodalAllCardsExpireDateFa.val();
    const url = '/CardsManagement/SetAllCardsExpireDateFa';//'@Url.Action("SetAllCardsExpireDateFa", "CardsManagement")'
    const token = getAntiForgeryToken();
    //fetch(url,
    //    {
    //        method: 'POST',
    //        headers: {
    //            'Content-Type': 'application/x-www-form-urlencoded',
    //            'RequestVerificationToken': token
    //        },
    //        body: new URLSearchParams({
    //            cardOrderId: cardOrderId2,
    //            expireDateFa: ExpireDate,
    //            __RequestVerificationToken: token
    //        })
    //    })
    //    .then(response => response.json())
    //    .then(data => {
    //        if (data.success) {
    //            $('#AllCardsExpireDateModal').modal('hide');

    //            alert(data.message);
    //            location.reload();
    //        }
    //        else {
    //            
    //            alert(data.message);
    //            $('#divMsgAllCardsExpireDateModal').removeClass('hidden');
    //            $('#spnMsgAllCardsExpireDateModal').text(data.message);
    //        }
    //    })
    //    .catch(error => {
    //        console.error('خطا در ارسال درخواست:', error);
    //        alert(data.message + "\n" + ".خطایی در ارتباط با سرور رخ داد. لطفاً دوباره تلاش کنید.");
    //    });
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: new URLSearchParams({
            cardOrderId: cardOrderId2,
            expireDateFa: ExpireDate,
            __RequestVerificationToken: token
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                $('#AllCardsExpireDateModal').modal('hide');
                alert(data.message);
                location.reload();
            } else {
                $('#divMsgAllCardsExpireDateModal').removeClass('hidden');
                $('#spnMsgAllCardsExpireDateModal').text(data.message);
            }
        })
        .catch(error => {
            console.error(error);
            alert(error.message);
        });

}
//function postForm(url, data) {
//    const token = getAntiForgeryToken();

//    return fetch(url, {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/x-www-form-urlencoded',
//            'RequestVerificationToken': token
//        },
//        body: new URLSearchParams({
//            ...data,
//            __RequestVerificationToken: token
//        })
//    })
//        .then(response => {
//            if (!response.ok) {
//                throw new Error(`HTTP ${response.status}`);
//            }

//            return response.json();
//        });
//}