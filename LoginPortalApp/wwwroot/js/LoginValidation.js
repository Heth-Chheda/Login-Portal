document.addEventListener('DOMContentLoaded', function () {
    // Toggle password visibility
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

    // Dynamically adjust the position of the toggle button based on input height
    const adjustTogglePosition = () => {
        const passwordFieldHeight = $('#Password').outerHeight();
        const buttonHeight = $('#toggle-password').outerHeight();
        const topPosition = (passwordFieldHeight - buttonHeight) / 2;
        $('#toggle-password').css('top', topPosition + 'px');
    };

    // Call the adjust position function initially and on window resize
    adjustTogglePosition();

    // Replacing jQuery resize with native JavaScript event listener
    window.addEventListener('resize', adjustTogglePosition);

    // Form validation for email
    $("#loginForm").validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true
            }
        },
        messages: {
            Email: {
                required: "Please enter your email.",
                email: "Please enter a valid email address."
            },
            Password: {
                required: "Please enter your password."
            }
        }
    });
});
