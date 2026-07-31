var uploadPictureId = null;
var uploadDocumentId = null;
var editorList = [];

$(document).ready(function () {
    initCodeprocessValues();
    initializeSelectedMenuScript();
});

String.prototype.GetInteger = function () {
    try {
        var output = parseInt(this);
        if (isNaN(output) === true)
            return 0;
        return output;
    }
    catch (ex) { return 0; }
};

String.prototype.GetFloat = function () {
    try {
        var output = parseFloat(this);
        if (isNaN(output) === true)
            return 0;
        return output;
    }
    catch (ex) { return 0; }
};

jQuery.fn.extend({
    selected: function () {
        var value = $(this).find(":selected").attr("value");
        return value === undefined ? null : value;
    }
});

function initializeSelectedMenuScript() {
    var selectedParent = $("body").attr("parent-menu");
    var selectedMenu = $("body").attr("selected-menu");

    if (selectedParent !== "" && selectedParent !== undefined) {
        $("#sidebar-menu li[parent-menu='" + selectedParent + "'] > a.waves-effect").addClass("subdrop");
        $("#sidebar-menu li[parent-menu='" + selectedParent + "'] > ul.list-unstyled").attr("style", "display: block");
    }

    if (selectedMenu !== "" && selectedMenu !== undefined) {
        $("#sidebar-menu li[selected-menu='" + selectedMenu + "']").addClass("active");
    }
}

function initCodeprocessValues() {
    var components = $("[codeprocess]");
    for (var i = 0; i < components.length; i++) {
        var cp = components[i];
        var value = $(cp).attr("codeprocess");
        var Id = $(cp).attr("id");
        if (value === "editor") {
            initCodeprocessEditor(Id);
        } else if (value === "date") {
            initCodeprocessDatetime(Id);
        } else if (value === "upload") {
            initCodeprocessUpload(Id);
        } else if (value === "upload-n") {
            initializeCodeprocessUploadScript(Id);
        } else if (value === "upload-n-1") {
            initializeCodeprocessUploadScript(Id);
        } else if (value === "upload-n-2") {
            initializeCodeprocessUploadScript(Id);
        } else if (value === "document") {
            initializeCodeprocessUploadDocumentScript(Id);
        } else if (value === "show-part") {
            initCodeprocessShowPart(Id);
        } else if (value === "persian-currency") {
            initCodeprocessPersianCurrency(Id);
        } else if (value === "time") {
            initCodeprocessTimePickerScript(Id);
        } else if (value == "upload-modal") {
            initUploadModalScript(Id);
        }
    }

    function initUploadModalScript(Id) {
        $("#" + Id).click(function () {
            $("#modalUpload").modal("show");
        });
    }

    function initCodeprocessTimePickerScript(Id) {
        $('#' + Id).timepicker();
    }

    function initCodeprocessPersianCurrency(Id) {
        var labelId = "lbl-currency-" + Id;
        $("#" + Id).keyup(function (value) {
            $("#" + labelId).remove();
            if (this.value !== "") {
                showCurrencyLabel(this.value);
            }
        });

        $("#" + Id).blur(function () {
            $("#" + labelId).remove();
        });

        $("#" + Id).focus(function () {
            $("#" + labelId).remove();
            if (this.value !== "") {
                showCurrencyLabel(this.value);
            }
        });

        function showCurrencyLabel(value) {
            var htmlValue = "<span id='" + labelId + "' class='currency-label'>";
            htmlValue += value.toString().toPersianLetter() + " تومان";
            htmlValue += "</span>";
            $("#" + Id).parent().append(htmlValue);
        }
    }

    function initCodeprocessShowPart(Id) {
        var selectedId = $("#" + Id).find(":selected").attr("value");
        $("[show-part='" + selectedId + "']").show();

        /// start
        var rebateProductId = $("#RebateProductId").val();
        if (isNaN(rebateProductId) == false && rebateProductId != undefined && rebateProductId != null && rebateProductId != "") {
            initializeDiscountColorAndSize(rebateProductId);
        }
        var request = createRequest();
        request.type = REQUEST_TYPE_GET;
        request.url = base_admin_url + "/store/product/SearchAjax";
        request.success = function (result) {
            var products = [];
            for (var i = 0; i < result.length; i++) {
                var codeValue = result[i].CodeValue != null ? "(" + result[i].CodeValue + ")" : ""
                var entity = {
                    value: result[i].Name + codeValue,
                    data: result[i].Id
                };
                products.push(entity);
            }
            $('#ProductBox').autocomplete({
                lookup: products,
                onSelect: function (suggestion) {
                    initializeDiscountColorAndSize(suggestion.data);
                }
            });
        };
        $.ajax(request);

        function initializeDiscountColorAndSize(productId) {
            $("#ProductId").val(productId);
            var colorId = $("#RebateColorId").val();
            var sizeId = $("#RebateSizeId").val();
            createRebateProductColorChanges(productId, colorId);
            createRebateProductSizeChanges(productId, sizeId);
            $("#colorSelectList").attr("productId", productId);
            $("#sizeSelectList").attr("productId", productId);
        }
        /// end


        $("#" + Id).change(function () {
            $("#colorList").addClass('display-none');
            $("#sizeList").addClass('display-none');
            selectedId = $(this).find(":selected").attr("value");
            $("[show-part]").hide();
            $("[show-part='" + selectedId + "']").show();
            if (selectedId != '4122') {
                $("#colorList").addClass('display-none');
                $("#sizeList").addClass('display-none');
                $(".productPart").addClass('display-none');
            }
            else {
                $("#colorList").removeClass('display-none');
                $("#sizeList").removeClass('display-none');
                $(".productPart").removeClass('display-none');
            }
        });
    }

    function initializeCodeprocessUploadDocumentScript(Id) {
        $("#" + Id).change(function () {
            var fileUpload = $("#" + Id).get(0);
            var files = fileUpload.files;

            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }
            var request = createRequest();
            request.url = base_admin_url + "/Upload/UploadDocument";
            request.type = "POST";
            request.data = data;
            request.contentType = false;
            request.processData = false;
            request.beforeSend = function () {
                $("#modalLoading").modal("show");
            }
            request.error = function () {
                createMessage(MESSAGE_TYPE_ERROR, "خطا", "خطا در هنگام آپلود تصویر");
            };
            request.success = function (entity) {
                uploadDocumentId = entity.Id;
                var newUrl = entity.Url.replace("SYSTEM_TYPE_PANEL", window.location.origin + base_admin_url);
                $("#inpUploadModalUrl").val(newUrl);
                $("#modalLoading").modal("hide");
            };
            $.ajax(request);
        });
    }

    function initializeCodeprocessUploadScript(Id) {
        $("#" + Id).dropify({
            messages: {
                'default': 'فایل را اینجا بیندازید',
                'replace': 'برای تغییر تصویر کلیک کنید',
                'remove': 'حذف',
                'error': 'خطا در هنگام ارسال تصویر'
            },
            error: {
                'fileSize': 'The file size is too big (1M max).'
            }
        });
    }

    function initCodeprocessUpload(Id) {
        $("#" + Id).dropify({
            messages: {
                'default': 'فایل را اینجا بیندازید',
                'replace': 'برای تغییر تصویر کلیک کنید',
                'remove': 'حذف',
                'error': 'خطا در هنگام ارسال تصویر'
            },
            error: {
                'fileSize': 'The file size is too big (1M max).'
            }
        });

        $("#" + Id).change(function () {
            var fileUpload = $("#" + Id).get(0);
            var files = fileUpload.files;

            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }
            var request = createRequest();
            request.url = base_admin_url + "/Upload/UploadPhoto";
            request.type = "POST";
            request.data = data;
            request.contentType = false;
            request.processData = false;
            request.beforeSend = function () {
                $("#modalLoading").modal("show");
            }
            request.error = function () {
                createMessage(MESSAGE_TYPE_ERROR, "خطا", "خطا در هنگام آپلود تصویر");
            };
            request.success = function (entity) {
                uploadPictureId = entity.Id;
                $("#modalLoading").modal("hide");
            };
            $.ajax(request);
        });
    }

    function initCodeprocessDatetime(Id) {
        //var objCal = new AMIB.persianCalendar(Id);

        var customOptions = {
            placeholder: "روز / ماه / سال"
            , twodigit: false
            , closeAfterSelect: true
            , nextButtonIcon: "fa fa-arrow-circle-o-right"
            , previousButtonIcon: "fa fa-arrow-circle-o-left"
            , buttonsColor: "blue"
            , forceFarsiDigits: true
            , markToday: true
            , markHolidays: true
            , highlightSelectedDay: true
            , sync: true
            , gotoToday: true
        };
        kamaDatepicker(Id, customOptions);
    }
}

function initCodeprocessEditor(Id) {
    tinymce.init({
        selector: "input#" + Id,
        height: 250,
        theme: 'modern',
        plugins: [
            'advlist autolink lists link image charmap print preview hr anchor pagebreak',
            'searchreplace wordcount visualblocks visualchars code fullscreen',
            'insertdatetime media nonbreaking save table contextmenu directionality',
            'emoticons template paste textcolor colorpicker textpattern imagetools image'
        ],
        toolbar1: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link',
        toolbar2: 'print preview media | forecolor backcolor emoticons | image code',
        images_upload_url: 'upload.php',
        images_upload_handler: function (blobInfo, success, failure) {
            var data = new FormData();
            data.append('file', blobInfo.blob(), blobInfo.filename());
            var request = createRequest();
            request.url = base_admin_url + "/Upload/UploadPhoto";
            request.type = "POST";
            request.data = data;
            request.contentType = false;
            request.processData = false;
            request.error = function () {
                createMessage(MESSAGE_TYPE_ERROR, "خطا", "خطا در هنگام آپلود تصویر");
            };
            request.success = function (entity) {
                let imageUrl = entity.Url.replace("SYSTEM_TYPE_PANEL", window.location.protocol + "//" + window.location.hostname + ":" + window.location.port);
                success(imageUrl);
            };
            $.ajax(request);
        },
    });
}

function clearDropDown(Id, isDefault) {
    $("#" + Id).find("option").remove();
    if (isDefault === true) {
        $("#" + Id).append("<option value='0'>انتخاب</option>");
    }
}

function bindDropDown(Id, entity, name, value, isDefault, selectedItem) {
    $("#" + Id).find("option").remove();
    var result = entity;
    if (isDefault === true) {
        $("#" + Id).append("<option value='0'>انتخاب</option>");
    }

    for (var i = 0; i < result.length; i++) {
        var item = result[i];
        var itemValue = item[value];
        if (selectedItem !== undefined && itemValue !== undefined && itemValue.toString() === selectedItem.toString()) {
            $("#" + Id).append("<option value='" + itemValue + "' selected='selected'>" + item[name] + "</option>");
        }
        else {
            $("#" + Id).append("<option value='" + itemValue + "'>" + item[name] + "</option>");
        }
    }
}


function getCustomFieldControl(item) {
    var itemValue = item.ProductFieldValue === null ? "" : item.ProductFieldValue;
    var itemId = item.ProductFieldItem === null ? 0 : item.ProductFieldItem;
    var listItemId = item.ProductFieldItemList === null ? [] : item.ProductFieldItemList;

    var html = "";
    html += "<div class='col-md-6'>";
    html += "<div class='form-group'>";
    html += "<label class='control-label col-md-4'>" + item.Name + "</label>";
    html += "<div class='col-md-8'>";

    if (item.Type.Label === "FIELD_TYPE_STRING") {
        html += "<input type='text' class='form-control' custom-id='" + item.Id + "' value='" + itemValue + "' custom-type='STRING' autocomplete='off' />";
    } else if (item.Type.Label === "FIELD_TYPE_INTEGER") {
        html += "<input type='text' class='form-control' custom-id='" + item.Id + "' value='" + itemValue + "' custom-type='INTEGER' autocomplete='off' />";
    } else if (item.Type.Label === "FIELD_TYPE_DATE") {
        var tempId = Math.floor(Math.random() * 1000000000);
        html += "<input id='" + tempId + "' type='text' class='form-control' custom-id='" + item.Id + "' value='" + itemValue + "' custom-type='DATETIME' autocomplete='off' />";
    } else if (item.Type.Label === "FIELD_TYPE_TEXT") {
        html += "<input type='text' class='form-control' custom-id='" + item.Id + "' value='" + itemValue + "' custom-type='STRING' autocomplete='off' />";
    } else if (item.Type.Label === "FIELD_TYPE_HTML") {
        html += "<input id='fieldId" + item.Id + "' type='text' class='form-control' custom-id='" + item.Id + "' value='" + itemValue + "' custom-type='HTML' autocomplete='off' />";
    } else if (item.Type.Label === "FIELD_TYPE_FILE") {
        html += "";
    } else if (item.Type.Label === "FIELD_TYPE_DROPDOWN") {
        html += "<select class='form-control' custom-type='DROPDOWN' custom-id='" + item.Id + "'>";
        html += "<option value='0'>انتخاب</option>";
        for (var i = 0; i < item.Items.length; i++) {
            var selectItem = item.Items[i];
            if (selectItem.Id === itemId) {
                html += "<option value='" + selectItem.Id + "' selected='selected'>" + selectItem.Value + "</option>";
            } else {
                html += "<option value='" + selectItem.Id + "'>" + selectItem.Value + "</option>";
            }
        }
        html += "<select>";
    } else if (item.Type.Label === "FIELD_TYPE_BOOLEAN") {
        var isSelected = itemValue === "True" ? " checked='checked' " : "";
        html += "<input type='checkbox' class='margin-checkbox' custom-id='" + item.Id + "' value='" + itemValue + "' " + isSelected + " custom-type='BOOLEAN' />";
    } else if (item.Type.Label === "FIELD_TYPE_CHECKBOX_LIST") {
        for (var j = 0; j < item.Items.length; j++) {
            var selectItemCheckbox = item.Items[j];
            var selectItemCheckboxValue = selectItemCheckbox.Id;
            html += "<label class='check-list'>";

            var isChecked = false;
            for (var k = 0; k < listItemId.length; k++) {
                if (listItemId[k] === selectItemCheckboxValue) {
                    isChecked = true;
                    break;
                }
            }

            if (isChecked === true) {
                html += "<input type='checkbox' class='margin-checkbox' custom-id='" + item.Id + "' value='" + selectItemCheckboxValue + "' custom-type='FIELD_TYPE_CHECKBOX_LIST' checked />";
            } else {
                html += "<input type='checkbox' class='margin-checkbox' custom-id='" + item.Id + "' value='" + selectItemCheckboxValue + "' custom-type='FIELD_TYPE_CHECKBOX_LIST' />";
            }
            html += "<span>" + selectItemCheckbox.Value + "</span></label>";
        }
    }
    html += "</div>";
    html += "</div>";
    html += "</div>";
    return html;
}

function getCheckedValue(obj) {
    return $(obj).is(':checked');
}

function getSelectedValue(obj) {
    var value = $(obj).find(":selected").attr("value");
    value = value === undefined ? parseInt($(obj).val()) : value;
    value = isNaN(value) ? 0 : value;
    return value;
}

function getCodeprocessBackUrl() {
    return $("[codeprocess-back-form]").attr("codeprocess-back-form");
}

function initializePermissionScript() {
    $("#btnSubmit").click(function () {
        
        var entity = {};
        var selectedItems = $("[aria-selected='true']");
        var selectedParents = $(".jstree-undetermined").closest("[role='treeitem']");

        entity.UserId = $("[name='UserId']").val();
        entity.PermissiongroupId = $("[name='PermissiongroupId']").val();        
        entity.PermissionList = [];

        for (var i = 0; i < selectedItems.length; i++) {
            var valueItem = parseInt($(selectedItems[i]).attr("value"));
            entity.PermissionList.push(valueItem);
        }

        for (var j = 0; j < selectedParents.length; j++) {
            var valueParent = parseInt($(selectedParents[j]).attr("value"));
            entity.PermissionList.push(valueParent);
        }

        var request = createRequest(entity);
        request.beforeSend = function () {
            //$("#uplOtherPicture").val("");
            $("#modalLoading").modal("show");
        };
        request.error = function () {
            createMessage(MESSAGE_TYPE_ERROR, "خطا", "خطا در هنگام ذخیره مجوزها    ");
        };
        request.success = function (result) {
            $("#modalLoading").modal("hide"); $("#modalLoading").modal("hide");
            
            if (result.type === MESSAGE_ERROR) {
                createMessage(MESSAGE_TYPE_ERROR, result.Body);
            } else if (result.type === MESSAGE_SUCCESS) {
                createMessage(MESSAGE_TYPE_SUCCESS, result.Body);
                var url = getCodeprocessBackUrl();
                if (url !== undefined) {
                    if (url.toUpperCase().startsWith("/PANEL") === false) {
                        url = base_admin_url + backUrl;
                    }
                    document.location = url;
                }
            }
        }
        $.ajax(request);
    });
}

//function initializeStepwiseDiscount() {

//    function getConditionValues(discount) {
//        $("#btnSubmit").click(function () {
//            var stepwiseDiscount = {};
//        discount.StepwiseDiscountCondition = [];

//        var customElements = $("[name='StepwiseDiscountCondition']:checked");

//        for (var i = 0; i < customElements.length; i++) {
//            var element = customElements[i];
//            var entity = {
//                ID: 0 ,
//                Value: $(element).value,
//                Label: ""
//            };
//            stepwiseDiscount.StepwiseDiscountCondition.push(entity);
//        }
//            console.log(customElements);
//        });
//    }
//    getConditionValues(customElements);
//}

function initializeStepwiseDiscount() {
    function getConditionValues(discount) {
        $("#btnSubmit").click(function () {
            var stepwiseDiscount = {
                StepwiseDiscountCondition: []
            };
            var id = $("#ID").val();
            // var stepwiseDiscount = {};
            // stepwiseDiscount.StepwiseDiscountCondition = [];

            var customElements = $("[name='StepwiseDiscountCondition']:checked");

            for (var i = 0; i < customElements.length; i++) {
                var element = customElements[i];
                var entity = {
                    StepwiseDiscountId: id,
                    Value: $(element).val(),
                    Label: ""
                };
                stepwiseDiscount.StepwiseDiscountCondition.push(entity);
            }

            console.log(stepwiseDiscount);
            return stepwiseDiscount;
        });
    }

    // ایجاد یک شیء تخفیف و ارسال به تابع
    var discount = {
        StepwiseDiscountCondition: []
    };
    getConditionValues(discount);
}

function initializeNewProductScript() {
    var product = {};
    var closeCount = 0;
    fillProductType();
    fillProductPacks();
    removeRelatedPicture();

    $(".dropify-clear").click(function () {
        uploadPictureId = null;
    });

    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(product));
    });

    function getCustomValues(product) {
        product.ProductCustomValue = [];

        var customElements = $("[custom-id]");

        for (var i = 0; i < customElements.length; i++) {
            var element = customElements[i];
            if ($(element).attr("custom-type") === "FIELD_TYPE_CHECKBOX_LIST") {
                if ($(element).prop("checked") === true) {
                    var entity = {
                        ID: 0,
                        FieldId: $(element).attr("custom-id").GetInteger(),
                        Value: null,
                        ItemId: $(element).val().GetInteger()
                    };
                    product.ProductCustomValue.push(entity);
                }
            } else {
                var entity = {
                    ID: 0,
                    FieldId: $(element).attr("custom-id").GetInteger(),
                    Value: getInputValue(element),
                    ItemId: getSelectValue(element)
                };
                product.ProductCustomValue.push(entity);
            }
        }
    }

    function getSelectValue(element) {
        var Value;
        var elementType = $(element).attr("custom-type");
        if (elementType === "DROPDOWN")
            Value = $(element).find(":selected").attr("value");
        else
            Value = null;
        return Value;
    }

    function getInputValue(element) {
        var Value;
        var elementType = $(element).attr("custom-type");
        if (elementType === "STRING")
            Value = $(element).val();
        else if (elementType === "BOOLEAN")
            Value = $(element).prop("checked") === true ? "True" : "False";
        else if (elementType === "INTEGER")
            Value = $(element).val();
        else if (elementType === "DROPDOWN")
            Value = "";
        else if (elementType === "DATETIME")
            Value = $(element).val();
        else
            Value = "";
        return Value;
    }

    $("#ShopId").change(function () {
        fillProductType();
    });

    $("#ProductTypeId").change(function () {
        fillCategory();
        fillBrand();
        fillCustomFields();
        fillColors();
        fillSize();
    });

    $("#ProductCategoryId").change(function () {
        fillSubCategory();
        fillCustomFields();
    });

    $("#ProductSubCategoryId").change(function () {
        fillCustomFields();
    });

    $("#BrandId").change(function () {
        fillCustomFields();
    });

    $("#uplOtherPicture").change(function () {
        var fileUpload = $("#uplOtherPicture").get(0);
        var files = fileUpload.files;

        var data = new FormData();
        for (var i = 0; i < files.length; i++) {
            data.append(files[i].name, files[i]);
        }
        var request = createRequest();
        request.url = base_admin_url + "/Upload/UploadPhoto";
        request.type = "POST";
        request.data = data;
        request.contentType = false;
        request.processData = false;
        request.beforeSend = function () {
            $("#uplOtherPicture").val("");
            $("#modalLoading").modal("show");
        };
        request.error = function () {
            createMessage(MESSAGE_TYPE_ERROR, "خطا", "خطا در هنگام آپلود تصویر");
        };
        request.success = function (entity) {
            addRelatedPicture(entity);
            $("#modalLoading").modal("hide");
        };
        $.ajax(request);
    });

    function removeRelatedPicture() {
        $("[other-picture-Id]").click(function () {
            $(this).remove();
        });
    }

    function addRelatedPicture(picture) {
        var panelUrl = $("#PanelUrl").val();
        var url = picture.Url;
        url = url.replace("SYSTEM_TYPE_PANEL", panelUrl);
        var picHtml = "<div other-picture-Id='" + picture.Id + "' class='col-pic-20'>";
        picHtml += "<label class='other-picture-item'>";
        picHtml += "<img src='" + url + "' />";
        picHtml += "<label class='delete-other-picture' picture-id='" + picture.Id + "'>";
        picHtml += "<i class='fa fa-close' aria-hidden='true'></i>";
        picHtml += "</label>";
        picHtml += "<label>";
        picHtml += "</div>";
        $(".related-picture-container > .row").prepend(picHtml);

        var allItems = $("[other-picture-Id]");
        if (allItems.length > 4) {
            $(".add-item").hide();
        }

        removeRelatedPicture();
    }

    function fillEntity() {
        product.ID = $("#ID").val();
        product.ShopId = $("#ShopId").selected() === null ? parseInt($("#ShopId").val()) : $("#ShopId").selected();
        product.ProductTypeId = $("#ProductTypeId").selected() === null ? parseInt($("#ProductTypeId").val()) : $("#ProductTypeId").selected();
        product.ProductCategoryId = $("#ProductCategoryId").selected() === null ? parseInt($("#ProductCategoryId").val()) : $("#ProductCategoryId").selected();
        product.ProductSubCategoryId = $("#ProductSubCategoryId").selected() === null ? parseInt($("#ProductSubCategoryId").val()) : $("#ProductSubCategoryId").selected();
        product.UnitId = $("#UnitId").selected() === null ? parseInt($("#UnitId").val()) : $("#UnitId").selected();;
        product.BrandId = $("#BrandId").selected() === null ? parseInt($("#BrandId").val()) : $("#BrandId").selected();
        product.StatusId = $("#StatusId").selected();
        product.Name = $("#Name").val();
        product.UrlAddress = $("#UrlAddress").val();
        product.Title = $("#Title").val();
        product.Summary = $("#Summary").val();
        product.CodeValue = $("#CodeValue").val();
        product.ShowNumber = $("#ShowNumber").val() !== undefined ? $("#ShowNumber").val().GetInteger() : 0;
        product.Description = tinymce.get('Description') !== null ? tinymce.get('Description').getContent() : "";
        product.Price = $("#Price").val() !== undefined ? $("#Price").val().GetInteger() : 0;
        product.BasePrice = $("#BasePrice").val() !== undefined ? $("#BasePrice").val().GetFloat() : null;
        product.ShowHomePage = $("#ShowHomePage").prop("checked");
        product.ProductCustomValue = [];
        product.PictureId = uploadPictureId === null ? $("#LastPictureId").val() : uploadPictureId;
        product.DocId = uploadDocumentId === null ? $("#LastDocId").val() : uploadDocumentId;
        product.Active = $("#Active").val() === "True" ? true : false;
        product.SyncDatetime = $("#SyncDatetime").val();
        product.AutoUpdateDatetime = $("#AutoUpdateDatetime").val();
        product.SyncId = $("#SyncId").val();
        product.Quantity = $("#Quantity").val();
        product.VisitCount = $("#VisitCount").val();
        product.CreateDatetime = $("#CreateDatetime").val();
        product.UpdateDatetime = $("#UpdateDatetime").val();
        product.SyncDatetime = $("#SyncDatetime").val();
        product.Weight = $("#Weight").val();
        product.ExpireDatetime = $("#ExpireDatetime").val() === undefined ? null : ToLatinDatetime($("#ExpireDatetime").val());
        product.SyncDisabledColorIds = $("#SyncDisabledColorIds").val();
        product.UrlAddress = $("#UrlAddress").val();
        product.OrderLimit = $("#OrderLimit").val();
        product.MinOrder = parseFloat($("#MinOrder").val());
        //product.MinOrder = $("#MinOrder").val();

        getCustomValues(product);

        product.ProductPicture = [];
        var relatedPictures = $("[other-picture-Id]");
        for (var i = 0; i < relatedPictures.length; i++) {
            var picId = $(relatedPictures[i]).attr("other-picture-Id");
            var picEntity = {
                PictureId: parseInt(picId)
            };
            product.ProductPicture.push(picEntity);
        }

        product.ProductPack = [];
        var packInputs = $(".pack.chk-container input");
        for (var t = 0; t < packInputs.length; t++) {
            var thisInput = packInputs[t];
            if ($(thisInput).prop("checked") === true) {
                var packId = $(thisInput).attr("pack-id");
                var packEntity = {
                    ProductId: product.ID,
                    PackId: parseInt(packId)
                };
                product.ProductPack.push(packEntity);
            }
        }

        product.ProductColor = [];
        var colorInputs = $(".color.chk-container input");
        for (var k = 0; k < colorInputs.length; k++) {
            var thisColorInput = colorInputs[k];
            if ($(thisColorInput).prop("checked") === true) {
                var colorId = $(thisColorInput).attr("color-id");
                var colorEntity = {
                    ProductId: product.ID,
                    ColorId: parseInt(colorId)
                };
                product.ProductColor.push(colorEntity);
            }
        }

        product.ProductSize = [];
        var sizeInputs = $(".size.chk-container input");
        for (var j = 0; j < sizeInputs.length; j++) {
            var thisSizeInput = sizeInputs[j];
            if ($(thisSizeInput).prop("checked") === true) {
                var sizeId = $(thisSizeInput).attr("size-id");
                var sizeEntity = {
                    ProductId: product.ID,
                    SizeId: parseInt(sizeId)
                };
                product.ProductSize.push(sizeEntity);
            }
        }

        product.ProductTag = [];
        var tagsValue = $("#Tag").val();
        if (tagsValue !== undefined) {
            var tags = tagsValue.split(",");
            for (var g = 0; g < tags.length; g++) {
                var tagEntity = {
                    ProductId: product.ID,
                    Name: tags[g]
                };
                product.ProductTag.push(tagEntity);
            }
        }
    }

    function fillProductType() {
        var selected = getSelectedValue($("#ShopId")) === undefined ? $("#ShopId").val() : getSelectedValue($("#ShopId"));
        var request = createRequest();
        request.type = REQUEST_TYPE_GET;
        request.url = base_admin_url + "/store/product/FillProductType?ShopId=" + selected;
        request.success = function (entity) {
            clearDropDown("ProductTypeId", true);
            clearDropDown("ProductCategoryId", true);
            clearDropDown("SubCategoryId", true);
            clearDropDown("BrandId", true);

            var isTypeSelected = $("#isTypeSelected").val() === "False" ? true : false;
            var  getValProductTypeId = bindDropDown("ProductTypeId", entity, "Name", "Id", isTypeSelected, $("#LastProductTypeId").val());
            //console.log("fillCategory", getValProductTypeId);
            //console.log("isTypeSelected", isTypeSelected);
            fillCategory();
            fillBrand();
            fillCustomFields();
            fillColors();
            fillSize();

            closeLoadingModal();
        }
        $.ajax(request);
    }

    function fillProductPacks() {
        var productId = $("#ID").val();
        if (productId !== undefined) {
            var isSynced = $("#IsSynced").val();
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillProductPack?ProductId=" + productId;
            request.success = function (entity) {

                $("#pack-container").empty();
                var result = entity;
                if (result.length > 0) {
                    $("#div-pack-part").show();
                    var html = "";
                    for (var i = 0; i < result.length; i++) {
                        var item = result[i];
                        var isSelected = item.IsSelected === true ? "checked='checked'" : "";
                        html += "<label class='chk-container pack'>";
                        html += "<input pack-id='" + item.Id + "' type='checkbox' " + isSelected + " " + (isSynced === "True" ? "disabled='disabled' readonly='true'" : "") + " />";
                        html += "<span class='checkmark'></span>";
                        html += item.Name;
                        html += "</label>";
                    }
                    $("#pack-container").html(html);
                } else {
                    $("#div-pack-part").hide();
                }
            }
            $.ajax(request);
        }
    }

    function fillCustomFields() {
        var productId = $("#ID").val();
        var selectedType = getSelectedValue($("#ProductTypeId"));
        var selectedCategory = getSelectedValue($("#ProductCategoryId"));
        var selectedSubCategory = getSelectedValue($("#ProductSubCategoryId"));
        var selectedBrandId = getSelectedValue($("#BrandId"));
        if (selectedType !== undefined) {
            selectedCategory = selectedCategory === undefined ? 0 : selectedCategory;
            selectedSubCategory = selectedSubCategory === undefined ? 0 : selectedSubCategory;
            selectedBrandId = selectedBrandId === undefined ? 0 : selectedBrandId;
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillProductCustomFields?TypeId=" + selectedType + "&CategoryId=" + selectedCategory + "&SubCategoryId=" + selectedSubCategory + "&BrandId=" + selectedBrandId + "&ProductId=" + productId;
            request.success = function (entity) {
                var result = entity;
                $("#div-custom-part").empty();
                if (result.length > 0) {
                    $("#div-custom-part").show();
                    var html = "";
                    for (var i = 0; i < result.length; i++) {
                        html += getCustomFieldControl(result[i]);
                    }
                    $("#div-custom-part").append(html);
                    var allDateValues = $("#div-custom-part [custom-type='DATETIME']");
                    for (var k = 0; k < allDateValues.length; k++) {
                        var itemId = $(allDateValues[k]).attr("id");
                        //var objCal = new AMIB.persianCalendar(itemId);
                        var customOptions = {
                            placeholder: "روز / ماه / سال"
                            , twodigit: false
                            , closeAfterSelect: true
                            , nextButtonIcon: "fa fa-arrow-circle-o-right"
                            , previousButtonIcon: "fa fa-arrow-circle-o-left"
                            , buttonsColor: "blue"
                            , forceFarsiDigits: true
                            , markToday: true
                            , markHolidays: true
                            , highlightSelectedDay: true
                            , sync: true
                            , gotoToday: true
                        };
                        kamaDatepicker(itemId, customOptions);
                    }
                } else {
                    $("#div-custom-part").hide();
                }
                fillEditors();
            };
            $.ajax(request);
        } else {
            $("#div-custom-part").hide();
        }
    }

    function fillColors() {
        var productId = $("#ID").val();
        var isSynced = $("#IsSynced").val();
        productId = productId === undefined ? 0 : productId;
        var selected = getSelectedValue($("#ProductTypeId"));
        if (selected !== undefined) {
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillColors?TypeId=" + selected + "&ProductId=" + productId;
            request.success = function (entity) {
                $("#color-container").empty();
                var result = entity;
                if (result.length > 0) {
                    $("#div-color-part").show();
                    var html = "";
                    for (var i = 0; i < result.length; i++) {
                        var item = result[i];
                        var isSelected = item.IsSelected === true ? "checked='checked'" : "";
                        html += "<label class='chk-container color' search-name='" + item.Name + "'>";
                        html += "<input color-id='" + item.Id + "' type='checkbox' " + isSelected + " " + (isSynced === "True" ? "disabled='disabled' readonly='true'" : "") + " />";
                        html += "<span class='checkmark' style='background-color:#" + item.Hex + "'></span>";
                        html += item.Name;
                        if (item.GroupName !== null && item.GroupName !== "") {
                            html += "<span>(" + item.GroupName + ")</span>";
                        }
                        html += "</label>";
                    }
                    $("#color-container").html(html);
                } else {
                    $("#div-color-part").hide();
                }
            }
            $.ajax(request);
        } else {
            $("#div-color-part").hide();
        }
    }

    function fillSize() {
        var selected = getSelectedValue($("#ProductTypeId"));
        var isSynced = $("#IsSynced").val();
        var productId = $("#ID").val();
        productId = productId === undefined ? 0 : productId;
        if (selected !== undefined) {
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillSize?TypeId=" + selected + "&ProductId=" + productId;
            request.success = function (entity) {
                $("#size-container").empty();
                var result = entity;
                if (result.length > 0) {
                    $("#div-size-part").show();
                    var html = "";
                    for (var i = 0; i < result.length; i++) {
                        var item = result[i];
                        var isSelected = item.IsSelected === true ? "checked='checked'" : "";
                        html += "<label class='chk-container size'>";
                        html += "<input size-id='" + item.Id + "' type='checkbox' " + isSelected + " " + (isSynced === "True" ? "disabled = 'disabled' readonly = 'true'" : "") + ">";
                        html += "<span class='checkmark' style='background-color:#" + item.Hex + "'></span>";
                        html += item.Name;
                        html += "</label>";
                    }
                    $("#size-container").html(html);
                } else {
                    $("#div-size-part").hide();
                }
            }
            $.ajax(request);
        } else {
            $("#div-size-part").hide();
        }
    }

    function fillBrand() {
        var selected = getSelectedValue($("#ProductTypeId"));
        if (selected !== undefined) {
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillProductBrand?TypeId=" + selected;
            request.success = function (entity) {
                clearDropDown("BrandId");
                bindDropDown("BrandId", entity, "Name", "Id", true, $("#LastBrandId").val());
                fillCustomFields();

                closeLoadingModal();
            }
            $.ajax(request);
        } else {
            closeLoadingModal();
        }
    }

    function fillCategory() {
        var selected = getSelectedValue($("#ProductTypeId"));
        if (selected !== undefined) {
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillProductCategory?TypeId=" + selected;
            request.success = function (entity) {
                clearDropDown("ProductCategoryId");
                clearDropDown("SubCategoryId");
                bindDropDown("ProductCategoryId", entity, "Name", "Id", true, $("#LastProductCategoryId").val());
                fillCustomFields();
                fillSubCategory();

                closeLoadingModal();
            }
            $.ajax(request);
        } else {
            closeLoadingModal();
        }
    }

    function fillSubCategory() {
        var selected = getSelectedValue($("#ProductCategoryId"));
        if (selected !== undefined) {
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillProductSubCategory?CategoryId=" + selected;
            request.success = function (entity) {
                bindDropDown("ProductSubCategoryId", entity, "Name", "Id", true, $("#LastProductSubCateogryId").val());
                fillCustomFields();
            }
            $.ajax(request);

            closeLoadingModal();
        } else {
            closeLoadingModal();
        }
    }

    function fillEditors() {
        var editors = $("[custom-type='HTML']");
        for (var i = 0; i < editors.length; i++) {
            var editorId = $(editors[i]).attr("id");
            var isFound = false;
            for (var j = 0; j < editorList.length; j++) {
                if (editorList[j] === editorId) {
                    isFound = true;
                }
            }
            if (isFound === false) {
                editorList.push(editorId);
                initCodeprocessEditor(editorId);
            }
        }
    }

    $("#txtSearchColors").keyup(function () {
        var searchValue = $(this).val();
        var allcolors = $(".chk-container.color[search-name]");
        if (searchValue === "") {
            $(allcolors).show();
        } else {
            for (var i = 0; i < allcolors.length; i++) {
                var thisColor = allcolors[i];
                if ($(thisColor).attr("search-name").indexOf(searchValue) !== -1) {
                    $(thisColor).show();
                } else {
                    $(thisColor).hide();
                }
            }
        }
    });

    //var currentId = parseInt($("#ID").val());
    //if (currentId > 0) {
    //    $("#modalLoading").modal("show");
    //}

    $("#modalLoading").modal("show");

    function closeLoadingModal() {
        closeCount++;
        //if (closeCount > 3) {
        //    $("#modalLoading").modal("hide");
        //}
        //console.log(closeCount);
    }
    $(document).ajaxStop(function () {
        // Hide loading modal
        $("#modalLoading").modal("hide");
    });
}

function initializeProductTypeScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.Name = $("#Name").val();
        entity.Label = $("#Label").val();
        entity.Active = $("#Active").prop("checked");
        entity.SyncId = $("#SyncId").val();
        entity.ShowNumber = $("#ShowNumber").val();
        entity.SyncDatetime = $("#SyncDatetime").val();
        entity.AutoUpdateDatetime = $("#AutoUpdateDatetime").val();
        entity.Description = tinymce.get('Description').getContent();
        entity.PictureId = uploadPictureId === null ? $("#LastPictureId").val() : uploadPictureId;
        entity.H1Title = $("#H1Title").val();
        entity.MetaDescription = $("#MetaDescription").val();
        entity.ProductTypeLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                    Description: ""
                };
                entity.ProductTypeLanguage.push(langEntity);
            }
        }
    }
}

function initializeColorScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        var request = createRequest(entity);
        request.success = function (result) {
            if (result.Type === MESSAGE_ERROR) {
                createMessage(MESSAGE_TYPE_ERROR, result.Body);
            } else if (result.Type === MESSAGE_SUCCESS) {
                createMessage(MESSAGE_TYPE_SUCCESS, result.Body);
                var backUrl = getCodeprocessBackUrl();
                var productTypeId = $("#ProductTypeId").val();
                if (productTypeId !== undefined && productTypeId !== "" && productTypeId != null) {
                    if (backUrl !== undefined) {
                        if (backUrl.toUpperCase().startsWith("/PANEL") == false) {
                            backUrl = base_admin_url + backUrl;
                        }
                        document.location = backUrl;
                    }
                }
                else {
                    history.back();
                }
            }
        }
        $.ajax(request);
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.Name = $("#Name").val();
        entity.HexValue = $("#HexValue").val();
        entity.ProductTypeId = $("#ProductTypeId").val();
        entity.ColorGroupId = $("#ColorGroupId").val();
        entity.SyncId = $("#SyncId").val();
        entity.SyncDatetime = $("#SyncDatetime").val();
        entity.ColorLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                    Description: ""
                };
                entity.ColorLanguage.push(langEntity);
            }
        }
    }
}

function initializeSizeScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.Name = $("#Name").val();
        entity.ProductTypeId = $("#ProductTypeId").val();
        entity.SyncId = $("#SyncId").val();
        entity.SyncDatetime = $("#SyncDatetime").val();
        entity.SizeLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                    Description: ""
                };
                entity.SizeLanguage.push(langEntity);
            }
        }
    }
}

function initializeProductCategoryScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.TypeId = getSelectedValue("#TypeId");
        entity.Active = $("#Active").val();
        entity.Name = $("#Name").val();
        entity.Label = $("#Label").val();
        entity.SyncId = $("#SyncId").val();
        entity.SyncDatetime = $("#SyncDatetime").val();
        entity.UpdateDatetime = $("#UpdateDatetime").val();
        entity.AutoUpdateDatetime = $("#AutoUpdateDatetime").val();
        entity.ShowNumber = $("#ShowNumber").val();
        entity.H1Title = $("#H1Title").val();
        entity.MetaDescription = $("#MetaDescription").val();
        entity.Description = tinymce.get('Description').getContent();
        entity.PictureId = uploadPictureId === null ? $("#LastPictureId").val() : uploadPictureId;
        entity.ProductCategoryLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                    Description: ""
                };
                entity.ProductCategoryLanguage.push(langEntity);
            }
        }
    }
}

function initializeProductCustomFieldScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.ProductTypeId = $("#ProductTypeId").val();
        entity.ProductCategoryId = $("#ProductCategoryId").val();
        entity.ProductSubCategoryId = $("#ProductSubCategoryId").val();
        entity.ProductBrandId = $("#ProductBrandId").val();
        entity.TypeId = getSelectedValue("#TypeId");
        entity.Name = $("#Name").val();
        entity.SyncId = $("#SyncId").val();
        entity.SyncName = $("#SyncName").val();
        entity.SyncDatetime = $("#SyncDatetime").val();
        entity.ShowNumber = $("#ShowNumber").val();
        entity.IsRequired = getCheckedValue("#IsRequired");
        entity.IsEditable = getCheckedValue("#IsEditable");
        entity.ProductCustomFieldLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                };
                entity.ProductCustomFieldLanguage.push(langEntity);
            }
        }
    }
}

function initializeProductCustomItemScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.FieldId = $("#FieldId").val();
        entity.SyncId = $("#SyncId").val();
        entity.SyncDatetime = $("#SyncDatetime").val();
        entity.Value = $("#Value").val();
        entity.Label = $("#Label").val();

        entity.Title = $("#Title").val();
        entity.H1Title = $("#H1Title").val();
        entity.Body = tinymce.get('Body') !== null ? tinymce.get('Body').getContent() : "";
        entity.MetaDescription = $("#MetaDescription").val();
        entity.PictureId = uploadPictureId === null ? $("#LastPictureId").val() : uploadPictureId;

        entity.ProductCustomItemLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Value: $(langObject).find("[lang-name='Value']").val(),
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                };
                entity.ProductCustomItemLanguage.push(langEntity);
            }
        }
    }
}

function initializeDiscountScript() {
    var request = createRequest();
    request.type = REQUEST_TYPE_GET;
    request.url = base_admin_url + "/store/product/SearchAjax";
    request.success = function (result) {
        var products = [];
        for (var i = 0; i < result.length; i++) {
            var codeValue = result[i].CodeValue != null ? "(" + result[i].CodeValue + ")" : ""
            var entity = {
                value: result[i].Name + codeValue,
                data: result[i].Id
            };
            products.push(entity);
        }
        $('#ProductBox').autocomplete({
            lookup: products,
            onSelect: function (suggestion) {
                initializeDiscountColorScript(suggestion.data);
                initializeDiscountSizeScript(suggestion.data);
            }
        });
    };
    $.ajax(request);

    function initializeDiscountColorScript(productId) {
        $("#ProductId").val(productId);
        var selectedColorId = parseInt($("#ColorId").val());

        var requestColor = createRequest();
        requestColor.type = REQUEST_TYPE_GET;
        requestColor.url = base_admin_url + "/store/discount/FillColor?productId=" + productId;
        requestColor.success = function (resultColor) {
            $("#ColorId").empty();
            $("#ColorId").append("<option value='0'>انتخاب</option>");
            if (resultColor.length > 0) {
                $("#divColorPart").show();
                for (var i = 0; i < resultColor.length; i++) {
                    var colorItem = resultColor[i];
                    if (colorItem.Id == selectedColorId) {
                        $("#ColorId").append("<option selected value='" + colorItem.Id + "'>" + colorItem.Name + "</option>");
                    } else {
                        $("#ColorId").append("<option value='" + colorItem.Id + "'>" + colorItem.Name + "</option>");
                    }
                }
            } else {
                $("#divColorPart").hide();
            }
        };
        $.ajax(requestColor);
    }
    function initializeDiscountSizeScript(productId) {
        $("#ProductId").val(productId);
        var selectedSizeId = parseInt($("#SizeId").val());

        var requestSize = createRequest();
        requestSize.type = REQUEST_TYPE_GET;
        requestSize.url = base_admin_url + "/store/discount/FillSize?productId=" + productId;
        requestSize.success = function (resultSize) {
            $("#SizeId").empty();
            $("#SizeId").append("<option value='0'>انتخاب</option>");
            if (resultSize.length > 0) {
                $("#divSizePart").show();
                for (var i = 0; i < resultSize.length; i++) {
                    var sizeItem = resultSize[i];
                    if (sizeItem.Id == selectedSizeId) {
                        $("#SizeId").append("<option selected value='" + sizeItem.Id + "'>" + sizeItem.Name + "</option>");
                    } else {
                        $("#SizeId").append("<option value='" + sizeItem.Id + "'>" + sizeItem.Name + "</option>");
                    }
                }
            } else {
                $("#divSizePart").hide();
            }
        };
        $.ajax(requestSize);
    }
}

function initializeCollectionScript() {
    var request = createRequest();
    request.type = REQUEST_TYPE_GET;
    request.url = base_admin_url + "/store/product/SearchAjax";
    request.success = function (result) {
        var products = [];
        for (var i = 0; i < result.length; i++) {
            var entity = {
                value: result[i].Name,
                data: result[i].Id
            };
            products.push(entity);
        }
        $('#ProductBox').autocomplete({
            lookup: products,
            onSelect: function (suggestion) {
                var html = "<tr class='product-row-item'>";
                html += "<td>" + ($(".product-row-item").length + 1) + "</td>";
                html += "<td>" + suggestion.value;
                html += "<input type='hidden' name='productId' value='" + suggestion.data + "' />";
                html += "<a onclick='removeCollectionRow(this)' class='remove-row-collection'><span class='glyphicon glyphicon-trash'></span></a>";
                html += "</tr>";
                $("#tblProducts tbody").append(html);
                $("#ProductBox").val(null);
                $("#ProductBox").focus();
            }
        });
    };
    $.ajax(request);
}

function removeCollectionRow(target) {
    $(target).closest(".product-row-item").remove();
}

function initializeProductSubCategoryScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.Name = $("#Name").val();
        entity.Label = $("#Label").val();
        entity.SyncId = $("#SyncId").val();
        entity.CategoryId = getSelectedValue("#CategoryId");
        entity.ProductSubCategoryLanguage = [];
        entity.PictureId = uploadPictureId === null ? $("#LastPictureId").val() : uploadPictureId;
        entity.SyncDatetime = $("#SyncDatetime").val();
        entity.UpdateDatetime = $("#UpdateDatetime").val();
        entity.AutoUpdateDatetime = $("#AutoUpdateDatetime").val();
        entity.ShowNumber = $("#ShowNumber").val();
        entity.Description = tinymce.get('Description').getContent();
        entity.H1Title = $("#H1Title").val();
        entity.MetaDescription = $("#MetaDescription").val();

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    CategoryId: entity.CategoryId,
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                    Description: ""
                };
                entity.ProductSubCategoryLanguage.push(langEntity);
            }
        }
    }
}

function initializeShopProfileScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        var shopId = parseInt($("#ID").val())
        entity.ID = shopId;
        entity.PictureId = uploadPictureId === null ? parseInt($("#PictureId").val()) : uploadPictureId;
        entity.Label = $("#Label").val();
        entity.UserCreatorId = parseInt($("#UserCreatorId").val());
        entity.WebsiteId = parseInt($("#WebsiteId").val());
        entity.Approved = $("#Approved").val();
        entity.Active = $("#Active").val();
        entity.Description = $("#Description").val();

        entity.Name = $("#Name").val();
        entity.TypeId = getSelectedValue("#TypeId");
        entity.CityId = getSelectedValue("#CityId");

        entity.ShopAddress = [];
        entity.ShopContact = [];
        entity.ShopProductType = [];
        entity.ShopPaymentType = [];

        var contactItems = $("[contact-id]");
        if (contactItems.length > 0) {
            for (var i = 0; i < contactItems.length; i++) {
                var value = $(contactItems[i]).val();
                if (value !== "") {
                    var typeId = parseInt($(contactItems[i]).attr("contact-id"));
                    var contactEntity = {
                        ShopId: shopId,
                        TypeId: typeId,
                        Value: value
                    };
                    entity.ShopContact.push(contactEntity);
                }
            }
        }

        var selectedTypes = $("[name='ProductType']:checked");
        for (var j = 0; i < selectedTypes.length; j++) {
            var valueType = $(selectedTypes[j]).val();
            var typeEntity = {
                ShopId: shopId,
                ProductTypeId: valueType
            };
            entity.ShopProductType.push(typeEntity);
        }

        var selectedPayment = $("[name='PaymentType']:checked");
        for (var k = 0; k < selectedPayment.length; k++) {
            var valuePayment = $(selectedPayment[k]).val();
            var paymentEntity = {
                ShopId: shopId,
                PaymentTypeId: valuePayment
            };
            entity.ShopPaymentType.push(paymentEntity);
        }

        var addEntity = {
            ID: parseInt($("#AddressId").val()),
            Address: $("#Address").val()
        };

        entity.ShopAddress.push(addEntity);
    }
}

function initializeReportByWeekdayScript() {
    var labels = [];
    var series = [];
    var visitCount = [];
    visitCount.push(0);
    var request = createRequest();
    request.type = REQUEST_TYPE_GET;
    request.url = base_admin_url + "/report/websiteview/reportbyweekday";
    request.success = function (entity) {
        for (var i = 0; i < entity.length; i++) {
            var temp = entity[i];
            if (i === 0)
                visitCount[0] = parseInt(temp.Value);
            labels.push(temp.Key);
            visitCount.push(parseInt(temp.Value));
        }
        series.push({
            name: 'بازدید روزانه',
            data: visitCount
        });

        new Chartist.Line('#simple-line-chart', {
            labels: labels,
            series: series
        },
            {
                plugins: [
                    Chartist.plugins.tooltip()
                ]
            }
        );
    }
    $.ajax(request);
}

function initializeOrderListScript() {
    $(document).ready(function () {
        $("[order-id]").click(function () {
            var isSelected = $(this).prop("checked");
            var orderId = $(this).attr("order-id");
            var selectedRow = $("tr[row-order-id='" + orderId + "']");
            $(selectedRow).attr("row-selected", isSelected);

            var selectedItems = $("[row-selected='true']");
            var selectedIds = "";
            for (var i = 0; i < selectedItems.length; i++) {
                var newRowId = $(selectedItems[i]).attr("row-order-id");
                selectedIds = selectedIds + "," + $(selectedItems[i]).attr("row-order-id");
            }
            var hrefValueStore = $(".goto-store").attr("href");
            var hrefValueProcess = $(".goto-process").attr("href");
            var hrefValueReady = $(".goto-ready").attr("href");

            var splitHrefStore = hrefValueStore.split("orderId=");
            var splitHrefProcess = hrefValueProcess.split("orderId=");
            var splitHrefReady = hrefValueReady.split("orderId=");

            hrefValueStore = splitHrefStore[0] + "orderId=0" + selectedIds;
            hrefValueProcess = splitHrefProcess[0] + "orderId=0" + selectedIds;
            hrefValueReady = splitHrefReady[0] + "orderId=0" + selectedIds;

            $(".goto-store").attr("href", hrefValueStore);
            $(".goto-process").attr("href", hrefValueProcess);
            $(".goto-ready").attr("href", hrefValueReady);
        });

        $("[select-all='true']").click(function () {
            var ischecked = $("[order-id]").prop("checked");
            $("[order-id]").prop("checked", ischecked);
            $("[order-id]").trigger("click");
        });

        $("#chkCheckAll").click(function () {
            var doAction = $(this).attr("do-action");
            if (doAction === "true") {
                $("[name='statusId']").prop("checked", true);
                $(this).attr("do-action", "false");
            } else {
                $("[name='statusId']").prop("checked", false);
                $(this).attr("do-action", "true");
            }
        });

        $("#chkCheckSuccessPayment").click(function () {
            $("[value-label='ORDER_STATUS_SUCCESS']").prop("checked", true);
            $("[value-label='ORDER_STATUS_PROCESS']").prop("checked", true);
            $("[value-label='ORDER_STATUS_STORE']").prop("checked", true);
            $("[value-label='ORDER_STATUS_READY']").prop("checked", true);
            $("[value-label='ORDER_STATUS_POST']").prop("checked", true);
            $("[value-label='ORDER_STATUS_CANCEL']").prop("checked", true);
            $("[value-label='ORDER_STATUS_CANCEL_CUSTOMER']").prop("checked", true);
        });
    });
}

function initializeOrderNewScript() {
    $(document).ready(function () {
        initializeAccountListScript();
    });

    function initializeAccountListScript() {
        var request = createRequest();
        request.type = REQUEST_TYPE_GET;
        request.url = base_admin_url + "/crm/account/SearchAjax";
        request.success = function (result) {
            var accountList = [];
            for (var i = 0; i < result.length; i++) {
                var entity = {
                    value: result[i].Name,
                    data: result[i].Id
                };
                accountList.push(entity);
            }
            $('#txtCustomer').autocomplete({
                lookup: accountList,
                onSelect: function (suggestion) {
                    $("#AccountId").val(suggestion.data);
                    $("#divAddressRow").show();
                    $("#lnkNewAddress").attr("href", base_admin_url + "/crm/account/address?accountId=" + suggestion.data + "&backUrl=" + base_admin_url + "/crm/accountorder/createnew/?accountId=" + suggestion.data);
                    initializeAccountAddressListScript(suggestion.data);
                }
            });
        }
        $.ajax(request);
    }

    function initializeAccountAddressListScript(accountId) {
        var request = createRequest();
        request.type = REQUEST_TYPE_GET;
        request.url = base_admin_url + "/crm/account/SearchAjaxAddress?accountId=" + accountId;
        request.success = function (result) {
            $("#drpAddress").empty();
            for (var i = 0; i < result.length; i++) {
                var entity = result[i];
                $("#drpAddress").append("<option value='" + entity.Id + "'>" + entity.Name + "</option>");
            }
        }
        $.ajax(request);
    }
}

function initializeOrderEditScript() {
    $("#lnkAddProduct").click(function () {
        $(".bs-example-modal-sm").modal("show");
    });

    var request = createRequest();
    request.type = REQUEST_TYPE_GET;
    request.url = base_admin_url + "/crm/accountorder/SearchAjaxProduct";
    request.success = function (result) {
        var productList = [];
        for (var i = 0; i < result.length; i++) {
            var entity = {
                value: result[i].Name,
                data: result[i].Id
            };
            if (result[i].Colors.length > 0) {
                entity.Colors = result[i].Colors;
            }
            if (result[i].Sizes.length > 0) {
                entity.Sizes = result[i].Sizes;
            }
            productList.push(entity);
        }
        $('#txtProductName').autocomplete({
            lookup: productList,
            onSelect: function (suggestion) {
                console.log(suggestion);
                $("#newProductId").val(suggestion.data);
                if (suggestion.Colors != null &&
                    suggestion.Colors != undefined &&
                    suggestion.Colors.length > 0) {
                    $("#divNewProductColorSelect").show();
                    $("#newProductOrderColorList").empty();
                    $("#newProductOrderColorList").append("<option value='0'>انتخاب</option>");
                    for (var i = 0; i < suggestion.Colors.length; i++) {
                        var colorItem = suggestion.Colors[i];
                        $("#newProductOrderColorList").append("<option value='" + colorItem.Id + "'>" + colorItem.Name + "</option>");
                    }
                }
                if (suggestion.Sizes != null &&
                    suggestion.Sizes != undefined &&
                    suggestion.Sizes.length > 0) {
                    $("#divNewProductSizeSelect").show();
                    $("#newProductOrderSizeList").empty();
                    $("#newProductOrderSizeList").append("<option value='0'>انتخاب</option>");
                    for (var i = 0; i < suggestion.Sizes.length; i++) {
                        var sizeItem = suggestion.Sizes[i];
                        $("#newProductOrderSizeList").append("<option value='" + sizeItem.Id + "'>" + sizeItem.Name + "</option>");
                    }
                }
            }
        });
    }
    $.ajax(request);

    $("#btnAddProduct").click(function () {
        var entity = {
            OrderId: parseInt($("#newProductOrderId").val()),
            Product: {
                Id: parseInt($("#newProductId").val())
            },
            Count: parseInt($("#txtProductCount").val()),
            ColorId: parseInt($("#newProductOrderColorList").val()),
            SizeId: parseInt($("#newProductOrderSizeList").val())
        };

        console.log(entity);

        var request = createRequest();
        request.type = REQUEST_TYPE_POST;
        request.data = JSON.stringify(entity);
        request.url = base_admin_url + "/crm/accountorder/AddProductToOrder";
        request.success = function (result) {
            if (result.Type == 2) {
                createMessage("success", "با موفقیت ثبت شد");
                location.reload();
            } else {
                createMessage("error", result.Body);
            }
        }
        $.ajax(request);
    });

    $("#drpChangeSendType").change(function () {
        $("#frpChangeSendType").submit();
    });
}

function initializeProductQuantityScript() {
    $("#btnSubmit").click(function () {
        var listQuantiy = [];
        var rowList = $("[quantity-id]");
        for (var i = 0; i < rowList.length; i++) {
            var item = rowList[i];
            var entity = {
                ID: parseInt($(item).attr("quantity-id")),
                Price: parseInt($(item).find("input.price").val()),
                Count: parseInt($(item).find("input.count").val()),
                ExpireDatetime: $(item).find("input.expire").val()
            };
            listQuantiy.push(entity);
        }
        $.ajax(createRequest(listQuantiy));
    });

    $("[quantity-sync-id]").click(function () {
        $("#mySmallModalLabel").html($(this).attr("value-name"));
        $("#btnQuantityUpdate").attr("value-id", $(this).attr("value-id"));
        $("#txtQuantitySyncId").val($(this).attr("quantity-sync-id"));
        $(".bs-example-modal-sm").modal("show");
    });

    $("#btnQuantityUpdate").click(function () {
        var request = createRequest();
        request.type = "POST";
        var valueId = $(this).attr("value-id");
        request.url = request.url + "update?id=" + valueId + "&syncId=" + $("#txtQuantitySyncId").val();
        request.success = function () {
            document.location = document.location;
        };
        $.ajax(request);
    });

    $("#lnkNewQuantity").click(function () {
        $(".bs-example-modal-sm-new-quantity").modal("show");
    });

    $("#btnNewQuantity").click(function () {
        var productId = $(this).attr("product-id");
        var syncId = $("#txtNewQuantitySyncId").val();
        var weight = $("#txtNewQuantityWeight").val();
        var count = $("#txtNewQuantityCount").val();
        var name = $("#txtNewQuantityName").val();
        var request = createRequest();
        request.type = "POST";
        request.url = base_admin_url + "/store/product/newquantity?productId=" + productId + "&name=" + name + "&syncId=" + syncId + "&count=" + count + "&weight=" + weight;
        console.log(request.url);
        request.success = function () {
            document.location = document.location;
        };
        $.ajax(request);
    });
}

function initializeExtraSendScript() {
    $("#btnUpdateWeight").click(function () {
        var btn = $(this);
        var orderId = $(btn).attr("order-id");
        var orderWeight = $("#txtOrderWeight").val();
        var request = createRequest();
        request.type = "GET";
        request.url = $(btn).attr("order-url") + "?Id=" + orderId + "&weight=" + orderWeight;
        request.success = function () {
            document.location = document.location;
        };
        $.ajax(request);
    });
}

function initializeColorSyncModalScript() {
    $("[data-toggle='modal']").click(function () {
        $(".modal-title").html($(this).attr("value-name"));
        $("#btnColorUpdate").attr("value-id", $(this).attr("value-id"));
        $("#txtColorSyncId").val($(this).attr("color-sync-id"));
        $(".bs-example-modal-sm").modal("show");
    });

    $("#btnColorUpdate").click(function () {
        var request = createRequest();
        request.type = "POST";
        var valueId = $(this).attr("value-id");
        request.url = request.url + "update?id=" + valueId + "&syncId=" + $("#txtColorSyncId").val();
        request.success = function () {
            document.location = document.location;
        };
        $.ajax(request);
    });
}

function initializeMenuPageScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.ParentId = getSelectedValue("#ParentId");
        entity.TypeId = getSelectedValue("#TypeId");
        entity.PostId = getSelectedValue("#PostId");
        entity.CategoryId = getSelectedValue("#CategoryId");
        entity.GalleryId = getSelectedValue("#GalleryId");
        entity.PictureId = uploadPictureId === null ? $("#LastPictureId").val() : uploadPictureId;
        entity.Active = getCheckedValue("#Active");
        entity.Name = $("#Name").val();
        entity.Link = $("#Link").val();
        entity.ShowNumber = parseInt($("#ShowNumber").val());
        entity.MenuLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    CategoryId: entity.CategoryId,
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                    Description: ""
                };
                entity.MenuLanguage.push(langEntity);
            }
        }
    }
}

function initializeDashboardPageScript() {
    $("#checkRebateOnline").click(function () {
        var codeValue = $("[name='codevalue']").val();
        var entity = {
            CodeValue: codeValue
        };
        var response = createRequest(entity);
        response.url = "/store/rebate/checkrebate";
        response.success = function (result) {
            if (result.Type === MESSAGE_ERROR) {
                createMessage(MESSAGE_TYPE_ERROR, result.Body);
            } else if (result.Type === MESSAGE_SUCCESS) {
                $(".btn-container").show();
                $("#btnPrintRebate").attr("href", "/store/rebate/print/" + codeValue);
            }
        };
        $.ajax(response);
    });

    $("#btnCancelRebate").click(function () {
        var codeValue = $("[name='codevalue']").val();
        var entity = {
            CodeValue: codeValue
        };
        var response = createRequest(entity);
        response.url = "/store/rebate/cancelrebate";
        response.success = function (result) {
            if (result.Type === MESSAGE_ERROR) {
                createMessage(MESSAGE_TYPE_ERROR, result.Body);
            } else if (result.Type === MESSAGE_SUCCESS) {
                $(".btn-container").hide();
                $("#btnPrintRebate").attr("href", "");
                $("[name='codevalue']").val("");
            }
        };
        $.ajax(response);
    });
}

function initializeOrderPageScript() {
    $("#extraSendTypeId").change(function () {
        var selectedId = parseInt($("#extraSendTypeId").find(":selected").attr("value"));
        if (selectedId === 0) {
            $("#btnExtraSend").hide();
        } else {
            $("#btnExtraSend").show();
        }
    });

    $("[order-item-id]").click(function () {
        var itemStatusId = $(this).attr("order-status-id");
        var itemLabel = $(this).attr("order-status-label");
        if (itemLabel === "ORDER_STATUS_POST") {
            $("#divOrderItemStatus").show();
        } else {
            $("#divOrderItemStatus").hide();
        }
        $("#txtOrderItemPostalCode").val($(this).attr("order-item-postal-code"));
        $("#drpOrderItemStatus").val(itemStatusId);
        $("#mySmallModalLabel").html($(this).attr("value-name"));
        $("#btnOrderItemUpdate").attr("order-item-id", $(this).attr("order-item-id"));
        $(".bs-example-modal-sm").modal("show");
    });

    $("#drpOrderItemStatus").change(function () {
        var selectedItem = $(this).find(":selected");
        var selectedLabel = $(selectedItem).attr("value-label");
        if (selectedLabel === "ORDER_STATUS_POST") {
            $("#divOrderItemStatus").show();
        } else {
            $("#divOrderItemStatus").hide();
        }
    });

    $("#btnOrderItemUpdate").click(function () {
        var data = {
            Id: parseInt($(this).attr("order-item-id")),
            StatusId: parseInt($("#drpOrderItemStatus").find(":selected").val()),
            PostalCode: $("#txtOrderItemPostalCode").val()
        };

        var request = createRequest();
        request.type = "POST";
        request.data = data;
        request.contentType = "application/json";
        request.url = base_admin_url + "/crm/accountorder/UpdateOrderItem";
        request.success = function (result) {
            console.log(result);

            //location.reload();
        };
        $.ajax(request);
    });
}

function initializeCategoryPageScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.WebsiteId = $("#WebsiteId").val();
        entity.TypeId = $("#TypeId").val();
        entity.Name = $("#Name").val();
        entity.Label = $("#Label").val();
        entity.CategoryLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val())
                };
                entity.CategoryLanguage.push(langEntity);
            }
        }
    }
}

function initializePostPageScript() {
    var entity = {};
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.CreateDateTime = $("#CreateDateTime").val();
        entity.WebsiteId = $("#WebsiteId").val();
        entity.CategoryId = $("#CategoryId").val();
        entity.Name = $("#Name").val();
        entity.Keywords = $("#Keywords").val();
        entity.Summary = $("#Summary").val();
        entity.UrlAddress = $("#UrlAddress").val();
        entity.ShowDateTime = $("#ShowDateTime").val();
        entity.ShowNumber = $("#ShowNumber").val();
        entity.Active = $("#Active").prop("checked");
        entity.PictureId = uploadPictureId === null ? $("#LastPictureId").val() : uploadPictureId;
        entity.Body = tinymce.get('Body') !== null ? tinymce.get('Body').getContent() : "";
        entity.PostLanguage = [];

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var languageId = parseInt($(langObject).find("[lang-name='LanguageId']").val());
                var bodyId = $(langObject).find("[lang-name='Body']").attr("id");
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    Summary: $(langObject).find("[lang-name='Summary']").val(),
                    Keywords: $(langObject).find("[lang-name='Keywords']").val(),
                    Body: tinymce.get(bodyId) !== null ? tinymce.get(bodyId).getContent() : "",
                    LanguageId: languageId
                };
                entity.PostLanguage.push(langEntity);
            }
        }

        entity.PostTag = [];
        var tagsValue = $("#Tag").val();
        if (tagsValue !== undefined) {
            var tags = tagsValue.split(",");
            for (var g = 0; g < tags.length; g++) {
                var thisValue = tags[g];
                if (thisValue !== null && thisValue !== "" && thisValue !== undefined) {
                    var tagEntity = {
                        PostId: entity.ID,
                        Name: thisValue
                    };
                    entity.PostTag.push(tagEntity);
                }
            }
        }
    }
}

function initializeUploadExcelScript() {
    $("#ProductTypeId").change(function () {
        var typeId = $(this).val();
        if (typeId === "0") {
            $("#ProductCategoryId").val(0);
            $("#ProductSubCategoryId").val(0);
        }
        $("#frmExcelFilter").submit();
    });

    $("#ProductCategoryId").change(function () {
        var categoryId = $(this).val();
        if (categoryId === "0") {
            $("#ProductSubCategoryId").val(0);
        }
        $("#frmExcelFilter").submit();
    });

    $("#ProductSubCategoryId").change(function () {
        $("#frmExcelFilter").submit();
    });
}

function initializeCreditBoxScript() {
    $(".action-info").click(function () {
        $(this).parent().find(".action-info-box").show();
        $(".action-info-box-overlay").show();
    });

    $(".action-info-box-overlay").click(function () {
        $(".action-info-box").hide();
        $(".action-info-box-overlay").hide();
    });
}

function initializeProductSetItemsScript() {
    var request = createRequest();
    request.type = REQUEST_TYPE_GET;
    request.url = base_admin_url + "/store/product/SearchAjaxDetails";
    request.success = function (result) {
        var products = [];
        for (var i = 0; i < result.length; i++) {
            var entityName = result[i].CodeValue == null ? result[i].Name : result[i].Name + " (" + result[i].CodeValue + ")";
            var entity = {
                value: entityName,
                data: result[i].Id,
                color: result[i].Colors,
                size: result[i].Sizes
            };
            products.push(entity);
        }
        $('#txtProductName').autocomplete({
            lookup: products,
            onSelect: function (suggestion) {
                $("#ProductId").val(suggestion.data);
                $("#drpSize").empty();
                $("#drpColor").empty();

                if (suggestion.size.length > 0) {
                    $("#divSizePart").show();
                    $("#drpSize").append("<option value='" + null + "'>" + "انتخاب کنید" + "</option>");
                    for (var i = 0; i < suggestion.size.length; i++) {
                        $("#drpSize").append("<option value='" + suggestion.size[i].Id + "'>" + suggestion.size[i].Name + "</option>");
                    }
                } else {
                    $("#divSizePart").hide();
                }

                if (suggestion.color.length > 0) {
                    $("#divColorPart").show();
                    $("#drpColor").append("<option value='" + null + "'>" + "انتخاب کنید" + "</option>");
                    for (var i = 0; i < suggestion.color.length; i++) {
                        $("#drpColor").append("<option value='" + suggestion.color[i].Id + "'>" + suggestion.color[i].Name + "</option>");
                    }
                } else {
                    $("#divColorPart").hide();
                }
            }
        });
    };
    $.ajax(request);

    $("#btnCreateItem").click(function () {
        var entity = {};
        entity.ProductSetId = $("#ProductSetId").val();
        entity.ProductId = $("#ProductId").val();
        entity.ColorId = $("#drpColor").val();
        entity.SizeId = $("#drpSize").val();
        entity.Count = $("#txtCount").val();
        $.ajax(createRequest(entity));
    });
}

function initializeReportSalePageScript() {
    var chart1, chart2, chart3, chart4, chart5, chart6;
    initializeSaleBarScript();
    initializeInsertedBarScript();
    initializeAddressBarScript();

    initializeSaleCountBarScript();
    initializeInsertedCountBarScript();
    initializeAddressCountBarScript();

    $("#btnShowCart").click(function () {
        chart1.destroy();
        chart2.destroy();
        chart3.destroy();
        chart4.destroy();
        chart5.destroy();
        chart6.destroy();
        initializeSaleBarScript();
        initializeInsertedBarScript();
        initializeAddressBarScript();

        initializeSaleCountBarScript();
        initializeInsertedCountBarScript();
        initializeAddressCountBarScript();
    });

    function initializeAddressBarScript() {
        var myLineLabels = [];
        var myLineData = [];

        var fromDate = $("#txtDateFrom").val();
        var toDate = $("#txtDateTo").val();

        var request = createRequest();
        request.url = base_admin_url + "/report/reportsale/getaddresschart?fromDate=" + fromDate + "&toDate=" + toDate;
        request.success = function (result) {
            for (var i = 0; i < result.length; i++) {
                myLineLabels.push(result[i].Key);
                myLineData.push(result[i].Value);
            }

            const ctx = document.getElementById('barChartAddress');

            chart4 = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: myLineLabels,
                    datasets: [{
                        label: '',
                        data: myLineData
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: {
                        display: false,
                        position: 'top',
                    },
                    title: {
                        display: false,
                        text: ''
                    },
                    tooltips: {
                        enabled: true
                    },
                    plugins: {
                        tooltip: {
                            enabled: true
                        }
                    }
                }
            });
        };
        $.ajax(request);
    }

    function initializeInsertedBarScript() {
        var myLineLabels = [];
        var myLineData = [];

        var fromDate = $("#txtDateFrom").val();
        var toDate = $("#txtDateTo").val();

        var request = createRequest();
        request.url = base_admin_url + "/report/reportsale/getinsertedchart?fromDate=" + fromDate + "&toDate=" + toDate;
        request.success = function (result) {
            for (var i = 0; i < result.length; i++) {
                myLineLabels.push(result[i].Key);
                myLineData.push(result[i].Value);
            }

            const ctx = document.getElementById('barChartInserted');

            chart3 = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: myLineLabels,
                    datasets: [{
                        label: '',
                        data: myLineData
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: {
                        display: false,
                        position: 'top',
                    },
                    title: {
                        display: false,
                        text: ''
                    },
                    tooltips: {
                        enabled: true
                    },
                    plugins: {
                        tooltip: {
                            enabled: true
                        }
                    }
                }
            });
        };
        $.ajax(request);
    }

    function initializeSaleBarScript() {
        var myLineLabels = [];
        var myLineData = [];

        var fromDate = $("#txtDateFrom").val();
        var toDate = $("#txtDateTo").val();

        var request = createRequest();
        request.url = base_admin_url + "/report/reportsale/getsalechart?fromDate=" + fromDate + "&toDate=" + toDate;
        request.success = function (result) {
            for (var i = 0; i < result.length; i++) {
                myLineLabels.push(result[i].Key);
                myLineData.push(result[i].Value);
            }

            const ctx = document.getElementById('barChart');

            chart1 = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: myLineLabels,
                    datasets: [{
                        label: '',
                        data: myLineData
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: {
                        display: false,
                        position: 'top',
                    },
                    title: {
                        display: false,
                        text: ''
                    },
                    tooltips: {
                        enabled: true
                    },
                    plugins: {
                        tooltip: {
                            enabled: true
                        }
                    }
                }
            });
        };
        $.ajax(request);
    }

    function initializeSaleCountBarScript() {
        var myLineLabels = [];
        var myLineData = [];

        var fromDate = $("#txtDateFrom").val();
        var toDate = $("#txtDateTo").val();

        var request = createRequest();
        request.url = base_admin_url + "/report/reportsale/getsalecountchart?fromDate=" + fromDate + "&toDate=" + toDate;
        request.success = function (result) {
            for (var i = 0; i < result.length; i++) {
                myLineLabels.push(result[i].Key);
                myLineData.push(result[i].Value);
            }

            const ctx = document.getElementById('barChartCountSuccess');

            chart2 = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: myLineLabels,
                    datasets: [{
                        label: '',
                        data: myLineData
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: {
                        display: false,
                        position: 'top',
                    },
                    title: {
                        display: false,
                        text: ''
                    },
                    tooltips: {
                        enabled: true
                    },
                    plugins: {
                        tooltip: {
                            enabled: true
                        }
                    }
                }
            });
        };
        $.ajax(request);
    }

    function initializeInsertedCountBarScript() {
        var myLineLabels = [];
        var myLineData = [];

        var fromDate = $("#txtDateFrom").val();
        var toDate = $("#txtDateTo").val();

        var request = createRequest();
        request.url = base_admin_url + "/report/reportsale/getinsertedcountchart?fromDate=" + fromDate + "&toDate=" + toDate;
        request.success = function (result) {
            for (var i = 0; i < result.length; i++) {
                myLineLabels.push(result[i].Key);
                myLineData.push(result[i].Value);
            }

            const ctx = document.getElementById('barChartCountInserted');

            chart5 = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: myLineLabels,
                    datasets: [{
                        label: '',
                        data: myLineData
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: {
                        display: false,
                        position: 'top',
                    },
                    title: {
                        display: false,
                        text: ''
                    },
                    tooltips: {
                        enabled: true
                    },
                    plugins: {
                        tooltip: {
                            enabled: true
                        }
                    }
                }
            });
        };
        $.ajax(request);
    }

    function initializeAddressCountBarScript() {
        var myLineLabels = [];
        var myLineData = [];

        var fromDate = $("#txtDateFrom").val();
        var toDate = $("#txtDateTo").val();

        var request = createRequest();
        request.url = base_admin_url + "/report/reportsale/getaddresscountchart?fromDate=" + fromDate + "&toDate=" + toDate;
        request.success = function (result) {
            for (var i = 0; i < result.length; i++) {
                myLineLabels.push(result[i].Key);
                myLineData.push(result[i].Value);
            }

            const ctx = document.getElementById('barChartCountAddress');

            chart6 = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: myLineLabels,
                    datasets: [{
                        label: '',
                        data: myLineData
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    legend: {
                        display: false,
                        position: 'top',
                    },
                    title: {
                        display: false,
                        text: ''
                    },
                    tooltips: {
                        enabled: true
                    },
                    plugins: {
                        tooltip: {
                            enabled: true
                        }
                    }
                }
            });
        };
        $.ajax(request);
    }
}

function initializeDigikalaPageScript() {
    $(".btn-search").click(function () {
        var row = $(this).closest("tr");
        var productId = $(row).find(".related-code").val();
        console.log(productId);

        var request = createRequest();
        request.type = REQUEST_TYPE_GET;
        request.url = base_admin_url + "/store/product/SearchAjaxDetails?productId=" + productId;
        request.success = function (result) {
            if (result.length > 0) {
                var product = result[0];

                $(row).find(".hidden-id").val(product.Id);

                $(row).find(".related-code").val(product.Name);

                $(row).find(".related-size").empty();
                $(row).find(".related-size").append("<option value='0'>انتخاب سایز</option>");
                for (var i = 0; i < product.Sizes.length; i++) {
                    $(row).find(".related-size").append("<option value='" + product.Sizes[i].Id + "'>" + product.Sizes[i].Name + "</option>");
                }

                $(row).find(".related-color").empty();
                $(row).find(".related-color").append("<option value='0'>انتخاب رنگ</option>");
                for (var i = 0; i < product.Colors.length; i++) {
                    $(row).find(".related-color").append("<option value='" + product.Colors[i].Id + "'>" + product.Colors[i].Name + "</option>");
                }
            }
        };
        $.ajax(request);
    });

    $(".related-size").change(function () {
        var row = $(this).closest("tr");
        var colorId = parseInt($(row).find(".related-color").val());
        var sizeId = parseInt($(row).find(".related-size").val());

        if (colorId != 0 && sizeId != 0) {
            $(row).find(".button-save").removeClass("disabled");
            $(row).find(".button-save").removeAttr("disabled");
        }
    });

    $(".related-color").change(function () {
        var row = $(this).closest("tr");
        var colorId = parseInt($(row).find(".related-color").val());
        var sizeId = parseInt($(row).find(".related-size").val());

        if (colorId != 0 && sizeId != 0) {
            $(row).find(".button-save").removeClass("disabled");
            $(row).find(".button-save").removeAttr("disabled");
        }
    });

    $(".button-save").click(function () {
        var row = $(this).closest("tr");
        var entity = {
            VariantCode: $(row).find(".variant-id").val(),
            ProductId: $(row).find(".hidden-id").val(),
            ColorId: $(row).find(".related-color").val(),
            SizeId: $(row).find(".related-size").val()
        }

        var request = createRequest();
        request.type = REQUEST_TYPE_POST;
        request.data = JSON.stringify(entity);
        request.url = base_admin_url + "/store/ProductDigikala/Save";
        console.log(request.url);
        request.success = function (result) {
            createMessage("success", result.Message);
            location.reload();
        };
        $.ajax(request);
        console.log(entity);
    });
}

function createRebateProductColorChanges(productId, colorId) {
    var request = createRequest();
    request.type = REQUEST_TYPE_GET;
    request.url = base_admin_url + "/store/rebate/FillProductColor?productId=" + productId;
    request.success = function (entity) {
        $('#colorSelectList').empty();
        $('#colorSelectList').append('<option value="null"> انتخاب کنید</option>');
        $.each(entity, function (index, item) {
            // Assuming the response contains an array of users with 'id' and 'name' properties
            if (isNaN(colorId) == false && colorId != undefined && colorId != null && colorId == item.Id) {
                $('#colorSelectList').append('<option value="' + item.Id + '" selected>' + item.Name + '</option>');
            }
            else {
                $('#colorSelectList').append('<option value="' + item.Id + '">' + item.Name + '</option>');
            }
            $("#colorList").removeClass('display-none');
        });
    }
    $.ajax(request);
}

function createRebateProductSizeChanges(productId, sizeId) {
    var request = createRequest();
    request.type = REQUEST_TYPE_GET;
    request.url = base_admin_url + "/store/rebate/FillProductSize?productId=" + productId;
    request.success = function (entity) {
        $('#sizeSelectList').empty();
        $('#sizeSelectList').append('<option value="null"> انتخاب کنید</option>');
        $.each(entity, function (index, item) {
            if (isNaN(sizeId) == false && sizeId != undefined && sizeId != null && sizeId == item.Id) {
                $('#sizeSelectList').append('<option value="' + item.Id + '" selected>' + item.Name + '</option>');
            }
            else {
                $('#sizeSelectList').append('<option value="' + item.Id + '">' + item.Name + '</option>');
            }
            $("#sizeList").removeClass('display-none');
        });
    }
    $.ajax(request);
}

//function checkProductAvailability(productId, colorId, sizeId) {
//    var request = createRequest();
//    request.type = REQUEST_TYPE_GET;
//    request.url = base_admin_url + "/store/rebate/CheckAvailability?productId=" + productId + "&colorId=" + colorId + "&sizeId=" + sizeId;
//    request.success = function (entity) {
//        console.log(entity);
//    }

//    $.ajax(request);
//}

function initializeProductSetScript() {

    var entity = {};
    fillProductType();
    var closeCount = 0;
    $("#btnSubmit").click(function () {
        fillEntity();
        $.ajax(createRequest(entity));
    });

    function fillEntity() {
        entity.ID = $("#ID").val();
        entity.Name = $("#Name").val();
        entity.Active = $("#Active").prop("checked");
        entity.IsMustAllBuyed = $("#IsMustAllBuyed").val();
        entity.ProductTypeId = $("#ProductTypeId").selected() === null ? parseInt($("#ProductTypeId").val()) : $("#ProductTypeId").selected();
        entity.ProductCategoryId = $("#ProductCategoryId").selected() === null ? parseInt($("#ProductCategoryId").val()) : $("#ProductCategoryId").selected();
        entity.ProductSubCategoryId = $("#ProductSubCategoryId").selected() === null ? parseInt($("#ProductSubCategoryId").val()) : $("#ProductSubCategoryId").selected();
        entity.Label = $("#Label").val();
        entity.PictureId = uploadPictureId === null ? $("#LastPictureId").val() : uploadPictureId;
        entity.Description = tinymce.get('Description').getContent();
        entity.H1Title = $("#H1Title").val();
        entity.MetaDescription = $("#MetaDescription").val();
        entity.ProductSetTag = [];
        var tagsValue = $("#Tag").val();
        if (tagsValue !== undefined) {
            var tags = tagsValue.split(",");
            for (var g = 0; g < tags.length; g++) {
                var tagEntity = {
                    ProductSetId: entity.ID,
                    Name: tags[g]
                };
                entity.ProductSetTag.push(tagEntity);
            }
        }

        var langValues = $("[lang-value]");
        if (langValues.length > 0) {
            for (var i = 0; i < langValues.length; i++) {
                var langObject = langValues[i];
                var langEntity = {
                    Name: $(langObject).find("[lang-name='Name']").val(),
                    CategoryId: entity.CategoryId,
                    LanguageId: parseInt($(langObject).find("[lang-name='LanguageId']").val()),
                    Description: ""
                };
                entity.ProductSubCategoryLanguage.push(langEntity);
            }
        }
    }
    function fillProductType() {
        var selected = getSelectedValue($("#LastProductTypeId")) === undefined ? $("#LastProductTypeId").val() : getSelectedValue($("#LastProductTypeId"));
        var request = createRequest();
        request.type = REQUEST_TYPE_GET;
        request.url = base_admin_url + "/store/product/FillProductType?TypeId=" + selected;
        request.success = function (entity) {
            clearDropDown("ProductTypeId", true);
            clearDropDown("ProductCategoryId", true);
            clearDropDown("SubCategoryId", true);
            var isTypeSelected = $("#isTypeSelected").val() === "False" ? true : false;
            //bindDropDown("ProductTypeId", entity, "Name", "Id", isTypeSelected, $("#LastProductTypeId").val());
            var getValProductTypeId = bindDropDown("ProductTypeId", entity, "Name", "Id", isTypeSelected, $("#LastProductTypeId").val());
            //console.log("fillCategory", getValProductTypeId);
            //console.log("isTypeSelected", isTypeSelected);
            fillCategory();
            closeLoadingModal();
        }
        $.ajax(request);
    }

    function fillCategory() {
        var selected = getSelectedValue($("#ProductTypeId"));
        //console.log("fillCategory", selected);
        if (selected !== undefined) {
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillProductCategory?TypeId=" + selected;
            request.success = function (entity) {
                clearDropDown("ProductCategoryId");
                clearDropDown("SubCategoryId");
                bindDropDown("ProductCategoryId", entity, "Name", "Id", true, $("#LastProductCategoryId").val());
                fillSubCategory();
                closeLoadingModal();
            }
            $.ajax(request);
        } else {
            closeLoadingModal();
        }
    }

    function fillSubCategory() {
        var selected = getSelectedValue($("#ProductCategoryId"));
        if (selected !== undefined ) {
            var request = createRequest();
            request.type = REQUEST_TYPE_GET;
            request.url = base_admin_url + "/store/product/FillProductSubCategory?CategoryId=" + selected;
            request.success = function (entity) {
                bindDropDown("ProductSubCategoryId", entity, "Name", "Id", true, $("#LastProductSubCateogryId").val());
            }
            $.ajax(request);
            closeLoadingModal();
        } else {
            console.log("undefined");
        }
    }

    $("#ProductTypeId").change(function () {
        fillCategory();
    });

    $("#ProductCategoryId").change(function () {
        fillSubCategory();
    });

    function closeLoadingModal() {
        closeCount++;
    }
 
}

function initializeStepwiseDiscountItemScript() {
    let index = 1;

    $('#addStepDisItem').on('click', function () {
        let newRow = `
        <div class="form-group item-row">
            <label class="control-label col-md-2">موقعیت</label>
            <div class="col-md-3">
                <input type="number" name="Items[${index}].Position" class="form-control" placeholder="مثلاً ${index + 1}" />
            </div>

            <label class="control-label col-md-2">درصد</label>
            <div class="col-md-3">
                <input type="number" name="Items[${index}].PercentValue" step="0.01" class="form-control" placeholder="مثلاً 10.5" />
            </div>

            <div class="col-md-2">
                <button type="button" class="btn btn-danger remove-item">حذف</button>
            </div>
        </div>`;
        $('#items-container').append(newRow);
        index++;
    });

    $(document).on('click', '.remove-item', function () {
        $(this).closest('.item-row').remove();
    });
}

function initializeStepwiseDiscountItemEditScript(Count) {
    let index = Count;

    $('#addStepDisItem').on('click', function () {
        let newRow = `
        <div class="form-group item-row">
            <label class="control-label col-md-2">موقعیت</label>
            <div class="col-md-3">
                <input type="number" name="Items[${index}].Position" class="form-control" placeholder="مثلاً ${index + 1}" />
            </div>
            <label class="control-label col-md-2">درصد</label>
            <div class="col-md-3">
                <input type="number" step="0.01" name="Items[${index}].PercentValue" class="form-control" placeholder="مثلاً 10.5" />
            </div>
            <div class="col-md-2">
                <button type="button" class="btn btn-danger remove-item">حذف</button>
            </div>
        </div>`;
        $('#items-container').append(newRow);
        index++;
    });

    $(document).on('click', '.remove-item', function () {
        $(this).closest('.item-row').remove();
    });
}