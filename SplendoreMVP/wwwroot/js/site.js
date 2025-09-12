// Load cart count on page load
document.addEventListener("DOMContentLoaded", () => {
    loadCartCount();
});

async function loadCartCount() {
    try {
        const response = await fetch(`/Cart/GetTotalItemInCart`);
        if (response.ok) {
            const result = await response.json();
            const cartCountEl = document.getElementById("cartCount");
            if (cartCountEl) {
                cartCountEl.innerText = result;
            }
        }
    } catch (err) {
        console.error("Error loading cart count:", err);
    }
}

async function addToCart(productId) {
    try {
        const response = await fetch(`/Cart/AddItem?productId=${productId}&redirect=0`);
        if (!response.ok) throw new Error("Failed to add item");

        const result = await response.json(); // assuming this is the new total count
        alert(`Item added to cart. Total items: ${result}`);

        // Update cart count
        const cartCountEl = document.getElementById("cartCount");
        if (cartCountEl) {
            cartCountEl.innerText = result;
            cartCountEl.classList.add("fw-bold", "text-success");
            setTimeout(() => cartCountEl.classList.remove("fw-bold", "text-success"), 1000);
        }
    } catch (err) {
        console.error("Error adding to cart:", err);
        alert("Something went wrong while adding to cart.");
    }
}
