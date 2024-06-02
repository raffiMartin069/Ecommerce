function formatDate(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

// Set the max attribute to today's date
const today = new Date();
document.getElementById('purchase-date').setAttribute('max', formatDate(today));

// Update the max date daily (if necessary, this is optional)
setInterval(() => {
    const newToday = new Date();
    document.getElementById('purchase-date').setAttribute('max', formatDate(newToday));
}, 86400000); // 864000