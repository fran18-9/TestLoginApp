document.getElementById('togglePassword').addEventListener('change', function () {
    const passInput = document.getElementById('password');
    if (this.checked) {
        passInput.type = 'text';
    } else {
        passInput.type = 'password';
    }
});
