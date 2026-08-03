
$(document).ready(function () {

    
   
    document.getElementById('btnConfirmOwner').addEventListener('click', setCardOwner);



    document.getElementById('setOwnerModal').addEventListener('shown.bs.modal', function () {
        var input = document.getElementById('mobileInput');
        if (input) {
            input.focus();
        }
    });

    document.getElementById('createPersonBtn').addEventListener('click', createPersonCall);

    // اضافه کردن رویداد Enter در فیلد موبایل
    // افزودن رویداد کلیک برای دکمه جستجو (با کلید Enter در فیلد موبایل)
    $("#mobileInput").on("keydown", function (event) {
        getPersonByMobileNo(event);
    });
});

function setCardOwner() {
    console.clear();
    
    let testt = document.getElementById("ownerPersonId");
    console.log(testt)
    let ownerPersonId = document.getElementById("ownerPersonId").value;
    console.log(`ownerPersonId: ${ownerPersonId}`);
    console.log(`cardOrderId2: ${cardOrderId2}`);
    let cardId = $('#setOwnerModal').data('cardId');
    
    console.log(`cardId2: ${cardId}`);
    setCurrentRow(cardId)

    if (!cardId || !ownerPersonId) {
        alert("خطا: شناسه کارت یا مالک ناقص است.");
        return;
    }
    // 3. دریافت توکن امنیتی:
    const token = getAntiForgeryToken();
    // 4. URL اکشن
    var url = '/CardsManagement/SetCardOwner'; //'@Url.Action("SetCardOwner", "CardsManagement")';
    // 5. فراخوانی با Fetch
    fetch(url,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: new URLSearchParams({
                cardId: cardId,
                ownerPersonId: ownerPersonId,
                __RequestVerificationToken: token
            })
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {

                $('#setOwnerModal').modal('hide');
                alert(data.message);

                location.reload();
            } else {
                alert("خطا: " + (data.message || "عملیات ناموفق بود."));
            }
        })
        .catch(error => {
            console.error('خطا در ارسال درخواست:', error);
            alert("خطایی در ارتباط با سرور رخ داد. لطفاً دوباره تلاش کنید.");
        });
}

function getPersonByMobileNo(e) {


    if (e.key !== "Enter")
        return;

    e.preventDefault();
    
    var mobile = $('#mobileInput').val().trim();

    if (!mobile || mobile.length !== 11 || !/^\d+$/.test(mobile)) {
        alert("لطفاً شماره موبایل معتبر (11 رقمی) وارد کنید.");
        return;
    }

    var cardId = $('#setOwnerModal').data('cardId');

    if (!cardId) {
        alert("شناسه کارت موجود نیست.");
        return;
    }

    $.get('/Person/GetPersonByMobile', { mobileNO: mobile, companyId: companyId })

        .done(function (person) {
            
            $('#createPersonBtn').addClass('hidden');

            if (person.success) {

                $('#ownerName').text(`${person.firstName} ${person.lastName}`);
                $('#ownerPersonId').val(person.id);

                $('#ownerResult').removeClass('hidden');
                $('#ownerNotFound').addClass('hidden');
                //$('#ownerResult').show();
                //$('#ownerNotFound').hide();

                $('#btnConfirmOwner').prop('disabled', false);
            }
            else {

                $('#ownerResult').addClass('hidden');
                //$('#ownerResult').hide();

                $('#ownerNotFound').removeClass('hidden');
                //$('#ownerNotFound').show();

                $('#ownerNOtFoundMessage').text(person.message);

                $('#ownerPersonId').val('');

                $('#btnConfirmOwner').prop('disabled', true);

                if (person.message.includes("یافت")) {
                    $('#createPersonBtn').removeClass('hidden');
                }
            }

        })

        .fail(function (xhr) {

            console.error(xhr);

            $('#ownerResult').addClass('hidden');
            $('#ownerNotFound').removeClass('hidden');
            $('#btnConfirmOwner').prop('disabled', true);

            alert("خطا در ارتباط با سرور");

        });


}


function createPersonCall() {
    
    const mobileInput = document.getElementById('mobileInput');
    const mobile = mobileInput.value.trim();
    // بررسی معتبر بودن شماره موبایل
    if (!mobile || mobile.length < 10) {
        alert('لطفاً یک شماره موبایل معتبر وارد کنید.');
        return;
    }
    // ساخت آدرس با پارامتر MobileNO
    const url = `/Person/Create?MobileNO=${encodeURIComponent(mobile)}`;
    // باز کردن صفحه در تب جدید
    const newTab = window.open('about:blank', '_blank');
    // آدرس واقعی را بعداً تنظیم کن (در همان متر اجرا)
    if (!newTab) {
        alert('مرورگر پاپ‌آپ را مسدود کرده است. لطفاً برای این سایت اجازه‌ی باز شدن تب جدید را بدهید.');
        return;
    }
    newTab.location.href = url;
}


function showSetCardOwnerModal(cardId) {
    
    setCurrentRow(cardId)
    var mobileInput = $('#mobileInput');
    //alert(""+cardId);
    // ذخیره cardId در متغیر جهت استفاده در تابع تأیید
    $('#setOwnerModal').data('cardId', cardId);
    // پاک کردن نتایج قبلی
    $('#ownerResult').addClass('hidden');
    $('#ownerNotFound').addClass('hidden');
    $('#btnConfirmOwner').prop('disabled', true);

    mobileInput.val('');
    // نمایش مودال
    $('#setOwnerModal').modal('show');

}


// Add this to reset modal when it's hidden
$('#setOwnerModal').on('hidden.bs.modal', function () {
    $('#ownerResult').hide();
    $('#ownerNotFound').hide();
    $('#mobileInput').val('');
    $('#ownerPersonId').val('');
    $('#btnConfirmOwner').prop('disabled', true);
    $('#createPersonBtn').addClass('hidden');
});















































