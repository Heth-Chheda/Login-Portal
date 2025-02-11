$(document).ready(function () {
    // Toggle password visibility for "New Password"
    $('#toggle-password').on('click', function () {
        const passwordField = $('#Password');
        const icon = $('#password-icon');

        if (passwordField.attr('type') === 'password') {
            passwordField.attr('type', 'text');
            icon.removeClass('bi-eye-slash').addClass('bi-eye');
        } else {
            passwordField.attr('type', 'password');
            icon.removeClass('bi-eye').addClass('bi-eye-slash');
        }
    });

    // Toggle password visibility for "Confirm Password"
    $('#toggle-confirm-password').on('click', function () {
        const confirmPasswordField = $('#ConfirmPassword');
        const icon = $('#confirm-password-icon');

        if (confirmPasswordField.attr('type') === 'password') {
            confirmPasswordField.attr('type', 'text');
            icon.removeClass('bi-eye-slash').addClass('bi-eye');
        } else {
            confirmPasswordField.attr('type', 'password');
            icon.removeClass('bi-eye').addClass('bi-eye-slash');
        }
    });

    // Password and confirm password validation
    $('#Password, #ConfirmPassword').on('keyup', function () {
        const password = $('#Password').val();
        const confirmPassword = $('#ConfirmPassword').val();

        // Check password match
        if (password !== confirmPassword) {
            $("#confirmPasswordError").text("Passwords do not match.");
            $("#password-match-message").text('');
        } else {
            $("#confirmPasswordError").text('');
            $("#password-match-message").text('Passwords match').css('color', 'green');
        }

        // Password strength check
        checkPasswordStrength(password);
    });

    function checkPasswordStrength(password) {
        const strengthMessage = $("#password-strength");
        const regexWeak = /^(?=.*[a-z]).{6,}$/;
        const regexMedium = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        const regexStrong = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?/~_`|\\/-]).{10,}$/;

        if (!password) {
            // Password cannot be empty
            strengthMessage.text('Password cannot be empty').css('color', 'red');
        } else if (password.length < 6) {
            strengthMessage.text('Weak password: Minimum 6 characters').css('color', 'red');
        } else if (regexMedium.test(password)) {
            strengthMessage.text('Medium password: Should contain at least one uppercase letter, one number').css('color', 'orange');
        } else if (regexStrong.test(password)) {
            strengthMessage.text('Strong password: Contains uppercase, number, and special character').css('color', 'green');
        } else {
            strengthMessage.text('Weak password: Add more complexity').css('color', 'red');
        }
    }
});
