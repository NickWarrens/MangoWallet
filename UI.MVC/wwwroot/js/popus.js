const exchangeModal = document.getElementById("exchange-modal");
const transferModal = document.getElementById("transfer-modal");

const exchangeBtn = document.getElementById("exchange-btn");
const transferBtn = document.getElementById("transfer-btn");

const closeExchange = document.getElementById("close-exchange");
const closeTransfer = document.getElementById("close-transfer");

exchangeBtn.addEventListener("click", () => {
    exchangeModal.style.display = "flex";
});

transferBtn.addEventListener("click", () => {
    transferModal.style.display = "flex";
});


closeExchange.addEventListener("click", () => {
    exchangeModal.style.display = "none";
});

closeTransfer.addEventListener("click", () => {
    transferModal.style.display = "none";
});

window.addEventListener("click", (event) => {
    if (event.target === exchangeModal) {
        exchangeModal.style.display = "none";
    }
    if (event.target === transferModal) {
        transferModal.style.display = "none";
    }
});
