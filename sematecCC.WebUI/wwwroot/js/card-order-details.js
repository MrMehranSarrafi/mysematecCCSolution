function setCurrentRow(cardId) {
    
    var row = document.querySelector(`tr[data-id="${cardId}"]`);
    if (row) {

        // هایلایت کردن سطر انتخاب شده (اختیاری - برای زیبایی)
        // حذف کلاس active از همه سطرها
        document.querySelectorAll('#tbody tr').forEach(tr => tr.classList.remove('active-row'));
        // افزودن کلاس به سطر فعلی
        row.classList.add('active-row');
    }
}
function ViewCardTransactions(cardId) {
    setCurrentRow(cardId)
    let url = '/CardsManagement/GetCardtransactions';
    $.get(url, { cardId: cardId }, function (data) {
        $('#Cardtransactions').html(data);
        $("#myTransactionsModal").modal("show");
    });
}
function ValidateAmount(input) {

    var value = input.value;

    // فقط عدد و نقطه اعشار مجاز باشد
    value = value.replace(/[^0-9.]/g, '');

    // جدا کردن قسمت صحیح و اعشار
    var parts = value.split('.');
    var intPart = parts[0] || '';
    var decPart = parts[1] || '';

    // اگر اعشار ندارد
    if (parts.length === 1) {
        if (intPart.length > 18) {
            intPart = intPart.substring(0, 18);
        }
    }
    // اگر اعشار دارد
    else {
        // محدود کردن رقم اعشار به حداکثر 3 رقم
        if (decPart.length > 3) {
            decPart = decPart.substring(0, 3);
        }

        // بررسی تعداد رقم صحیح بر اساس رقم اعشار
        if (decPart.length === 3) {
            // 15 رقم صحیح با 3 رقم اعشار
            if (intPart.length > 15) {
                intPart = intPart.substring(0, 15);
            }
        } else if (decPart.length === 2) {
            // 17 رقم صحیح با 2 رقم اعشار
            if (intPart.length > 17) {
                intPart = intPart.substring(0, 17);
            }
        } else if (decPart.length === 1) {
            // 16 رقم صحیح با 1 رقم اعشار
            if (intPart.length > 16) {
                intPart = intPart.substring(0, 16);
            }
        }
    }

    // مقدار نهایی
    var newValue = intPart + (decPart ? '.' + decPart : '');

    // فقط در صورت تغییر، مقدار را بروزرسانی کن
    if (input.value !== newValue) {
        input.value = newValue;
    }
}
function DisableCard(cardId) {
    setCurrentRow(cardId)
    if (!confirm("آیا از غیرفعال کردن این کارت مطمئن هستید؟"))
        return;

    $.ajax({
        url: '/CardsManagement/DisableCard',
        type: 'POST',
        data: {
            cardId: cardId,
            __RequestVerificationToken: getAntiForgeryToken()
        },
        success: function (res) {
            if (res.success) {

                //location.reload();    //revise to avoid full-page reload
                // updateCardRow(cardId, false); // false => غیرفعال
                sessionStorage.setItem('selectedCardId', cardId);
                alert(res.message);
                location.reload();
            } else {
                alert(res.message);
            }
        },
        error: function () {
            alert("خطای امنیتی یا سرور");
        }
    });
}
function EnableCard(cardId) {
    setCurrentRow(cardId)
    if (!confirm("آیا از فعال کردن این کارت مطمئن هستید؟"))
        return;

    $.ajax(
        {
            url: '/CardsManagement/EnableCard',
            type: 'POST',
            data: {
                cardId: cardId,
                __RequestVerificationToken: getAntiForgeryToken()
            },
            success: function (res) {
                if (res.success) {
                    //location.reload();   کل صفحه را رفرش کرده و اکشن این صفحه را دوباره صدا میزنه، پس Not Efficient or revisable
                    // updateCardRow(cardId, true); // true => فعال
                    sessionStorage.setItem('selectedCardId', cardId);
                    alert(res.message);
                    location.reload();
                }
                else {
                    alert(res.message);
                }
            },
            error: function () {
                alert('خطای امنیتی یا سرور');
            }
        });

}
function showIncreaseCardCreditModal(cardId, cardNo) {
    setCurrentRow(cardId)
    var cardNoStr = String(cardNo).padStart(16, '0');
    console.log(cardNoStr);
    $('#incModalCardNoSpn').text(' ' + cardNoStr);

    $('#incModalAmount').val('')
    $('#incModalDescription').val('')
    $('#incModalErrorMessage').addClass('hidden');
    // ذخیره cardId در مودال
    $('#increaseCardCreditModal').data('card-id', cardId);
    $('#increaseCardCreditModal').modal('show');
}
function showDecreaseCardCreditModal(cardId, cardNo) {
    setCurrentRow(cardId)
    var cardNoStr = String(cardNo).padStart(16, '0');
    console.log(cardNoStr);
    $('#decModalCardNoSpn').text(' ' + cardNoStr);

    $('#decModalAmount').val('')
    $('#decModalDescription').val('')
    $('#decModalErrorMessage').addClass('hidden');
    // ذخیره cardId در مودال
    $('#decreaseCardCreditModal').data('card-id', cardId);
    $('#decreaseCardCreditModal').modal('show');
}
function increaseCreditCard() {

    var cardId = $('#increaseCardCreditModal').data('card-id');
    var amount = $('#incModalAmount').val();
    var description = $('#incModalDescription').val();

    // اعتبارسنجی اولیه
    if (!cardId || cardId === 0) {
        showError('شناسه کارت معتبر نیست', $('#incModalErrorMessageSpan'), $('#incModalErrorMessage'));
        return;
    }

    if (!amount || parseFloat(amount) <= 0) {
        showError('لطفا مبلغ را وارد کنید', $('#incModalErrorMessageSpan'), $('#incModalErrorMessage'));
        return;
    }
    if (description == '') {
        showError('توضیحات آن را وارد نمایید', $('#incModalErrorMessageSpan'), $('#incModalErrorMessage'));
        return;
    }
    // ساخت مدل برای ارسال
    var model = {
        CardId: cardId,
        Amount: parseFloat(amount),
        Description: description || ''
    };
    $('#incModalIncreaseCreditCardBtn').prop('disabled', true);
    // ارسال با
    
    $.ajax({
        url: '/CardsManagement/IncrementCardCredit', // یا مسیر کامل
        type: 'POST',
        data: JSON.stringify(model),//ان طرف باید [FromBody]
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            $('#incModalIncreaseCreditCardBtn').prop('disabled', false);
            if (result.success) {
                // موفقیت
                $('#increaseCardCreditModal').modal('hide');
                sessionStorage.setItem('selectedCardId', cardId)
                alert(result.message);
                // optionally reload grid or update UI
                location.reload(); // یا refresh جدول ???

            } else {
                
                // خطا
                $('#incModalIncreaseCreditCardBtn').prop('disabled', false);
                showError(result.message || 'خطا در افزایش اعتبار', $('#incModalErrorMessageSpan'), $('#incModalErrorMessage'));
            }
        },
        error: function (xhr, status, error) {
            $('#incModalIncreaseCreditCardBtn').prop('disabled', false);
            showError('خطا در ارتباط با سرور', $('#incModalErrorMessageSpan'), $('#incModalErrorMessage'));
            console.error(error);
        }
    });
}

function showModalsetAllCardsExpireDate() {
    // $('#ExpireDateModal').data('cardOrderId', cardOrderId);//it is global
    $('#txtmodalAllCardsExpireDateFa').text();
    $('#divMsgAllCardsExpireDateModal').addClass('hidden');
    // نمایش مودال
    $('#AllCardsExpireDateModal').modal('show');
}

// تابع نمایش خطا
function showError(message, spn, div) {
    
    spn.text(message);
    div.removeClass('hidden');
}
function hideError(spn, div) {
    spn.text('');
    div.addClass('hidden');
}
function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}
// تابع آپدیت ردیف جدول
function updateCardRow(cardId, isActive) {
    // پیدا کردن ردیف با CardId: <tr>
    var row1 = $("#tbody tr").filter(function () {
        return $(this).find("td:first").text().trim() == cardId;
    });
    console.log(row1)
    //After using unobtrusive: <tr data-id="card.Id">
    var row = $('#tbody tr[data-id="' + cardId + '"]');
    console.log(row)

    // اگر ردیف پیدا شد، فقط بخش دکمه و وضعیت را آپدیت می‌کنیم
    if (row.length) {
        var btnHtml = '';
        var badgeHtml = '';

        if (isActive) {
            btnHtml = `<button type="button" class="btn btn-danger btn-sm" onclick="DisableCard(${cardId})">غیرفعال کردن</button>`;
            badgeHtml = `<span class="badge bg-success ">فعال</span>`;
        } else {
            btnHtml = `<button type="button" class="btn btn-primary btn-sm" onclick="EnableCard(${cardId})" ">فعال کردن</button>`;
            badgeHtml = `<span class="badge bg-secondary">غیرفعال</span>`;
        }

        //    jquery pseudo-class  با  :
        row.find("td:eq(5) .d-flex").html(btnHtml + ' ' + badgeHtml);//وضعیت  - دکمه
        //غلط:   row.find("td .d-flex").eq(5) .html(btnHtml + ' ' + badgeHtml);//غلط ، معادل نیست

        // معادل بدون :eq()
        //row.find("td").eq(5).find(".d-flex").html(btnHtml + ' ' + badgeHtml);//method chaining
    }
}
function FixFaDate(txtFaDate) {
    var value = txtFaDate.value;
    let v = value.replace(/[^0-9]/g, '');

    // اضافه کردن / بعد از سال
    if (v.length > 4)
        v = v.slice(0, 4) + '/' + v.slice(4);

    // بررسی ماه (01-12)
    if (v.length > 5) {
        let monthStr = v.slice(5, 7);
        let month = parseInt(monthStr);

        if (month > 12) {
            // اگر ماه بیشتر از 12 شد → تبدیل به 01
            v = v.slice(0, 5) + '01' + v.slice(7);
        } else if (month > 0 && month <= 9 && monthStr.length === 2) {
            // اگر ماه یک رقمی بود (مثل 3) → تبدیل به 03
            v = v.slice(0, 5) + '0' + month + v.slice(7);
        }
    }

    // اضافه کردن / بعد از ماه
    if (v.length > 7)
        v = v.slice(0, 7) + '/' + v.slice(7);

    // بررسی رقم اول روز (position 8)
    if (v.length > 8) {
        let dayFirst = v.charAt(8);
        if (dayFirst === '0' || dayFirst === '/') {
            // هیچ تغییری نده
        } else if (dayFirst >= '1' && dayFirst <= '3') {
            // درسته
        } else if (dayFirst >= '4' && dayFirst <= '9') {
            // تبدیل به 0
            v = v.slice(0, 8) + '0' + v.slice(9);
        }
    }

    // بررسی رقم دوم روز (اگر بیشتر از 31 شد، حذف شود)
    if (v.length > 9) {
        let dayStr = v.slice(8, 10);
        let day = parseInt(dayStr);
        if (day > 31) {
            v = v.slice(0, 9);
        }
    }

    txtFaDate.value = v.slice(0, 10);
}
function showModalSetExpireDate(cardId) {
    setCurrentRow(cardId)
    $('#ExpireDateModal').data('cardId', cardId);
    //ریست کردن قبلی:
    $('#txtmodalExpireDateFa').text();
    $('#divMsgExpireDateModal').addClass('hidden');
    // نمایش مودال
    $('#ExpireDateModal').modal('show');
}

