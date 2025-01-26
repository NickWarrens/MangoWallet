document.addEventListener("DOMContentLoaded", () => {
    const modals = {
        exchange: document.getElementById("exchange-modal"),
        transfer: document.getElementById("transfer-modal"),
        manageCurrency: document.getElementById("manage-currency-modal"),
        addUserCurrency: document.getElementById("add-user-currency-modal"),
        steal: document.getElementById("steal-modal"),
    };

    const buttons = {
        exchange: document.getElementById("exchange-btn"),
        transfer: document.getElementById("transfer-btn"),
        manageCurrency: document.getElementById("manage-currency-btn"),
        addUserCurrency: document.getElementById("add-user-currency-btn"),
        steal: document.getElementById("steal-btn"),
    };

    const closeButtons = {
        exchange: document.getElementById("close-exchange"),
        transfer: document.getElementById("close-transfer"),
        manageCurrency: document.getElementById("close-manage-currency"),
        addUserCurrency: document.getElementById("close-add-user-currency"),
        steal: document.getElementById("close-steal"),
    };

    // Open modal
    const openModal = (modal) => {
        modal.style.display = "flex";
    };

    // Close modal
    const closeModal = (modal) => {
        modal.style.display = "none";
    };

    // Add event listeners for open buttons
    Object.entries(buttons).forEach(([key, button]) => {
        button.addEventListener("click", () => {
            openModal(modals[key]);

            if (key === "addUserCurrency") {
                // Populate user options dynamically
                fetch("/Home/GetAllUsers")
                    .then((response) => response.json())
                    .then((users) => {
                        const userKeySelect = document.getElementById("userKey");
                        userKeySelect.innerHTML = ""; // Clear existing options
                        users.forEach((user) => {
                            const option = document.createElement("option");
                            option.value = user.key;
                            option.textContent = `${user.name} (Wallet: ${user.walletKey})`;
                            userKeySelect.appendChild(option);
                        });
                    });
            }
        });
    });

    // Add event listeners for close buttons
    Object.entries(closeButtons).forEach(([key, button]) => {
        button.addEventListener("click", () => {
            closeModal(modals[key]);
        });
    });

    // Close modal when clicking outside the modal content
    window.addEventListener("click", (event) => {
        Object.values(modals).forEach((modal) => {
            if (event.target === modal) {
                closeModal(modal);
            }
        });
    });

    // Close modal after form submission
    Object.values(modals).forEach((modal) => {
        const form = modal.querySelector("form");
        if (form) {
            form.addEventListener("submit", () => {
                closeModal(modal);
            });
        }
    });
});
