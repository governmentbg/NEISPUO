function goBack() {
    window.location.href = document.referrer
}
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('cancelLogout')
        .addEventListener('click', goBack);
});
