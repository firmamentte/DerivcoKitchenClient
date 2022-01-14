window.onload = () => {
    loadCartSummary()
}

window.onscroll = (e: Event) => {

    e.preventDefault()

    toggleScrollTopButton()

    if (isInViewport(document.querySelector("#moreItemIcon")) && isVisible(document.querySelector("#moreItemIcon"))) {
        getMoreItems()
    }
}

document.addEventListener("DOMContentLoaded", () => {

    const _cartSummary = document.querySelector(".cart-summary")

    _cartSummary.addEventListener("click", () => {

        if (parseInt(removeWhiteSpaceAndComma(_cartSummary.querySelector(".lblQuantity").textContent.trim())) > 0) {
            viewCart()
        }
    })
})

const loadCartSummary = () => {

    fetch("/OnlineStore/CartSummary").
        then(handleError).
        then(jsonDataType).
        then((data) => {

            document.querySelector(".cart-summary-popup .cart-label-qty .lblQuantity").textContent = data.quantity
            document.querySelector(".cart-summary-popup .cart-label-sub-total .lblSubTotal").textContent = data.subTotal

            showCartSummaryPopupButton()
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const viewCart = () => {

    showPopupFormProgressBar()

    fetch("/OnlineStore/ViewCart").
        then(handleError).
        then(htmlDataType).
        then((data) => {
            showPopupFormHtml(data)
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const updateCartSummary = () => {

    fetch("/OnlineStore/CartSummary").
        then(handleError).
        then(jsonDataType).
        then((data) => {

            document.querySelector(".cart-label-qty .lblQuantity").textContent = data.quantity
            document.querySelector(".cart-label-sub-total .lblSubTotal").textContent = data.subTotal

            reCenterCartSummaryPopup()
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const updateCartItemQuantity = (menuItemId: string, quantity: number) => {

    showPopupFormProgressBar()

    const _parameters =
        `menuItemId=${menuItemId}
         &quantity=${quantity}`

    fetch("/OnlineStore/UpdateItemQuantity", postOptions(removeLineBreaks(_parameters))).
        then(handleError).
        then(htmlDataType).
        then((data) => {

            document.querySelector("#divLineItemGrid").innerHTML = data

            updateCartSummary();

            hidePopupFormProgressBar()
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const searchMenuItemsByMenuCategoryName = (menuCategoryName) => {

    showPopupFormProgressBar()

    fetch(`/OnlineStore/SearchMenuItemsByMenuCategoryName?menuCategoryName=${menuCategoryName}`).
        then(handleError).
        then(htmlDataType).
        then((dataItems) => {

            reCenterCartSummaryPopup()

            disableImgContextMenu(document.querySelectorAll("#divMainItemGrid .item-body img"))

            document.querySelector("#divMainItemGrid .item-body").innerHTML = dataItems

            const _buttons = document.querySelectorAll("#menuCategoryDiv input")
            _buttons.forEach((button) => {

                button.classList.remove("button-9E0000")
                button.classList.add("button-none")
            })

            const _button = document.querySelector(`#menuCategoryDiv #${CSS.escape(menuCategoryName)}`)
            _button.classList.remove("button-none")
            _button.classList.add("button-9E0000")

            let _hdName = <HTMLInputElement>document.querySelector("#hdName")
            _hdName.value = ''

            hidePopupFormProgressBar()
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const searchItem = () => {

    showPopupFormProgressBar()

    fetch("/OnlineStore/SearchMenuItem").
        then(handleError).
        then(htmlDataType).
        then((data) => {

            document.querySelector("#popupFormToShow").innerHTML = data

            let _hdName = <HTMLInputElement>document.querySelector("#hdName")
            let _name = <HTMLInputElement>document.querySelector("#Name")

            _name.value = _hdName.value

            showPopupForm()
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const submitSearchItem = () => {

    toggleButtonProgressBar(document.querySelector("#navSearchItem"), document.querySelector("#progressBarSearchItem"))

    fetch("/OnlineStore/SearchMenuItem", postOptions(serialize(document.querySelector("#formSearchItem")))).
        then(handleError).
        then(htmlDataType).
        then((dataItems) => {

            reCenterCartSummaryPopup()

            disableImgContextMenu(document.querySelectorAll("#divMainItemGrid .item-body img"))

            document.querySelector("#divMainItemGrid .item-body").innerHTML = dataItems

            let _hdName = <HTMLInputElement>document.querySelector("#hdName")
            let _name = <HTMLInputElement>document.querySelector("#Name")

            _hdName.value = _name.value

            hidePopupForm()
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const getMoreItems = () => {

    showMoreProgressBar(
        document.querySelector("#moreItemIcon"),
        document.querySelector("#moreItemText"),
        document.querySelector("#moreItemError"))

    let _parameters =
        `?skip=${document.querySelectorAll("#divMainItemGrid .item-body .item-grid .container .item").length}`

    fetch(`OnlineStore/GetMoreMenuItemsByCriteria${removeLineBreaks(_parameters)}`).
        then(handleError).
        then(htmlDataType).
        then((data) => {

            const _data = document.createElement("div")
            _data.innerHTML = data

            if (!!_data.querySelector(".item")) {

                document.querySelector("#divMainItemGrid .item-body .item-grid .container").insertAdjacentHTML("beforeend", data)
                disableImgContextMenu(document.querySelectorAll("#divMainItemGrid .item-body .item-grid .container img"))

                hideMoreProgressBar(
                    document.querySelector("#moreItemIcon"),
                    document.querySelector("#moreItemText"),
                    document.querySelector("#moreItemError"))
            }
            else {
                hideMoreIcon(
                    document.querySelector("#moreItemIcon"),
                    document.querySelector("#moreItemText"),
                    document.querySelector("#moreItemError"))
            }
        }).
        catch((error) => {
            showMoreError(
                document.querySelector("#moreItemIcon"),
                document.querySelector("#moreItemText"),
                document.querySelector("#moreItemError"))
        })
}

const orderItem = (itemMenuId) => {

    showPopupFormProgressBar()

    fetch(`/OnlineStore/AddToCart?itemMenuId=${itemMenuId}`).
        then(handleError).
        then(htmlDataType).
        then((data) => {

            showPopupFormHtml(data)

            let _quantity = <HTMLInputElement>document.querySelector("#Quantity")
            _quantity.value = ''
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const customerAccount = () => {

    showPopupFormProgressBar()

    fetch("/Shared/CustomerAccount").
        then(handleError).
        then(htmlDataType).
        then((data) => {
            showPopupFormHtml(data)
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const signIn = () => {

    toggleButtonProgressBar(document.querySelector("#navWelcome"), document.querySelector("#progressBarWelcome"))

    fetch("/ApplicationUser/SignIn").
        then(handleError).
        then(htmlDataType).
        then((data) => {
            showPopupFormHtml(data)
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const submitSignIn = () => {

    const _messageSignIn = clearErrorMessageDiv(document.querySelector("#messageSignIn"))

    validateSignIn(_messageSignIn)

    if (!isErrorMessageDivEmpty(_messageSignIn)) {
        return
    }

    toggleButtonProgressBar(document.querySelector("#navSignIn"), document.querySelector("#progressBarSignIn"))

    fetch("/ApplicationUser/SignIn", postOptions(serialize(document.querySelector("#formSignIn")))).
        then(handleError).
        then(() => {
            hidePopupForm()
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const signUp = () => {

    toggleButtonProgressBar(document.querySelector("#navWelcome"), document.querySelector("#progressBarWelcome"))

    fetch("/ApplicationUser/SignUp",).
        then(handleError).
        then(htmlDataType).
        then((signUpResp) => {
            showPopupFormHtml(signUpResp)
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

const checkOut = () => {

    toggleButtonProgressBar(document.querySelector("#navViewCart"), document.querySelector("#progressBarViewCart"))

    window.location.assign("/OnlineStore/ShouldAuthenticateCustomer")
}

const showCartSummaryPopupButton = () => {

    const _cartSummaryPopup: HTMLDivElement = document.querySelector(".cart-summary-popup")

    centerCartSummaryPopup(_cartSummaryPopup)

    _cartSummaryPopup.classList.add("show-cart-summary-popup")
}

const submitAddToCart = () => {

    const _messageAddToCart = clearErrorMessageDiv(document.querySelector("#messageAddToCart"))

    const _quantity = <HTMLInputElement>document.querySelector("#Quantity")
    if (!(!!_quantity.value.trim())) {
        appendErrorMessage(_messageAddToCart, "Quantity required")
    }
    else {
        if (isNaN(_quantity.value.trim() as unknown as number)) {
            appendErrorMessage(_messageAddToCart, "Numeric Quantity required")
        }
        else {
            if (parseInt(_quantity.value.trim()) <= 0) {
                appendErrorMessage(_messageAddToCart, "Invalid Quantity")
            }
        }
    }

    if (!isErrorMessageDivEmpty(_messageAddToCart)) {
        return
    }

    toggleButtonProgressBar(document.querySelector("#navAddToCart"), document.querySelector("#progressBarAddToCart"))

    fetch("/OnlineStore/AddToCart", postOptions(serialize(document.querySelector("#formAddToCart")))).
        then(handleError).
        then(() => {
            updateCartSummary()
            hidePopupForm()
        }).
        catch((error) => {
            showErrorPopupForm(error)
        })
}

function centerCartSummaryPopup(cartSummaryPopup: HTMLDivElement) {

    if (!!cartSummaryPopup) {
        cartSummaryPopup.style.cssText = "top: 11px;right:0px;"
    }
}