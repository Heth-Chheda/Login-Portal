$(function () {
    let isEmailTaken = false;

    // Real-time email format validation
    $("#Email").on("input", function () {
        const email = $(this).val();
        $(".text-danger").text(""); // Clear any existing error messages

        // Simple email format validation
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            $(this).next(".text-danger").text("Please enter a valid email address.");
            return;
        }
    });

    // Real-time password validation
    $("#Password").on("input", function () {
        const password = $(this).val();
        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
        $(".text-danger").text(""); // Clear any existing error messages
        if (!passwordRegex.test(password)) {
            $(this).next(".text-danger").text("Password must be at least 8 characters long, include one uppercase letter, one lowercase letter, one digit, and one special character.");
        }
    });

    // Real-time confirm password validation
    $("#ConfirmPassword").on("input", function () {
        const confirmPassword = $(this).val();
        const password = $("#Password").val();
        $(".text-danger").text(""); // Clear any existing error messages
        if (confirmPassword !== password) {
            $(this).next(".text-danger").text("Passwords do not match.");
        }
    });

    // Real-time Date of Birth validation
    $("#DateOfBirth").on("input", function () {
        const dob = new Date($(this).val());
        const today = new Date();
        const age = today.getFullYear() - dob.getFullYear();
        const monthDifference = today.getMonth() - dob.getMonth();
        if (monthDifference < 0 || (monthDifference === 0 && today.getDate() < dob.getDate())) {
            age--;
        }
        $(".text-danger").text(""); // Clear any existing error messages
        if (age < 10) {
            $(this).next(".text-danger").text("You must be at least 10 years old.");
        }
    });

    // Real-time Phone number validation
    $("#PhoneNumber").on("input", function () {
        const phoneNumber = $(this).val();
        const phoneRegex = /^\d{10}$/;
        $(".text-danger").text(""); // Clear any existing error messages
        if (!phoneRegex.test(phoneNumber)) {
            $(this).next(".text-danger").text("Please enter a valid 10-digit phone number.");
        }
    });

    // Form submission handler
    $("form").on("submit", function (event) {
        const email = $("#Email").val();

        // Prevent form submission if email format is invalid
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            event.preventDefault();
            $("#Email").next(".text-danger").text("Please enter a valid email address.");
            return;
        }

        // Handle password and confirm password validation
        const password = $("#Password").val();
        const confirmPassword = $("#ConfirmPassword").val();
        if (password !== confirmPassword) {
            event.preventDefault();
            $("#ConfirmPassword").next(".text-danger").text("Passwords do not match.");
            return;
        }

        // Prevent form submission if email is already taken (server-side check)
        // Note: This is now checked on the server side when the form is posted
        // The server will return an error message if the email is taken

        // Proceed with form submission if all validations pass
        // You can replace the form submission here with an AJAX call if necessary
        // On successful registration, redirect to login page

        // Simulating successful registration and redirecting to the login page
        setTimeout(function () {
            window.location.href = "/Login";  // Redirect to login page
        }, 1000);  // Delay before redirecting (optional)
    });
});
