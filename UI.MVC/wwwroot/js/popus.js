document.addEventListener("DOMContentLoaded", () => {
    const exchangeModal = document.getElementById("exchange-modal");
    const transferModal = document.getElementById("transfer-modal");

    const exchangeBtn = document.getElementById("exchange-btn");
    const transferBtn = document.getElementById("transfer-btn");

    const closeExchange = document.getElementById("close-exchange");
    const closeTransfer = document.getElementById("close-transfer");

    const manageCurrencyModal = document.getElementById("manage-currency-modal");
    const manageCurrencyBtn = document.getElementById("manage-currency-btn");
    const closeManageCurrency = document.getElementById("close-manage-currency");

    const addUserCurrencyModal = document.getElementById("add-user-currency-modal");
    const addUserCurrencyBtn = document.getElementById("add-user-currency-btn");
    const closeAddUserCurrency = document.getElementById("close-add-user-currency");
    const userKeySelect = document.getElementById("userKey");

    // Open manage currency modal
    manageCurrencyBtn.addEventListener("click", () => {
        manageCurrencyModal.style.display = "flex";
    });

    // Close manage currency modal
    closeManageCurrency.addEventListener("click", () => {
        manageCurrencyModal.style.display = "none";
    });

    // Close manage currency modal when clicking outside of it
    window.addEventListener("click", (event) => {
        if (event.target === manageCurrencyModal) {
            manageCurrencyModal.style.display = "none";
        }
    });

    // Open exchange modal
    exchangeBtn.addEventListener("click", () => {
        exchangeModal.style.display = "flex";
    });

    // Open transfer modal
    transferBtn.addEventListener("click", () => {
        transferModal.style.display = "flex";
    });

    // Close exchange modal
    closeExchange.addEventListener("click", () => {
        exchangeModal.style.display = "none";
    });

    // Close transfer modal
    closeTransfer.addEventListener("click", () => {
        transferModal.style.display = "none";
    });

    // Close modals when clicking outside of them
    window.addEventListener("click", (event) => {
        if (event.target === exchangeModal) {
            exchangeModal.style.display = "none";
        }
        if (event.target === transferModal) {
            transferModal.style.display = "none";
        }
    });
});