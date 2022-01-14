window.onload = function () {
    loadCartSummary();
};
window.onscroll = function (e) {
    e.preventDefault();
    toggleScrollTopButton();
    if (isInViewport(document.querySelector("#moreItemIcon")) && isVisible(document.querySelector("#moreItemIcon"))) {
        getMoreItems();
    }
};
document.addEventListener("DOMContentLoaded", function () {
    var _cartSummary = document.querySelector(".cart-summary");
    _cartSummary.addEventListener("click", function () {
        if (parseInt(removeWhiteSpaceAndComma(_cartSummary.querySelector(".lblQuantity").textContent.trim())) > 0) {
            viewCart();
        }
    });
});
var loadCartSummary = function () {
    fetch("/OnlineStore/CartSummary").
        then(handleError).
        then(jsonDataType).
        then(function (data) {
        document.querySelector(".cart-summary-popup .cart-label-qty .lblQuantity").textContent = data.quantity;
        document.querySelector(".cart-summary-popup .cart-label-sub-total .lblSubTotal").textContent = data.subTotal;
        showCartSummaryPopupButton();
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var viewCart = function () {
    showPopupFormProgressBar();
    fetch("/OnlineStore/ViewCart").
        then(handleError).
        then(htmlDataType).
        then(function (data) {
        showPopupFormHtml(data);
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var updateCartSummary = function () {
    fetch("/OnlineStore/CartSummary").
        then(handleError).
        then(jsonDataType).
        then(function (data) {
        document.querySelector(".cart-label-qty .lblQuantity").textContent = data.quantity;
        document.querySelector(".cart-label-sub-total .lblSubTotal").textContent = data.subTotal;
        reCenterCartSummaryPopup();
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var updateCartItemQuantity = function (menuItemId, quantity) {
    showPopupFormProgressBar();
    var _parameters = "menuItemId=".concat(menuItemId, "\n         &quantity=").concat(quantity);
    fetch("/OnlineStore/UpdateItemQuantity", postOptions(removeLineBreaks(_parameters))).
        then(handleError).
        then(htmlDataType).
        then(function (data) {
        document.querySelector("#divLineItemGrid").innerHTML = data;
        updateCartSummary();
        hidePopupFormProgressBar();
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var searchMenuItemsByMenuCategoryName = function (menuCategoryName) {
    showPopupFormProgressBar();
    fetch("/OnlineStore/SearchMenuItemsByMenuCategoryName?menuCategoryName=".concat(menuCategoryName)).
        then(handleError).
        then(htmlDataType).
        then(function (dataItems) {
        reCenterCartSummaryPopup();
        disableImgContextMenu(document.querySelectorAll("#divMainItemGrid .item-body img"));
        document.querySelector("#divMainItemGrid .item-body").innerHTML = dataItems;
        var _buttons = document.querySelectorAll("#menuCategoryDiv input");
        _buttons.forEach(function (button) {
            button.classList.remove("button-9E0000");
            button.classList.add("button-none");
        });
        var _button = document.querySelector("#menuCategoryDiv #".concat(CSS.escape(menuCategoryName)));
        _button.classList.remove("button-none");
        _button.classList.add("button-9E0000");
        var _hdName = document.querySelector("#hdName");
        _hdName.value = '';
        hidePopupFormProgressBar();
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var searchItem = function () {
    showPopupFormProgressBar();
    fetch("/OnlineStore/SearchMenuItem").
        then(handleError).
        then(htmlDataType).
        then(function (data) {
        document.querySelector("#popupFormToShow").innerHTML = data;
        var _hdName = document.querySelector("#hdName");
        var _name = document.querySelector("#Name");
        _name.value = _hdName.value;
        showPopupForm();
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var submitSearchItem = function () {
    toggleButtonProgressBar(document.querySelector("#navSearchItem"), document.querySelector("#progressBarSearchItem"));
    fetch("/OnlineStore/SearchMenuItem", postOptions(serialize(document.querySelector("#formSearchItem")))).
        then(handleError).
        then(htmlDataType).
        then(function (dataItems) {
        reCenterCartSummaryPopup();
        disableImgContextMenu(document.querySelectorAll("#divMainItemGrid .item-body img"));
        document.querySelector("#divMainItemGrid .item-body").innerHTML = dataItems;
        var _hdName = document.querySelector("#hdName");
        var _name = document.querySelector("#Name");
        _hdName.value = _name.value;
        hidePopupForm();
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var getMoreItems = function () {
    showMoreProgressBar(document.querySelector("#moreItemIcon"), document.querySelector("#moreItemText"), document.querySelector("#moreItemError"));
    var _parameters = "?skip=".concat(document.querySelectorAll("#divMainItemGrid .item-body .item-grid .container .item").length);
    fetch("OnlineStore/GetMoreMenuItemsByCriteria".concat(removeLineBreaks(_parameters))).
        then(handleError).
        then(htmlDataType).
        then(function (data) {
        var _data = document.createElement("div");
        _data.innerHTML = data;
        if (!!_data.querySelector(".item")) {
            document.querySelector("#divMainItemGrid .item-body .item-grid .container").insertAdjacentHTML("beforeend", data);
            disableImgContextMenu(document.querySelectorAll("#divMainItemGrid .item-body .item-grid .container img"));
            hideMoreProgressBar(document.querySelector("#moreItemIcon"), document.querySelector("#moreItemText"), document.querySelector("#moreItemError"));
        }
        else {
            hideMoreIcon(document.querySelector("#moreItemIcon"), document.querySelector("#moreItemText"), document.querySelector("#moreItemError"));
        }
    }).
        catch(function (error) {
        showMoreError(document.querySelector("#moreItemIcon"), document.querySelector("#moreItemText"), document.querySelector("#moreItemError"));
    });
};
var orderItem = function (itemMenuId) {
    showPopupFormProgressBar();
    fetch("/OnlineStore/AddToCart?itemMenuId=".concat(itemMenuId)).
        then(handleError).
        then(htmlDataType).
        then(function (data) {
        showPopupFormHtml(data);
        var _quantity = document.querySelector("#Quantity");
        _quantity.value = '';
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var customerAccount = function () {
    showPopupFormProgressBar();
    fetch("/Shared/CustomerAccount").
        then(handleError).
        then(htmlDataType).
        then(function (data) {
        showPopupFormHtml(data);
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var signIn = function () {
    toggleButtonProgressBar(document.querySelector("#navWelcome"), document.querySelector("#progressBarWelcome"));
    fetch("/ApplicationUser/SignIn").
        then(handleError).
        then(htmlDataType).
        then(function (data) {
        showPopupFormHtml(data);
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var submitSignIn = function () {
    var _messageSignIn = clearErrorMessageDiv(document.querySelector("#messageSignIn"));
    validateSignIn(_messageSignIn);
    if (!isErrorMessageDivEmpty(_messageSignIn)) {
        return;
    }
    toggleButtonProgressBar(document.querySelector("#navSignIn"), document.querySelector("#progressBarSignIn"));
    fetch("/ApplicationUser/SignIn", postOptions(serialize(document.querySelector("#formSignIn")))).
        then(handleError).
        then(function () {
        hidePopupForm();
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var signUp = function () {
    toggleButtonProgressBar(document.querySelector("#navWelcome"), document.querySelector("#progressBarWelcome"));
    fetch("/ApplicationUser/SignUp").
        then(handleError).
        then(htmlDataType).
        then(function (signUpResp) {
        showPopupFormHtml(signUpResp);
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
var checkOut = function () {
    toggleButtonProgressBar(document.querySelector("#navViewCart"), document.querySelector("#progressBarViewCart"));
    window.location.assign("/OnlineStore/ShouldAuthenticateCustomer");
};
var showCartSummaryPopupButton = function () {
    var _cartSummaryPopup = document.querySelector(".cart-summary-popup");
    centerCartSummaryPopup(_cartSummaryPopup);
    _cartSummaryPopup.classList.add("show-cart-summary-popup");
};
var submitAddToCart = function () {
    var _messageAddToCart = clearErrorMessageDiv(document.querySelector("#messageAddToCart"));
    var _quantity = document.querySelector("#Quantity");
    if (!(!!_quantity.value.trim())) {
        appendErrorMessage(_messageAddToCart, "Quantity required");
    }
    else {
        if (isNaN(_quantity.value.trim())) {
            appendErrorMessage(_messageAddToCart, "Numeric Quantity required");
        }
        else {
            if (parseInt(_quantity.value.trim()) <= 0) {
                appendErrorMessage(_messageAddToCart, "Invalid Quantity");
            }
        }
    }
    if (!isErrorMessageDivEmpty(_messageAddToCart)) {
        return;
    }
    toggleButtonProgressBar(document.querySelector("#navAddToCart"), document.querySelector("#progressBarAddToCart"));
    fetch("/OnlineStore/AddToCart", postOptions(serialize(document.querySelector("#formAddToCart")))).
        then(handleError).
        then(function () {
        updateCartSummary();
        hidePopupForm();
    }).
        catch(function (error) {
        showErrorPopupForm(error);
    });
};
function centerCartSummaryPopup(cartSummaryPopup) {
    if (!!cartSummaryPopup) {
        cartSummaryPopup.style.cssText = "top: 11px;right:0px;";
    }
}
//# sourceMappingURL=index.js.map