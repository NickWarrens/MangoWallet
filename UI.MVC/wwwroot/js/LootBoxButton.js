document.addEventListener("DOMContentLoaded", () => {
    const lootBoxButton = document.getElementById("open-lootbox-button");
    const lootBoxForm = lootBoxButton?.closest("form");

    if (lootBoxButton && lootBoxForm) {
        lootBoxButton.onclick = (event) => {
            event.preventDefault();

            lootBoxButton.disabled = true;
            lootBoxButton.textContent = "Opening.";
            let dots = 1;

            // Change button text every second
            const animationInterval = setInterval(() => {
                dots = (dots % 3) + 1;
                lootBoxButton.textContent = `Opening${'.'.repeat(dots)}`;
            }, 1000);

            // Submit form after 10 seconds
            setTimeout(() => {
                clearInterval(animationInterval);
                lootBoxForm.submit();
            }, 10000);
        };
    }
});
