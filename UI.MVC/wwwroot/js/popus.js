document.addEventListener("DOMContentLoaded", () => {
    const exchangeModal = document.getElementById("exchange-modal");
    const transferModal = document.getElementById("transfer-modal");
    const manageCurrencyModal = document.getElementById("manage-currency-modal");
    const addUserCurrencyModal = document.getElementById("add-user-currency-modal");

    const exchangeBtn = document.getElementById("exchange-btn");
    const transferBtn = document.getElementById("transfer-btn");
    const manageCurrencyBtn = document.getElementById("manage-currency-btn");
    const addUserCurrencyBtn = document.getElementById("add-user-currency-btn");

    const closeExchange = document.getElementById("close-exchange");
    const closeTransfer = document.getElementById("close-transfer");
    const closeManageCurrency = document.getElementById("close-manage-currency");
    const closeAddUserCurrency = document.getElementById("close-add-user-currency");

    // Open Exchange Modal
    exchangeBtn.addEventListener("click", () => {
        exchangeModal.style.display = "flex";
    });

    // Open Transfer Modal
    transferBtn.addEventListener("click", () => {
        transferModal.style.display = "flex";
    });

    // Open Manage Currency Modal
    manageCurrencyBtn.addEventListener("click", () => {
        manageCurrencyModal.style.display = "flex";
    });

    // Open Add Currency to User Modal
    addUserCurrencyBtn.addEventListener("click", () => {
        addUserCurrencyModal.style.display = "flex";

        // Populate user options dynamically (if needed)
        fetch("/Home/GetAllUsers")
            .then(response => response.json())
            .then(users => {
                const userKeySelect = document.getElementById("userKey");
                userKeySelect.innerHTML = ""; // Clear existing options
                users.forEach(user => {
                    const option = document.createElement("option");
                    option.value = user.key;
                    option.textContent = `${user.name} (Wallet: ${user.walletKey})`;
                    userKeySelect.appendChild(option);
                });
            });
    });

    // Close Modals
    closeExchange.addEventListener("click", () => {
        exchangeModal.style.display = "none";
    });

    closeTransfer.addEventListener("click", () => {
        transferModal.style.display = "none";
    });

    closeManageCurrency.addEventListener("click", () => {
        manageCurrencyModal.style.display = "none";
    });

    closeAddUserCurrency.addEventListener("click", () => {
        addUserCurrencyModal.style.display = "none";
    });

    // Close modals when clicking outside of them
    window.addEventListener("click", (event) => {
        if (event.target === exchangeModal) exchangeModal.style.display = "none";
        if (event.target === transferModal) transferModal.style.display = "none";
        if (event.target === manageCurrencyModal) manageCurrencyModal.style.display = "none";
        if (event.target === addUserCurrencyModal) addUserCurrencyModal.style.display = "none";
    });
});
