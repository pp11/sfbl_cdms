let togglePassword = document.getElementById('togglePassword');
let password = document.getElementById('password');
togglePassword.addEventListener('click', (event) => {
    passwordFiledType()

})
function passwordFiledType() {
    const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
    password.setAttribute('type', type);
    if (type === 'password') {
        togglePassword.innerHTML = `<i class="fas fa-eye"></i>`
    }
    else {
        togglePassword.innerHTML = `<i class="fas fa-eye-slash"></i>`
    }
}



function validateForm() {
    let userName = document.forms["myForm"]["UserName"].value;
    let password = document.forms["myForm"]["Password"].value;
    let errorMessage = document.getElementById('error-message');
    if (userName === "" || password === "") {
        errorMessage.innerHTML = '<span style="color:red; font-family: cursive;" ><i class="fas fa-exclamation-circle"></i> User Name && Password  must be filled out.</span>';
        return false;
    }
}
